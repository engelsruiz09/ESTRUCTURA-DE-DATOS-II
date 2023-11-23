using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LAB01_EDII.Arbol;

namespace LAB01_EDII.Modelo
{
    public class Credenciales
    {
        public static PublicKey publicKey; 
        public static PrivateKey privateKey;

        //clase para almacenar las credenciales
        public class Credencial
        {
            public string[] Companies { get; set; }
            public string Recluiter { get; set; }
            public string Password { get; set; }
            public byte[] PasswordCifrado { get; set; }
        }

        // HashSets para reclutadores y compañías
        public static HashSet<string> reclutadoresUnicos = new HashSet<string>();
        public static HashSet<string> companiasUnicas = new HashSet<string>();

        public static List<Credencial> ObtenerCredenciales(AVL<Persona> arbol, PublicKey publicKey)
        {
            List<Credencial> listaCredenciales = new List<Credencial>();

            arbol.InOrder(persona =>
            {
                try
                {

                    //cargar el reclutador al HashSet
                    reclutadoresUnicos.Add(persona.recluiter);

                    foreach (var compania in persona.companies)
                    {
                        //se cargan las compañías al HashSet
                        companiasUnicas.Add(compania);

                        // contraseña para cada reclutador-compañía
                        string passwordAleatorio = GenerarPasswordAleatorio(persona.recluiter, compania);

                        // Cifrar contraseña
                        byte[] passwordCifrada = CifrarPassword(passwordAleatorio, publicKey);

                        // Añadir credencial a la lista
                        listaCredenciales.Add(new Credencial
                        {
                            Companies = new string[] { compania },
                            Recluiter = persona.recluiter,
                            Password = passwordAleatorio,
                            PasswordCifrado = passwordCifrada
                        });

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Se produjo una excepción: {ex.Message}");
                    Console.WriteLine($"Pila de llamadas: {ex.StackTrace}");
                }

            });

            return listaCredenciales;
        }


        // Para cifrar una contraseña
        public static byte[] CifrarPassword(string password, PublicKey publicKey)
        {
            BigInteger mensaje = new BigInteger(Encoding.UTF8.GetBytes(password));
            Console.WriteLine($"K: {publicKey.K}");
            Console.WriteLine($"N: {publicKey.N}");

            if (publicKey.K.Sign < 0)
            {
                throw new ArgumentException("El exponente (K) no puede ser negativo", nameof(publicKey.K));
            }

            BigInteger mensajeCifrado = BigInteger.ModPow(mensaje, publicKey.K, publicKey.N); //K y N de la PublicKey, N = producto p q, 
            return mensajeCifrado.ToByteArray();
        }

        // Para descifrar una contraseña
        public static string DescifrarPassword(byte[] passwordCifrado, PrivateKey privateKey)
        {
            BigInteger mensajeCifrado = new BigInteger(passwordCifrado);
            BigInteger mensaje = BigInteger.ModPow(mensajeCifrado, privateKey.J, privateKey.N); //J y N de la PrivateKey
            return Encoding.UTF8.GetString(mensaje.ToByteArray());
        }

        private static string GenerarPasswordAleatorio(string reclutador, string compania) // Añadimos parámetros
        {
            //dos primeras letras de reclutador y compañía
            var inicio = reclutador.Substring(0, 2) + compania.Substring(0, 2);

            // Agregar al final
            return inicio + "@LAB5";
        }

    }
}
