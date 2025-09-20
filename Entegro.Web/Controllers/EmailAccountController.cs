using Entegro.Application.DTOs.Common;
using Entegro.Application.DTOs.Customer;
using Entegro.Application.DTOs.EmailAccount;
using Entegro.Application.Interfaces.Services;
using Entegro.Web.Models.Platform.Messaging;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    [Authorize]
    public class EmailAccountController : Controller
    {
        private readonly IEmailAccountService _emailAccountService;
        private readonly IMapper _mapper;

        public EmailAccountController(IEmailAccountService emailAccountService,IMapper mapper)
        {
            _emailAccountService = emailAccountService;
            _mapper = mapper;
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
            EmailAccountModel model = new EmailAccountModel();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(EmailAccountModel model)
        {
            if (ModelState.IsValid)
            {
                var createDto = _mapper.Map<CreateEmailAccountDto>(model);
                await _emailAccountService.AddAsync(createDto);
                return Json(new { success = true });
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var emailAccount = await _emailAccountService.GetByIdAsync(id);
            if (emailAccount == null)
            {
                return NotFound();
            }

            var model = _mapper.Map<EmailAccountModel>(emailAccount);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EmailAccountModel model)
        {
            if (ModelState.IsValid)
            {
                var updateDto = _mapper.Map<UpdateEmailAccountDto>(model);
                await _emailAccountService.UpdateAsync(updateDto);

                return Json(new { success = true });
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EmailAccountList([FromBody] GridCommand gridCommand)
        {
            var result = await _emailAccountService.GetPagedAsync(gridCommand);
            return Json(new
            {
                draw = gridCommand.Draw,
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
