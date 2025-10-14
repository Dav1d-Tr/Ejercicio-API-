using System.Text.Json.Serialization;

namespace FrontendBlazorApi.Models
{
    // Clase genérica para mapear la respuesta de la API
    public class RespuestaApi<T>
    {
        public T? Datos { get; set; }
    }
}
