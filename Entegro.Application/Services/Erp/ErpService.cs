using Entegro.Application.DTOs.Erp;
using Entegro.Application.DTOs.Marketplace.Trendyol;
using Entegro.Application.Interfaces.Services.Erp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Entegro.Application.Services.Erp
{
    public class ErpService : IErpService
    {
        private readonly HttpClient _httpClient;

        public ErpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri($"https://localhost:7024/api/");

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<ErpProductDto>> GetProductsAsync(string erpType,int pageSize = 50)
        {
            var allProducts = new List<ErpProductDto>();
            bool moreData = true;
            int page = 1;

            while (moreData)
            {
                var url = $"{erpType}/products?pageSize={pageSize}&page={page}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<ErpResponse<ErpProductDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (data?.Content == null || !data.Content.Any())
                {
                    break;
                }

                allProducts.AddRange(data.Content);

                page += 1;

                if (page >= data.TotalPages)
                {
                    moreData = false;
                }
            }

            return allProducts;
        }
    }
}
