using Cargohub.interfaces;
using Cargohub.models;
using Cargohub.services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<ICrudService<Warehouse, int>, WarehouseService>();
var app = builder.Build();

app.UseAuthorization();
app.UseRouting();
app.MapControllers();

app.Run("http://localhost:5000");
