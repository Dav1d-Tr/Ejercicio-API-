using System.Text.Json.Serialization;

namespace FrontendBlazorApi.Models
{
    public class Producto
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public int Stock { get; set; }

        [JsonPropertyName("valorunitario")]   // Mapea con el JSON de la API
        public double ValorUnitario { get; set; }
    }

    // Clase genérica para mapear la respuesta de la API
    //public class RespuestaApi<T>
    //{
        //public T? Datos { get; set; }
    //}
}
