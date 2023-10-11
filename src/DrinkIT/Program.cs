using DrinkIT.Domain.Models.OrderAggregate;
using DrinkIT.Extensions;
using DrinkIT.Infrastructure.Ordering.Repositories;
using DrinkIT.Ordering.Commands;
using DrinkIT.Ordering.Queries;
using DrinkIT.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));
    options.Filters.Add(new ConsumesAttribute(MediaTypeNames.Application.Json));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOrderingDbContext(builder.Configuration);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CommandResult>());

builder.Services.AddTransient<IExternalPaymentSystem, ExternalPaymentSystemMock>();
builder.Services.AddTransient<IOrderRepository, OrderRepository>();
builder.Services.AddTransient<IOrdersQueries, OrdersQueries>(provider =>
    new OrdersQueries(provider.GetService<ILogger<OrdersQueries>>()!, builder.Configuration.GetConnectionString("DefaultConnection")!));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();


public partial class Program { }