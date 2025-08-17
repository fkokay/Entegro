using Entegro.Application.Interfaces.Repositories;
using Entegro.Application.Interfaces.Services;
using Entegro.Application.Interfaces.Services.Commerce;
using Entegro.Application.Mappings;
using Entegro.Application.Services;
using Entegro.Application.Services.Commerce;
using Entegro.Infrastructure.Data;
using Entegro.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.Name = "EntegroAuthentication";
    options.CookieManager = new ChunkingCookieManager();
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.LoginPath = "/Identity/Login";
    options.LogoutPath = "/Identity/Logout";
    options.AccessDeniedPath = "/Home/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
    options.Cookie.MaxAge = options.ExpireTimeSpan;
});

builder.Services.AddDbContext<EntegroContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IBrandService, BrandService>();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped<IProductCategoryMappingRepository, ProductCategoryMappingRepository>();
builder.Services.AddScoped<IProductCategoryMappingService, ProductCategoryMappingService>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<IMediaFolderRepository, MediaFolderRepository>();
builder.Services.AddScoped<IMediaFolderService, MediaFolderService>();


builder.Services.AddScoped<IMediaFileRepository, MediaFileRepository>();
builder.Services.AddScoped<IMediaFileService, MediaFileService>();


builder.Services.AddScoped<IProductAttributeRepository, ProductAttributeRepository>();
builder.Services.AddScoped<IProductAttributeService, ProductAttributeService>();

builder.Services.AddScoped<IProductAttributeValueRepository, ProductAttributeValueRepository>();
builder.Services.AddScoped<IProductAttributeValueService, ProductAttributeValueService>();

builder.Services.AddScoped<IProductAttributeMappingRepository, ProductAttributeMappingRepository>();
builder.Services.AddScoped<IProductAttributeMappingService, ProductAttributeMappingService>();

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();


builder.Services.AddScoped<IProductVariantAttributeCombinationService, ProductVariantAttributeCombinationService>();
builder.Services.AddScoped<IProductVariantAttributeCombinationRepository, ProductVariantAttributeCombinationRepository>();

builder.Services.AddScoped<IIntegrationSystemRepository, IntegrationSystemRepository>();
builder.Services.AddScoped<IIntegrationSystemService, IntegrationSystemService>();

builder.Services.AddScoped<IIntegrationSystemParameterRepository, IntegrationSystemParameterRepository>();
builder.Services.AddScoped<IIntegrationSystemParameterService, IntegrationSystemParameterService>();

builder.Services.AddScoped<IIntegrationSystemLogRepository, IntegrationSystemLogRepository>();
builder.Services.AddScoped<IIntegrationSystemLogService, IntegrationSystemLogService>();


builder.Services.AddScoped<ISmartstoreService, SmartstoreService>();
builder.Services.AddHttpClient();

var app = builder.Build();

var supportedCultures = new[] { new CultureInfo("en-US") };

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
