using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class MediaController : Controller
    {
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Upload(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Dosya alınamadı");

            // folder bilgisi gelmemişse varsay
            if (string.IsNullOrWhiteSpace(folder))
                folder = "Default";

            // wwwroot/media/Brand
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "media", folder);

            // Klasörü oluştur (yoksa)
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            // Benzersiz dosya adı
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Json(new { success = true, message = "Dosya başarıyla yüklendi", fileName = fileName });
        }
    }
}
