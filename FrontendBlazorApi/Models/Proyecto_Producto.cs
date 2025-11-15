using System;
using System.Text.Json.Serialization;

namespace FrontendBlazorApi.Models
{
    public class Proyecto_Producto
    {
        [JsonPropertyName("idProducto")]
        public int IdProducto { get; set; }

        [JsonPropertyName("idProyecto")]
        public int IdProyecto { get; set; }

        [JsonPropertyName("fechaAsociacion")]
        public DateTime FechaAsociacion { get; set; } = DateTime.Now;
    }
}