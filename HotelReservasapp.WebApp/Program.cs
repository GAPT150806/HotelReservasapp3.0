using HotelReservasapp.BusinessLogic.Interfaces;
using HotelReservasapp.BusinessLogic.Interfaces.Services;
using HotelReservasapp.BusinessLogic.Services;
using HotelReservasapp.DataAccess;
using HotelReservasapp.DataAccess.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// DATA ACCESS
builder.Services.AddDataAccess(builder.Configuration);

// BUSINESS LOGIC
builder.Services.AddScoped<IHotelService, HotelService>();

builder.Services.AddScoped<IFloorService, FloorService>();

builder.Services.AddScoped<IBedroomTypeService, BedroomTypeService>();

builder.Services.AddScoped<IBedroomService, BedroomService>();

builder.Services.AddScoped<IGuestService, GuestService>();

builder.Services.AddScoped<IReservationService, ReservationService>();

builder.Services.AddScoped<INotificationService, NotificationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

//Borrar
app.MapGet("/diagnostico/base-datos", async (
    HotelReservationDbContext context) =>
{
    var conectado = await context.Database.CanConnectAsync();

    return conectado
        ? Results.Ok(new { mensaje = "Conexión correcta con SGRHotel." })
        : Results.Problem("No se pudo conectar con la base de datos.");
});
//================
app.Run();
