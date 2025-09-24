using Entegro.Application.DTOs.Category;
using Entegro.Application.DTOs.Commerce.Smartstore;
using Entegro.Application.Interfaces.Services.Commerce;
using Entegro.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Application.Services.Commerce.Smartstore
{
    public class SmartstoreCategoryWriter : ICommerceCategoryWriter
    {
        private readonly SmartstoreClient _smartstoreClient;
        private readonly ILogger<SmartstoreCategoryWriter> _logger;
        public SmartstoreCategoryWriter(SmartstoreClient smartstoreClient, ILogger<SmartstoreCategoryWriter> logger)
        {
            _smartstoreClient = smartstoreClient;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CategoryDto?> CategoryExistsAsync(SmartstoreApiContext context, string categoryName)
        {
            return await _smartstoreClient.CategoryExistsAsync(context, categoryName);
        }

        public async Task<int> CreateCategoryAsync(SmartstoreApiContext context, CategoryDto category)
        {
            return await _smartstoreClient.CreateCategoryAsync(context, category);
        }

        public Task DeleteCategoryAsync(SmartstoreApiContext context, int categoryId)
        {
            return _smartstoreClient.DeleteCategoryAsync(context, categoryId);
        }

        public Task UpdateCategoryAsync(SmartstoreApiContext context, CategoryDto category, int id)
        {
            return _smartstoreClient.UpdateCategoryAsync(context, category, id);
        }
    }
}
