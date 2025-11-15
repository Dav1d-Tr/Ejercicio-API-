using System;
using System.Text.Json.Serialization;

namespace FrontendBlazorApi.Models
{
    public class Actividad
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("idEntregable")]
        public int IdEntregable { get; set; }

        [JsonPropertyName("titulo")]
        public string Titulo { get; set; } = string.Empty;

        [JsonPropertyName("descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        [JsonPropertyName("fechaInicio")]
        public DateTime FechaInicio { get; set; } = DateTime.Now;

        [JsonPropertyName("fechaFinPrevista")]
        public DateTime FechaFinPrevista { get; set; } = DateTime.Now;

        [JsonPropertyName("fechaModificacion")]
        public DateTime FechaModificacion { get; set; } = DateTime.Now;

        [JsonPropertyName("fechaFinalizacion")]
        public DateTime FechaFinalizacion { get; set; } = DateTime.Now;
 
        [JsonPropertyName("prioridad")]
        public int Prioridad { get; set; }

        [JsonPropertyName("porcentajeAvance")]
        public int PorcentajeAvance { get; set; }
    }
}
