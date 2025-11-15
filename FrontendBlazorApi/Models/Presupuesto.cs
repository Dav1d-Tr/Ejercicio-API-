using System;
using System.Text.Json.Serialization;

namespace FrontendBlazorApi.Models
{
    public class Presupuesto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("idProyecto")]
        public int IdProyecto { get; set; }

        [JsonPropertyName("montoSolicitado")]
        public decimal MontoSolicitado { get; set; }

        [JsonPropertyName("estado")]
        public string Estado { get; set; } = string.Empty;

        [JsonPropertyName("montoAprobado")]
        public decimal MontoAprobado { get; set; }

        [JsonPropertyName("periodoAnio")]
        public int PeriodoAnio { get; set; }

        [JsonPropertyName("fechaSolicitud")]
        public DateTime FechaSolicitud { get; set; } = DateTime.Now;

        [JsonPropertyName("fechaAprobacion")]
        public DateTime FechaAprobacion { get; set; } = DateTime.Now;

        [JsonPropertyName("observaciones")]
        public string Observaciones { get; set; } = string.Empty;

    }
}