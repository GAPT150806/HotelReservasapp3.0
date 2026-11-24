
using HotelReservasapp.BusinessLogic.Interfaces;
using HotelReservasapp.BusinessLogic.Interfaces.Services;
using HotelReservasapp.BusinessLogic.Services;
using HotelReservasapp.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Controladores
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
