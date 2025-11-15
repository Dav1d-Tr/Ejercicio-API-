using System;
using System.Text.Json.Serialization;

namespace FrontendBlazorApi.Models
{
    public class Responsable
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("idTipoResponsable")]
        public int IdTipoResponsable { get; set; }

        [JsonPropertyName("idUsuario")]
        public int IdUsuario { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;

    }
}
