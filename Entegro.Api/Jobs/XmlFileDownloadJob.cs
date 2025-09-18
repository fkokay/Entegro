using Quartz;

namespace Entegro.Api.Jobs
{
    public class XmlFileDownloadJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var url = context.JobDetail.JobDataMap.GetString("XmlUrl");
                var folderName = context.JobDetail.JobDataMap.GetString("FolderName") ?? "";
                var uploadedFileName = context.JobDetail.JobDataMap.GetString("UploadedFileName");

                if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(uploadedFileName))
                {
                    throw new ArgumentException("XmlUrl or UploadedFileName is missing.");
                }


                using var httpClient = new HttpClient();
                var xmlContent = await httpClient.GetStringAsync(url);

                // App_Data/Media/Storage/{folderName}/{uploadedFileName}
                var appDataPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "App_Data",
                    "Media",
                    "Storage",
                    folderName
                );

                Directory.CreateDirectory(appDataPath);

                var fullFilePath = Path.Combine(appDataPath, uploadedFileName);
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
