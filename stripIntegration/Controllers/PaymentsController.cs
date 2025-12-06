using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using stripIntegration.Data;
using stripIntegration.Dtos;
using Customer = stripIntegration.Models.Customer;

namespace stripIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentsController> _logger;
        private readonly AppDbContext _dbContext;

        public PaymentsController(IConfiguration configuration, ILogger<PaymentsController> logger, AppDbContext dbContext)
        {
            _configuration = configuration;
            _logger = logger;
            _dbContext = dbContext;
        }

        /*************  ✨ Windsurf Command ⭐   *************/
        /// <summary>   
        /// Creates a new customer in the database if one does not exist already.
        /// </summary>      
        /// <param name="email">The email  address of the customer.</param>
        /*******  e7ec6207-781f-484b-bcdc-a2bc625d6925  *******/
        private async Task<Customer> CreateCustomer(string email)
        {
            var customer = await _dbContext.Customers
                .FirstOrDefaultAsync(c => c.Email == email);
            if (customer == null)
            {
                customer = new Customer
                {
                    Email = email
                };
                _dbContext.Customers.Add(customer);
            }
            await CommitChangesAsync(); // Save to get the generated customer ID
            return customer;
        }

        private async Task<Models.Order> CreateOrder(int customerId)
        {
            var customerExists = await _dbContext.Customers.AnyAsync(c => c.Id == customerId);
            if (!customerExists)
            {
                throw new ArgumentException("Customer does not exist", nameof(customerId));
            }

            // Generate order number (could be last order ID + 1, or use current timestamp, etc.)
            var lastOrder = await _dbContext.Orders.OrderByDescending(o => o.Id).FirstOrDefaultAsync();
            var nextOrderNumber = (lastOrder?.Id ?? 0) + 1001; // Start from 1001 instead of 1

            var order = new Models.Order
            {
                CustomerId = customerId,
                OrderNumber = nextOrderNumber,
                CreatedAt = DateTime.UtcNow,
                OrderStatus = Models.enOrderStatus.Pending,
                PaymentStatus = Models.enPaymentStatus.Pending
            };

            _dbContext.Orders.Add(order);
            await CommitChangesAsync(); // Save to get the generated order ID
            return order;
        }

        private async Task CreateOrderItems(List<OrderItemDto> items, int orderId)
        {
            foreach (var item in items)
            {
                var product = await _dbContext.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    var orderItem = new Models.OrderItem
                    {
                        OrderId = orderId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price
                    };
                    _dbContext.OrderItems.Add(orderItem);
                }
            }
            await CommitChangesAsync(); // Save to get the generated order item IDs
        }

        private async Task CommitChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        [HttpPost("create-session")]
        public async Task<IActionResult> CreateCheckoutSession(CreateCheckoutSessionRequest request)
        {
            // Validate request and quantities
            if (request == null || request.Items == null || !request.Items.Any())
                return BadRequest(new { error = "Request must include at least one item." });

            var invalidItem = request.Items.FirstOrDefault(i => i.Quantity <= 0);
            if (invalidItem != null)
                return BadRequest(new { error = $"Invalid quantity for product {invalidItem.ProductId}. Quantity must be greater than zero." });

            foreach (var it in request.Items)
            {
                var product = await _dbContext.Products.FindAsync(it.ProductId);
                if (product == null)
                {
                    return BadRequest(new { error = $"Product not found: {it.ProductId}" });
                }
                if (product.Quantity < it.Quantity)
                {
                    return BadRequest(new { error = $"Insufficient quantity for product {it.ProductId}. Available quantity: {product.Quantity}" });
                }
            }

            Customer customer = new();
            Models.Order order = new();

            // Create customer, order, and order items within a transaction
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                customer = await CreateCustomer(request.CustomerEmail);
                order = await CreateOrder(customer.Id);
                await CreateOrderItems(request.Items, order.Id);                
                
            }
            catch (ArgumentException  ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new { error = ex.Message });
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { error = "Unexpected error", details = ex.Message });
            }

            // calculate total amount
            order.TotalAmount = _dbContext.OrderItems
                .Include(i => i.Product)
                .Where(i => i.OrderId == order.Id)
                .Sum(i => i.UnitPrice * i.Quantity);

            // Read API key from configuration or environment variable
            var apiKey = _configuration["Stripe:SecretKey"] ?? Environment.GetEnvironmentVariable("STRIPE_API_KEY");
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return Problem(detail: "Stripe secret key is not configured. Set Stripe:SecretKey in configuration or STRIPE_API_KEY env variable.", statusCode: 500);
            }

            StripeConfiguration.ApiKey = apiKey;

            // Convert decimal amount (e.g., dollars) to the smallest currency unit (e.g., cents)
            // Assumption: Amount is provided in major currency unit (e.g., 10.50 means $10.50)
            // var unitAmount = Convert.ToInt64(decimal.Round(request.Amount * 100m));

            var successUrl = $"https://stripintegrationapi.onrender.com/dashboard.html?session_id={{CHECKOUT_SESSION_ID}}";
            var cancelUrl = $"https://stripintegrationapi.onrender.com/cancel";

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                Mode = "payment",
                CustomerEmail = request.CustomerEmail,
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
                ClientReferenceId = order.Id.ToString(),
                PaymentIntentData = new SessionPaymentIntentDataOptions()
                {
                    Metadata = new Dictionary<string, string>
                    {
                        { "orderId", order.Id.ToString() },
                        { "customerId", customer.Id.ToString() },
                        {"items", JsonSerializer.Serialize(request.Items) }
                    }
                },
                LineItems = _dbContext.OrderItems.Include(i => i.Product)
                .Where(o => o.OrderId == order.Id)
                .Select(i => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = Convert.ToInt64(decimal.Round(i.UnitPrice * 100m)),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = i.Product.Name
                        }
                    },
                    Quantity = i.Quantity
                }).ToList()
            };

            var service = new SessionService();
            Session session;
            try
            {
                session = service.Create(options);
            }
            catch (StripeException ex)
            {
                await transaction.RollbackAsync();
                return Problem(detail: ex.Message, statusCode: 502);
            }

            order.StripeSessionId = session.Id;
            order.PaymentIntentId = session.PaymentIntentId;

            await CommitChangesAsync();
            await transaction.CommitAsync(); return Ok(new { url = session.Url });
        }

        // POST /api/payments/cart/summary
        [HttpPost("cart/summary")]
        public async Task<IActionResult> CartSummary(List<OrderItemDto> items)
        {
            if (items == null || !items.Any())
                return BadRequest(new { error = "Items are required" });

            var invalid = items.FirstOrDefault(i => i.Quantity <= 0);
            if (invalid != null)
                return BadRequest(new { error = $"Invalid quantity for product {invalid.ProductId}. Quantity must be greater than zero." });

            decimal total = 0m;
            foreach (var it in items)
            {
                var product = await _dbContext.Products.FindAsync(it.ProductId);
                if (product == null)
                {
                    return BadRequest(new { error = $"Product not found: {it.ProductId}" });
                }
                total += product.Price * it.Quantity;
            }

            return Ok(new { total = Math.Round(total, 2) });
        }

        private async Task<bool> UpdateStock(List<OrderItemDto> items)
        {
            // check stock availability
            foreach (var item in items)
            {
                var product = await _dbContext.Products.FindAsync(item.ProductId);

                if (product == null || product.Quantity < item.Quantity)
                {
                    return false;
                }
            }

            // if all good, update stock
            foreach (var item in items)
            {
                var product = await _dbContext.Products.FindAsync(item.ProductId);
                product!.Quantity -= item.Quantity;
            }
            await CommitChangesAsync();
            return true;
        }

        private async Task RestoreStock(int orderId)
        {
            var orderItems = await _dbContext.OrderItems.Include(oi => oi.Product).Where(oi => oi.OrderId == orderId).ToListAsync();
            foreach (var item in orderItems)
            {
                item.Product.Quantity += item.Quantity;
            }
            await CommitChangesAsync();
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            // Read the raw body as JSON
            string json;
            using (var reader = new StreamReader(Request.Body))
            {
                json = await reader.ReadToEndAsync();
            }

            var sigHeader = Request.Headers["Stripe-Signature"].FirstOrDefault();
            var webhookSecret = _configuration["Stripe:WebhookSecret"] ?? Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET");

            if (string.IsNullOrWhiteSpace(sigHeader) || string.IsNullOrWhiteSpace(webhookSecret))
            {
                _logger.LogWarning("Missing Stripe signature header or webhook secret.");
                return BadRequest();
            }

            Event stripeEvent;
            try
            {
                stripeEvent = EventUtility.ConstructEvent(json, sigHeader, webhookSecret);
            }
            catch (StripeException ex)
            {
                _logger.LogWarning(ex, "Stripe webhook signature verification failed.");
                return BadRequest();
            }
            var order = new Models.Order();
            try
            {
                switch (stripeEvent.Type)
                {
                    case "checkout.session.completed":
                        var session = stripeEvent.Data.Object as Session;
                        _logger.LogInformation("checkout.session.completed received. SessionId={SessionId}", session?.Id);

                        if (session != null && !string.IsNullOrEmpty(session.ClientReferenceId))
                        {
                            if (int.TryParse(session.ClientReferenceId, out int orderId))
                            {
                                order = await _dbContext.Orders.FindAsync(orderId);
                                if (order != null && order.OrderStatus != Models.enOrderStatus.Paid)
                                {
                                    order.OrderStatus = Models.enOrderStatus.Processing;
                                    order.PaymentStatus = Models.enPaymentStatus.Processing;
                                    break;
                                }
                            }
                        }

                        break;
                    case "payment_intent.succeeded":
                        var intent = stripeEvent.Data.Object as PaymentIntent;
                        _logger.LogInformation("payment_intent.succeeded received. PaymentIntentId={PaymentIntentId}", intent?.Id);

                        if (intent != null && intent.Metadata != null && intent.Metadata.TryGetValue("orderId", out var orderIdStr))
                        {
                            if (int.TryParse(orderIdStr, out int orderId))
                            {
                                order = await _dbContext.Orders.FindAsync(orderId);
                                if (order != null && order.PaymentStatus != Models.enPaymentStatus.Paid)
                                {
                                    // Update stock based on metadata items
                                    var items = JsonSerializer.Deserialize<List<OrderItemDto>>(intent.Metadata["items"]);
                                    await UpdateStock(items!);

                                    order.PaymentIntentId = intent.Id;
                                    order.OrderStatus = Models.enOrderStatus.Paid;
                                    order.PaymentStatus = Models.enPaymentStatus.Paid;
                                    order.PaidAt = DateTime.UtcNow;
                                }
                            }
                        }
                        break;
                    default:
                        _logger.LogInformation("Received unhandled event type: {EventType}", stripeEvent.Type);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Stripe webhook event.");
                return BadRequest();
            }

            await CommitChangesAsync();
            return Ok();
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders([FromQuery] string customerEmail)
        {
            if (string.IsNullOrWhiteSpace(customerEmail))
                return BadRequest(new { error = "Customer email is required" });

            var customer = await _dbContext.Customers!
                .Include(c => c.Orders)!
                .ThenInclude(o => o.OrderItems)!
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(c => c.Email == customerEmail);

            if (customer == null)
                return NotFound(new { error = "Customer not found" });

            var orderResponseDtos = customer!.Orders!.Select(o => new Dtos.OrderResponseDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                CreatedAt = o.CreatedAt,
                TotalAmount = o.TotalAmount,
                PaymentStatus = o.PaymentStatus.ToString(),
                OrderStatus = o.OrderStatus.ToString(),
                PaidAt = o.PaidAt,
                RefundedAt = o.RefundedAt,
                Items = o.OrderItems!.Select(oi => new Dtos.OrderItemResponseDto
                {
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    Total = oi.Quantity * oi.UnitPrice
                }).ToList()
            }).ToList();

            return Ok(orderResponseDtos);
        }

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _dbContext.Products.Select(p => new
        {
            p.Id,
            p.Name,
            p.Price,
            p.Quantity
        }).ToListAsync();

        return Ok(products);
    }

    [HttpPost("refund")]
    public async Task<IActionResult> Refund([FromQuery] int orderId)
        {
            if (orderId <= 0)
            {
                return BadRequest(new { error = "Invalid order ID" });
            }

            var order = await _dbContext.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                return NotFound(new { error = "Order not found" });
            }

            if (order.PaymentStatus != Models.enPaymentStatus.Paid)
            {
                return BadRequest(new { error = "Can only refund paid orders" });
            }

            if (string.IsNullOrEmpty(order.PaymentIntentId))
            {
                return BadRequest(new { error = "No payment intent available for refund" });
            }

            // Check for compatibility: ensure order hasn't been refunded yet
            if (!string.IsNullOrEmpty(order.RefundId) || order.RefundedAt.HasValue)
            {
                return BadRequest(new { error = "Order already refunded" });
            }

            // Read API key from configuration or environment variable
            var apiKey = _configuration["Stripe:SecretKey"] ?? Environment.GetEnvironmentVariable("STRIPE_API_KEY");
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return Problem(detail: "Stripe secret key is not configured. Set Stripe:SecretKey in configuration or STRIPE_API_KEY env variable.", statusCode: 500);
            }

            StripeConfiguration.ApiKey = apiKey;

            var refundService = new RefundService();
            var refundOptions = new RefundCreateOptions
            {
                PaymentIntent = order.PaymentIntentId,
                Reason = RefundReasons.RequestedByCustomer,
                // Amount is optional; if not specified, full amount is refunded
            };

            Refund? refund = null;
            try
            {
                refund = await refundService.CreateAsync(refundOptions);
                _logger.LogInformation("Refund created successfully for OrderId={OrderId}, RefundId={RefundId}", orderId, refund.Id);
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Failed to create refund for OrderId={OrderId}", orderId);
                return Problem(detail: ex.Message, statusCode: 502);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during refund for OrderId={OrderId}", orderId);
                return StatusCode(500, new { error = "Unexpected error", details = ex.Message });
            }

            // Update order status
            order.OrderStatus = Models.enOrderStatus.Refunded;
            order.PaymentStatus = Models.enPaymentStatus.Refunded;
            order.RefundId = refund.Id;
            order.RefundedAt = DateTime.UtcNow;

            await RestoreStock(orderId);

            await CommitChangesAsync();

            return Ok(new { message = "Refund processed successfully", orderId = orderId });
        }


    }
}
