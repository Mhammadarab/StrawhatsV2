using Cargohub.interfaces;
using Cargohub.models;
using Cargohub.services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<ICrudService<Warehouse, int>, WarehouseService>();
builder.Services.AddSingleton<ICrudService<ItemLine, int>, ItemLineService>();
builder.Services.AddSingleton<ICrudService<Transfer, int>, TransferService>();
builder.Services.AddSingleton<ICrudService<Order, int>, OrderService>();

builder.Services.AddSingleton<ICrudService<ItemGroup, int>, ItemGroupService>();
builder.Services.AddSingleton<ICrudService<Inventory, int>, InventoryService>();



var app = builder.Build();

app.UseAuthorization();
app.UseRouting();
app.MapControllers();

app.Run("http://localhost:5000");
