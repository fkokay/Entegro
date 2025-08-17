using Entegro.Application.DTOs.IntegrationSystem;
using Entegro.Application.DTOs.IntegrationSystemParameter;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Entities;
using Entegro.Domain.Enums;
using Entegro.Infrastructure.Migrations;
using Entegro.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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

        public async Task<IActionResult> Erp()
        {
            var integrationSystemErp = await _integrationSystemService.GetByTypeIdAsync((int)IntegrationSystemType.ERP);
            if (integrationSystemErp == null)
            {
                return View();
            }

            var erpType = integrationSystemErp.Parameters.Where(m => m.Key == "ErpType").FirstOrDefault();
            if (erpType == null)
            {
                return NotFound();
            }

            switch (erpType.Value)
            {
                case "Logo":
                    var apiUrl = integrationSystemErp.Parameters.Where(m => m.Key == "ApiUrl").FirstOrDefault();
                    var apiUser = integrationSystemErp.Parameters.Where(m => m.Key == "ApiUser").FirstOrDefault();
                    var apiPassword = integrationSystemErp.Parameters.Where(m => m.Key == "ApiPassword").FirstOrDefault();

                    LogoErpSettingsViewModel model = new LogoErpSettingsViewModel();
                    model.IntegrationSystemId = integrationSystemErp.Id;
                    model.ErpType = erpType.Value;
                    model.ApiUrl = apiUrl?.Value;
                    model.ApiUser = apiUser?.Value;
                    model.ApiPassword = apiPassword?.Value;


                    return View($"Erp.Logo",model);
                case "Netsis":
                    return View($"Erp.Netsis");
                case "Opak":
                    return View($"Erp.Opak");
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Erp(string erpType)
        {
            CreateIntegrationSystemDto createIntegrationSystem = new CreateIntegrationSystemDto();
            createIntegrationSystem.Name = $"{erpType} ERP Entegrasyonu";
            createIntegrationSystem.IntegrationSystemTypeId = (int)IntegrationSystemType.ERP;
            createIntegrationSystem.Description = $"{erpType} ERP Entegrasyonu için gerekli ayarlar.";

            var integrationSystemId = await _integrationSystemService.AddAsync(createIntegrationSystem);

            CreateIntegrationSystemParameterDto createIntegrationSystemParameter = new CreateIntegrationSystemParameterDto();
            createIntegrationSystemParameter.IntegrationSystemId = integrationSystemId;
            createIntegrationSystemParameter.Key = "ErpType";
            createIntegrationSystemParameter.Value = erpType;

            await _integrationSystemParameterService.AddAsync(createIntegrationSystemParameter);

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> ErpParameterLogo(LogoErpSettingsViewModel model)
        {
            var apiUrl = await _integrationSystemParameterService.GetByKeyAsync("ApiUrl");
            if (apiUrl == null)
            {
                CreateIntegrationSystemParameterDto createIntegrationSystemParameter = new CreateIntegrationSystemParameterDto();
                createIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                createIntegrationSystemParameter.Key = "ApiUrl";
                createIntegrationSystemParameter.Value = model.ApiUrl;

                await _integrationSystemParameterService.AddAsync(createIntegrationSystemParameter);
            }
            else
            {
                UpdateIntegrationSystemParameterDto updateIntegrationSystemParameter = new UpdateIntegrationSystemParameterDto();
                updateIntegrationSystemParameter.Id = apiUrl.Id;
                updateIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                updateIntegrationSystemParameter.Key = "ApiUrl";
                updateIntegrationSystemParameter.Value = model.ApiUrl;

                await _integrationSystemParameterService.UpdateAsync(updateIntegrationSystemParameter);
            }

            var apiUser = await _integrationSystemParameterService.GetByKeyAsync("ApiUser");
            if (apiUser == null)
            {
                CreateIntegrationSystemParameterDto createIntegrationSystemParameter = new CreateIntegrationSystemParameterDto();
                createIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                createIntegrationSystemParameter.Key = "ApiUser";
                createIntegrationSystemParameter.Value = model.ApiUser;

                await _integrationSystemParameterService.AddAsync(createIntegrationSystemParameter);
            }
            else
            {
                UpdateIntegrationSystemParameterDto updateIntegrationSystemParameter = new UpdateIntegrationSystemParameterDto();
                updateIntegrationSystemParameter.Id = apiUser.Id;
                updateIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                updateIntegrationSystemParameter.Key = "ApiUser";
                updateIntegrationSystemParameter.Value = model.ApiUser;

                await _integrationSystemParameterService.UpdateAsync(updateIntegrationSystemParameter);
            }

            var apiPassword = await _integrationSystemParameterService.GetByKeyAsync("ApiPassword");
            if (apiPassword == null)
            {
                CreateIntegrationSystemParameterDto createIntegrationSystemParameter = new CreateIntegrationSystemParameterDto();
                createIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                createIntegrationSystemParameter.Key = "ApiPassword";
                createIntegrationSystemParameter.Value = model.ApiPassword;

                await _integrationSystemParameterService.AddAsync(createIntegrationSystemParameter);
            }
            else
            {
                UpdateIntegrationSystemParameterDto updateIntegrationSystemParameter = new UpdateIntegrationSystemParameterDto();
                updateIntegrationSystemParameter.Id = apiPassword.Id;
                updateIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                updateIntegrationSystemParameter.Key = "ApiPassword";
                updateIntegrationSystemParameter.Value = model.ApiPassword;

                await _integrationSystemParameterService.UpdateAsync(updateIntegrationSystemParameter);
            }

            return View($"Erp.{model.ErpType}",model);
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
