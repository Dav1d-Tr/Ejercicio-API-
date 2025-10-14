using System.Text.Json.Serialization;

namespace FrontendBlazorApi.Models
{
    public class Entregable
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; } = DateTime.Now;
        public DateTime FechaFinPrevista { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;
        public DateTime FechaFinalizacion { get; set; } = DateTime.Now;
    }

    // Clase genérica para mapear la respuesta de la API
}

    
