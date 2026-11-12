
namespace HotelReservasapp_.WebApp.Models.ViewModels.Dashboard;

public sealed class BedroomStateSummaryViewModel
{
    public string Nombre { get; set; } = string.Empty;

    public int Cantidad { get; set; }

    public decimal Porcentaje { get; set; }

    public string CssClass { get; set; } = string.Empty;
}
