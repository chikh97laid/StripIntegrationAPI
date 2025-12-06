using Microsoft.EntityFrameworkCore;
using stripIntegration.Data;
using stripIntegration.Seeds;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//this for using sql server database
// builder.Services.AddDbContext<AppDbContext>(options =>
// options.UseSqlServer(builder.Configuration.GetConnectionString("myConnection")));

builder.Services.AddDbContext<AppDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("myConnection")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Get the database context
        var dbContext = services.GetRequiredService<AppDbContext>();
        
        // 1. Apply any pending migrations (create tables)
        dbContext.Database.Migrate();

        // 2. Seed initial data
        Extensions.SeedData(dbContext); 
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

app.UseStaticFiles();
app.MapFallbackToFile("checkout.html");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");

app.Run();
