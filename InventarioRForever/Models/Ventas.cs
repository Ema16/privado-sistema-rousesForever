namespace InventarioRForever.Models
{
	public class Ventas
	{

		public int CodPedido { get; set; }

		public string? NombreProducto { get; set; }

		public int? Cantidad { get; set; }

		public string? NombreSucursal { get; set; }

		public string? NombreColor { get; set; }

		public string? NombreCalidad { get; set; }

		public DateTime? FechaVenta { get; set;}

	}
}
