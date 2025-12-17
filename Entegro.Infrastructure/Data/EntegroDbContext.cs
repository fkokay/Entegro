using Entegro.Data;
using Entegro.Data.Hooks;
using Entegro.Domain.Entities.Catalog;
using Entegro.Domain.Entities.Checkout;
using Entegro.Domain.Entities.Common;
using Entegro.Domain.Entities.Content;
using Entegro.Domain.Entities.Import;
using Entegro.Domain.Entities.Integration;
using Entegro.Domain.Entities.Platform.Identity;
using Entegro.Domain.Entities.Platform.Logging;
using Entegro.Domain.Entities.Platform.Messaging;
using Entegro.Domain.Entities.Platform.Scheduling;
using Entegro.Domain.Entities.Setttings;
using Entegro.Scheduling;
using Microsoft.EntityFrameworkCore;

namespace Entegro.Infrastructure.Data
{
    public abstract class AsyncDbSaveHook<TEntity> : AsyncDbSaveHook<EntegroDbContext, TEntity>
      where TEntity : class
    {
    }

    public abstract class DbSaveHook<TEntity> : DbSaveHook<EntegroDbContext, TEntity>
        where TEntity : class
    {
    }

    public class EntegroDbContext : HookingDbContext
    {
        public EntegroDbContext(DbContextOptions<EntegroDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AddressMap());
            modelBuilder.ApplyConfiguration(new BrandMap());
            modelBuilder.ApplyConfiguration(new CategoryMap());
            modelBuilder.ApplyConfiguration(new CityMap());
            modelBuilder.ApplyConfiguration(new CountryMap());
            modelBuilder.ApplyConfiguration(new CustomerMap());
            modelBuilder.ApplyConfiguration(new DistrictMap());
            modelBuilder.ApplyConfiguration(new EmailAccountMap());
            modelBuilder.ApplyConfiguration(new IntegrationSystemMap());
            modelBuilder.ApplyConfiguration(new IntegrationSystemLogMap());
            modelBuilder.ApplyConfiguration(new IntegrationSystemParameterMap());
            modelBuilder.ApplyConfiguration(new LogMap());
            modelBuilder.ApplyConfiguration(new MediaFileMap());
            modelBuilder.ApplyConfiguration(new MediaFolderMap());
            modelBuilder.ApplyConfiguration(new OrderMap());
            modelBuilder.ApplyConfiguration(new OrderItemMap());
            modelBuilder.ApplyConfiguration(new ProductMap());
            modelBuilder.ApplyConfiguration(new ProductAttributeMap());
            modelBuilder.ApplyConfiguration(new ProductAttributeValueMap());
            modelBuilder.ApplyConfiguration(new ProductCategoryMap());
            modelBuilder.ApplyConfiguration(new ProductIntegrationMap());
            modelBuilder.ApplyConfiguration(new ProductMediaFileMap());
            modelBuilder.ApplyConfiguration(new ProductVariantAttributeMap());
            modelBuilder.ApplyConfiguration(new ProductVariantAttributeCombinationMap());
            modelBuilder.ApplyConfiguration(new ProductVariantAttributeValueMap());
            modelBuilder.ApplyConfiguration(new SpecificationAttributeMap());
            modelBuilder.ApplyConfiguration(new SpecificationAttributeOptionMap());
            modelBuilder.ApplyConfiguration(new TownMap());
            modelBuilder.ApplyConfiguration(new UserMap());
            modelBuilder.ApplyConfiguration(new ImportProfileMap());
            modelBuilder.ApplyConfiguration(new SettingMap());
            modelBuilder.ApplyConfiguration(new CustomerAddressMappingMap());
            modelBuilder.ApplyConfiguration(new TaskDescriptorMap());
            modelBuilder.ApplyConfiguration(new TaskExecutionInfoMap());
            modelBuilder.ApplyConfiguration(new ActivityLogMap());
            modelBuilder.ApplyConfiguration(new TaskExecutionInfoMap());
            modelBuilder.ApplyConfiguration(new CrossSellProductMap());
            modelBuilder.ApplyConfiguration(new RelatedProductMap());
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductMediaFile> ProductMediaFiles { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<ProductAttributeValue> ProductAttributeValues { get; set; }
        public DbSet<ProductVariantAttribute> ProductVariantAttributes { get; set; }
        public DbSet<ProductVariantAttributeValue> ProductVariantAttributeValues { get; set; }
        public DbSet<ProductVariantAttributeCombination> ProductVariantAttributeCombinations { get; set; }
        public DbSet<ProductIntegration> ProductIntegrations { get; set; }
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
        public DbSet<MediaFolder> MediaFolders { get; set; }
        public DbSet<MediaFile> MediaFiles { get; set; }
        public DbSet<SpecificationAttribute> SpecificationAttributes { get; set; }
        public DbSet<SpecificationAttributeOption> SpecificationAttributeOptions { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<EmailAccount> EmailAccounts { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<OrderNote> OrderNotes { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<ShipmentItem> ShipmentItems { get; set; }
        public DbSet<ProductSpecificationAttribute> ProductSpecificationAttributes { get; set; }
        public DbSet<ReturnRequest> ReturnRequests { get; set; }
        public DbSet<ReturnRequestItem> ReturnRequestItems { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ImportProfile> ImportProfiles { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<CustomerAddressMapping> CustomerAddressMappings { get; set; }
        public DbSet<TaskDescriptor> TaskDescriptors { get; set; }
        public DbSet<TaskExecutionInfo> TaskExecutionInfos { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<ActivityLogType> ActivityLogTypes { get; set; }
        public DbSet<CrossSellProduct> CrossSellProducts { get; set; }
        public DbSet<RelatedProduct> RelatedProducts { get; set; }
    }
}
