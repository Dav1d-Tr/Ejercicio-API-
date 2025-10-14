using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;// Si instalas MySqlConnector usa: using MySqlConnector;
using webapicsharp.Repositorios.Abstracciones;
using webapicsharp.Servicios.Abstracciones;
using webapicsharp.Servicios.Utilidades;

namespace webapicsharp.Repositorios
{
    public class RepositorioLecturaMariaDB : IRepositorioLecturaTabla
    {
        private readonly IProveedorConexion _proveedorConexion;

        public RepositorioLecturaMariaDB(IProveedorConexion proveedorConexion)
        {
            _proveedorConexion = proveedorConexion ?? throw new ArgumentNullException(nameof(proveedorConexion));
        }

        private MySqlConnection GetConexion(string cadena)
        {
            return new MySqlConnection(cadena);
        }

        private string FormatearNombreTabla(string? esquema, string tabla)
        {
            if (string.IsNullOrWhiteSpace(esquema))
                return $"`{tabla}`";
            return $"`{esquema}`.`{tabla}`"; // esquema como database (opcional)
        }

        public async Task<IReadOnlyList<Dictionary<string, object?>>> ObtenerFilasAsync(
            string nombreTabla,
            string? esquema,
            int? limite)
        {
            if (string.IsNullOrWhiteSpace(nombreTabla))
                throw new ArgumentException("El nombre de la tabla no puede estar vacío.", nameof(nombreTabla));

            string objeto = FormatearNombreTabla(esquema, nombreTabla);
            string sql = $"SELECT * FROM {objeto}";

            if (limite.HasValue && limite.Value > 0)
                sql += $" LIMIT {limite.Value}";

            var resultados = new List<Dictionary<string, object?>>();

            try
            {
                string cadena = _proveedorConexion.ObtenerCadenaConexion();
                await using var conexion = GetConexion(cadena);
                await conexion.OpenAsync();

                await using var comando = new MySqlCommand(sql, conexion);
                await using var lector = await comando.ExecuteReaderAsync();

                while (await lector.ReadAsync())
                {
                    var fila = new Dictionary<string, object?>();

                    for (int i = 0; i < lector.FieldCount; i++)
                    {
                        var nombreCol = lector.GetName(i);
                        var valor = await lector.IsDBNullAsync(i) ? null : lector.GetValue(i);
                        fila[nombreCol] = valor;
                    }

                    resultados.Add(fila);
                }

                return resultados.AsReadOnly();
            }
            catch (MySqlException ex)
            {
                throw new InvalidOperationException(
                    $"Error MariaDB al obtener filas de '{esquema ?? "(default)"}.{nombreTabla}': {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Error inesperado al obtener filas de '{esquema ?? "(default)"}.{nombreTabla}': {ex.Message}", ex);
            }
        }

        public async Task<IReadOnlyList<Dictionary<string, object?>>> ObtenerPorClaveAsync(
            string nombreTabla,
            string? esquema,
            string nombreClave,
            string valor)
        {
            if (string.IsNullOrWhiteSpace(nombreTabla))
                throw new ArgumentException("El nombre de la tabla no puede estar vacío.", nameof(nombreTabla));

            if (string.IsNullOrWhiteSpace(nombreClave))
                throw new ArgumentException("El nombre de la clave no puede estar vacío.", nameof(nombreClave));

            string objeto = FormatearNombreTabla(esquema, nombreTabla);
            string sql = $"SELECT * FROM {objeto} WHERE `{nombreClave}` = @valor";

            var resultados = new List<Dictionary<string, object?>>();

            try
            {
                string cadena = _proveedorConexion.ObtenerCadenaConexion();
                await using var conexion = GetConexion(cadena);
                await conexion.OpenAsync();

                await using var comando = new MySqlCommand(sql, conexion);
                comando.Parameters.AddWithValue("@valor", valor);

                await using var lector = await comando.ExecuteReaderAsync();

                while (await lector.ReadAsync())
                {
                    var fila = new Dictionary<string, object?>();
                    for (int i = 0; i < lector.FieldCount; i++)
                    {
                        var nombreCol = lector.GetName(i);
                        var valorCol = await lector.IsDBNullAsync(i) ? null : lector.GetValue(i);
                        fila[nombreCol] = valorCol;
                    }
                    resultados.Add(fila);
                }

                return resultados.AsReadOnly();
            }
            catch (MySqlException ex)
            {
                throw new InvalidOperationException(
                    $"Error MariaDB al filtrar '{esquema ?? "(default)"}.{nombreTabla}' por '{nombreClave}': {ex.Message}", ex);
            }
        }

        public async Task<bool> CrearAsync(
            string nombreTabla,
            string? esquema,
            Dictionary<string, object?> datos,
            string? camposEncriptar = null)
        {
            if (string.IsNullOrWhiteSpace(nombreTabla))
                throw new ArgumentException("El nombre de la tabla no puede estar vacío.", nameof(nombreTabla));
            if (datos == null || !datos.Any())
                throw new ArgumentException("Los datos no pueden estar vacíos.", nameof(datos));

            var datosFinales = new Dictionary<string, object?>(datos);

            if (!string.IsNullOrWhiteSpace(camposEncriptar))
            {
                var campos = camposEncriptar.Split(',').Select(c => c.Trim()).Where(c => !string.IsNullOrEmpty(c));
                foreach (var campo in campos)
                {
                    if (datosFinales.ContainsKey(campo) && datosFinales[campo] != null)
                    {
                        datosFinales[campo] = EncriptacionBCrypt.Encriptar(datosFinales[campo]!.ToString()!);
                    }
                }
            }

            string objeto = FormatearNombreTabla(esquema, nombreTabla);
            var columnas = string.Join(", ", datosFinales.Keys.Select(k => $"`{k}`"));
            var parametros = string.Join(", ", datosFinales.Keys.Select((k, i) => $"@p{i}"));
            string sql = $"INSERT INTO {objeto} ({columnas}) VALUES ({parametros})";

            try
            {
                string cadena = _proveedorConexion.ObtenerCadenaConexion();
                await using var conexion = GetConexion(cadena);
                await conexion.OpenAsync();

                await using var comando = new MySqlCommand(sql, conexion);
                int idx = 0;
                foreach (var kvp in datosFinales)
                {
                    comando.Parameters.AddWithValue($"@p{idx}", kvp.Value ?? DBNull.Value);
                    idx++;
                }

                int filas = await comando.ExecuteNonQueryAsync();
                return filas > 0;
            }
            catch (MySqlException ex)
            {
                throw new InvalidOperationException(
                    $"Error MariaDB al insertar en '{esquema ?? "(default)"}.{nombreTabla}': {ex.Message}", ex);
            }
        }

        public async Task<int> ActualizarAsync(
            string nombreTabla,
            string? esquema,
            string nombreClave,
            string valorClave,
            Dictionary<string, object?> datos,
            string? camposEncriptar = null)
        {
            if (string.IsNullOrWhiteSpace(nombreTabla))
                throw new ArgumentException("El nombre de la tabla no puede estar vacío.", nameof(nombreTabla));
            if (string.IsNullOrWhiteSpace(nombreClave))
                throw new ArgumentException("El nombre de la clave no puede estar vacío.", nameof(nombreClave));
            if (datos == null || !datos.Any())
                throw new ArgumentException("Los datos a actualizar no pueden estar vacíos.", nameof(datos));

            var datosFinales = new Dictionary<string, object?>(datos);

            if (!string.IsNullOrWhiteSpace(camposEncriptar))
            {
                var campos = camposEncriptar.Split(',').Select(c => c.Trim());
                foreach (var campo in campos)
                {
                    if (datosFinales.ContainsKey(campo) && datosFinales[campo] != null)
                    {
                        datosFinales[campo] = EncriptacionBCrypt.Encriptar(datosFinales[campo]!.ToString()!);
                    }
                }
            }

            string objeto = FormatearNombreTabla(esquema, nombreTabla);
            var sets = string.Join(", ", datosFinales.Keys.Select((k, i) => $"`{k}` = @p{i}"));
            string sql = $"UPDATE {objeto} SET {sets} WHERE `{nombreClave}` = @valorClave";

            try
            {
                string cadena = _proveedorConexion.ObtenerCadenaConexion();
                await using var conexion = GetConexion(cadena);
                await conexion.OpenAsync();

                await using var comando = new MySqlCommand(sql, conexion);
                int idx = 0;
                foreach (var kvp in datosFinales)
                {
                    comando.Parameters.AddWithValue($"@p{idx}", kvp.Value ?? DBNull.Value);
                    idx++;
                }
                comando.Parameters.AddWithValue("@valorClave", valorClave);

                int filas = await comando.ExecuteNonQueryAsync();
                return filas;
            }
            catch (MySqlException ex)
            {
                throw new InvalidOperationException(
                    $"Error MariaDB al actualizar '{esquema ?? "(default)"}.{nombreTabla}': {ex.Message}", ex);
            }
        }

        public async Task<int> EliminarAsync(
            string nombreTabla,
            string? esquema,
            string nombreClave,
            string valorClave)
        {
            if (string.IsNullOrWhiteSpace(nombreTabla))
                throw new ArgumentException("El nombre de la tabla no puede estar vacío.", nameof(nombreTabla));
            if (string.IsNullOrWhiteSpace(nombreClave))
                throw new ArgumentException("El nombre de la clave no puede estar vacío.", nameof(nombreClave));

            string objeto = FormatearNombreTabla(esquema, nombreTabla);
            string sql = $"DELETE FROM {objeto} WHERE `{nombreClave}` = @valorClave";

            try
            {
                string cadena = _proveedorConexion.ObtenerCadenaConexion();
                await using var conexion = GetConexion(cadena);
                await conexion.OpenAsync();

                await using var comando = new MySqlCommand(sql, conexion);
                comando.Parameters.AddWithValue("@valorClave", valorClave);

                int filas = await comando.ExecuteNonQueryAsync();
                return filas;
            }
            catch (MySqlException ex)
            {
                throw new InvalidOperationException(
                    $"Error MariaDB al eliminar de '{esquema ?? "(default)"}.{nombreTabla}': {ex.Message}", ex);
            }
        }

        public async Task<string?> ObtenerHashContrasenaAsync(
            string nombreTabla,
            string? esquema,
            string campoUsuario,
            string campoContrasena,
            string valorUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreTabla))
                throw new ArgumentException("El nombre de la tabla no puede estar vacío.", nameof(nombreTabla));
            if (string.IsNullOrWhiteSpace(campoUsuario))
                throw new ArgumentException("El campo de usuario no puede estar vacío.", nameof(campoUsuario));
            if (string.IsNullOrWhiteSpace(campoContrasena))
                throw new ArgumentException("El campo de contraseña no puede estar vacío.", nameof(campoContrasena));

            string objeto = FormatearNombreTabla(esquema, nombreTabla);
            string sql = $"SELECT `{campoContrasena}` FROM {objeto} WHERE `{campoUsuario}` = @valorUsuario LIMIT 1";

            try
            {
                string cadena = _proveedorConexion.ObtenerCadenaConexion();
                await using var conexion = GetConexion(cadena);
                await conexion.OpenAsync();

                await using var comando = new MySqlCommand(sql, conexion);
                comando.Parameters.AddWithValue("@valorUsuario", valorUsuario);

                var resultado = await comando.ExecuteScalarAsync();
                return resultado?.ToString();
            }
            catch (MySqlException ex)
            {
                throw new InvalidOperationException(
                    $"Error MariaDB al obtener hash de contraseña en '{esquema ?? "(default)"}.{nombreTabla}': {ex.Message}", ex);
            }
        }
    }
}
