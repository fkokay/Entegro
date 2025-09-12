using Entegro.Application.DTOs.IntegrationSystem;
using Entegro.Application.DTOs.IntegrationSystemParameter;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Enums;
using Entegro.Web.Models.Integration;
using Entegro.Web.Models.Integration.Cargo;
using Entegro.Web.Models.Integration.Commerce;
using Entegro.Web.Models.Integration.EInvoice;
using Entegro.Web.Models.Integration.Erp;
using Entegro.Web.Models.Integration.Marketplace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    [Authorize]
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
        [HttpGet]
        public async Task<IActionResult> Erp()
        {
            var allErpIntegrationSystem = await _integrationSystemService.GetAllAsync((int)IntegrationSystemType.ERP);

            var model = new ErpListViewModel();
            model.ErpList = allErpIntegrationSystem.Select(m => new ErpIntegrationSystemViewModel
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                IntegrationSystemTypeId = m.IntegrationSystemTypeId,
                IntegrationSystemType = m.IntegrationSystemType,
                IntegrationSystemTypeLabelHint = m.IntegrationSystemTypeLabelHint,
                ErpType = m.IntegrationSystemParameters.Where(p => p.Key == "ErpType").Select(p => p.Value).FirstOrDefault() ?? ""
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Erp(ErpIntegrationSystemViewModel model)
        {
            var createIntegrationSystem = new CreateIntegrationSystemDto();
            createIntegrationSystem.Name = model.Name;
            createIntegrationSystem.IntegrationSystemTypeId = (int)IntegrationSystemType.ERP;
            createIntegrationSystem.Description = model.Description;

            var integrationSystemId = await _integrationSystemService.AddAsync(createIntegrationSystem);

            var erpTypeIntegrationSystemParameter = new CreateIntegrationSystemParameterDto();
            erpTypeIntegrationSystemParameter.IntegrationSystemId = integrationSystemId;
            erpTypeIntegrationSystemParameter.Key = "ErpType";
            erpTypeIntegrationSystemParameter.Value = model.ErpType ?? "";

            await _integrationSystemParameterService.AddAsync(erpTypeIntegrationSystemParameter);


            return Json(new { success = true });
        }

        public async Task<IActionResult> ErpSettings(int integrationSystemId)
        {

            var integrationSystemErp = await _integrationSystemService.GetByIdAsync(integrationSystemId);
            if (integrationSystemErp == null)
            {
                return View();
            }


            var erpType = integrationSystemErp.IntegrationSystemParameters.Where(m => m.Key == "ErpType" & m.IntegrationSystemId == integrationSystemId).FirstOrDefault();
            if (erpType == null)
            {
                return NotFound();
            }


            switch (erpType.Value)
            {
                case "Logo":
                    var id = integrationSystemErp.Id;
                    var name = integrationSystemErp.Name;
                    var description = integrationSystemErp.Description;
                    var apiUrl = integrationSystemErp.IntegrationSystemParameters.Where(m => m.Key == "ApiUrl").FirstOrDefault();
                    var apiUser = integrationSystemErp.IntegrationSystemParameters.Where(m => m.Key == "ApiUser").FirstOrDefault();
                    var apiPassword = integrationSystemErp.IntegrationSystemParameters.Where(m => m.Key == "ApiPassword").FirstOrDefault();

                    LogoErpSettingsViewModel model = new LogoErpSettingsViewModel();
                    model.Id = id;
                    model.Name = name;
                    model.Description = description;
                    model.IntegrationSystemTypeId = integrationSystemErp.IntegrationSystemTypeId;
                    model.IntegrationSystemId = integrationSystemErp.Id;
                    model.ErpType = erpType.Value;
                    model.ApiUrl = apiUrl?.Value;
                    model.ApiUser = apiUser?.Value;
                    model.ApiPassword = apiPassword?.Value;


                    return View($"Erp.Logo", model);
                case "Netsis":
                    var idForNetsis = integrationSystemErp.Id;
                    var nameForNetsis = integrationSystemErp.Name;
                    var descriptionForNetsis = integrationSystemErp.Description;
                    var apiUrlForNetsis = integrationSystemErp.IntegrationSystemParameters.Where(m => m.Key == "ApiUrl").FirstOrDefault();
                    var apiUserForNetsis = integrationSystemErp.IntegrationSystemParameters.Where(m => m.Key == "ApiUser").FirstOrDefault();
                    var apiPasswordForNetsis = integrationSystemErp.IntegrationSystemParameters.Where(m => m.Key == "ApiPassword").FirstOrDefault();

                    NetsisErpSettingsViewModel modelForNetsis = new NetsisErpSettingsViewModel();
                    modelForNetsis.Id = idForNetsis;
                    modelForNetsis.Name = nameForNetsis;
                    modelForNetsis.Description = descriptionForNetsis;
                    modelForNetsis.IntegrationSystemTypeId = integrationSystemErp.IntegrationSystemTypeId;
                    modelForNetsis.IntegrationSystemId = integrationSystemErp.Id;
                    modelForNetsis.ErpType = erpType.Value;
                    modelForNetsis.ApiUrl = apiUrlForNetsis?.Value;
                    modelForNetsis.ApiUser = apiUserForNetsis?.Value;
                    modelForNetsis.ApiPassword = apiPasswordForNetsis?.Value;
                    return View($"Erp.Netsis", modelForNetsis);
                case "Opak":
                    var idForOpak = integrationSystemErp.Id;
                    var nameForOpak = integrationSystemErp.Name;
                    var descriptionForOpak = integrationSystemErp.Description;
                    var apiUrlForOpak = integrationSystemErp.IntegrationSystemParameters.Where(m => m.Key == "ApiUrl").FirstOrDefault();
                    var apiUserForOpak = integrationSystemErp.IntegrationSystemParameters.Where(m => m.Key == "ApiUser").FirstOrDefault();
                    var apiPasswordForOpak = integrationSystemErp.IntegrationSystemParameters.Where(m => m.Key == "ApiPassword").FirstOrDefault();

                    OpakErpSettingsViewModel modelForOpak = new OpakErpSettingsViewModel();
                    modelForOpak.Id = idForOpak;
                    modelForOpak.Name = nameForOpak;
                    modelForOpak.Description = descriptionForOpak;
                    modelForOpak.IntegrationSystemTypeId = integrationSystemErp.IntegrationSystemTypeId;
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
        public async Task<IActionResult> ErpParameterLogo(LogoErpSettingsViewModel model)
        {
            var apiUrl = await _integrationSystemParameterService.GetByKeyAsync("ApiUrl", model.IntegrationSystemId);

            //mağaza bilgileri güncelle
            await _integrationSystemService.UpdateAsync(new UpdateIntegrationSystemDto
            {
                Id = model.Id,
                Description = model.Description,
                IntegrationSystemTypeId = model.IntegrationSystemTypeId,
                Name = model.Name
            });

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


            return RedirectToAction("erp");
        }

        [HttpPost]
        public async Task<IActionResult> ErpParameterNetsis(NetsisErpSettingsViewModel model)
        {
            //mağaza bilgileri güncelle
            await _integrationSystemService.UpdateAsync(new UpdateIntegrationSystemDto
            {
                Id = model.Id,
                Description = model.Description,
                IntegrationSystemTypeId = model.IntegrationSystemTypeId,
                Name = model.Name
            });

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


            return RedirectToAction("erp");
        }

        [HttpPost]
        public async Task<IActionResult> ErpParameterOpak(OpakErpSettingsViewModel model)
        {
            //mağaza bilgileri güncelle
            await _integrationSystemService.UpdateAsync(new UpdateIntegrationSystemDto
            {
                Id = model.Id,
                Description = model.Description,
                IntegrationSystemTypeId = model.IntegrationSystemTypeId,
                Name = model.Name
            });
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


            return RedirectToAction("erp");
        }

        [HttpPost]
        public async Task<IActionResult> ErpDelete([FromBody] int integrationSystemId)
        {
            var isSuccess = await _integrationSystemService.DeleteAsync(integrationSystemId);
            if (isSuccess)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Silinecek Erp Bulunamadı" });
        }
        #endregion

        #region E-Ticaret Entegrasyonları 
        [HttpGet]
        public async Task<IActionResult> Commerce()
        {
            var allErpIntegrationSystem = await _integrationSystemService.GetAllAsync((int)IntegrationSystemType.Commerce);

            var model = new CommerceListViewModel();
            model.CommerceList = allErpIntegrationSystem.Select(m => new CommerceIntegrationSystemViewModel
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                IntegrationSystemTypeId = m.IntegrationSystemTypeId,
                IntegrationSystemType = m.IntegrationSystemType,
                IntegrationSystemTypeLabelHint = m.IntegrationSystemTypeLabelHint,
                CommerceType = m.IntegrationSystemParameters.Where(p => p.Key == "CommerceType").Select(p => p.Value).FirstOrDefault() ?? ""
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Commerce(CommerceIntegrationSystemViewModel model)
        {
            var createIntegrationSystem = new CreateIntegrationSystemDto();
            createIntegrationSystem.Name = model.Name;
            createIntegrationSystem.IntegrationSystemTypeId = (int)IntegrationSystemType.Commerce;
            createIntegrationSystem.Description = model.Description;

            var integrationSystemId = await _integrationSystemService.AddAsync(createIntegrationSystem);

            var commerceTypeIntegrationSystemParameter = new CreateIntegrationSystemParameterDto();
            commerceTypeIntegrationSystemParameter.IntegrationSystemId = integrationSystemId;
            commerceTypeIntegrationSystemParameter.Key = "CommerceType";
            commerceTypeIntegrationSystemParameter.Value = model.CommerceType;

            await _integrationSystemParameterService.AddAsync(commerceTypeIntegrationSystemParameter);

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> CommerceSettings(int integrationSystemId)
        {

            var integrationSystemCommerce = await _integrationSystemService.GetByIdAsync(integrationSystemId);
            if (integrationSystemCommerce == null)
            {
                return View();
            }


            var commerceType = integrationSystemCommerce.IntegrationSystemParameters.Where(m => m.Key == "CommerceType" & m.IntegrationSystemId == integrationSystemId).FirstOrDefault();
            if (commerceType == null)
            {
                return NotFound();
            }
            switch (commerceType.Value)
            {
                case "Smartstore":
                    var id = integrationSystemCommerce.Id;
                    var name = integrationSystemCommerce.Name;
                    var description = integrationSystemCommerce.Description;
                    var apiUrl = integrationSystemCommerce.IntegrationSystemParameters.Where(m => m.Key == "ApiUrl" & m.IntegrationSystemId == integrationSystemId).FirstOrDefault();
                    var apiUser = integrationSystemCommerce.IntegrationSystemParameters.Where(m => m.Key == "ApiUser" & m.IntegrationSystemId == integrationSystemId).FirstOrDefault();
                    var apiPassword = integrationSystemCommerce.IntegrationSystemParameters.Where(m => m.Key == "ApiPassword" & m.IntegrationSystemId == integrationSystemId).FirstOrDefault();

                    SmartstoreCommerceSettingsViewModel model = new SmartstoreCommerceSettingsViewModel();
                    model.Id = id;
                    model.Name = name;
                    model.Description = description;
                    model.IntegrationSystemTypeId = integrationSystemCommerce.IntegrationSystemTypeId;
                    model.IntegrationSystemId = integrationSystemId;
                    model.CommerceType = commerceType.Value;
                    model.ApiUrl = apiUrl?.Value;
                    model.ApiUser = apiUser?.Value;
                    model.ApiPassword = apiPassword?.Value;

                    return View($"Commerce.Smartstore", model);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CommerceParameterSmartstore(SmartstoreCommerceSettingsViewModel model)
        {
            var apiUrl = await _integrationSystemParameterService.GetByKeyAsync("ApiUrl", model.IntegrationSystemId);

            //mağaza bilgileri güncelle
            await _integrationSystemService.UpdateAsync(new UpdateIntegrationSystemDto
            {
                Id = model.Id,
                Description = model.Description,
                IntegrationSystemTypeId = model.IntegrationSystemTypeId,
                Name = model.Name
            });

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

            return RedirectToAction("Commerce");
        }

        [HttpPost]
        public async Task<IActionResult> CommerceDelete([FromBody] int integrationSystemId)
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
            var allErpIntegrationSystem = await _integrationSystemService.GetAllAsync((int)IntegrationSystemType.Cargo);

            var model = new CargoListViewModel();
            model.CargoList = allErpIntegrationSystem.Select(m => new CargoIntegrationSystemViewModel
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                IntegrationSystemTypeId = m.IntegrationSystemTypeId,
                IntegrationSystemType = m.IntegrationSystemType,
                IntegrationSystemTypeLabelHint = m.IntegrationSystemTypeLabelHint,
                CargoType = m.IntegrationSystemParameters.Where(p => p.Key == "CargoType").Select(p => p.Value).FirstOrDefault() ?? ""
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Cargo(CargoIntegrationSystemViewModel model)
        {
            var createIntegrationSystem = new CreateIntegrationSystemDto();
            createIntegrationSystem.Name = model.Name;
            createIntegrationSystem.IntegrationSystemTypeId = (int)IntegrationSystemType.Cargo;
            createIntegrationSystem.Description = model.Description;

            var integrationSystemId = await _integrationSystemService.AddAsync(createIntegrationSystem);

            var cargoTypeIntegrationSystemParameter = new CreateIntegrationSystemParameterDto();
            cargoTypeIntegrationSystemParameter.IntegrationSystemId = integrationSystemId;
            cargoTypeIntegrationSystemParameter.Key = "CargoType";
            cargoTypeIntegrationSystemParameter.Value = model.CargoType;

            await _integrationSystemParameterService.AddAsync(cargoTypeIntegrationSystemParameter);


            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> CargoSettings(int integrationSystemId)
        {

            var integrationSystemCargo = await _integrationSystemService.GetByIdAsync(integrationSystemId);
            if (integrationSystemCargo == null)
            {
                return View();
            }


            var cargoType = integrationSystemCargo.IntegrationSystemParameters.Where(m => m.Key == "CargoType" & m.IntegrationSystemId == integrationSystemId).FirstOrDefault();
            if (cargoType == null)
            {
                return NotFound();
            }
            switch (cargoType.Value)
            {
                case "Yurtici":
                    var id = integrationSystemCargo.Id;
                    var name = integrationSystemCargo.Name;
                    var description = integrationSystemCargo.Description;
                    var apiUrl = integrationSystemCargo.IntegrationSystemParameters.Where(m => m.Key == "ApiUrl" & m.IntegrationSystemId == integrationSystemId).FirstOrDefault();
                    var apiUser = integrationSystemCargo.IntegrationSystemParameters.Where(m => m.Key == "ApiUser" & m.IntegrationSystemId == integrationSystemId).FirstOrDefault();
                    var apiPassword = integrationSystemCargo.IntegrationSystemParameters.Where(m => m.Key == "ApiPassword" & m.IntegrationSystemId == integrationSystemId).FirstOrDefault();

                    YurticiCargoSettingsViewModel yurticiCargoSettings = new YurticiCargoSettingsViewModel();
                    yurticiCargoSettings.Id = id;
                    yurticiCargoSettings.Name = name;
                    yurticiCargoSettings.Description = description;
                    yurticiCargoSettings.IntegrationSystemTypeId = integrationSystemCargo.IntegrationSystemTypeId;
                    yurticiCargoSettings.IntegrationSystemId = integrationSystemCargo.Id;
                    yurticiCargoSettings.CargoType = cargoType.Value;
                    yurticiCargoSettings.ApiUrl = apiUrl?.Value;
                    yurticiCargoSettings.ApiUser = apiUser?.Value;
                    yurticiCargoSettings.ApiPassword = apiPassword?.Value;

                    return View($"Cargo.Yurtici", yurticiCargoSettings);

                case "PTT":
                    var idForPTT = integrationSystemCargo.Id;
                    var nameForPTT = integrationSystemCargo.Name;
                    var descriptionForPTT = integrationSystemCargo.Description;
                    var musteriId = integrationSystemCargo.IntegrationSystemParameters.Where(m => m.Key == "MusteriId" & m.IntegrationSystemId == integrationSystemId).FirstOrDefault();
                    var password = integrationSystemCargo.IntegrationSystemParameters.Where(m => m.Key == "Password" & m.IntegrationSystemId == integrationSystemId).FirstOrDefault();
                    var barkodStartPrefix = integrationSystemCargo.IntegrationSystemParameters.Where(m => m.Key == "BarkodStartPrefix" & m.IntegrationSystemId == integrationSystemId).FirstOrDefault();
                    var barkodEndPrefix = integrationSystemCargo.IntegrationSystemParameters.Where(m => m.Key == "BarkodEndPrefix" & m.IntegrationSystemId == integrationSystemId).FirstOrDefault();

                    PTTCargoSettingsViewModel pTTCargoSettings = new PTTCargoSettingsViewModel();
                    pTTCargoSettings.Id = idForPTT;
                    pTTCargoSettings.Name = nameForPTT;
                    pTTCargoSettings.Description = descriptionForPTT;
                    pTTCargoSettings.IntegrationSystemTypeId = integrationSystemCargo.IntegrationSystemTypeId;
                    pTTCargoSettings.IntegrationSystemId = integrationSystemCargo.Id;
                    pTTCargoSettings.CargoType = cargoType.Value;
                    pTTCargoSettings.MusteriId = musteriId?.Value;
                    pTTCargoSettings.Password = password?.Value;
                    pTTCargoSettings.BarkodStartPrefix = barkodStartPrefix?.Value;
                    pTTCargoSettings.BarkodEndPrefix = barkodEndPrefix?.Value;

                    return View($"Cargo.PTT", pTTCargoSettings);

                case "Aras":
                    var username = integrationSystemCargo.IntegrationSystemParameters.Where(m => m.Key == "Username" & m.IntegrationSystemId == integrationSystemId).FirstOrDefault();
                    var passwordAras = integrationSystemCargo.IntegrationSystemParameters.Where(m => m.Key == "Password" & m.IntegrationSystemId == integrationSystemId).FirstOrDefault();
                    var idForAras = integrationSystemCargo.Id;
                    var nameForAras = integrationSystemCargo.Name;
                    var descriptionForAras = integrationSystemCargo.Description;

                    ArasCargoSettingsViewModel arasCargoSettings = new ArasCargoSettingsViewModel();
                    arasCargoSettings.Id = idForAras;
                    arasCargoSettings.Name = nameForAras;
                    arasCargoSettings.Description = descriptionForAras;
                    arasCargoSettings.IntegrationSystemTypeId = integrationSystemCargo.IntegrationSystemTypeId;
                    arasCargoSettings.IntegrationSystemId = integrationSystemId;
                    arasCargoSettings.CargoType = cargoType.Value;
                    arasCargoSettings.Username = username?.Value;
                    arasCargoSettings.Password = passwordAras?.Value;

                    return View($"Cargo.Aras", arasCargoSettings);

            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CargoParameterYurtici(YurticiCargoSettingsViewModel model)
        {
            //mağaza bilgileri güncelle
            await _integrationSystemService.UpdateAsync(new UpdateIntegrationSystemDto
            {
                Id = model.Id,
                Description = model.Description,
                IntegrationSystemTypeId = model.IntegrationSystemTypeId,
                Name = model.Name
            });
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
            //mağaza bilgileri güncelle
            await _integrationSystemService.UpdateAsync(new UpdateIntegrationSystemDto
            {
                Id = model.Id,
                Description = model.Description,
                IntegrationSystemTypeId = model.IntegrationSystemTypeId,
                Name = model.Name
            });
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
            //mağaza bilgileri güncelle
            await _integrationSystemService.UpdateAsync(new UpdateIntegrationSystemDto
            {
                Id = model.Id,
                Description = model.Description,
                IntegrationSystemTypeId = model.IntegrationSystemTypeId,
                Name = model.Name
            });
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
            var allErpIntegrationSystem = await _integrationSystemService.GetAllAsync((int)IntegrationSystemType.Marketplace);

            var model = new MarketplaceListViewModel();
            model.MarketplaceList = allErpIntegrationSystem.Select(m => new MarketplaceIntegrationSystemViewModel
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                IntegrationSystemTypeId = m.IntegrationSystemTypeId,
                IntegrationSystemType = m.IntegrationSystemType,
                IntegrationSystemTypeLabelHint = m.IntegrationSystemTypeLabelHint,
                MarketplaceType = m.IntegrationSystemParameters.Where(p => p.Key == "MarketplaceType").Select(p => p.Value).FirstOrDefault() ?? ""
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult MarketplaceTest(int? IntegrationSystemId, string? MarketplaceType)
        {
            var result = true;

            if (result)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "Bağlantı başarısız." });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Marketplace(MarketplaceIntegrationSystemViewModel model)
        {
            var createIntegrationSystem = new CreateIntegrationSystemDto();
            createIntegrationSystem.Name = model.Name;
            createIntegrationSystem.IntegrationSystemTypeId = (int)IntegrationSystemType.Marketplace;
            createIntegrationSystem.Description = model.Description;

            var integrationSystemId = await _integrationSystemService.AddAsync(createIntegrationSystem);

            var marketplaceTypeIntegrationSystemParameter = new CreateIntegrationSystemParameterDto();
            marketplaceTypeIntegrationSystemParameter.IntegrationSystemId = integrationSystemId;
            marketplaceTypeIntegrationSystemParameter.Key = "MarketplaceType";
            marketplaceTypeIntegrationSystemParameter.Value = model.MarketplaceType;

            await _integrationSystemParameterService.AddAsync(marketplaceTypeIntegrationSystemParameter);


            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> MarketplaceSettings(int integrationSystemId)
        {

            var integrationSystemMarketplace = await _integrationSystemService.GetByIdAsync(integrationSystemId);
            if (integrationSystemMarketplace == null)
            {
                return View();
            }

            var marketPlaceType = integrationSystemMarketplace.IntegrationSystemParameters.Where(m => m.Key == "MarketplaceType" & m.IntegrationSystemId == integrationSystemId).FirstOrDefault();
            if (marketPlaceType == null)
            {
                return NotFound();
            }
            switch (marketPlaceType.Value)
            {
                case "Trendyol":
                    return TrendyolMarketplaceSettings(integrationSystemMarketplace, marketPlaceType.Value);
                case "N11":
                    return N11MarketplaceSettings(integrationSystemMarketplace, marketPlaceType.Value);
                case "Pazarama":
                    return PazaramaMarketplaceSettings(integrationSystemMarketplace, marketPlaceType.Value);
                case "Idefix":
                    return IdefixMarketplaceSettings(integrationSystemMarketplace, marketPlaceType.Value);
                case "CicekSepeti":
                    return CicekSepetiMarketplaceSettings(integrationSystemMarketplace, marketPlaceType.Value);
                case "Hepsiburada":
                    return HepsiburadaMarketplaceSettings(integrationSystemMarketplace, marketPlaceType.Value);
            }
            return NotFound();
        }

        private IActionResult TrendyolMarketplaceSettings(IntegrationSystemDto integrationSystemMarketplace, string marketPlaceType)
        {
            var id = integrationSystemMarketplace.Id;
            var name = integrationSystemMarketplace.Name;
            var description = integrationSystemMarketplace.Description;
            var apiUser = integrationSystemMarketplace.IntegrationSystemParameters.Where(m => m.Key == "ApiUser" & m.IntegrationSystemId == integrationSystemMarketplace.Id).FirstOrDefault();
            var apiPassword = integrationSystemMarketplace.IntegrationSystemParameters.Where(m => m.Key == "ApiPassword" & m.IntegrationSystemId == integrationSystemMarketplace.Id).FirstOrDefault();
            var supplierId = integrationSystemMarketplace.IntegrationSystemParameters.Where(m => m.Key == "SupplierId" & m.IntegrationSystemId == integrationSystemMarketplace.Id).FirstOrDefault();

            TrendyolMarketplaceSettingsViewModel model = new TrendyolMarketplaceSettingsViewModel();
            model.Id = id;
            model.Name = name;
            model.Description = description;
            model.IntegrationSystemTypeId = integrationSystemMarketplace.IntegrationSystemTypeId;
            model.IntegrationSystemId = integrationSystemMarketplace.Id;
            model.MarketplaceType = marketPlaceType;
            model.ApiUser = apiUser?.Value;
            model.ApiPassword = apiPassword?.Value;
            model.SupplierId = supplierId?.Value;

            return View($"Marketplace.{marketPlaceType}", model);
        }
        private IActionResult N11MarketplaceSettings(IntegrationSystemDto integrationSystemMarketplace, string marketPlaceType)
        {
            var id = integrationSystemMarketplace.Id;
            var name = integrationSystemMarketplace.Name;
            var description = integrationSystemMarketplace.Description;
            var appSecret = integrationSystemMarketplace.IntegrationSystemParameters.Where(m => m.Key == "AppSecret" & m.IntegrationSystemId == integrationSystemMarketplace.Id).FirstOrDefault();
            var appKey = integrationSystemMarketplace.IntegrationSystemParameters.Where(m => m.Key == "AppKey" & m.IntegrationSystemId == integrationSystemMarketplace.Id).FirstOrDefault();

            N11MarketplaceSettingsViewModel model = new N11MarketplaceSettingsViewModel();
            model.Id = id;
            model.Name = name;
            model.Description = description;
            model.IntegrationSystemTypeId = integrationSystemMarketplace.IntegrationSystemTypeId;
            model.IntegrationSystemId = integrationSystemMarketplace.Id;
            model.MarketplaceType = marketPlaceType;
            model.AppSecret = appSecret?.Value;
            model.AppKey = appKey?.Value;
            return View($"Marketplace.{marketPlaceType}", model);
        }
        private IActionResult PazaramaMarketplaceSettings(IntegrationSystemDto integrationSystemMarketplace, string marketPlaceType)
        {
            var id = integrationSystemMarketplace.Id;
            var name = integrationSystemMarketplace.Name;
            var description = integrationSystemMarketplace.Description;
            var clientId = integrationSystemMarketplace.IntegrationSystemParameters.Where(m => m.Key == "ClientId" & m.IntegrationSystemId == integrationSystemMarketplace.Id).FirstOrDefault();
            var clientSecret = integrationSystemMarketplace.IntegrationSystemParameters.Where(m => m.Key == "ClientSecret" & m.IntegrationSystemId == integrationSystemMarketplace.Id).FirstOrDefault();

            PazaramaMarketplaceSettingsViewModel model = new PazaramaMarketplaceSettingsViewModel();
            model.Id = id;
            model.Name = name;
            model.Description = description;
            model.IntegrationSystemTypeId = integrationSystemMarketplace.IntegrationSystemTypeId;
            model.IntegrationSystemId = integrationSystemMarketplace.Id;
            model.MarketplaceType = marketPlaceType;
            model.ClientId = clientId?.Value;
            model.ClientSecret = clientSecret?.Value;

            return View($"Marketplace.{marketPlaceType}", model);
        }
        private IActionResult IdefixMarketplaceSettings(IntegrationSystemDto integrationSystemMarketplace, string marketPlaceType)
        {
            var id = integrationSystemMarketplace.Id;
            var name = integrationSystemMarketplace.Name;
            var description = integrationSystemMarketplace.Description;
            var apiUser = integrationSystemMarketplace.IntegrationSystemParameters.Where(m => m.Key == "ApiUser" & m.IntegrationSystemId == integrationSystemMarketplace.Id).FirstOrDefault();
            var supplierId = integrationSystemMarketplace.IntegrationSystemParameters.Where(m => m.Key == "SupplierId" & m.IntegrationSystemId == integrationSystemMarketplace.Id).FirstOrDefault();

            IdefixMarketplaceSettingsViewModel model = new IdefixMarketplaceSettingsViewModel();
            model.Id = id;
            model.Name = name;
            model.Description = description;
            model.IntegrationSystemTypeId = integrationSystemMarketplace.IntegrationSystemTypeId;
            model.IntegrationSystemId = integrationSystemMarketplace.Id;
            model.MarketplaceType = marketPlaceType;
            model.ApiUser = apiUser?.Value;
            model.SupplierId = supplierId?.Value;

            return View($"Marketplace.{marketPlaceType}", model);
        }
        private IActionResult CicekSepetiMarketplaceSettings(IntegrationSystemDto integrationSystemMarketplace, string marketPlaceType)
        {
            var id = integrationSystemMarketplace.Id;
            var name = integrationSystemMarketplace.Name;
            var description = integrationSystemMarketplace.Description;
            var apiUser = integrationSystemMarketplace.IntegrationSystemParameters.Where(m => m.Key == "ApiUser" & m.IntegrationSystemId == integrationSystemMarketplace.Id).FirstOrDefault();
            var supplierId = integrationSystemMarketplace.IntegrationSystemParameters.Where(m => m.Key == "SupplierId" & m.IntegrationSystemId == integrationSystemMarketplace.Id).FirstOrDefault();

            CicekSepetiMarketplaceSettingsViewModel model = new CicekSepetiMarketplaceSettingsViewModel();
            model.Id = id;
            model.Name = name;
            model.Description = description;
            model.IntegrationSystemTypeId = integrationSystemMarketplace.IntegrationSystemTypeId;
            model.IntegrationSystemId = integrationSystemMarketplace.Id;
            model.MarketplaceType = marketPlaceType;
            model.ApiUser = apiUser?.Value;
            model.SupplierId = supplierId?.Value;

            return View($"Marketplace.{marketPlaceType}", model);
        }
        private IActionResult HepsiburadaMarketplaceSettings(IntegrationSystemDto integrationSystemMarketplace, string marketPlaceType)
        {
            var id = integrationSystemMarketplace.Id;
            var name = integrationSystemMarketplace.Name;
            var description = integrationSystemMarketplace.Description;
            var apiUser = integrationSystemMarketplace.IntegrationSystemParameters.Where(m => m.Key == "ApiUser" & m.IntegrationSystemId == integrationSystemMarketplace.Id).FirstOrDefault();
            var apiPassword = integrationSystemMarketplace.IntegrationSystemParameters.Where(m => m.Key == "ApiPassword" & m.IntegrationSystemId == integrationSystemMarketplace.Id).FirstOrDefault();
            var merchantId = integrationSystemMarketplace.IntegrationSystemParameters.Where(m => m.Key == "MerchantId" & m.IntegrationSystemId == integrationSystemMarketplace.Id).FirstOrDefault();
            var userAgent = integrationSystemMarketplace.IntegrationSystemParameters.Where(m => m.Key == "UserAgent" & m.IntegrationSystemId == integrationSystemMarketplace.Id).FirstOrDefault();

            HepsiburadaMarketplaceSettingsViewModel model = new HepsiburadaMarketplaceSettingsViewModel();
            model.Id = id;
            model.Name = name;
            model.Description = description;
            model.IntegrationSystemTypeId = integrationSystemMarketplace.IntegrationSystemTypeId;
            model.IntegrationSystemId = integrationSystemMarketplace.Id;
            model.MarketplaceType = marketPlaceType;
            model.ApiUser = apiUser?.Value;
            model.ApiPassword = apiPassword?.Value;
            model.MerchantId = merchantId?.Value;
            model.UserAgent = userAgent?.Value;

            return View($"Marketplace.{marketPlaceType}", model);
        }

        [HttpPost]
        public async Task<IActionResult> MarketplaceParameterTrendyol(TrendyolMarketplaceSettingsViewModel model)
        {
            //mağaza bilgileri güncelle
            await _integrationSystemService.UpdateAsync(new UpdateIntegrationSystemDto
            {
                Id = model.Id,
                Description = model.Description,
                IntegrationSystemTypeId = model.IntegrationSystemTypeId,
                Name = model.Name
            });

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

            var supplierId = await _integrationSystemParameterService.GetByKeyAsync("SupplierId", model.IntegrationSystemId);
            if (supplierId == null)
            {
                CreateIntegrationSystemParameterDto createIntegrationSystemParameter = new CreateIntegrationSystemParameterDto();
                createIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                createIntegrationSystemParameter.Key = "SupplierId";
                createIntegrationSystemParameter.Value = model.SupplierId;

                await _integrationSystemParameterService.AddAsync(createIntegrationSystemParameter);
            }
            else
            {
                UpdateIntegrationSystemParameterDto updateIntegrationSystemParameter = new UpdateIntegrationSystemParameterDto();
                updateIntegrationSystemParameter.Id = supplierId.Id;
                updateIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                updateIntegrationSystemParameter.Key = "SupplierId";
                updateIntegrationSystemParameter.Value = model.SupplierId;

                await _integrationSystemParameterService.UpdateAsync(updateIntegrationSystemParameter);
            }


            return RedirectToAction("marketplace");
        }
        [HttpPost]
        public async Task<IActionResult> MarketplaceParameterN11(N11MarketplaceSettingsViewModel model)
        {
            //mağaza bilgileri güncelle
            await _integrationSystemService.UpdateAsync(new UpdateIntegrationSystemDto
            {
                Id = model.Id,
                Description = model.Description,
                IntegrationSystemTypeId = model.IntegrationSystemTypeId,
                Name = model.Name
            });

            var appSecret = await _integrationSystemParameterService.GetByKeyAsync("AppSecret", model.IntegrationSystemId);
            if (appSecret == null)
            {
                CreateIntegrationSystemParameterDto createIntegrationSystemParameter = new CreateIntegrationSystemParameterDto();
                createIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                createIntegrationSystemParameter.Key = "AppSecret";
                createIntegrationSystemParameter.Value = model.AppSecret;

                await _integrationSystemParameterService.AddAsync(createIntegrationSystemParameter);
            }
            else
            {
                UpdateIntegrationSystemParameterDto updateIntegrationSystemParameter = new UpdateIntegrationSystemParameterDto();
                updateIntegrationSystemParameter.Id = appSecret.Id;
                updateIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                updateIntegrationSystemParameter.Key = "AppSecret";
                updateIntegrationSystemParameter.Value = model.AppSecret;

                await _integrationSystemParameterService.UpdateAsync(updateIntegrationSystemParameter);
            }

            var appKey = await _integrationSystemParameterService.GetByKeyAsync("AppKey", model.IntegrationSystemId);
            if (appKey == null)
            {
                CreateIntegrationSystemParameterDto createIntegrationSystemParameter = new CreateIntegrationSystemParameterDto();
                createIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                createIntegrationSystemParameter.Key = "AppKey";
                createIntegrationSystemParameter.Value = model.AppKey;

                await _integrationSystemParameterService.AddAsync(createIntegrationSystemParameter);
            }
            else
            {
                UpdateIntegrationSystemParameterDto updateIntegrationSystemParameter = new UpdateIntegrationSystemParameterDto();
                updateIntegrationSystemParameter.Id = appKey.Id;
                updateIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                updateIntegrationSystemParameter.Key = "AppKey";
                updateIntegrationSystemParameter.Value = model.AppKey;

                await _integrationSystemParameterService.UpdateAsync(updateIntegrationSystemParameter);
            }

            return RedirectToAction("marketplace");
        }
        [HttpPost]
        public async Task<IActionResult> MarketplaceParameterPazarama(PazaramaMarketplaceSettingsViewModel model)
        {
            //mağaza bilgileri güncelle
            await _integrationSystemService.UpdateAsync(new UpdateIntegrationSystemDto
            {
                Id = model.Id,
                Description = model.Description,
                IntegrationSystemTypeId = model.IntegrationSystemTypeId,
                Name = model.Name
            });

            var clientId = await _integrationSystemParameterService.GetByKeyAsync("ClientId", model.IntegrationSystemId);
            if (clientId == null)
            {
                CreateIntegrationSystemParameterDto createIntegrationSystemParameter = new CreateIntegrationSystemParameterDto();
                createIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                createIntegrationSystemParameter.Key = "ClientId";
                createIntegrationSystemParameter.Value = model.ClientId;

                await _integrationSystemParameterService.AddAsync(createIntegrationSystemParameter);
            }
            else
            {
                UpdateIntegrationSystemParameterDto updateIntegrationSystemParameter = new UpdateIntegrationSystemParameterDto();
                updateIntegrationSystemParameter.Id = clientId.Id;
                updateIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                updateIntegrationSystemParameter.Key = "ClientId";
                updateIntegrationSystemParameter.Value = model.ClientId;

                await _integrationSystemParameterService.UpdateAsync(updateIntegrationSystemParameter);
            }

            var clientSecret = await _integrationSystemParameterService.GetByKeyAsync("ClientSecret", model.IntegrationSystemId);
            if (clientSecret == null)
            {
                CreateIntegrationSystemParameterDto createIntegrationSystemParameter = new CreateIntegrationSystemParameterDto();
                createIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                createIntegrationSystemParameter.Key = "ClientSecret";
                createIntegrationSystemParameter.Value = model.ClientSecret;

                await _integrationSystemParameterService.AddAsync(createIntegrationSystemParameter);
            }
            else
            {
                UpdateIntegrationSystemParameterDto updateIntegrationSystemParameter = new UpdateIntegrationSystemParameterDto();
                updateIntegrationSystemParameter.Id = clientSecret.Id;
                updateIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                updateIntegrationSystemParameter.Key = "ClientSecret";
                updateIntegrationSystemParameter.Value = model.ClientSecret;

                await _integrationSystemParameterService.UpdateAsync(updateIntegrationSystemParameter);
            }
            return RedirectToAction("marketplace");
        }
        [HttpPost]
        public async Task<IActionResult> MarketplaceParameterIdefix(IdefixMarketplaceSettingsViewModel model)
        {
            //mağaza bilgileri güncelle
            await _integrationSystemService.UpdateAsync(new UpdateIntegrationSystemDto
            {
                Id = model.Id,
                Description = model.Description,
                IntegrationSystemTypeId = model.IntegrationSystemTypeId,
                Name = model.Name
            });

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
            var supplierId = await _integrationSystemParameterService.GetByKeyAsync("SupplierId", model.IntegrationSystemId);
            if (supplierId == null)
            {
                CreateIntegrationSystemParameterDto createIntegrationSystemParameter = new CreateIntegrationSystemParameterDto();
                createIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                createIntegrationSystemParameter.Key = "SupplierId";
                createIntegrationSystemParameter.Value = model.SupplierId;

                await _integrationSystemParameterService.AddAsync(createIntegrationSystemParameter);
            }
            else
            {
                UpdateIntegrationSystemParameterDto updateIntegrationSystemParameter = new UpdateIntegrationSystemParameterDto();
                updateIntegrationSystemParameter.Id = supplierId.Id;
                updateIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                updateIntegrationSystemParameter.Key = "SupplierId";
                updateIntegrationSystemParameter.Value = model.SupplierId;

                await _integrationSystemParameterService.UpdateAsync(updateIntegrationSystemParameter);
            }
            return RedirectToAction("marketplace");
        }
        [HttpPost]
        public async Task<IActionResult> MarketplaceParameterCicekSepeti(CicekSepetiMarketplaceSettingsViewModel model)
        {
            //mağaza bilgileri güncelle
            await _integrationSystemService.UpdateAsync(new UpdateIntegrationSystemDto
            {
                Id = model.Id,
                Description = model.Description,
                IntegrationSystemTypeId = model.IntegrationSystemTypeId,
                Name = model.Name
            });

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
            var supplierId = await _integrationSystemParameterService.GetByKeyAsync("SupplierId", model.IntegrationSystemId);
            if (supplierId == null)
            {
                CreateIntegrationSystemParameterDto createIntegrationSystemParameter = new CreateIntegrationSystemParameterDto();
                createIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                createIntegrationSystemParameter.Key = "SupplierId";
                createIntegrationSystemParameter.Value = model.SupplierId;

                await _integrationSystemParameterService.AddAsync(createIntegrationSystemParameter);
            }
            else
            {
                UpdateIntegrationSystemParameterDto updateIntegrationSystemParameter = new UpdateIntegrationSystemParameterDto();
                updateIntegrationSystemParameter.Id = supplierId.Id;
                updateIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                updateIntegrationSystemParameter.Key = "SupplierId";
                updateIntegrationSystemParameter.Value = model.SupplierId;

                await _integrationSystemParameterService.UpdateAsync(updateIntegrationSystemParameter);
            }
            return RedirectToAction("marketplace");
        }
        [HttpPost]
        public async Task<IActionResult> MarketplaceParameterHepsiburada(HepsiburadaMarketplaceSettingsViewModel model)
        {
            //mağaza bilgileri güncelle
            await _integrationSystemService.UpdateAsync(new UpdateIntegrationSystemDto
            {
                Id = model.Id,
                Description = model.Description,
                IntegrationSystemTypeId = model.IntegrationSystemTypeId,
                Name = model.Name
            });

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
            var merchantId = await _integrationSystemParameterService.GetByKeyAsync("MerchantId", model.IntegrationSystemId);
            if (merchantId == null)
            {
                CreateIntegrationSystemParameterDto createIntegrationSystemParameter = new CreateIntegrationSystemParameterDto();
                createIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                createIntegrationSystemParameter.Key = "MerchantId";
                createIntegrationSystemParameter.Value = model.MerchantId;

                await _integrationSystemParameterService.AddAsync(createIntegrationSystemParameter);
            }
            else
            {
                UpdateIntegrationSystemParameterDto updateIntegrationSystemParameter = new UpdateIntegrationSystemParameterDto();
                updateIntegrationSystemParameter.Id = merchantId.Id;
                updateIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                updateIntegrationSystemParameter.Key = "MerchantId";
                updateIntegrationSystemParameter.Value = model.MerchantId;

                await _integrationSystemParameterService.UpdateAsync(updateIntegrationSystemParameter);
            }
            var userAgent = await _integrationSystemParameterService.GetByKeyAsync("UserAgent", model.IntegrationSystemId);
            if (userAgent == null)
            {
                CreateIntegrationSystemParameterDto createIntegrationSystemParameter = new CreateIntegrationSystemParameterDto();
                createIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                createIntegrationSystemParameter.Key = "UserAgent";
                createIntegrationSystemParameter.Value = model.UserAgent;

                await _integrationSystemParameterService.AddAsync(createIntegrationSystemParameter);
            }
            else
            {
                UpdateIntegrationSystemParameterDto updateIntegrationSystemParameter = new UpdateIntegrationSystemParameterDto();
                updateIntegrationSystemParameter.Id = userAgent.Id;
                updateIntegrationSystemParameter.IntegrationSystemId = model.IntegrationSystemId;
                updateIntegrationSystemParameter.Key = "UserAgent";
                updateIntegrationSystemParameter.Value = model.MerchantId;

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
            var allErpIntegrationSystem = await _integrationSystemService.GetAllAsync((int)IntegrationSystemType.EInvoice);

            var model = new EInvoiceListViewModel();
            model.EInvoiceList = allErpIntegrationSystem.Select(m => new EInvoiceIntegrationSystemViewModel
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                IntegrationSystemTypeId = m.IntegrationSystemTypeId,
                IntegrationSystemType = m.IntegrationSystemType,
                IntegrationSystemTypeLabelHint = m.IntegrationSystemTypeLabelHint,
                EInvoiceType = m.IntegrationSystemParameters.Where(p => p.Key == "EInvoiceType").Select(p => p.Value).FirstOrDefault() ?? ""
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EInvoice(EInvoiceIntegrationSystemViewModel model)
        {
            var createIntegrationSystem = new CreateIntegrationSystemDto();
            createIntegrationSystem.Name = model.Name;
            createIntegrationSystem.IntegrationSystemTypeId = (int)IntegrationSystemType.EInvoice;
            createIntegrationSystem.Description = model.Description;

            var integrationSystemId = await _integrationSystemService.AddAsync(createIntegrationSystem);

            var eInvoiceTypeIntegrationSystemParameter = new CreateIntegrationSystemParameterDto();
            eInvoiceTypeIntegrationSystemParameter.IntegrationSystemId = integrationSystemId;
            eInvoiceTypeIntegrationSystemParameter.Key = "EInvoiceType";
            eInvoiceTypeIntegrationSystemParameter.Value = model.EInvoiceType;

            await _integrationSystemParameterService.AddAsync(eInvoiceTypeIntegrationSystemParameter);


            return Json(new { success = true });
        }


        [HttpGet]
        public async Task<IActionResult> EInvoiceSettings(int integrationSystemId)
        {

            var integrationSystemEinvoice = await _integrationSystemService.GetByIdAsync(integrationSystemId);
            if (integrationSystemEinvoice == null)
            {
                return View();
            }


            var eInvoiceType = integrationSystemEinvoice.IntegrationSystemParameters.Where(m => m.Key == "EInvoiceType" & m.IntegrationSystemId == integrationSystemId).FirstOrDefault();
            if (eInvoiceType == null)
            {
                return NotFound();
            }
            switch (eInvoiceType.Value)
            {
                case "TrendyolEfaturam":
                    var id = integrationSystemEinvoice.Id;
                    var name = integrationSystemEinvoice.Name;
                    var description = integrationSystemEinvoice.Description;
                    var apiUrl = integrationSystemEinvoice.IntegrationSystemParameters.Where(m => m.Key == "ApiUrl" & m.IntegrationSystemId == integrationSystemId).FirstOrDefault();
                    var apiUser = integrationSystemEinvoice.IntegrationSystemParameters.Where(m => m.Key == "ApiUser" & m.IntegrationSystemId == integrationSystemId).FirstOrDefault();
                    var apiPassword = integrationSystemEinvoice.IntegrationSystemParameters.Where(m => m.Key == "ApiPassword" & m.IntegrationSystemId == integrationSystemId).FirstOrDefault();

                    TrendyolEFaturamSettingsViewModel model = new TrendyolEFaturamSettingsViewModel();
                    model.Id = id;
                    model.Name = name;
                    model.Description = description;
                    model.IntegrationSystemTypeId = integrationSystemEinvoice.IntegrationSystemTypeId;
                    model.IntegrationSystemId = integrationSystemId;
                    model.EInvoiceType = eInvoiceType.Value;
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
            await _integrationSystemService.UpdateAsync(new UpdateIntegrationSystemDto
            {
                Id = model.Id,
                Description = model.Description,
                IntegrationSystemTypeId = model.IntegrationSystemTypeId,
                Name = model.Name
            });

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
