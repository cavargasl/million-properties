using Million.API.Repository;
using Million.API.Services;
using Million.API.Settings;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Configure MongoDB Settings
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection(nameof(MongoDbSettings)));

// Register Repositories (Singleton for MongoDB collections)
builder.Services.AddSingleton<IOwnerRepository, OwnerRepository>();
builder.Services.AddSingleton<IPropertyRepository, PropertyRepository>();
builder.Services.AddSingleton<IPropertyImageRepository, PropertyImageRepository>();
builder.Services.AddSingleton<IPropertyTraceRepository, PropertyTraceRepository>();

// Register Services (Scoped for business logic)
builder.Services.AddScoped<IOwnerService, OwnerService>();
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IPropertyImageService, PropertyImageService>();
builder.Services.AddScoped<IPropertyTraceService, PropertyTraceService>();

// Legacy service (if still needed)
builder.Services.AddSingleton<MongoDbService>();

// Add Controllers with JSON configuration
builder.Services.AddControllers();

// Configure Swagger/OpenAPI
builder.Services.AddOpenApi();

// Add CORS if needed
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
