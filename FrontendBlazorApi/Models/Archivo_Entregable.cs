using System;
using System.Text.Json.Serialization;

namespace FrontendBlazorApi.Models
{
    public class Archivo_Entregable
    {
        [JsonPropertyName("idArchivo")]
        public int IdArchivo { get; set; }

        [JsonPropertyName("idEntregable")]
        public int IdEntregable { get; set; }

    }
}
