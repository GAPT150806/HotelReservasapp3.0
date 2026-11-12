using HotelReservasapp.BusinessLogic.Interfaces;
using HotelReservasapp.BusinessLogic.Interfaces.Services;
using HotelReservasapp.BusinessLogic.Services;
using HotelReservasapp.DataAccess;

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

app.Run();
