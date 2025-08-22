using Entegro.Application.DTOs.MediaFolder;
using Entegro.Application.Interfaces.Services;
using Entegro.Domain.Enums;
using Entegro.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;

namespace Entegro.Web.Controllers
{
    public class MediaController : Controller
    {
        private readonly static IDictionary<string, string[]> _defaultExtensionsMap = new Dictionary<string, string[]>
        {
            ["image"] = ["png", "jpg", "jpeg", "jfif", "gif", "webp", "bmp", "avif", "svg", "ico"],
            ["video"] = ["mp4", "m4v", "mkv", "wmv", "avi", "asf", "mpg", "mpeg", "webm", "flv", "ogv", "mov", "3gp"],
            ["audio"] = ["mp3", "wav", "wma", "aac", "flac", "oga", "wav", "m4a", "ogg"],
            ["document"] = ["pdf", "doc", "docx", "ppt", "pptx", "pps", "ppsx", "docm", "odt", "ods", "dot", "dotx", "dotm", "psd", "xls", "xlsx", "rtf"],
            ["text"] = ["txt", "xml", "csv", "htm", "html", "json", "css", "js"],
            ["bin"] = []
        };

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

        [HttpGet("media/{mediaId}/{folder}/{fileName}")]
        public IActionResult MediaFile(string mediaId, string folder, string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "Media", "Storage", folder, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            // İçerik tipini belirle
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            var contentType = extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, contentType);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(
            string path,
            string[] typeFilter = null,
            bool isTransient = false,
            DuplicateFileHandling duplicateFileHandling = DuplicateFileHandling.ThrowError,
            string directory = "",
            string entityType = "")
        {
            var numFiles = Request.Form.Files.Count;
            var result = new List<object>(numFiles);

            var appDataPath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data");
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            var mediaPath = Path.Combine(appDataPath, "Media");
            if (!Directory.Exists(mediaPath))
            {
                Directory.CreateDirectory(mediaPath);
            }

            var storagePath = Path.Combine(Directory.GetCurrentDirectory(), mediaPath, "Storage");
            if (!Directory.Exists(storagePath))
            {
                Directory.CreateDirectory(storagePath);
            }

            for (int i = 0; i < numFiles; i++)
            {
                try
                {
                    MediaFolderDto? mediaFolder = null;
                    if (!string.IsNullOrEmpty(directory))
                    {
                        path = Path.Combine(storagePath, path, directory);

                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        mediaFolder = await _mediaFolderService.GetMediaFolderByNameAsync(directory);
                        if (mediaFolder == null)
                        {
                            mediaFolder = await _mediaFolderService.CreateFolderAsync(directory, parentId: null);
                        }
                    }
                    else
                    {
                        directory = path;
                        path = Path.Combine(storagePath, path);

                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        mediaFolder = await _mediaFolderService.GetMediaFolderByNameAsync(directory);
                        if (mediaFolder == null)
                        {
                            mediaFolder = await _mediaFolderService.CreateFolderAsync(directory, parentId: null);
                        }
                    }

                    var uploadedFile = Request.Form.Files[i];
                    var fileName = uploadedFile.FileName;
                    var filePath = Path.Combine(path, fileName);
                    int? mediaFolderId = mediaFolder == null ? null : mediaFolder.Id;

                    string title = Path.GetFileNameWithoutExtension(fileName);
                    var extension = Path.GetExtension(fileName).TrimStart('.').ToLower();
                    if (typeFilter != null && typeFilter.Length > 0)
                    {

                    }
                    else
                    {

                    }

                    if (duplicateFileHandling == DuplicateFileHandling.Rename)
                    {
                        int index = 1;
                        while (true)
                        {
                            fileName = title + "-" + index + "." + extension;
                            var existMediaFile = await _mediaFileService.GetByNameAndFolderAsync(fileName, mediaFolderId);
                            if (existMediaFile == null)
                            {
                                break;
                            }

                            index++;
                        }
                        var renameFilePath = Path.Combine(path, fileName);

                        using (var stream = new FileStream(renameFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            await uploadedFile.CopyToAsync(stream);
                        }
                    }
                    else if (duplicateFileHandling == DuplicateFileHandling.Overwrite)
                    {
                        using (var stream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None))
                        {
                            await uploadedFile.CopyToAsync(stream);
                        }
                    }
                    else
                    {
                        var existMediaFile = await _mediaFileService.GetByNameAndFolderAsync(fileName, mediaFolderId);
                        if (existMediaFile != null)
                        {
                            throw new DuplicateMediaFileException("'{0}' dosyası zaten var.", existMediaFile, "");
                        }

                        using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            await uploadedFile.CopyToAsync(stream);
                        }
                    }

                    var createMediaFile = await _mediaFileService.BuildMediaFileDtoAsync(uploadedFile, fileName, mediaFolderId);
                    createMediaFile.IsTransient = true;

                    int mediaFileId = 0;
                    if (duplicateFileHandling == DuplicateFileHandling.Rename || duplicateFileHandling == DuplicateFileHandling.ThrowError)
                    {
                        mediaFileId = await _mediaFileService.AddAsync(createMediaFile);
                    }
                    else
                    {
                        var existMediaFile = await _mediaFileService.GetByNameAndFolderAsync(fileName, mediaFolderId);
                        if (existMediaFile != null)
                        {
                            mediaFileId = existMediaFile.Id;
                        }
                    }

                    var mediaFile = await _mediaFileService.GetByIdAsync(mediaFileId);


                    dynamic o = new ExpandoObject();
                    o.id = mediaFile.Id;
                    o.folderId = mediaFile.FolderId;
                    o.mime = mediaFile.MimeType;
                    o.type = mediaFile.MediaType;
                    o.createdOn = mediaFile.CreatedOn;
                    o.path = $"{mediaFile.Folder?.Name}/{mediaFile.Name}";
                    o.version = mediaFile.UpdatedOn.Ticks;
                    o.url = $"/media/{mediaFile.Id}/{mediaFile.Folder?.Name}/{mediaFile.Name}";
                    o.thumbUrl = $"/media/{mediaFile.Id}/{mediaFile.Folder?.Name}/{mediaFile.Name}?size=256";
                    o.lastUpdated = mediaFile.UpdatedOn;
                    o.size = mediaFile.Size;
                    o.name = mediaFile.Name;
                    o.dir = mediaFile.Folder?.Name;
                    o.title = mediaFile.Title;
                    o.ext = mediaFile.Extension;
                    o.dimensions = $"{mediaFile.Width}, {mediaFile.Height}";
                    o.success = true;
                    o.createdOn = mediaFile.CreatedOn.ToString();
                    o.lastUpdated = mediaFile.UpdatedOn.ToString();
                    result.Add(o);
                }
                catch (DuplicateMediaFileException dex)
                {
                    var dupe = dex.File;

                    dynamic o = new ExpandoObject();
                    o.id = dupe.Id;
                    o.folderId = dupe.FolderId;
                    o.mime = dupe.MimeType;
                    o.type = dupe.MediaType;
                    o.createdOn = dupe.CreatedOn;
                    o.path = $"{dupe.Folder?.Name}/{dupe.Name}";
                    o.version = dupe.UpdatedOn.Ticks;
                    o.url = $"/media/{dupe.Id}/{dupe.Folder?.Name}/{dupe.Name}";
                    o.thumbUrl = $"/media/{dupe.Id}/{dupe.Folder?.Name}/{dupe.Name}?size=256";
                    o.lastUpdated = dupe.UpdatedOn;
                    o.size = dupe.Size;
                    o.name = dupe.Name;
                    o.dir = dupe.Folder?.Name;
                    o.title = dupe.Title;
                    o.ext = dupe.Extension;
                    o.dimensions = $"{dupe.Width}, {dupe.Height}";
                    o.dupe = true;
                    o.errMessage = dex.Message;
                    o.uniquePath = $"{dupe.Folder?.Name}/{dupe.Name}";
                    o.createdOn = dupe.CreatedOn.ToString();
                    o.lastUpdated = dupe.UpdatedOn.ToString();

                    result.Add(o);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return Json(result.Count == 1 ? result[0] : result);
        }

        public IActionResult FileConflictResolutionDialog()
        {
            return PartialView();
        }

        //[HttpPost("upload")]
        //[Consumes("multipart/form-data")]
        //public async Task<IActionResult> Upload(IFormFile file, [FromForm] string folder, [FromForm] bool overwrite = false, [FromForm] int? id = null)
        //{
        //    if (file == null || file.Length == 0)
        //        return BadRequest("Dosya yok.");


        //    folder = string.IsNullOrWhiteSpace(folder) ? "default" : Path.GetFileName(folder);


        //    var ext = Path.GetExtension(file.FileName);
        //    if (string.IsNullOrEmpty(ext) || !AllowedImageExtensions.Contains(ext))
        //        return BadRequest("Geçersiz dosya türü. Lütfen bir görsel dosyası yükleyin.");


        //    var mediaFolder = await _mediaFolderService.GetMediaFolderByNameAsync(folder);
        //    if (mediaFolder == null)
        //    {
        //        mediaFolder = await _mediaFolderService.CreateFolderAsync(folder, parentId: null);
        //    }


        //    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", folder);
        //    if (!Directory.Exists(uploadPath))
        //        Directory.CreateDirectory(uploadPath);

        //    var safeFileName = Path.GetFileName(file.FileName);
        //    var filePath = Path.Combine(uploadPath, safeFileName);


        //    var existing = await _mediaFileService.GetByNameAndFolderAsync(safeFileName, mediaFolder.Id);


        //    bool physicalExists = System.IO.File.Exists(filePath);


        //    if ((existing != null || physicalExists) && !overwrite)
        //    {
        //        return Conflict(new
        //        {
        //            message = "Bu isimde bir dosya zaten var. Üzerine yazılsın mı?",
        //            filename = safeFileName,
        //            folder
        //        });
        //    }


        //    using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
        //    {
        //        await file.CopyToAsync(stream);
        //    }


        //    var builtDto = await _mediaFileService.BuildMediaFileDtoAsync(file, mediaFolder.Id);
        //    int fileId;
        //    if (existing != null && overwrite)
        //    {
        //        await _mediaFileService.OverwriteByNameAsync(safeFileName, mediaFolder.Id, builtDto);
        //        fileId = existing.Id;


        //    }
        //    else if (existing == null)
        //    {
        //        var newId = await _mediaFileService.AddAsync(builtDto);
        //        fileId = newId;
        //        mediaFolder.FilesCount += 1;
        //        await _mediaFolderService.UpdateFilesCountAsync(mediaFolder.Id, mediaFolder.FilesCount);
        //    }
        //    else
        //    {
        //        fileId = existing.Id;

        //    }
        //    if (folder.Equals("Brand", StringComparison.OrdinalIgnoreCase))
        //        await _brandService.UpdateBrandImageAsync(id.Value, fileId);

        //    else if (folder.Equals("Category", StringComparison.OrdinalIgnoreCase))
        //        await _categoryService.UpdateCategoryImageAsync(id.Value, fileId);

        //    return Ok(new
        //    {
        //        action = overwrite ? "overwritten" : (existing == null ? "created" : "noop"),
        //        id = fileId,
        //        filename = safeFileName,
        //        folder
        //    });
        //}


        //[HttpPost]
        //public async Task<IActionResult> Delete([FromForm] string folder, [FromForm] int? mediaFolderId, [FromForm] int? id = null)
        //{
        //    if (folder.Equals("Brand", StringComparison.OrdinalIgnoreCase))
        //        await _brandService.DeleteBrandImageAsync(id.Value);

        //    else if (folder.Equals("Category", StringComparison.OrdinalIgnoreCase))
        //        await _categoryService.DeleteCategoryImageAsync(id.Value);

        //    await _mediaFileService.DeleteAsync(mediaFolderId.Value);
        //    return Ok(new
        //    {
        //        action = "deleted",
        //        id = mediaFolderId,
        //        folder
        //    });
        //}
    }
}
