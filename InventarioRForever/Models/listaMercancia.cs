namespace InventarioRForever.Models
{
	public class listaMercancia
	{
		public int CodProveedorMaterial { get; set; }

		public int? Cantidad { get; set; }

		public int? CodMaterial { get; set; }

		public int? stock { get; set; }

		public DateTime? CantidadProducida { get; set; }

		public string? NombreMaterial { get; set; }

		public string? NombreTipoMaterial { get; set; }

		public string? NombreProveedor { get; set; }
	}
}
