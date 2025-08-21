using Entegro.Application.DTOs.IntegrationSystem;
using Entegro.Application.DTOs.IntegrationSystemParameter;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Enums;
using Entegro.Web.Models;
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
        #region Erp Entegrasyonları
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


                    return View($"Erp.Logo", model);
                case "Netsis":

                    var apiUrlForNetsis = integrationSystemErp.Parameters.Where(m => m.Key == "ApiUrl").FirstOrDefault();
                    var apiUserForNetsis = integrationSystemErp.Parameters.Where(m => m.Key == "ApiUser").FirstOrDefault();
                    var apiPasswordForNetsis = integrationSystemErp.Parameters.Where(m => m.Key == "ApiPassword").FirstOrDefault();

                    NetsisErpSettingsViewModel modelForNetsis = new NetsisErpSettingsViewModel();
                    modelForNetsis.IntegrationSystemId = integrationSystemErp.Id;
                    modelForNetsis.ErpType = erpType.Value;
                    modelForNetsis.ApiUrl = apiUrlForNetsis?.Value;
                    modelForNetsis.ApiUser = apiUserForNetsis?.Value;
                    modelForNetsis.ApiPassword = apiPasswordForNetsis?.Value;
                    return View($"Erp.Netsis", modelForNetsis);
                case "Opak":
                    var apiUrlForOpak = integrationSystemErp.Parameters.Where(m => m.Key == "ApiUrl").FirstOrDefault();
                    var apiUserForOpak = integrationSystemErp.Parameters.Where(m => m.Key == "ApiUser").FirstOrDefault();
                    var apiPasswordForOpak = integrationSystemErp.Parameters.Where(m => m.Key == "ApiPassword").FirstOrDefault();

                    OpakErpSettingsViewModel modelForOpak = new OpakErpSettingsViewModel();
                    modelForOpak.IntegrationSystemId = integrationSystemErp.Id;
                    modelForOpak.ErpType = erpType.Value;
                    modelForOpak.ApiUrl = apiUrlForOpak?.Value;
                    modelForOpak.ApiUser = apiUserForOpak?.Value;
                    modelForOpak.ApiPassword = apiPasswordForOpak?.Value;
                    return View($"Erp.Opak", modelForOpak);
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

            return View($"Erp.{model.ErpType}", model);
        }

        [HttpPost]
        public async Task<IActionResult> ErpDelete([FromBody] int integrationSystemId)
        {
            var isSuccess = await _integrationSystemService.DeleteAsync(integrationSystemId);
            if (isSuccess)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Silinecek Entegrasyon Bulunamadı" });
        }
        #endregion


        #region E-Ticaret Entegrasyonları 
        [HttpGet]
        public async Task<IActionResult> ECommerce()
        {
            var integrationSystemCommerce = await _integrationSystemService.GetByTypeIdAsync((int)IntegrationSystemType.Commerce);
            if (integrationSystemCommerce == null)
            {
                return View();
            }


            var commerceType = integrationSystemCommerce.Parameters.Where(m => m.Key == "CommerceType").FirstOrDefault();
            if (commerceType == null)
            {
                return NotFound();
            }

            var myCommerce = await _integrationSystemService.GetAllAsync();
            ViewBag.MyCommerce = myCommerce.Where(x => x.IntegrationSystemTypeId == 2).
                Select(m => new IntegrationSystemViewModel
                {
                    Id = m.Id,
                    Description = m.Description,
                    IntegrationSystemTypeId = m.IntegrationSystemTypeId,
                    Name = m.Name
                });


            return View(integrationSystemCommerce);
        }


        [HttpPost]
        public async Task<IActionResult> ECommerce(CreateIntegrationSystemViewModel model)
        {
            var createIntegrationSystem = new CreateIntegrationSystemDto
            {
                Name = model.ModalName,
                IntegrationSystemTypeId = (int)IntegrationSystemType.Commerce,
                Description = model.ModalDescription
            };

            var integrationSystemId = await _integrationSystemService.AddAsync(createIntegrationSystem);

            // Ana parametre
            await _integrationSystemParameterService.AddAsync(new CreateIntegrationSystemParameterDto
            {
                IntegrationSystemId = integrationSystemId,
                Key = "CommerceType",
                Value = model.IntegrationSystemType
            });


            return Json(new { success = true });
        }

        public async Task<IActionResult> ECommerceSettings(int integrationSystemCommerceId)
        {

            var integrationSystemCommerce = await _integrationSystemService.GetByIdAsync(integrationSystemCommerceId);
            if (integrationSystemCommerce == null)
            {
                return View();
            }


            var commerceType = integrationSystemCommerce.Parameters.Where(m => m.Key == "CommerceType" & m.IntegrationSystemId == integrationSystemCommerceId).FirstOrDefault();
            if (commerceType == null)
            {
                return NotFound();
            }
            switch (commerceType.Value)
            {
                case "Smartstore":

                    var apiUrl = integrationSystemCommerce.Parameters.Where(m => m.Key == "ApiUrl" & m.IntegrationSystemId == integrationSystemCommerceId).FirstOrDefault();
                    var apiUser = integrationSystemCommerce.Parameters.Where(m => m.Key == "ApiUser" & m.IntegrationSystemId == integrationSystemCommerceId).FirstOrDefault();
                    var apiPassword = integrationSystemCommerce.Parameters.Where(m => m.Key == "ApiPassword" & m.IntegrationSystemId == integrationSystemCommerceId).FirstOrDefault();

                    SmartstoreCommerceSettingsViewModel model = new SmartstoreCommerceSettingsViewModel();
                    model.IntegrationSystemId = integrationSystemCommerceId;
                    model.CommerceType = commerceType.Value;
                    model.ApiUrl = apiUrl?.Value;
                    model.ApiUser = apiUser?.Value;
                    model.ApiPassword = apiPassword?.Value;

                    return View($"ECommerce.Smartstore", model);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CommerceParameterSmartstore(SmartstoreCommerceSettingsViewModel model)
        {
            var apiUrl = await _integrationSystemParameterService.GetByKeyAsync("ApiUrl", model.IntegrationSystemId);
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

            var apiUser = await _integrationSystemParameterService.GetByKeyAsync("ApiUser", model.IntegrationSystemId);
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

            var apiPassword = await _integrationSystemParameterService.GetByKeyAsync("ApiPassword", model.IntegrationSystemId);
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

            //return View($"ECommerce.{model.CommerceType}", model);
            return RedirectToAction("ecommerce");
        }

        [HttpPost]
        public async Task<IActionResult> ECommerceDelete([FromBody] int integrationSystemId)
        {
            var isSuccess = await _integrationSystemService.DeleteAsync(integrationSystemId);
            if (isSuccess)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Silinecek E-Ticaret Bulunamadı" });
        }
        #endregion


        #region Kargo Entegrasyonları
        [HttpGet]
        public async Task<IActionResult> Cargo()
        {
            var integrationSystemCargo = await _integrationSystemService.GetByTypeIdAsync((int)IntegrationSystemType.Cargo);
            if (integrationSystemCargo == null)
            {
                return View();
            }


            var commerceType = integrationSystemCargo.Parameters.Where(m => m.Key == "CargoType").FirstOrDefault();
            if (commerceType == null)
            {
                return NotFound();
            }

            var myCargo = await _integrationSystemService.GetAllAsync();
            ViewBag.MyCargo = myCargo.Where(x => x.IntegrationSystemTypeId == 4).
                Select(m => new IntegrationSystemViewModel
                {
                    Id = m.Id,
                    Description = m.Description,
                    IntegrationSystemTypeId = m.IntegrationSystemTypeId,
                    Name = m.Name
                });


            return View(integrationSystemCargo);
        }
        [HttpPost]
        public async Task<IActionResult> Cargo(CreateIntegrationSystemViewModel model)
        {
            var createIntegrationSystem = new CreateIntegrationSystemDto
            {
                Name = model.ModalName,
                IntegrationSystemTypeId = (int)IntegrationSystemType.Cargo,
                Description = model.ModalDescription
            };

            var integrationSystemId = await _integrationSystemService.AddAsync(createIntegrationSystem);

            // Ana parametre
            await _integrationSystemParameterService.AddAsync(new CreateIntegrationSystemParameterDto
            {
                IntegrationSystemId = integrationSystemId,
                Key = "CargoType",
                Value = model.IntegrationSystemType
            });


            return Json(new { success = true });
        }

        public async Task<IActionResult> CargoSettings(int integrationSystemCargoId)
        {

            var integrationSystemCargo = await _integrationSystemService.GetByIdAsync(integrationSystemCargoId);
            if (integrationSystemCargo == null)
            {
                return View();
            }


            var commerceType = integrationSystemCargo.Parameters.Where(m => m.Key == "CargoType" & m.IntegrationSystemId == integrationSystemCargoId).FirstOrDefault();
            if (commerceType == null)
            {
                return NotFound();
            }
            switch (commerceType.Value)
            {
                case "Yurtici":

                    var apiUrl = integrationSystemCargo.Parameters.Where(m => m.Key == "ApiUrl" & m.IntegrationSystemId == integrationSystemCargoId).FirstOrDefault();
                    var apiUser = integrationSystemCargo.Parameters.Where(m => m.Key == "ApiUser" & m.IntegrationSystemId == integrationSystemCargoId).FirstOrDefault();
                    var apiPassword = integrationSystemCargo.Parameters.Where(m => m.Key == "ApiPassword" & m.IntegrationSystemId == integrationSystemCargoId).FirstOrDefault();

                    YurticiCargoSettingsViewModel model = new YurticiCargoSettingsViewModel();
                    model.IntegrationSystemId = integrationSystemCargoId;
                    model.CommerceType = commerceType.Value;
                    model.ApiUrl = apiUrl?.Value;
                    model.ApiUser = apiUser?.Value;
                    model.ApiPassword = apiPassword?.Value;

                    return View($"Cargo.Yurtici", model);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CargoParameterYurtici(YurticiCargoSettingsViewModel model)
        {
            var apiUrl = await _integrationSystemParameterService.GetByKeyAsync("ApiUrl", model.IntegrationSystemId);
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

            var apiUser = await _integrationSystemParameterService.GetByKeyAsync("ApiUser", model.IntegrationSystemId);
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

            var apiPassword = await _integrationSystemParameterService.GetByKeyAsync("ApiPassword", model.IntegrationSystemId);
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


            return RedirectToAction("cargo");
        }

        [HttpPost]
        public async Task<IActionResult> CargoDelete([FromBody] int integrationSystemId)
        {
            var isSuccess = await _integrationSystemService.DeleteAsync(integrationSystemId);
            if (isSuccess)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Silinecek Kargo Bulunamadı" });
        }
        #endregion




        public IActionResult Marketplace()
        {
            return View();
        }



        public IActionResult EInvoice()
        {
            return View();
        }
    }
}
