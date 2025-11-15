using System;
using System.Text.Json.Serialization;

namespace FrontendBlazorApi.Models
{
    public class Producto_Entregable
    {
        [JsonPropertyName("idProducto")]
        public int IdProducto { get; set; }

        [JsonPropertyName("idEntregable")]
        public int IdEntregable { get; set; }

        [JsonPropertyName("fechaAsociacion")]
        public DateTime FechaAsociacion { get; set; }
    }
}