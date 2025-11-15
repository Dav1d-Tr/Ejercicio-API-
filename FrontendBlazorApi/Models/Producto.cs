using System;
using System.Text.Json.Serialization;

namespace FrontendBlazorApi.Models
{
    public class Producto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("idTipoProducto")]
        public int IdTipoProducto { get; set; }

        [JsonPropertyName("codigo")]
        public string Codigo { get; set; } = string.Empty;

        [JsonPropertyName("titulo")]
        public string Titulo { get; set; } = string.Empty;

        [JsonPropertyName("descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        [JsonPropertyName("fechaInicio")]
        public DateTime FechaInicio { get; set; } = DateTime.Now;

        [JsonPropertyName("fechaFinPrevista")]
        public DateTime FechaFinPrevista { get; set; }

        [JsonPropertyName("fechaModificacion")]
        public DateTime FechaModificacion { get; set; }

        [JsonPropertyName("fechaFinalizacion")]        
        public DateTime FechaFinalizacion { get; set; }

        [JsonPropertyName("rutaLogo")]
        public string RutaLogo { get; set; } = string.Empty;

    }
}
