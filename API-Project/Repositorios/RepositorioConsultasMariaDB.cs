using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient; // Para mantener compatibilidad con la interfaz
using MySqlConnector;           // Driver MariaDB/MySQL
using webapicsharp.Repositorios.Abstracciones;        // Para IRepositorioConsultas
using webapicsharp.Servicios.Abstracciones;   

namespace webapicsharp.Repositorios
{
    public class RepositorioConsultasMariaDB : IRepositorioConsultas
    {
        private readonly string _connectionString;

        public RepositorioConsultasMariaDB(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MariaDB")!;

        }

        public async Task<DataTable> EjecutarConsultaParametrizadaAsync(string consultaSQL, List<SqlParameter>? parametros)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand(consultaSQL, conn);

            if (parametros != null)
            {
                foreach (var p in parametros)
                {
                    cmd.Parameters.AddWithValue(p.ParameterName, p.Value ?? DBNull.Value);
                }
            }

            var dt = new DataTable();
            using var reader = await cmd.ExecuteReaderAsync();
            dt.Load(reader);
            return dt;
        }

        public async Task<(bool esValida, string? mensajeError)> ValidarConsultaAsync(string consultaSQL, List<SqlParameter>? parametros)
        {
            try
            {
                await EjecutarConsultaParametrizadaAsync(consultaSQL, parametros);
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<DataTable> EjecutarProcedimientoAlmacenadoAsync(string nombreSP, List<SqlParameter>? parametros)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand(nombreSP, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parametros != null)
            {
                foreach (var p in parametros)
                {
                    cmd.Parameters.AddWithValue(p.ParameterName, p.Value ?? DBNull.Value);
                }
            }

            var dt = new DataTable();
            using var reader = await cmd.ExecuteReaderAsync();
            dt.Load(reader);
            return dt;
        }

        public async Task<string?> ObtenerEsquemaTablaAsync(string nombreTabla, string esquemaPredeterminado)
        {
            var sql = @"SELECT table_schema 
                        FROM information_schema.tables 
                        WHERE table_name = @tabla 
                        LIMIT 1";

            var parametros = new List<SqlParameter> { new SqlParameter("@tabla", nombreTabla) };
            var dt = await EjecutarConsultaParametrizadaAsync(sql, parametros);

            return dt.Rows.Count > 0 ? dt.Rows[0]["table_schema"].ToString() : null;
        }

        public async Task<DataTable> ObtenerEstructuraTablaAsync(string nombreTabla, string esquema)
        {
            var sql = @"SELECT column_name, data_type, is_nullable 
                        FROM information_schema.columns 
                        WHERE table_schema = @esquema AND table_name = @tabla";

            var parametros = new List<SqlParameter> {
                new SqlParameter("@esquema", esquema),
                new SqlParameter("@tabla", nombreTabla)
            };

            return await EjecutarConsultaParametrizadaAsync(sql, parametros);
        }

        public async Task<DataTable> ObtenerEstructuraBaseDatosAsync(string? nombreBD)
        {
            var sql = @"SELECT table_schema, table_name, column_name, data_type 
                        FROM information_schema.columns 
                        ORDER BY table_schema, table_name, ordinal_position";

            return await EjecutarConsultaParametrizadaAsync(sql, null);
        }
    }
}
