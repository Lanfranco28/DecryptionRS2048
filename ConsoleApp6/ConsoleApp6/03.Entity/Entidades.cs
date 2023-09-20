using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecryptionRS2048.Entity
{
    public class Entidades
    {
        public class Archivo
        {
            public string nombreArchivo { get; set; }
            public string extension { get; set; }
            public string peso { get; set; }
            public bool activo { get; set; }
            //public IEnumerable<DetalleArchivo> detalle { get; set; }
        }

        public class DetalleArchivo
        {
            public int idArchivo { get; set; }
            public string campo1 { get; set; }
            public string campo2 { get; set; }
            public bool activo { get; set; }
            public string observaciones { get; set; }
        }

        public class Error
        {
            public int idError { get; set; }
            public string descripcion { get; set; }
            public int idArchivo { get; set; }
            public DateTime fechaCreacion { get; set; }
            public int numeroLinea { get; set; }
        }
    }
}
