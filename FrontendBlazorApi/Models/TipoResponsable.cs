using System.Text.Json.Serialization;

namespace FrontendBlazorApi.Models
{
    public class TipoResponsable
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }

    // Clase genérica para mapear la respuesta de la API

}

    
