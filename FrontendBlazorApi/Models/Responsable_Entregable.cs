using System;
using System.Text.Json.Serialization;

namespace FrontendBlazorApi.Models
{
    public class Responsable_Entregable
    {
        [JsonPropertyName("idResponsable")]
        public int IdResponsable { get; set; }

        [JsonPropertyName("idEntregable")]
        public int IdEntregable { get; set; }

        [JsonPropertyName("fechaAsociacion")]
        public DateTime FechaAsociacion { get; set; }

    }
}
