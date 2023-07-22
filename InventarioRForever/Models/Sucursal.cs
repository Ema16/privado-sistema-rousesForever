using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InventarioRForever.Models;

public partial class Sucursal
{

    public int CodSucursal { get; set; }

    [Required(ErrorMessage = "El nombre la sucursal es necesario")]
    public string? NombreSucursal { get; set; }


    [Required(ErrorMessage = "La dirección de la sucursal es necesario")]
    public string? Direccion { get; set; }


    [Required(ErrorMessage = "El nombre del municipio es necesario")]
    public string? Municipio { get; set; }


    [Required(ErrorMessage = "El nombre del departamento es necesario")]
    public string? Departamento { get; set; }


    [Required(ErrorMessage = "El telefono es necesario")]
    public string? Telefono { get; set; }

    [Required(ErrorMessage = "Las observaciones son necesarias")]
    public string? Observacion { get; set; }

    public virtual ICollection<Ventum> Venta { get; set; } = new List<Ventum>();
}
