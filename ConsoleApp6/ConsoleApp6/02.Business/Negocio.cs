using DecryptionRS2048.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DecryptionRS2048.Entity.Entidades;

namespace DecryptionRS2048.Business
{
    public class Negocio
    {
        private string _connectionString;
        public Negocio(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<IEnumerable<Archivo>> ObtenerTodosArchivo()
        {
            return await new Data(_connectionString).ObtenerTodosArchivos();
        }
        public async Task<int> Guardar(Archivo request)
        {
            try
            {

                return await new Data(_connectionString).Guardar(request);
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

                return await new Data(_connectionString).GuardarDetalle(request);
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

                return await new Data(_connectionString).GuardarError(request);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public async Task<IEnumerable<DetalleArchivo>> ObtenerArchivoPorIdentificador(string request)
        {
            try
            {

                return await new Data(_connectionString).ObtenerArchivoPorIdentificador(request);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
