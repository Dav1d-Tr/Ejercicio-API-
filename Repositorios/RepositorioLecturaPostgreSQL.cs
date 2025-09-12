using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using webapicsharp.Repositorios.Abstracciones;
using webapicsharp.Servicios.Abstracciones;

namespace webapicsharp.Repositorios
{
    public class RepositorioLecturaPostgreSQL : IRepositorioLecturaTabla
    {
        private readonly IProveedorConexion _proveedorConexion;

        public RepositorioLecturaPostgreSQL(IProveedorConexion proveedorConexion)
        {
            _proveedorConexion = proveedorConexion ?? throw new ArgumentNullException(nameof(proveedorConexion));
        }

        public async Task<IReadOnlyList<Dictionary<string, object?>>> ObtenerFilasAsync(
            string nombreTabla,
            string? esquema,
            int? limite)
        {
            if (string.IsNullOrWhiteSpace(nombreTabla))
                throw new ArgumentException("El nombre de la tabla no puede estar vacía.", nameof(nombreTabla));

            string esquemaFinal = string.IsNullOrWhiteSpace(esquema) ? "public" : esquema.Trim();
            string sql = $"SELECT * FROM \"{esquemaFinal}\".\"{nombreTabla}\"";

            if (limite.HasValue && limite.Value > 0)
                sql += $" LIMIT {limite.Value}";

            try
            {
                string cadenaConexion = _proveedorConexion.ObtenerCadenaConexion();
                await using var conexion = new NpgsqlConnection(cadenaConexion);
                await conexion.OpenAsync();

                await using var comando = new NpgsqlCommand(sql, conexion);
                await using var lector = await comando.ExecuteReaderAsync();

                var resultado = new List<Dictionary<string, object?>>();
                while (await lector.ReadAsync())
                {
                    var fila = new Dictionary<string, object?>();
                    for (int i = 0; i < lector.FieldCount; i++)
                    {
                        fila[lector.GetName(i)] = await lector.IsDBNullAsync(i) ? null : lector.GetValue(i);
                    }
                    resultado.Add(fila);
                }

                return resultado.AsReadOnly();
            }
            catch (NpgsqlException ex)
            {
                throw new InvalidOperationException(
                    $"Error PostgreSQL al obtener filas de '{esquemaFinal}.{nombreTabla}': {ex.Message}", ex);
            }
        }

        public async Task<IReadOnlyList<Dictionary<string, object?>>> ObtenerPorClaveAsync(
            string nombreTabla,
            string? esquema,
            string nombreClave,
            string valor)
        {
            if (string.IsNullOrWhiteSpace(nombreTabla) || string.IsNullOrWhiteSpace(nombreClave))
                throw new ArgumentException("Nombre de tabla o clave no válidos.");

            string esquemaFinal = string.IsNullOrWhiteSpace(esquema) ? "public" : esquema.Trim();
            string sql = $"SELECT * FROM \"{esquemaFinal}\".\"{nombreTabla}\" WHERE \"{nombreClave}\" = @valor";

            try
            {
                string cadenaConexion = _proveedorConexion.ObtenerCadenaConexion();
                await using var conexion = new NpgsqlConnection(cadenaConexion);
                await conexion.OpenAsync();

                await using var comando = new NpgsqlCommand(sql, conexion);
                comando.Parameters.AddWithValue("@valor", valor);

                await using var lector = await comando.ExecuteReaderAsync();
                var resultado = new List<Dictionary<string, object?>>();

                while (await lector.ReadAsync())
                {
                    var fila = new Dictionary<string, object?>();
                    for (int i = 0; i < lector.FieldCount; i++)
                    {
                        fila[lector.GetName(i)] = await lector.IsDBNullAsync(i) ? null : lector.GetValue(i);
                    }
                    resultado.Add(fila);
                }

                return resultado.AsReadOnly();
            }
            catch (NpgsqlException ex)
            {
                throw new InvalidOperationException(
                    $"Error PostgreSQL al obtener filas por clave de '{esquemaFinal}.{nombreTabla}': {ex.Message}", ex);
            }
        }

        public async Task<bool> CrearAsync(string nombreTabla, string? esquema, Dictionary<string, object?> datos, string? camposEncriptar = null)
        {
            if (string.IsNullOrWhiteSpace(nombreTabla))
                throw new ArgumentException("El nombre de la tabla no puede estar vacío.", nameof(nombreTabla));
            if (datos == null || !datos.Any())
                throw new ArgumentException("Los datos no pueden estar vacíos.", nameof(datos));

            string esquemaFinal = string.IsNullOrWhiteSpace(esquema) ? "public" : esquema.Trim();
            var datosFinales = new Dictionary<string, object?>(datos);

            // Encriptación opcional con BCrypt
            if (!string.IsNullOrWhiteSpace(camposEncriptar))
            {
                var campos = camposEncriptar.Split(',').Select(c => c.Trim()).Where(c => !string.IsNullOrEmpty(c));
                foreach (var campo in campos)
                {
                    if (datosFinales.ContainsKey(campo) && datosFinales[campo] != null)
                    {
                        string valorOriginal = datosFinales[campo]?.ToString() ?? "";
                        datosFinales[campo] = webapicsharp.Servicios.Utilidades.EncriptacionBCrypt.Encriptar(valorOriginal);
                    }
                }
            }

            var columnas = string.Join(", ", datosFinales.Keys.Select(k => $"\"{k}\""));
            var parametros = string.Join(", ", datosFinales.Keys.Select((k, i) => $"@p{i}"));

            string consultaSql = $"INSERT INTO \"{esquemaFinal}\".\"{nombreTabla}\" ({columnas}) VALUES ({parametros})";

            try
            {
                string cadenaConexion = _proveedorConexion.ObtenerCadenaConexion();
                await using var conexion = new NpgsqlConnection(cadenaConexion);
                await conexion.OpenAsync();

                await using var comando = new NpgsqlCommand(consultaSql, conexion);

                int index = 0;
                foreach (var kvp in datosFinales)
                {
                    comando.Parameters.AddWithValue($"@p{index}", kvp.Value ?? DBNull.Value);
                    index++;
                }

                int filasAfectadas = await comando.ExecuteNonQueryAsync();
                return filasAfectadas > 0;
            }
            catch (NpgsqlException ex)
            {
                throw new InvalidOperationException(
                    $"Error PostgreSQL al insertar en '{esquemaFinal}.{nombreTabla}': {ex.Message}", ex);
            }
        }

        public async Task<int> ActualizarAsync(string nombreTabla, string? esquema, string nombreClave, string valorClave, Dictionary<string, object?> datos, string? camposEncriptar = null)
        {
            if (string.IsNullOrWhiteSpace(nombreTabla) || string.IsNullOrWhiteSpace(nombreClave))
                throw new ArgumentException("Tabla o clave no válidos.");
            if (datos == null || !datos.Any())
                throw new ArgumentException("Los datos no pueden estar vacíos.");

            string esquemaFinal = string.IsNullOrWhiteSpace(esquema) ? "public" : esquema.Trim();
            var datosFinales = new Dictionary<string, object?>(datos);

            // Encriptar si corresponde
            if (!string.IsNullOrWhiteSpace(camposEncriptar))
            {
                var campos = camposEncriptar.Split(',').Select(c => c.Trim());
                foreach (var campo in campos)
                {
                    if (datosFinales.ContainsKey(campo))
                        datosFinales[campo] = webapicsharp.Servicios.Utilidades.EncriptacionBCrypt.Encriptar(datosFinales[campo]?.ToString() ?? "");
                }
            }

            var sets = datosFinales.Keys.Select((k, i) => $"\"{k}\" = @p{i}");
            string consultaSql = $"UPDATE \"{esquemaFinal}\".\"{nombreTabla}\" SET {string.Join(", ", sets)} WHERE \"{nombreClave}\" = @valorClave";

            try
            {
                string cadenaConexion = _proveedorConexion.ObtenerCadenaConexion();
                await using var conexion = new NpgsqlConnection(cadenaConexion);
                await conexion.OpenAsync();

                await using var comando = new NpgsqlCommand(consultaSql, conexion);

                int index = 0;
                foreach (var kvp in datosFinales)
                {
                    comando.Parameters.AddWithValue($"@p{index}", kvp.Value ?? DBNull.Value);
                    index++;
                }
                comando.Parameters.AddWithValue("@valorClave", valorClave);

                return await comando.ExecuteNonQueryAsync();
            }
            catch (NpgsqlException ex)
            {
                throw new InvalidOperationException(
                    $"Error PostgreSQL al actualizar '{esquemaFinal}.{nombreTabla}': {ex.Message}", ex);
            }
        }

        public async Task<int> EliminarAsync(string nombreTabla, string? esquema, string nombreClave, string valorClave)
        {
            string esquemaFinal = string.IsNullOrWhiteSpace(esquema) ? "public" : esquema.Trim();
            string consultaSql = $"DELETE FROM \"{esquemaFinal}\".\"{nombreTabla}\" WHERE \"{nombreClave}\" = @valorClave";

            try
            {
                string cadenaConexion = _proveedorConexion.ObtenerCadenaConexion();
                await using var conexion = new NpgsqlConnection(cadenaConexion);
                await conexion.OpenAsync();

                await using var comando = new NpgsqlCommand(consultaSql, conexion);
                comando.Parameters.AddWithValue("@valorClave", valorClave);

                return await comando.ExecuteNonQueryAsync();
            }
            catch (NpgsqlException ex)
            {
                throw new InvalidOperationException(
                    $"Error PostgreSQL al eliminar de '{esquemaFinal}.{nombreTabla}': {ex.Message}", ex);
            }
        }

        public async Task<string?> ObtenerHashContrasenaAsync(string nombreTabla, string? esquema, string campoUsuario, string campoContrasena, string valorUsuario)
        {
            string esquemaFinal = string.IsNullOrWhiteSpace(esquema) ? "public" : esquema.Trim();
            string consultaSql = $"SELECT \"{campoContrasena}\" FROM \"{esquemaFinal}\".\"{nombreTabla}\" WHERE \"{campoUsuario}\" = @valorUsuario";

            try
            {
                string cadenaConexion = _proveedorConexion.ObtenerCadenaConexion();
                await using var conexion = new NpgsqlConnection(cadenaConexion);
                await conexion.OpenAsync();

                await using var comando = new NpgsqlCommand(consultaSql, conexion);
                comando.Parameters.AddWithValue("@valorUsuario", valorUsuario);

                var resultado = await comando.ExecuteScalarAsync();
                return resultado?.ToString();
            }
            catch (NpgsqlException ex)
            {
                throw new InvalidOperationException(
                    $"Error PostgreSQL al obtener hash de contraseña en '{esquemaFinal}.{nombreTabla}': {ex.Message}", ex);
            }
        }
    }
}
