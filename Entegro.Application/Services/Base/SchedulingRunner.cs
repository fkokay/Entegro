using Entegro.Application.Interfaces.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Services.Base
{
    public class SchedulingRunner:ISchedulingRunner
    {
        private readonly ISettingService _settingService;
        private readonly HttpClient _httpClient;

        public SchedulingRunner(ISettingService settingService, HttpClient httpClient)
        {
            _settingService = settingService;
            _httpClient = httpClient;
        }

        public async Task<string> RunAsync(string type, int taskId, int? parameter)
        {
            var systemUrl = await _settingService.GetByKeyAsync("SystemUrl");
            if (!Uri.TryCreate(systemUrl.Value, UriKind.Absolute, out var baseUri))
                throw new Exception("SystemUrl hatalı");

            _httpClient.BaseAddress = baseUri;

            var url = $"/Scheduling/Run?type={type}&taskId={taskId}&parameter={parameter}";
            var response = await _httpClient.PostAsync(url, null);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
