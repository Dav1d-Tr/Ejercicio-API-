using System;
using System.Text.Json.Serialization;

namespace FrontendBlazorApi.Models
{
    public class ObjetivoEstrategico
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("idVariable")]
        public int IdVariable { get; set; }

        [JsonPropertyName("titulo")]
        public string Titulo { get; set; }

        [JsonPropertyName("descripcion")]
        public string Descripcion { get; set; }
    }
}
