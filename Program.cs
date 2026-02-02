using System.Text;
using Api.API.EndPoints.Inventory;
using Api.Application.Interfaces;
using Api.Application.Services;
using Api.Infrastructure.Repositories;
using Api.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Api;
using Api.Infrastructure.Repositories.Interface;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Enter your JWT token like: **Bearer &lt;your token&gt;**",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme,
        },
    };

    c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement { { jwtSecurityScheme, Array.Empty<string>() } });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ISizeRepository, SizeRepository>();
builder.Services.AddScoped<ISizeService, SizeService>();

builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IMenuRepository, MenuRepository>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IProductNutritionRepository, ProductNutritionRepository>();
builder.Services.AddScoped<INutritionService, NutritionService>();
builder.Services.AddScoped<IShopRepository, ShopRepository>();
builder.Services.AddScoped<IShopService, ShopService>();
builder.Services.AddScoped<ISliderRepository, SliderRepository>();
builder.Services.AddScoped<ISliderService, SliderService>();
builder.Services.AddScoped<IGalleryFilterRepository, GalleryFilterRepository>();
builder.Services.AddScoped<IGalleryFilterService, GalleryFilterService>();
builder.Services.AddScoped<IGalleryRepository, GalleryRepository>();
builder.Services.AddScoped<IGalleryService, GalleryService>();
builder.Services.AddScoped<IEnquiryRepository, EnquiryRepository>();
builder.Services.AddScoped<IEnquiryService, EnquiryService>();
builder.Services.AddScoped<IChemicalRepository, ChemicalRepository>();
builder.Services.AddScoped<IChemicalService, ChemicalService>();
builder.Services.AddScoped<IGradeRepository, GradeRepository>();
builder.Services.AddScoped<IGradeService, GradeService>();
builder.Services.AddScoped<IColourRepository, ColourRepository>();
builder.Services.AddScoped<IColourService, ColourService>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IGramageRepository, GramageRepository>();
builder.Services.AddScoped<IGramageService, GramageService>();
builder.Services.AddScoped<IWidthRepository, WidthRepository>();
builder.Services.AddScoped<IWidthService, WidthService>();
builder.Services.AddScoped<IPVCproductListRepository, PVCproductListRepository>();
builder.Services.AddScoped<IPVCproductListService, PVCproductListService>();


builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer",
        options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            };
        }
    );

builder.Services.AddAuthorization();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => { policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
});

// Program.cs
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

var app = builder.Build();
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

app.UseHttpsRedirection();
app.UseAuthentication(); // JWT
app.UseAuthorization(); // Role-based
app.UseStaticFiles(); // To serve from wwwroot

app.MapFallbackToFile("/index.html");

// Your custom APIs
app.MapCategoryEndpoints();
app.MapSliderEndpoints();
app.MapAuthEndpoints();
app.MapSizeEndpoints();
app.MapRoleEndpoints();
app.MapProductEndpoints();
app.MapEmployeeEndpoints();
app.MapNewsEndpoints();
app.MapMenuEndpoints();
app.MapNutritionEndpoints();
app.MapShopEndpoints();
app.MapGalleryFilterEndpoints();
app.MapGalleryEndpoints();
app.MapEnquiryEndpoints();
app.MapChemicalEndpoints();
app.MapGradeEndpoints();
app.MapColourEndpoints();
app.MapCustomerEndpoints();
app.MapSupplierEndpoints();
app.MapGramageEndpoints();
app.MapWidthEndpoints();
app.MapPVCproductListEndpoints();









app.MapGet(
    "/admin-data",
    [Authorize(Roles = "Admin")]
() =>
    {
        return Results.Ok("Welcome Admin");
    }
);
app.UseCors("AllowAll");

app.Run();
