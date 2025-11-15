using System;
using System.Text.Json.Serialization;

namespace FrontendBlazorApi.Models
{
    public class Estado_Proyecto
    {
        [JsonPropertyName("idProyecto")]
        public int IdProyecto { get; set; }

        [JsonPropertyName("idEstado")]
        public int IdEstado { get; set; }

    }
}
