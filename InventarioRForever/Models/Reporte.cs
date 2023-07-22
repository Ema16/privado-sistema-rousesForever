namespace InventarioRForever.Models
{
	public class Reporte
	{

		public DateTime? Fecha1 { get; set; }

		public DateTime? Fecha2 { get; set; }

		public int CodUsuario { get; set; }

		public int CodProducto { get; set; }

        public int CodMaterial { get; set; }
    }
}
