using System;
using System.Text.Json.Serialization;

namespace FrontendBlazorApi.Models
{
    public class MetaEstrategica
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("idObjetivo")]
        public int IdObjetivo { get; set; }

        [JsonPropertyName("titulo")]
        public string Titulo { get; set; }

        [JsonPropertyName("descripcion")]
        public string Descripcion { get; set; }
    }
}
