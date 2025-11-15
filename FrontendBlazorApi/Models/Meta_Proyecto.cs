using System;
using System.Text.Json.Serialization;

namespace FrontendBlazorApi.Models
{
    public class Meta_Proyecto
    {
        [JsonPropertyName("idMeta")]
        public int IdMeta { get; set; }

        [JsonPropertyName("idProyecto")]
        public int IdProyecto { get; set; }

        [JsonPropertyName("fechaAsociacion")]
        public DateTime FechaAsociacion { get; set; } = DateTime.Now;
    }
}