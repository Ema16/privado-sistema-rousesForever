using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventarioRForever.Models
{
    public class ViewModel
    {

        //Materiales
        public int CodInventario { get; set; }
        public int ? Stock { get; set; }
        public int ? stockDevuelto { get; set; }
        public DateTime ? FechaInventario { get; set; }

        public string? NombreMaterial { get; set; }


		//Productos
		public int CodProducto { get; set; }

		public string? NombreProducto { get; set; }

        public string? EstadoProducto { get; set; }

        public string? NombreCalidad { get; set; }

        public string? NombreColor { get; set; }

    }
}