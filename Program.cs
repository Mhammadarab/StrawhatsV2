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
builder.Services.AddSingleton<ICrudService<Inventory, int>, InventoryService>();

builder.Services.AddSingleton<UserService>();

builder.Services.AddSingleton<CrossDockingService>();
builder.Services.AddSingleton<ShipmentService>();


var app = builder.Build();

app.UseAuthorization();
app.Use(async (ctx, next) =>
{
    var apiKey = ctx.Request.Headers["API_KEY"].FirstOrDefault();
    if (apiKey == null)
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

    var path = ctx.Request.Path.Value.Split('/').Skip(3).FirstOrDefault();
    var method = ctx.Request.Method.ToLower();

    if (!AuthProvider.HasAccess(user, path, method))
    {
        ctx.Response.StatusCode = 403;
        await ctx.Response.WriteAsync("Access denied");
        return;
    }

    await next.Invoke();
});

app.Use(async (context, next) =>
{
    await next.Invoke();
    
    var apiKey = context.Request.Headers["API_KEY"].FirstOrDefault();
    var msg = $"{DateTime.Now} API_KEY: {apiKey} Made a {context.Request.Method} Request to API_KEY: \n";
    
    await System.IO.File.AppendAllTextAsync("log.txt", msg);
});

app.UseRouting();
app.MapControllers();

app.Run("http://[::]:3000");

