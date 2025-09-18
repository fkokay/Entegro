using Entegro.Application.Interfaces.Services;
using Quartz;

namespace Entegro.Api.Jobs
{
    public class XmlFileDownloadJob : IJob
    {
        private readonly IImportProfileService _importProfileService;

        public XmlFileDownloadJob(IImportProfileService importProfileService)
        {
            _importProfileService = importProfileService;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var profileId = context.JobDetail.JobDataMap.GetInt("ProfileId");

                var profile = await _importProfileService.GetByIdAsync(profileId);
                if (profile == null)
                {
                    Console.WriteLine("Profil bulunamadı.");
                    return;
                }

                string fileName = $"{profileId}_{profile.ProfileName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xml";
                var url = profile.MediaFileUrl;
                var folderName = "document"; // veya profile’dan alabilirsin

                if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(fileName))
                {
                    throw new ArgumentException("XmlUrl or UploadedFileName is missing.");
                }

                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120 Safari/537.36");
                var xmlContent = await httpClient.GetStringAsync(url);

                var appDataPath = Path.Combine(
                    Directory.GetParent(Directory.GetCurrentDirectory()).FullName,
                    "Entegro.Web",
                    "App_Data",
                    "Media",
                    "Storage",
                    folderName
                );

                Directory.CreateDirectory(appDataPath);

                var fullFilePath = Path.Combine(appDataPath, fileName);
                await File.WriteAllTextAsync(fullFilePath, xmlContent);

                Console.WriteLine($"XML file saved to: {fullFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading or saving XML file: {ex.Message}");
            }
        }
    }
}
