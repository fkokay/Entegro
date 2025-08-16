using Entegro.Application.DTOs.IntegrationSystem;
using Entegro.Application.DTOs.IntegrationSystemParameter;
using Entegro.Application.Interfaces.Services;
using Entegro.Infrastructure.Migrations;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class SettingsController : Controller
    {
        private readonly IIntegrationSystemService _integrationSystemService;
        private readonly IIntegrationSystemParameterService _integrationSystemParameterService;
        public SettingsController(IIntegrationSystemService integrationSystemService, IIntegrationSystemParameterService integrationSystemParameterService)
        {
            _integrationSystemService = integrationSystemService;
            _integrationSystemParameterService = integrationSystemParameterService;
        }

        public IActionResult GeneralCommon()
        {
            return View();
        }

        public IActionResult Erp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Erp(string erpType)
        {
            IntegrationSystemDto integrationSystem = new IntegrationSystemDto();
            integrationSystem.Name = $"{erpType} ERP Entegrasyonu";
            integrationSystem.IntegrationSystemType = Entegro.Domain.Enums.IntegrationSystemType.ERP;
            integrationSystem.Description = $"{erpType} ERP Entegrasyonu için gerekli ayarlar.";

            IntegrationSystemParameterDto integrationSystemParameter = new IntegrationSystemParameterDto();
            integrationSystemParameter.IntegrationSystemId = integrationSystem.Id;
            integrationSystemParameter.Key = "ErpType";
            integrationSystemParameter.Value = erpType;

            return View($"Erp.{erpType}");
        }

        public IActionResult ECommerce()
        {
            return View();
        }

        public IActionResult Marketplace()
        {
            return View();
        }

        public IActionResult Cargo()
        {
            return View();
        }

        public IActionResult EInvoice()
        {
            return View();
        }
    }
}
