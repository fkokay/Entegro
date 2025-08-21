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
                    Name = m.Name,
                    IntegrationSystemParameter = m.Parameters.FirstOrDefault(p => p.Key == "CargoType")
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

                case "PTT":
                    var musteriId = integrationSystemCargo.Parameters.Where(m => m.Key == "MusteriId" & m.IntegrationSystemId == integrationSystemCargoId).FirstOrDefault();
                    var password = integrationSystemCargo.Parameters.Where(m => m.Key == "Password" & m.IntegrationSystemId == integrationSystemCargoId).FirstOrDefault();
                    var barkodStartPrefix = integrationSystemCargo.Parameters.Where(m => m.Key == "BarkodStartPrefix" & m.IntegrationSystemId == integrationSystemCargoId).FirstOrDefault();
                    var barkodEndPrefix = integrationSystemCargo.Parameters.Where(m => m.Key == "BarkodEndPrefix" & m.IntegrationSystemId == integrationSystemCargoId).FirstOrDefault();

                    PTTCargoSettingsViewModel modelPTT = new PTTCargoSettingsViewModel();
                    modelPTT.IntegrationSystemId = integrationSystemCargoId;
                    modelPTT.CommerceType = commerceType.Value;
                    modelPTT.MusteriId = musteriId?.Value;
                    modelPTT.Password = password?.Value;
                    modelPTT.BarkodStartPrefix = barkodStartPrefix?.Value;
                    modelPTT.BarkodEndPrefix = barkodEndPrefix?.Value;

                    return View($"Cargo.PTT", modelPTT);

                case "Aras":
                    var username = integrationSystemCargo.Parameters.Where(m => m.Key == "Username" & m.IntegrationSystemId == integrationSystemCargoId).FirstOrDefault();
                    var passwordAras = integrationSystemCargo.Parameters.Where(m => m.Key == "Password" & m.IntegrationSystemId == integrationSystemCargoId).FirstOrDefault();
                    ;

                    ArasCargoSettingsViewModel modelAras = new ArasCargoSettingsViewModel();
                    modelAras.IntegrationSystemId = integrationSystemCargoId;
                    modelAras.CommerceType = commerceType.Value;
                    modelAras.Username = username?.Value;
                    modelAras.Password = passwordAras?.Value;

                    return View($"Cargo.Aras", modelAras);

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
        public async Task<IActionResult> CargoParameterPTT(PTTCargoSettingsViewModel model)
        {
            var customerId = await _integrationSystemParameterService.GetByKeyAsync("MusteriId", model.IntegrationSystemId);
            if (customerId == null)
            {
                CreateIntegrationSystemParameterDto createIntegrationSystemParameter = new CreateIntegrationSystemParameterDto();
                createIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                createIntegrationSystemParameter.Key = "MusteriId";
                createIntegrationSystemParameter.Value = model.MusteriId;

                await _integrationSystemParameterService.AddAsync(createIntegrationSystemParameter);
            }
            else
            {
                UpdateIntegrationSystemParameterDto updateIntegrationSystemParameter = new UpdateIntegrationSystemParameterDto();
                updateIntegrationSystemParameter.Id = customerId.Id;
                updateIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                updateIntegrationSystemParameter.Key = "MusteriId";
                updateIntegrationSystemParameter.Value = model.MusteriId;

                await _integrationSystemParameterService.UpdateAsync(updateIntegrationSystemParameter);
            }

            var password = await _integrationSystemParameterService.GetByKeyAsync("Password", model.IntegrationSystemId);
            if (password == null)
            {
                CreateIntegrationSystemParameterDto createIntegrationSystemParameter = new CreateIntegrationSystemParameterDto();
                createIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                createIntegrationSystemParameter.Key = "Password";
                createIntegrationSystemParameter.Value = model.Password;

                await _integrationSystemParameterService.AddAsync(createIntegrationSystemParameter);
            }
            else
            {
                UpdateIntegrationSystemParameterDto updateIntegrationSystemParameter = new UpdateIntegrationSystemParameterDto();
                updateIntegrationSystemParameter.Id = password.Id;
                updateIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                updateIntegrationSystemParameter.Key = "Password";
                updateIntegrationSystemParameter.Value = model.Password;

                await _integrationSystemParameterService.UpdateAsync(updateIntegrationSystemParameter);
            }

            var barkodStartPrefix = await _integrationSystemParameterService.GetByKeyAsync("BarkodStartPrefix", model.IntegrationSystemId);
            if (barkodStartPrefix == null)
            {
                CreateIntegrationSystemParameterDto createIntegrationSystemParameter = new CreateIntegrationSystemParameterDto();
                createIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                createIntegrationSystemParameter.Key = "BarkodStartPrefix";
                createIntegrationSystemParameter.Value = model.BarkodStartPrefix;

                await _integrationSystemParameterService.AddAsync(createIntegrationSystemParameter);
            }
            else
            {
                UpdateIntegrationSystemParameterDto updateIntegrationSystemParameter = new UpdateIntegrationSystemParameterDto();
                updateIntegrationSystemParameter.Id = barkodStartPrefix.Id;
                updateIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                updateIntegrationSystemParameter.Key = "BarkodStartPrefix";
                updateIntegrationSystemParameter.Value = model.BarkodStartPrefix;

                await _integrationSystemParameterService.UpdateAsync(updateIntegrationSystemParameter);
            }
            var barkodEndPrefix = await _integrationSystemParameterService.GetByKeyAsync("BarkodEndPrefix", model.IntegrationSystemId);
            if (barkodEndPrefix == null)
            {
                CreateIntegrationSystemParameterDto createIntegrationSystemParameter = new CreateIntegrationSystemParameterDto();
                createIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                createIntegrationSystemParameter.Key = "BarkodEndPrefix";
                createIntegrationSystemParameter.Value = model.BarkodEndPrefix;

                await _integrationSystemParameterService.AddAsync(createIntegrationSystemParameter);
            }
            else
            {
                UpdateIntegrationSystemParameterDto updateIntegrationSystemParameter = new UpdateIntegrationSystemParameterDto();
                updateIntegrationSystemParameter.Id = barkodEndPrefix.Id;
                updateIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                updateIntegrationSystemParameter.Key = "BarkodStartPrefix";
                updateIntegrationSystemParameter.Value = model.BarkodStartPrefix;

                await _integrationSystemParameterService.UpdateAsync(updateIntegrationSystemParameter);
            }


            return RedirectToAction("cargo");
        }
        [HttpPost]
        public async Task<IActionResult> CargoParameterAras(ArasCargoSettingsViewModel model)
        {
            var username = await _integrationSystemParameterService.GetByKeyAsync("Username", model.IntegrationSystemId);
            if (username == null)
            {
                CreateIntegrationSystemParameterDto createIntegrationSystemParameter = new CreateIntegrationSystemParameterDto();
                createIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                createIntegrationSystemParameter.Key = "Username";
                createIntegrationSystemParameter.Value = model.Username;

                await _integrationSystemParameterService.AddAsync(createIntegrationSystemParameter);
            }
            else
            {
                UpdateIntegrationSystemParameterDto updateIntegrationSystemParameter = new UpdateIntegrationSystemParameterDto();
                updateIntegrationSystemParameter.Id = username.Id;
                updateIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                updateIntegrationSystemParameter.Key = "Username";
                updateIntegrationSystemParameter.Value = model.Username;

                await _integrationSystemParameterService.UpdateAsync(updateIntegrationSystemParameter);
            }

            var password = await _integrationSystemParameterService.GetByKeyAsync("Password", model.IntegrationSystemId);
            if (password == null)
            {
                CreateIntegrationSystemParameterDto createIntegrationSystemParameter = new CreateIntegrationSystemParameterDto();
                createIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                createIntegrationSystemParameter.Key = "Password";
                createIntegrationSystemParameter.Value = model.Password;

                await _integrationSystemParameterService.AddAsync(createIntegrationSystemParameter);
            }
            else
            {
                UpdateIntegrationSystemParameterDto updateIntegrationSystemParameter = new UpdateIntegrationSystemParameterDto();
                updateIntegrationSystemParameter.Id = password.Id;
                updateIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                updateIntegrationSystemParameter.Key = "Password";
                updateIntegrationSystemParameter.Value = model.Password;

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


        #region Pazaryeri Entegrasyonları
        [HttpGet]
        public async Task<IActionResult> Marketplace()
        {
            var integrationSystemMarketplace = await _integrationSystemService.GetByTypeIdAsync((int)IntegrationSystemType.Marketplace);
            if (integrationSystemMarketplace == null)
            {
                return View();
            }


            var commerceType = integrationSystemMarketplace.Parameters.Where(m => m.Key == "MarketplaceType").FirstOrDefault();
            if (commerceType == null)
            {
                return NotFound();
            }

            var myMarketPlace = await _integrationSystemService.GetAllAsync();
            ViewBag.MyMarketPlace = myMarketPlace.Where(x => x.IntegrationSystemTypeId == 3).
                Select(m => new IntegrationSystemViewModel
                {
                    Id = m.Id,
                    Description = m.Description,
                    IntegrationSystemTypeId = m.IntegrationSystemTypeId,
                    Name = m.Name
                });


            return View(integrationSystemMarketplace);
        }


        [HttpPost]
        public async Task<IActionResult> Marketplace(CreateIntegrationSystemViewModel model)
        {
            var createIntegrationSystem = new CreateIntegrationSystemDto
            {
                Name = model.ModalName,
                IntegrationSystemTypeId = (int)IntegrationSystemType.Marketplace,
                Description = model.ModalDescription
            };

            var integrationSystemId = await _integrationSystemService.AddAsync(createIntegrationSystem);

            // Ana parametre
            await _integrationSystemParameterService.AddAsync(new CreateIntegrationSystemParameterDto
            {
                IntegrationSystemId = integrationSystemId,
                Key = "MarketplaceType",
                Value = model.IntegrationSystemType
            });


            return Json(new { success = true });
        }

        public async Task<IActionResult> MarketplaceSettings(int integrationSystemMarketplaceId)
        {

            var integrationSystemMarketplace = await _integrationSystemService.GetByIdAsync(integrationSystemMarketplaceId);
            if (integrationSystemMarketplace == null)
            {
                return View();
            }


            var marketPlaceType = integrationSystemMarketplace.Parameters.Where(m => m.Key == "MarketplaceType" & m.IntegrationSystemId == integrationSystemMarketplaceId).FirstOrDefault();
            if (marketPlaceType == null)
            {
                return NotFound();
            }
            switch (marketPlaceType.Value)
            {
                case "Trendyol":

                    var apiUrl = integrationSystemMarketplace.Parameters.Where(m => m.Key == "ApiUrl" & m.IntegrationSystemId == integrationSystemMarketplaceId).FirstOrDefault();
                    var apiUser = integrationSystemMarketplace.Parameters.Where(m => m.Key == "ApiUser" & m.IntegrationSystemId == integrationSystemMarketplaceId).FirstOrDefault();
                    var apiPassword = integrationSystemMarketplace.Parameters.Where(m => m.Key == "ApiPassword" & m.IntegrationSystemId == integrationSystemMarketplaceId).FirstOrDefault();

                    TrendyolMarketplaceSettingsViewModel model = new TrendyolMarketplaceSettingsViewModel();
                    model.IntegrationSystemId = integrationSystemMarketplaceId;
                    model.CommerceType = marketPlaceType.Value;
                    model.ApiUrl = apiUrl?.Value;
                    model.ApiUser = apiUser?.Value;
                    model.ApiPassword = apiPassword?.Value;

                    return View($"Marketplace.Trendyol", model);
            }
            return NotFound();
        }


        [HttpPost]
        public async Task<IActionResult> MarketplaceParameterTrendyol(TrendyolMarketplaceSettingsViewModel model)
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


            return RedirectToAction("marketplace");
        }

        [HttpPost]
        public async Task<IActionResult> MarketplaceDelete([FromBody] int integrationSystemId)
        {
            var isSuccess = await _integrationSystemService.DeleteAsync(integrationSystemId);
            if (isSuccess)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Silinecek Pazaryeri Bulunamadı" });
        }
        #endregion


        #region E-Fatura Entegrasyonları

        [HttpGet]
        public async Task<IActionResult> EInvoice()
        {
            var integrationSystemEinvoice = await _integrationSystemService.GetByTypeIdAsync((int)IntegrationSystemType.EInvoice);
            if (integrationSystemEinvoice == null)
            {
                return View();
            }


            var commerceType = integrationSystemEinvoice.Parameters.Where(m => m.Key == "EinvoiceType").FirstOrDefault();
            if (commerceType == null)
            {
                return NotFound();
            }

            var myEinvoice = await _integrationSystemService.GetAllAsync();
            ViewBag.MyEinvoice = myEinvoice.Where(x => x.IntegrationSystemTypeId == 5).
                Select(m => new IntegrationSystemViewModel
                {
                    Id = m.Id,
                    Description = m.Description,
                    IntegrationSystemTypeId = m.IntegrationSystemTypeId,
                    Name = m.Name
                });


            return View(integrationSystemEinvoice);
        }


        [HttpPost]
        public async Task<IActionResult> EInvoice(CreateIntegrationSystemViewModel model)
        {
            var createIntegrationSystem = new CreateIntegrationSystemDto
            {
                Name = model.ModalName,
                IntegrationSystemTypeId = (int)IntegrationSystemType.EInvoice,
                Description = model.ModalDescription
            };

            var integrationSystemId = await _integrationSystemService.AddAsync(createIntegrationSystem);

            // Ana parametre
            await _integrationSystemParameterService.AddAsync(new CreateIntegrationSystemParameterDto
            {
                IntegrationSystemId = integrationSystemId,
                Key = "EinvoiceType",
                Value = model.IntegrationSystemType
            });


            return Json(new { success = true });
        }

        public async Task<IActionResult> EInvoiceSettings(int integrationSystemEinvoiceId)
        {

            var integrationSystemEinvoice = await _integrationSystemService.GetByIdAsync(integrationSystemEinvoiceId);
            if (integrationSystemEinvoice == null)
            {
                return View();
            }


            var einvoiceType = integrationSystemEinvoice.Parameters.Where(m => m.Key == "EinvoiceType" & m.IntegrationSystemId == integrationSystemEinvoiceId).FirstOrDefault();
            if (einvoiceType == null)
            {
                return NotFound();
            }
            switch (einvoiceType.Value)
            {
                case "TrendyolEfaturam":

                    var apiUrl = integrationSystemEinvoice.Parameters.Where(m => m.Key == "ApiUrl" & m.IntegrationSystemId == integrationSystemEinvoiceId).FirstOrDefault();
                    var apiUser = integrationSystemEinvoice.Parameters.Where(m => m.Key == "ApiUser" & m.IntegrationSystemId == integrationSystemEinvoiceId).FirstOrDefault();
                    var apiPassword = integrationSystemEinvoice.Parameters.Where(m => m.Key == "ApiPassword" & m.IntegrationSystemId == integrationSystemEinvoiceId).FirstOrDefault();

                    TrendyolEFaturamSettingsViewModel model = new TrendyolEFaturamSettingsViewModel();
                    model.IntegrationSystemId = integrationSystemEinvoiceId;
                    model.CommerceType = einvoiceType.Value;
                    model.ApiUrl = apiUrl?.Value;
                    model.ApiUser = apiUser?.Value;
                    model.ApiPassword = apiPassword?.Value;

                    return View($"EInvoice.TrendyolEfaturam", model);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CommerceParameterTrendyolEFaturam(SmartstoreCommerceSettingsViewModel model)
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


            return RedirectToAction("einvoice");
        }

        [HttpPost]
        public async Task<IActionResult> EInvoiceDelete([FromBody] int integrationSystemId)
        {
            var isSuccess = await _integrationSystemService.DeleteAsync(integrationSystemId);
            if (isSuccess)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Silinecek E-Fatura Bulunamadı" });
        }
        #endregion

    }
}
