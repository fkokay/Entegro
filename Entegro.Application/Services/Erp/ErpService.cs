using Entegro.Application.DTOs.Erp;
using Entegro.Application.DTOs.Marketplace.CicekSepeti;
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
        private readonly IHttpClientFactory _httpClientFactory;

        public ErpService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateHttpClient(ErpApiContext context)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(context.BaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        public async Task<List<ErpProductDto>> GetProductsAsync(ErpApiContext context,int pageSize = 50)
        {
            using var client = CreateHttpClient(context);

            var allProducts = new List<ErpProductDto>();
            bool moreData = true;
            int page = 1;

            while (moreData)
            {
                var url = $"{context.ErpType}/products?pageSize={pageSize}&page={page}";
                var response = await client.GetAsync(url);
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
