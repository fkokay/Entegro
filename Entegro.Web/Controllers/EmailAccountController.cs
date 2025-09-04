using Entegro.Application.DTOs.EmailAccount;
using Entegro.Application.Interfaces.Services;
using Entegro.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class EmailAccountController : Controller
    {
        private readonly IEmailAccountService _emailAccountService;

        public EmailAccountController(IEmailAccountService emailAccountService)
        {
            _emailAccountService = emailAccountService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return List();
        }
        [HttpGet]
        public IActionResult List()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Create()
        {
            EmailAccountViewModel model = new EmailAccountViewModel();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(EmailAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var createDto = new CreateEmailAccountDto();
                createDto.Host = model.Host;
                createDto.Password = model.Password;
                createDto.UserDefaultCredentials = model.UserDefaultCredentials;
                createDto.Password = model.Password;
                createDto.Username = model.Username;
                createDto.DisplayName = model.DisplayName;
                createDto.Port = model.Port;
                createDto.Email = model.Email;
                createDto.Host = model.Host;

                await _emailAccountService.AddAsync(createDto);
                return Json(new { success = true });
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            EmailAccountViewModel model = new EmailAccountViewModel();

            var account = await _emailAccountService.GetByIdAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            model.Id = account.Id;
            model.Email = account.Email;
            model.Password = account.Password;
            model.Username = account.Username;
            model.DisplayName = account.DisplayName;
            model.Port = account.Port;
            model.Email = account.Email;
            model.EnableSsl = account.EnableSsl;
            model.Host = account.Host;
            model.Port = account.Port;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EmailAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var updateDto = new UpdateEmailAccountDto();
                updateDto.Host = model.Host;
                updateDto.Password = model.Password;
                updateDto.UserDefaultCredentials = model.UserDefaultCredentials;
                updateDto.Password = model.Password;
                updateDto.Username = model.Username;
                updateDto.DisplayName = model.DisplayName;
                updateDto.Port = model.Port;
                updateDto.Email = model.Email;
                updateDto.Host = model.Host;

                return Json(new { success = true });
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EmailAccountList([FromBody] GridCommand model)
        {

            int pageNumber = model.Start / model.Length;
            int pageSize = model.Length;


            var result = await _emailAccountService.GetPagedAsync(pageNumber, model.Length);

            return Json(new
            {
                draw = model.Draw,
                recordsTotal = result.TotalCount,
                recordsFiltered = result.TotalCount,
                data = result.Items
            });

        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _emailAccountService.DeleteAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
