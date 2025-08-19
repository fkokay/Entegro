using Entegro.Application.DTOs.MediaFolder;
using Entegro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Entegro.Web.Controllers
{
    public class MediaController : Controller
    {
        private readonly IMediaFolderService _mediaFolderService;
        private readonly IMediaFileService _mediaFileService;

        public MediaController(IMediaFolderService mediaFolderService, IMediaFileService mediaFileService)
        {
            _mediaFolderService = mediaFolderService;
            _mediaFileService = mediaFileService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file, [FromForm] string folder)
        {

            if (file == null || file.Length == 0)
                return BadRequest("Dosya yok.");


            folder = string.IsNullOrWhiteSpace(folder) ? "default" : Path.GetFileName(folder);


            var mediaFolder = await _mediaFolderService.GetMediaFolderByNameAsync(folder);

            if (mediaFolder == null)
            {
                var createDto = new CreateMediaFolderDto
                {
                    Name = folder,
                    ParentId = null
                };

                mediaFolder = await _mediaFolderService.CreateFolderAsync(createDto.Name, createDto.ParentId);
            }


            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", folder);
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);


            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadPath, uniqueFileName);


            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }


            var mediaFile = await _mediaFileService.BuildMediaFileDtoAsync(file, mediaFolder.Id);
            await _mediaFileService.AddAsync(mediaFile);

            mediaFolder.FilesCount += 1;
            await _mediaFolderService.UpdateFilesCountAsync(mediaFolder.Id, mediaFolder.FilesCount);

            return Ok(new
            {
                filename = file.FileName,
                folder
            });

        }
    }
}
