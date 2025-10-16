using Entegro.Application.DTOs.ProductMediaFile;
using Entegro.Application.Interfaces.Services.Base;
using Quartz;
using System.Text.Json;

namespace Entegro.Api.Jobs
{
    [DisallowConcurrentExecution]
    public class TrendyolImageJob : IJob
    {
        private readonly ISettingService _settingService;
        private readonly ILogger<TrendyolImageJob> _logger;
        private readonly IProductMediaFileMappingService _productMediaFileMappingService;
        private readonly IProductService _productService;


        public TrendyolImageJob(ISettingService settingService, ILogger<TrendyolImageJob> logger, IProductMediaFileMappingService productMediaFileMappingService, IProductService productService)
        {
            _settingService = settingService;
            _logger = logger;
            _productMediaFileMappingService = productMediaFileMappingService;
            _productService = productService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var productId = context.JobDetail.JobDataMap.GetInt("ProductId");
                var profile = await _productService.GetProductByIdAsync(productId);
                if (profile == null)
                {
                    _logger.LogError("ProductId: {ProductId} ürünü bulunamadı", productId);
                    return;
                }
                var systemUrl = await _settingService.GetByKeyAsync("SystemUrl");
                if (systemUrl == null || string.IsNullOrWhiteSpace(systemUrl.Value))
                {
                    _logger.LogError("SystemUrl tanımlı değil veya boş");
                    return;
                }

                if (!Uri.TryCreate(systemUrl.Value, UriKind.Absolute, out var baseUri))
                {
                    _logger.LogError("Geçersiz SystemUrl: {Url}", systemUrl.Value);
                    return;
                }

                using var httpClient = new HttpClient
                {
                    BaseAddress = baseUri
                };


                try
                {
                    var images = new List<string>();
                    List<int> mediaFiles = await UploadImagesAsync(images, httpClient);
                    foreach (var item in mediaFiles)
                    {
                        CreateProductMediaFileDto createProductMediaFile = new CreateProductMediaFileDto();
                        createProductMediaFile.MediaFileId = item;
                        //createProductMediaFile.ProductId = productId;

                        await _productMediaFileMappingService.AddAsync(createProductMediaFile);
                    }


                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "{i} sıralı ürün aktarımı sırasında hata oluştu", ex.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktarım sırasında bir hata oluştur");
            }
        }




        public async Task<List<int>> UploadImagesAsync(List<string> imageUrls, HttpClient httpClient)
        {
            List<int> fileIds = new();

            if (imageUrls != null && imageUrls.Any())
            {
                var multipartContent = new MultipartFormDataContent();
                multipartContent.Add(new StringContent("catalog"), "path");
                multipartContent.Add(new StringContent("False"), "isTransient");

                foreach (var imageUrl in imageUrls)
                {
                    try
                    {
                        var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
                        var imageName = Path.GetFileName(imageUrl);

                        if (string.IsNullOrWhiteSpace(imageName))
                            imageName = "default.jpg";

                        var byteContent = new ByteArrayContent(imageBytes);
                        byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                        multipartContent.Add(byteContent, "upload-file", imageName);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"HATA (indirilemedi): {imageUrl} → {ex.Message}");
                    }
                }

                var uploadUrl = "media/upload";
                var response = await httpClient.PostAsync(uploadUrl, multipartContent);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();

                    using var document = JsonDocument.Parse(result);
                    var root = document.RootElement;

                    if (root.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in root.EnumerateArray())
                        {
                            if (item.TryGetProperty("id", out var idProp))
                            {
                                int imageId = idProp.GetInt32();
                                fileIds.Add(imageId);
                                Console.WriteLine($"id: {imageId}");
                            }
                        }
                    }
                    else if (root.ValueKind == JsonValueKind.Object)
                    {
                        if (root.TryGetProperty("id", out var idProp))
                        {
                            int imageId = idProp.GetInt32();
                            fileIds.Add(imageId);
                            Console.WriteLine($"id: {imageId}");
                        }
                    }
                }
                else
                {
                    throw new Exception("Resim yükleme başarısız oldu. Durum kodu:" + response.StatusCode);
                }
            }

            return fileIds;
        }
    }
}
