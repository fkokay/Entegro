using Entegro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class MediaController : Controller
    {
        private readonly IMediaFolderService _mediaFolderService;
        private readonly IMediaFileService _mediaFileService;
        private readonly IBrandService _brandService;
        private readonly ICategoryService _categoryService;
        private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
         {
             ".jpg",".jpeg",".png",".gif",".webp",".bmp",".tiff"
         };

        public MediaController(IMediaFolderService mediaFolderService, IMediaFileService mediaFileService, IBrandService brandService, ICategoryService categoryService)
        {
            _mediaFolderService = mediaFolderService;
            _mediaFileService = mediaFileService;
            _brandService = brandService;
            _categoryService = categoryService;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload(IFormFile file, [FromForm] string folder, [FromForm] bool overwrite = false, [FromForm] int? id = null)
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
            int fileId;
            if (existing != null && overwrite)
            {
                await _mediaFileService.OverwriteByNameAsync(safeFileName, mediaFolder.Id, builtDto);
                fileId = existing.Id;


            }
            else if (existing == null)
            {
                var newId = await _mediaFileService.AddAsync(builtDto);
                fileId = newId;
                mediaFolder.FilesCount += 1;
                await _mediaFolderService.UpdateFilesCountAsync(mediaFolder.Id, mediaFolder.FilesCount);
            }
            else
            {
                fileId = existing.Id;

            }
            if (folder.Equals("Brand", StringComparison.OrdinalIgnoreCase))
                await _brandService.UpdateBrandImageAsync(id.Value, fileId);

            else if (folder.Equals("Category", StringComparison.OrdinalIgnoreCase))
                await _categoryService.UpdateCategoryImageAsync(id.Value, fileId);

            return Ok(new
            {
                action = overwrite ? "overwritten" : (existing == null ? "created" : "noop"),
                id = fileId,
                filename = safeFileName,
                folder
            });
        }


        [HttpPost]
        public async Task<IActionResult> Delete([FromForm] string folder, [FromForm] int? mediaFolderId, [FromForm] int? id = null)
        {
            if (folder.Equals("Brand", StringComparison.OrdinalIgnoreCase))
                await _brandService.DeleteBrandImageAsync(id.Value);

            else if (folder.Equals("Category", StringComparison.OrdinalIgnoreCase))
                await _categoryService.DeleteCategoryImageAsync(id.Value);

            await _mediaFileService.DeleteAsync(mediaFolderId.Value);
            return Ok(new
            {
                action = "deleted",
                id = mediaFolderId,
                folder
            });
        }
    }
}
