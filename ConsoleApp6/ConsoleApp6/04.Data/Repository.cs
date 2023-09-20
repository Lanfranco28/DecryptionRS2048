using Presto.Core.SQL.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DecryptionRS2048.Entity.Entidades;

namespace DecryptionRS2048.Repository
{
    public class Data : BaseUnitOfWork
    {
        public Data(string connectionString) : base(connectionString) { }

        public async Task<IEnumerable<Archivo>> ObtenerTodosArchivos()
        {
            try
            {
                string sql = @"SELECT [NOMBRE_ARCHIVO] nombreArchivo
                                     ,[EXTENSION] extension
                                     ,[PESO] peso
                                     ,[ACTIVO] activo
                                FROM [dbo].[archivo]";
                

                return await this.ExecuteReaderAsync<Archivo>(sql, CommandType.Text, null, 30);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<int> Guardar(Archivo request)
        {
            try
            {
                string sql = @"INSERT INTO archivo (NOMBRE_ARCHIVO, EXTENSION, PESO, ACTIVO)
                                    output inserted.ID_ARCHIVO
                                   VALUES(@NOMBRE_ARCHIVO, @EXTENSION, @PESO, @ACTIVO)";

                var parm = new Parameter[]
                {
                    new Parameter("@NOMBRE_ARCHIVO", request.nombreArchivo),
                    new Parameter("@EXTENSION", request.extension),
                    new Parameter("@PESO", request.peso),
                    new Parameter("@ACTIVO", request.activo)
                };
                return await this.ExecuteScalarAsync<int>(sql, commandType: System.Data.CommandType.Text, parm,commandTimeout: 30 );
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        public async Task<int> GuardarDetalle(DetalleArchivo request)
        {
            try
            {
                string sql = @"INSERT INTO detalle_archivo (ID_ARCHIVO, CAMPO1, CAMPO2, ACTIVO, OBSERVACIONES)
                                    output inserted.ID_DETALLE_ARCHIVO
                                   VALUES(@ID_ARCHIVO, @CAMPO1, @CAMPO2, @ACTIVO, @OBSERVACIONES)";

                var parm = new Parameter[]
                {
                    new Parameter("@ID_ARCHIVO", request.idArchivo),
                    new Parameter("@CAMPO1", request.campo1),
                    new Parameter("@CAMPO2", request.campo2),
                    new Parameter("@ACTIVO", request.activo),
                    new Parameter("@OBSERVACIONES", request.observaciones)
                };
                return await this.ExecuteScalarAsync<int>(sql, commandType: System.Data.CommandType.Text, parm, commandTimeout: 30);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<int> GuardarError(Error request)
        {
            try
            {
                string sql = @"INSERT INTO error (DESCRIPCION, ID_ARCHIVO, FECHA_CREACION, NUMERO_LINEA)
                                    output inserted.ID_ERROR
                                   VALUES(@DESCRIPCION, @ID_ARCHIVO, @FECHA_CREACION, @NUMERO_LINEA)";

                var parm = new Parameter[]
                {
                    new Parameter("@DESCRIPCION", request.descripcion),
                    new Parameter("@ID_ARCHIVO", request.idArchivo),
                    new Parameter("@FECHA_CREACION", DateTime.Now),
                     new Parameter("@NUMERO_LINEA", request.numeroLinea),
                };
                return await this.ExecuteScalarAsync<int>(sql, commandType: System.Data.CommandType.Text, parm, commandTimeout: 30);
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        public async Task<IEnumerable<DetalleArchivo>> ObtenerArchivoPorIdentificador(string campo1)
        {
            try
            {
                string sql = @"SELECT [ID_ARCHIVO]
                                    ,[CAMPO1]
                                    ,[CAMPO2]
                                    ,[ACTIVO]
                                    ,[OBSERVACIONES]
                                FROM [dbo].[detalle_archivo]
                                WHERE  CAMPO1 = @CAMPO1";
                var parm = new Parameter[]
                {
                    new Parameter("@CAMPO1", campo1),
                };


                return await this.ExecuteReaderAsync<DetalleArchivo>(sql, CommandType.Text, null, 30);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
