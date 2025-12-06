using Microsoft.EntityFrameworkCore;
using stripIntegration.Data;

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
app.UseStaticFiles();
// app.MapFallbackToFile("checkout.html");

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
