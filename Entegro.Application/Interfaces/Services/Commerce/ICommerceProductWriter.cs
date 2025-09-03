using Entegro.Application.DTOs.Commerce;
using Entegro.Application.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Interfaces.Services.Commerce
{
    public interface ICommerceProductWriter
    {
        Task UpsertProductAsync(UpsertProductRequest request);

        /// <summary>
        /// Birden fazla ürünü toplu olarak ekler veya günceller.
        /// </summary>
        Task UpsertProductsAsync(IEnumerable<UpsertProductRequest> requests);

        /// <summary>
        /// Belirtilen SKU'ya sahip ürünü siler.
        /// </summary>
        Task DeleteProductAsync(string sku);

        /// <summary>
        /// Toplu ürün silme işlemi.
        /// </summary>
        Task DeleteProductsAsync(IEnumerable<string> skus);
    }
}
