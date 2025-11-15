using System;
using System.Text.Json.Serialization;

namespace FrontendBlazorApi.Models
{
    public class Archivo
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("idUsuario")]
        public int IdUsuario { get; set; }

        [JsonPropertyName("ruta")]
        public string Ruta { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; }

        [JsonPropertyName("tipo")]
        public string Tipo { get; set; }

        [JsonPropertyName("fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
