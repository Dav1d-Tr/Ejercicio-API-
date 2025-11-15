using System;
using System.Text.Json.Serialization;

namespace FrontendBlazorApi.Models
{
    public class Proyecto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("idProyectoPadre")]
        public int IdProyectoPadre { get; set; }

        [JsonPropertyName("idResponsable")]
        public int IdResponsable { get; set; }

        [JsonPropertyName("IdTipoProyecto")]
        public int IdTipoProyecto { get; set; }

        [JsonPropertyName("codigo")]
        public string Codigo { get; set; }

        [JsonPropertyName("titulo")]
        public string Titulo { get; set; }

        [JsonPropertyName("descripcion")]
        public string Descripcion { get; set; }

        [JsonPropertyName("fechaInicio")]
        public DateTime FechaInicio { get; set; } = DateTime.Now;

        [JsonPropertyName("fechaFinPrevista")]
        public DateTime FechaFinPrevista { get; set; } = DateTime.Now;

        [JsonPropertyName("fechaModificacion")]
        public DateTime FechaModificacion { get; set; } = DateTime.Now;

        [JsonPropertyName("fechaFinalizacion")]
        public DateTime FechaFinalizacion { get; set; } = DateTime.Now;

        [JsonPropertyName("rutaLogo")]
        public string RutaLogo { get; set; }

    }
}
