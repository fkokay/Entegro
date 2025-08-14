using Entegro.ERP.Abstractions.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Entegro.ERP.Api.Controllers
{
    [ApiController]
    [Route("api/{erpType}")]
    public class ErpController : ControllerBase
    {
        private readonly IErpDatabaseInitializerFactory _erpDatabaseInitializerFactory;
        private readonly IErpProductReaderFactory _erpProductReaderFactory;
        private readonly IErpProductStockReaderFactory _erpProductStockReaderFactory;
        private readonly IErpProductPriceReaderFactory _erpProductPriceReaderFactory;
        private readonly IErpCustomerReaderFactory _erpCustomerReaderFactory;
        private readonly IErpCustomerBalanceReaderFactory _erpCustomerBalanceReaderFactory;
        private readonly IErpOrderReaderFactory _erpOrderReaderFactory;
        private readonly IConfiguration _configuration;

        public ErpController(
            IErpDatabaseInitializerFactory erpDatabaseInitializerFactory, 
            IErpProductReaderFactory erpProductReaderFactory, 
            IErpProductStockReaderFactory erpProductStockReaderFactory,
            IErpProductPriceReaderFactory erpProductPriceReaderFactory,
            IErpCustomerReaderFactory erpCustomerReaderFactory,
            IErpCustomerBalanceReaderFactory erpCustomerBalanceReaderFactory,
            IErpOrderReaderFactory erpOrderReaderFactory,
            IConfiguration configuration)
        {
            _erpDatabaseInitializerFactory = erpDatabaseInitializerFactory;
            _erpProductReaderFactory = erpProductReaderFactory;
            _erpProductStockReaderFactory = erpProductStockReaderFactory;
            _erpProductPriceReaderFactory = erpProductPriceReaderFactory;
            _erpCustomerReaderFactory = erpCustomerReaderFactory;
            _erpCustomerBalanceReaderFactory = erpCustomerBalanceReaderFactory;
            _erpOrderReaderFactory = erpOrderReaderFactory;
            _configuration = configuration;
        }

        [HttpPost("database-initialize")]
        public async Task<IActionResult> DatabaseInitialize([FromRoute] string erpType)
        {
            string connectionString = GetConnectionString(erpType) ?? "";
            if (connectionString.IsNullOrEmpty())
            {
                return NotFound();

            }
            var erpDatabaseInitializer = _erpDatabaseInitializerFactory.CreateDatabaseInitializer(erpType,connectionString);

            await erpDatabaseInitializer.EnsureViewsAsync();

            return Ok($"{erpType} view'ları kontrol edildi ve gerekiyorsa oluşturuldu.");
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("products")]
        public async Task<IActionResult> GetProducts([FromRoute] string erpType, int page, int pageSize)
        {
            string connectionString = GetConnectionString(erpType) ?? "";
            if (connectionString.IsNullOrEmpty())
            {
                return NotFound();
            }

            var erpProductReader = _erpProductReaderFactory.Create(erpType, connectionString);

            var products = await erpProductReader.GetProductsAsync(page, pageSize);
            if (products == null) return NotFound();

            return Ok(products);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("product-stocks")]
        public async Task<IActionResult> GetProductStocks([FromRoute] string erpType, int page, int pageSize)
        {
            string connectionString = GetConnectionString(erpType) ?? "";
            if (connectionString.IsNullOrEmpty())
            {
                return NotFound();
            }

            var erpProductStockReader = _erpProductStockReaderFactory.Create(erpType, connectionString);

            var productStocks = await erpProductStockReader.GetProductStocksAsync(page, pageSize);
            if (productStocks == null) return NotFound();

            return Ok(productStocks);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("product-prices")]
        public async Task<IActionResult> GetProductPrices([FromRoute] string erpType, int page, int pageSize)
        {
            string connectionString = GetConnectionString(erpType) ?? "";
            if (connectionString.IsNullOrEmpty())
            {
                return NotFound();
            }

            var erpProductPriceReader = _erpProductPriceReaderFactory.Create(erpType, connectionString);

            var productPrices = await erpProductPriceReader.GetProductPricesAsync(page, pageSize);
            if (productPrices == null) return NotFound();

            return Ok(productPrices);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("customer")]
        public async Task<IActionResult> GetCustomers([FromRoute] string erpType, int page, int pageSize)
        {
            string connectionString = GetConnectionString(erpType) ?? "";
            if (connectionString.IsNullOrEmpty())
            {
                return NotFound();
            }

            var erpCustomerReader = _erpCustomerReaderFactory.Create(erpType, connectionString);

            var customers = await erpCustomerReader.GetCustomersAsync(page, pageSize);
            if (customers == null) return NotFound();

            return Ok(customers);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("customer-balances")]
        public async Task<IActionResult> GetCustomerBalances([FromRoute] string erpType, int page, int pageSize)
        {
            string connectionString = GetConnectionString(erpType) ?? "";
            if (connectionString.IsNullOrEmpty())
            {
                return NotFound();
            }

            var erpCustomerBalanceReader = _erpCustomerBalanceReaderFactory.Create(erpType, connectionString);

            var customerBalances = await erpCustomerBalanceReader.GetCustomerBalancesAsync(page, pageSize);
            if (customerBalances == null) return NotFound();

            return Ok(customerBalances);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders([FromRoute] string erpType, int page, int pageSize)
        {
            string connectionString = GetConnectionString(erpType) ?? "";
            if (connectionString.IsNullOrEmpty())
            {
                return NotFound();
            }

            var erpOrderReader = _erpOrderReaderFactory.Create(erpType, connectionString);

            var orders = await erpOrderReader.GetOrdersAsync(page, pageSize);
            if (orders == null) return NotFound();

            return Ok(orders);
        }

        private string? GetConnectionString(string erpType)
        {
            return erpType.ToLower() switch
            {
                "logo" => _configuration.GetConnectionString("Logo"),
                "netsis" => _configuration.GetConnectionString("Netsis"),
                "opak" => _configuration.GetConnectionString("Opak"),
                _ => throw new ArgumentException($"Unsupported ERP type: {erpType}")
            };
        }
    }
}
