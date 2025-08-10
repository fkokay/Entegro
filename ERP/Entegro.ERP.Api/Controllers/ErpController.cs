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
        private readonly IErpProductReaderFactory _erpProductReaderFactory;
        private readonly IConfiguration _configuration;

        public ErpController(IErpProductReaderFactory erpProductReaderFactory, IConfiguration configuration)
        {
            _erpProductReaderFactory = erpProductReaderFactory;
            _configuration = configuration;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("products")]
        public async Task<IActionResult> GetProducts([FromRoute] string erpType,int page, int pageSize)
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
