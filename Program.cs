using Cargohub.interfaces;
using Cargohub.models;
using Cargohub.services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<AdminOnly>();
});

builder.Services.AddControllers();
builder.Services.AddSingleton<ICrudService<Warehouse, int>, WarehouseService>();
builder.Services.AddSingleton<IItemService, ItemService>();
builder.Services.AddSingleton<ICrudService<ItemLine, int>, ItemLineService>();
builder.Services.AddSingleton<ItemLineService>();
builder.Services.AddSingleton<ICrudService<ItemType, int>, ItemTypeService>();
builder.Services.AddSingleton<ICrudService<Transfer, int>, TransferService>();
builder.Services.AddSingleton<ICrudService<Order, int>, OrderService>();
builder.Services.AddSingleton<ICrudService<Shipment, int>, ShipmentService>();
builder.Services.AddSingleton<ICrudService<Supplier, int>, SupplierService>();


builder.Services.AddSingleton<ICrudService<ItemGroup, int>, ItemGroupService>();
builder.Services.AddSingleton<ICrudService<Client, int>, ClientsService>();
builder.Services.AddSingleton<ICrudService<Location, int>, LocationsService>();
builder.Services.AddSingleton<InventoryService>();
builder.Services.AddSingleton<ICrudService<Classifications, int>, ClassificationService>();
builder.Services.AddSingleton<ICrudService<Item, string>, ItemService>();
builder.Services.AddSingleton<InventoryService>();

builder.Services.AddSingleton<CrossDockingService>();
builder.Services.AddSingleton<ShipmentService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Define Swagger Docs for v1 and v2
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Cargohub API - V1",
        Version = "v1",
        Description = "API documentation for version 1 of Cargohub API"
    });
    options.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Cargohub API - V2",
        Version = "v2",
        Description = "API documentation for version 2 of Cargohub API"
    });
    // Add API key security definition
    options.AddSecurityDefinition("API_KEY", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "API_KEY",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Provide your API key to access this API."
    });

    // Add global security requirement for API key
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "API_KEY"
                }
            },
            Array.Empty<string>()
        }
    });

    // Include controllers in their respective version docs based on the route
    options.DocInclusionPredicate((docName, apiDesc) =>
    {
        // Get the version from the route (e.g., /api/v1 or /api/v2)
        var version = apiDesc.RelativePath.Split('/')[1]; // Extract "v1" or "v2"

        return docName == version; // Include the endpoint if it matches the version
    });

    // Group actions by their GroupName (e.g., Users, Clients)
    options.TagActionsBy(apiDesc => new[] { apiDesc.GroupName });
});



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cargohub API - V1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "Cargohub API - V2");
    });
}


app.UseAuthorization();
app.Use(async (ctx, next) =>
{
    var path = ctx.Request.Path.Value;

    if (path.StartsWith("/swagger") || path.StartsWith("/index.html") || path.Contains("/swagger"))
    {
        await next();
        return;
    }

    var apiKey = ctx.Request.Headers["API_KEY"].FirstOrDefault();
    if (string.IsNullOrEmpty(apiKey))
    {
        ctx.Response.StatusCode = 401;
        await ctx.Response.WriteAsync("API key is missing");
        return;
    }

    var user = AuthProvider.GetUser(apiKey);
    if (user == null)
    {
        ctx.Response.StatusCode = 401;
        await ctx.Response.WriteAsync("Invalid API key");
        return;
    }

    await next();
});


app.UseRouting();
app.MapControllers();

app.Run("http://[::]:3000");

