
namespace HotelReservasapp_.WebApp.Models.ViewModels.Dashboard;

public sealed class DashboardViewModel
{
    public int TotalHoteles { get; set; }
    public int HotelesActivos { get; set; }

    public int TotalPisos { get; set; }
    public int PisosActivos { get; set; }

    public int TotalHabitaciones { get; set; }
    public int TotalTiposHabitacion { get; set; }

    public List<BedroomStateSummaryViewModel> EstadosHabitaciones { get; set; } = [];

    public List<HotelCardViewModel> Hoteles { get; set; } = [];
}

public sealed class HotelCardViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public int TotalPisos { get; set; }
    public int TotalHabitaciones { get; set; }
}
