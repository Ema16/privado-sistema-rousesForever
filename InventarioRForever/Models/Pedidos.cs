using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace InventarioRForever.Models
{
	public class Pedidos
	{
		//Datos pedido
		public int CodPedido { get; set; }

		public int CodUsuario { get; set; }
		public string? NombreProducto { get; set; }

		public DateTime? FechaPedido { get; set; }

		public string? EstadoPedido { get; set; }

		public int CodVenta { get; set; }

		public int? precioVenta { get; set; }

		public int? Cantidad { get; set; }

		public int? CodFactura { get; set; }

		public int CodMetodoPago { get; set; }

		public string? parcialTotal { get; set; }

		//Datos Usuario
		public string? Nombre1 { get; set; }

		public string? Nombre2 { get; set; }

		public string? OtrosNombres { get; set; }

		public string? Apellido1 { get; set; }

		public string? Apellido2 { get; set; }

		public string? Nit { get; set; }

		public string? Direccion { get; set; }

		public string? Contacto { get; set; }

		public string? Correo { get; set; }

		public int CodRolUsuario { get; set; }

		[Display(Name = "Full Name Usuario")]
		public string? FullName
		{
			get
			{
				return Nombre1 + " " + Nombre2+" "+ OtrosNombres + " " + Apellido1+" " + Apellido2; 
			}
		}

		//Datos Metodo de pago
		public string? MetodoPago1 { get; set; }


		//Datos factura
		public int? ImporteTotal { get; set; }
	}
}
