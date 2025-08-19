using Entegro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class MediaController : Controller
    {
        private readonly IMediaFolderService _mediaFolderService;
        private readonly IMediaFileService _mediaFileService;
        private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
         {
             ".jpg",".jpeg",".png",".gif",".webp",".bmp",".tiff"
         };

        public MediaController(IMediaFolderService mediaFolderService, IMediaFileService mediaFileService)
        {
            _mediaFolderService = mediaFolderService;
            _mediaFileService = mediaFileService;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload(IFormFile file, [FromForm] string folder, [FromForm] bool overwrite = false)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Dosya yok.");


            folder = string.IsNullOrWhiteSpace(folder) ? "default" : Path.GetFileName(folder);


            var ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(ext) || !AllowedImageExtensions.Contains(ext))
                return BadRequest("Geçersiz dosya türü. Lütfen bir görsel dosyası yükleyin.");


            var mediaFolder = await _mediaFolderService.GetMediaFolderByNameAsync(folder);
            if (mediaFolder == null)
            {
                mediaFolder = await _mediaFolderService.CreateFolderAsync(folder, parentId: null);
            }


            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", folder);
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var safeFileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploadPath, safeFileName);


            var existing = await _mediaFileService.GetByNameAndFolderAsync(safeFileName, mediaFolder.Id);


            bool physicalExists = System.IO.File.Exists(filePath);


            if ((existing != null || physicalExists) && !overwrite)
            {
                return Conflict(new
                {
                    message = "Bu isimde bir dosya zaten var. Üzerine yazılsın mı?",
                    filename = safeFileName,
                    folder
                });
            }


            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await file.CopyToAsync(stream);
            }


            var builtDto = await _mediaFileService.BuildMediaFileDtoAsync(file, mediaFolder.Id);

            if (existing != null && overwrite)
            {
                await _mediaFileService.OverwriteByNameAsync(safeFileName, mediaFolder.Id, builtDto);


                return Ok(new
                {
                    action = "overwritten",
                    id = existing.Id,
                    filename = safeFileName,
                    folder
                });
            }
            else if (existing == null)
            {

                var newId = await _mediaFileService.AddAsync(builtDto);


                mediaFolder.FilesCount += 1;
                await _mediaFolderService.UpdateFilesCountAsync(mediaFolder.Id, mediaFolder.FilesCount);

                return Ok(new
                {
                    action = "created",
                    id = newId,
                    filename = safeFileName,
                    folder
                });
            }
            else
            {

                return Ok(new
                {
                    action = "noop",
                    id = existing.Id,
                    filename = safeFileName,
                    folder
                });
            }
        }
    }
}
