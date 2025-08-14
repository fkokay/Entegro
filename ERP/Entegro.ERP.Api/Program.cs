using Entegro.ERP.Abstractions.Interfaces;
using Entegro.ERP.Application.Factories;
using Entegro.ERP.Logo.Install;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IErpDatabaseInitializerFactory, ErpDatabaseInitializerFactory>();
builder.Services.AddScoped<IErpProductReaderFactory, ErpProductReaderFactory>();
builder.Services.AddScoped<IErpProductPriceReaderFactory, ErpProductPriceReaderFactory>();
builder.Services.AddScoped<IErpProductStockReaderFactory, ErpProductStockReaderFactory>();
builder.Services.AddScoped<IErpCustomerReaderFactory, ErpCustomerReaderFactory>();
builder.Services.AddScoped<IErpCustomerBalanceReaderFactory, ErpCustomerBalanceReaderFactory>();
builder.Services.AddScoped<IErpOrderReaderFactory, ErpOrderReaderFactory>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
