using Entegro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entegro.Infrastructure.Data
{
    public class EntegroContext :DbContext
    {
        public EntegroContext(DbContextOptions<EntegroContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImageMapping> ProductImages { get; set; }
        public DbSet<ProductCategoryMapping> ProductCategories { get; set; }
        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<ProductAttributeValue> ProductAttributeValues { get; set; }
        public DbSet<ProductAttributeMapping> ProductAttributeMappings { get; set; }
        public DbSet<ProductVariantAttributeCombination> ProductVariants { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Town> Towns { get; set; }
        public DbSet<District> Districts { get; set; }

        public DbSet<IntegrationSystem> IntegrationSystems { get; set; }
        public DbSet<IntegrationSystemParameter> IntegrationSystemParameters { get; set; }
        public DbSet<IntegrationSystemLog> IntegrationSystemLogs { get; set; }

        public DbSet<MediaFolder> MediaFolders{ get; set; }
        public DbSet<MediaFile> MediaFiles { get; set; }
    }
}
