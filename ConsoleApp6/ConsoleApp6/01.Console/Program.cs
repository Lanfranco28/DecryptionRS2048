using DecryptionRS2048._05.Util;
using DecryptionRS2048.Business;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static DecryptionRS2048.Entity.Entidades;

namespace DecryptionRS2048
{
    class Program
    {
        private static string ConnectionString = Constante.STRING_CONNECTION;

        static async Task Main(string[] args)
        {
            await LeerArchivoAsync();
            //string encriptado = EncriptarCampo("Objeto 01 - Campo encriptado");
            //string encriptado2 = EncriptarCampo("Objeto 02 - Campo encriptado");
            //string nada = string.Empty;
        }

        private static async Task LeerArchivoAsync()
        {

            string[] archivosTxt;
            List<string> archivosPorProcesar = new List<string>();
            try
            {
                // Ruta de la carpeta que deseas explorar
                string carpeta = Constante.FILE_PATH;
                if (!Directory.Exists(carpeta))
                {
                    Console.WriteLine("No existe la carpeta.");
                    throw new Exception("No se encontraron archivos .txt en la carpeta.");
                }

                // Obtener la lista de archivos .txt en la carpeta
                archivosTxt = Directory.GetFiles(carpeta, "*.txt");

                Negocio negocio = new(ConnectionString);
                var archivosGuardados = await negocio.ObtenerTodosArchivo();

                if (archivosGuardados.Any())
                {
                    foreach (var item in archivosTxt)
                    {
                        var archivoEncontrado = archivosGuardados.Any(x => x.nombreArchivo.Trim() == Path.GetFileName(item).Trim());
                        if (!archivoEncontrado)
                        {
                            archivosPorProcesar.Add(item);
                        }
                    }

                }
                else
                {
                    archivosPorProcesar = archivosTxt.ToList();
                }


                if (!archivosPorProcesar.Any())
                {
                    Console.WriteLine("No se encontraron archivos por procesar .txt en la carpeta.");
                    throw new Exception("No se encontraron archivos por procesar .txt en la carpeta.");
                }
                Console.WriteLine("Archivos .txt encontrados en la carpeta:");
                foreach (string archivo in archivosTxt)
                {
                    await EjecutarPorArchivo(archivo);
                }
            }
            catch (Exception e)
            {
                string pathLog = Path.Combine(Constante.FILE_PATH_LOG + "", "log.txt");
                Console.WriteLine("Exception: " + e.Message);
                System.IO.File.AppendAllText(pathLog, e.Message + " " + Environment.NewLine);
                System.IO.File.AppendAllText(pathLog, "------------------------------------------------------------------------------" + " " + Environment.NewLine);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }
        }

        private static async Task EjecutarPorArchivo(string filePath)
        {
            String line;

            // Obtener el nombre del archivo de la ruta
            string fileName = Path.GetFileName(filePath);

            if (File.Exists(filePath))
            {
                var contenido = File.ReadLines(filePath);

                //Se envia la ruta y el nombre del archivo al constructor StreamReader
                StreamReader sr = new StreamReader(filePath);
                //Leer la primera linea del texto
                line = sr.ReadLine();


                var objArchivo = new Archivo
                {
                    nombreArchivo = fileName,
                    extension = "txt",
                    peso = "SN",
                    activo = true
                };
                Negocio negocio = new(ConnectionString);

                int fnGuardar = await negocio.Guardar(objArchivo);
                int lineNumber = 0;

                // Iterar a través de las líneas
                while (line != null)
                {
                    try
                    {
                        lineNumber++; // Incrementa el número de línea

                        var cadena = line.Split("|");
                        var objArchivoDetalle = new DetalleArchivo
                        {
                            idArchivo = fnGuardar,
                            campo1 = cadena[0].ToString(),
                            campo2 = DesencriptarCampo(cadena[1].ToString()),
                            activo = true,
                            observaciones = string.Empty
                        };
                        int fnGuardarDetalle = await negocio.GuardarDetalle(objArchivoDetalle);

                        // Verifica si la línea contiene la palabra clave
                        //if (line.Contains("20600768841")) // Reemplaza "20600768841" con tu palabra clave
                        //{
                        //    // Imprime el número de línea y la línea con la palabra clave en la consola
                        //    Console.WriteLine("Línea " + lineNumber + ": " + line);
                        //}

                        // Leer la siguiente línea
                        line = sr.ReadLine();
                    }
                    catch (Exception ex)
                    {
                        var objError = new Error
                        {
                            descripcion = ex.Message,
                            idArchivo = fnGuardar,
                            fechaCreacion = DateTime.Now,
                            numeroLinea = lineNumber,
                        };
                        int archivoError = await negocio.GuardarError(objError);

                        // Imprime el número de línea y la línea con el error en la consola (opcional)
                        //Console.WriteLine("Error en la línea " + lineNumber + ": " + line);

                        // Leer la siguiente línea
                        line = sr.ReadLine();
                    }
                }


                //cerrar el archivo
                sr.Close();
            }
            else
            {
                Console.WriteLine("El archivo no existe.");
            }
            Console.WriteLine("Archivo procesado satisfactoriamente.");
            //Console.ReadKey();
        }

        #region Otros métodos
        private static void GenerarLlaves()
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                // Exporta la clave pública en formato XML
                string publicKeyXml = rsa.ToXmlString(false);

                // Exporta la clave privada en formato XML
                string privateKeyXml = rsa.ToXmlString(true);

                // Guarda las claves en archivos o donde desees
                System.IO.File.WriteAllText("publicKey.xml", publicKeyXml);
                System.IO.File.WriteAllText("privateKey.xml", privateKeyXml);

                Console.WriteLine("Claves RSA generadas y guardadas.");
            }
        }

        private static string DesencriptarCampo(string texto)
        {
            // Genera un par de claves RSA
            string privateKeyXml = System.IO.File.ReadAllText("privateKey.xml");

            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                try
                {
                    rsa.FromXmlString(privateKeyXml);

                    // Desencripta el texto con la clave privada
                    return DesencriptarTexto(texto, rsa);
                }
                catch(Exception ex)
                {
                    throw new Exception("El campo no se pudo desencriptar ya que es un tipo de encriptacion no valida.");
                }
                finally
                {
                    // Asegúrate de liberar los recursos del objeto RSA
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        private static string EncriptarCampo(string texto)
        {
            // Genera un par de claves RSA
            string publicKeyXml = System.IO.File.ReadAllText("publicKey.xml");

            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                try
                {
                    rsa.FromXmlString(publicKeyXml);

                    // Desencripta el texto con la clave privada
                    return EncriptarTexto(texto, rsa);
                }
                finally
                {
                    // Asegúrate de liberar los recursos del objeto RSA
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        static string EncriptarTexto(string texto, RSACryptoServiceProvider rsa)
        {
            // Convierte el texto en bytes
            byte[] bytesTexto = Encoding.UTF8.GetBytes(texto);

            // Encripta los bytes con la clave pública
            byte[] bytesEncriptados = rsa.Encrypt(bytesTexto, false);

            // Convierte los bytes en una representación legible (por ejemplo, Base64)
            return Convert.ToBase64String(bytesEncriptados);
        }

        static string DesencriptarTexto(string textoEncriptado, RSACryptoServiceProvider rsa)
        {
            // Convierte la representación legible (Base64) en bytes
            byte[] bytesEncriptados = Convert.FromBase64String(textoEncriptado);

            // Desencripta los bytes con la clave privada
            byte[] bytesDesencriptados = rsa.Decrypt(bytesEncriptados, false);

            // Convierte los bytes desencriptados nuevamente en texto
            return Encoding.UTF8.GetString(bytesDesencriptados);
        }

        #endregion Otros métodos
    }
}


