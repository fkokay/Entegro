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
        private readonly IConfiguration _configuration;

        public ErpController(IErpDatabaseInitializerFactory erpDatabaseInitializerFactory, IErpProductReaderFactory erpProductReaderFactory, IConfiguration configuration)
        {
            _erpDatabaseInitializerFactory = erpDatabaseInitializerFactory;
            _erpProductReaderFactory = erpProductReaderFactory;
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
