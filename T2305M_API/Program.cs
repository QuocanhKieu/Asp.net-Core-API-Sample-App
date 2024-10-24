using Microsoft.EntityFrameworkCore;
using T2305M_API.Entities;
using T2305M_API.Services.Implements;
using T2305M_API.Services;
using T2305M_API.Repositories.Implements;
using T2305M_API.Repositories;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using T2305M_API.DTO.UserArticle.Requirements;
using T2305M_API.DTO.UserArticle.Handlers;
using T2305M_API.Models;
using T2305M_API.DTO.AutoMapping;



var builder = WebApplication.CreateBuilder(args);

// Add CORS policy access
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        //policy.WithOrigins("http://localhost:3000");
        policy.AllowAnyOrigin();
        //policy.WithMethods("POST");
        policy.AllowAnyMethod();
        policy.AllowAnyHeader();
    });
});
//****************
// add AUTH JWT Bearer
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(
        options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["JWT:Issuer"],
                ValidAudience = builder.Configuration["JWT:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
            };
        }
    );

// Add authorize policy

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UpdateUserArticlePolicy", policy => policy.AddRequirements(
        new UpdateUserArticleRequirement()));
});
// Add authorize handler, a handler can do without a policy name 
builder.Services.AddSingleton<IAuthorizationHandler,
        UpdateUserArticleHandler>();

//****************
// connect db
T2305mApiContext.ConnectionString = builder.Configuration.GetConnectionString("T2305M_API");
// attach the DbContext for Dependency Injection Ready
//builder.Services.AddDbContext<T2305mApiContext>(
//    options => options.UseSqlServer(T2305mApiContext.ConnectionString)
//);

builder.Services.AddDbContext<T2305mApiContext>(options =>
    options.UseLazyLoadingProxies() // Enable lazy loading
           .UseSqlServer(T2305mApiContext.ConnectionString));

// Configure SMTP settings from appsettings.json
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddAutoMapper(typeof(MappingProfile)); // This will scan for all profiles in the assembly

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICultureRepository, CultureRepository>();
builder.Services.AddScoped<ICultureService, CultureService>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<SearchServiceImpl>();
builder.Services.AddScoped<IHistoryRepository, HistoryRepository>();
builder.Services.AddScoped<IHistoryService, HistoryService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<ICreatorRepository, CreatorRepository>();
builder.Services.AddScoped<IUserEventRepository, UserEventRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserArticleService, UserArticleService>();
builder.Services.AddScoped<IUserArticleService, UserArticleService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserEventRepository, UserEventRepository>();
builder.Services.AddScoped<IUserArticleRepository, UserArticleRepository>();
builder.Services.AddScoped<IUserNotificationRepository, UserNotificationRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IEventTicketRepository, EventTicketRepository>();
builder.Services.AddScoped<IPaypalService, PaypalService>();

// Register EmailService
builder.Services.AddTransient<EmailService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetService<T2305mApiContext>();

    if (context != null)
    {
        DatabaseSeeder.Seed(context);
    }
    else
    {
        // Log an error or handle the situation where the context is null
        throw new InvalidOperationException("T2305mApiContext is not available.");
    }
}


app.Run();

