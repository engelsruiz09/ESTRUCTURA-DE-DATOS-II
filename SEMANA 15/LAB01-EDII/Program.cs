using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.IO;
using LAB01_EDII.Modelo;
using LAB01_EDII.Arbol;
using System.IO.Ports;
using System.Data;
using System.Numerics;
using System.Reflection;
using static LAB01_EDII.Modelo.Credenciales;


namespace LAB01_EDII
{
    public class Program
    {
        private static int countIns = 0;
        private static int countDel = 0;
        private static int countPat = 0;

        private static AVL<Persona> arbolPersonas = new AVL<Persona>();
        private static List<Persona> personas = new List<Persona>();
        private static CodificacionHuffman codificadorhuffman = new CodificacionHuffman();

        public static FirmaDigital firmaGlobal;
        private static List<Credencial> credencialesGeneradas = new List<Credencial>();

        private static PublicKey publicKey; 
        private static PrivateKey privateKey;
        public static void Main(string[] args)
        {
            try
            {
                //credencialesGeneradas = new List<Credencial>();
                PrimeGenerator primeGen = new PrimeGenerator();
                BigInteger p = primeGen.GeneratePrime();
                BigInteger q = primeGen.GeneratePrime();

                firmaGlobal = new FirmaDigital(p, q);

                publicKey = firmaGlobal.GetPublicKey();
                privateKey = firmaGlobal.GetPrivateKey();

               
                string route = @"C:\Users\julio\Downloads\LAB01-EDII\inputslab5\input.csv";
                //string route = @"C:\Users\julio\Downloads\inputlab2.csv";
                // Obtener la lista de credenciales

                if (File.Exists(route))
                {
                    string[] FileData = File.ReadAllLines(route);
                    foreach (var item in FileData)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            string[] fila = item.Split(';');

                            Persona? persona = JsonSerializer.Deserialize<Persona>(fila[1]);
                            // //fila[0] tiene la acción (INSERT, PATCH, DELETE).
                            //fila[1] contiene la serializacion json.
                            if (fila[0] == "INSERT")
                            {
                                countIns++;
                                // persona! le esta diciendo al compilador: "Confia en mi, en este punto, persona no es null, asi que no me adviertas sobre la posibilidad de que pueda ser null".
                                arbolPersonas.Add(persona!, Delegates.DpiComparison);
                            }
                            else if (fila[0] == "DELETE")
                            {
                                countDel++;
                                arbolPersonas.Delete(persona!, Delegates.DpiComparison);
                            }
                            else if (fila[0] == "PATCH")
                            {
                                countPat++;
                                arbolPersonas.Patch(persona!, Delegates.DpiComparison);
                            }
                        }
                    }
                    credencialesGeneradas = ObtenerCredenciales(arbolPersonas, publicKey);
                    ImprimirCredenciales(credencialesGeneradas);
                    Console.ReadKey();
                    Login();

                    string flag;
                    do
                    {
                        //Console.Clear();
                        //Login();
                        Console.Clear();
                        Mostrardisplay();
                        Console.WriteLine("*********** Menú: *************");
                        Console.WriteLine("1- Mostrar Registros completos");
                        Console.WriteLine("2- Buscar Registros");
                        Console.WriteLine("3- Buscar DPI, Codificar, Decodificar");
                        Console.WriteLine("4- Buscar DPI, Conversaciones");
                        Console.WriteLine("5- Salir");
                        Console.WriteLine("Por favor elija una opción del sistema:");
                        flag = Console.ReadLine();

                        switch (flag)
                        {
                            case "1":
                                todosregistros(FileData);
                                break;
                            case "2":
                                buscarregistros(arbolPersonas);
                                break;
                            case "3":
                                buscarYCodificarDecodificar(arbolPersonas);
                                break;
                            case "4":
                                Console.Clear();
                                Mostrardisplay();
                                Console.WriteLine("Ingrese el DPI para buscar las conversaciones:");
                                string dpi = Console.ReadLine();
                                Console.WriteLine("Ingrese la llave para descifrar las conversaciones:");
                                string llave = Console.ReadLine();//una llave de 8 bytes equivalente a 64bits por el DES 56 se usan realmente para cifrar y los 8 restantes como bits de paridad 

                                // Procesar y almacenar las conversaciones (esto solo se hace una vez)
                                ProcesoConversacion(dpi, llave);

                                // Mostrar las conversaciones para el DPI dado
                                DisplayConversacionesDPI(dpi, llave);

                                break;
                            case "5":
                                Console.WriteLine("Saliendo...");
                                break;
                            default:
                                Console.WriteLine("Opción no válida.");
                                break;
                        }
                        Console.ReadKey();

                    } while (flag != "5");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"El sistema ha presentado un error inesperado: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }


        }
        public static void ImprimirCredenciales(List<Credencial> credenciales)
        {
            foreach (var cred in credenciales)
            {
                Console.WriteLine($"Reclutador: {cred.Recluiter}");
                Console.WriteLine($"Compañía: {string.Join(", ", cred.Companies)}");
                Console.WriteLine($"Contraseña: {cred.Password}");
                Console.WriteLine("---------------------------------");
            }
        }

        public static void Login()
        {
            bool accesoConcedido = false;
            
            do
            {
                
                Console.Clear();
                Mostrardisplay();

                Console.WriteLine("***************************************");
                Console.WriteLine("*                                     *");
                Console.WriteLine("*           Ingreso al Sistema        *");
                Console.WriteLine("*                                     *");
                Console.WriteLine("***************************************");

                Console.WriteLine("*                                     *");
                Console.Write("*   Ingrese Reclutador: ");
                string reclutador = Console.ReadLine();
                Console.WriteLine("*                                     *");

                Console.WriteLine("*                                     *");
                Console.Write("*   Ingrese Compañía: ");
                string compania = Console.ReadLine();
                Console.WriteLine("*                                     *");

                Console.WriteLine("*                                     *");
                Console.Write("*   Ingrese Password: ");
                string password = Console.ReadLine();
                Console.WriteLine("*                                     *");

                Console.WriteLine("***************************************");

                if (ValidarCredenciales(reclutador, compania, password, credencialesGeneradas, privateKey))
                {
                    Console.WriteLine("Acceso concedido.");
                    accesoConcedido = true;
                }
                else
                {

                    Console.WriteLine("Acceso denegado. ¿Desea intentar nuevamente? (S/N)");
                    char opcion = Console.ReadKey().KeyChar;
                    if (opcion == 'N' || opcion == 'n')
                    {
                        Environment.Exit(0); // Finaliza el programa si el usuario decide no intentar nuevamente
                    }
                }

            } while (!accesoConcedido);
        }


        public static bool ValidarCredenciales(string reclutador, string compania, string passwordIngresado, List<Credencial> listaCredenciales, PrivateKey privateKey)
        {
            var credencialEncontrada = listaCredenciales.FirstOrDefault(cred => cred.Recluiter == reclutador && cred.Companies.Contains(compania));

            if (credencialEncontrada != null)
            {
                string passwordDescifrado = DescifrarPassword(credencialEncontrada.PasswordCifrado, privateKey);
                return passwordDescifrado == passwordIngresado;
            }
            else
            {
                return false;
            }
        }


        private static void Mostrardisplay()
        {
            Console.WriteLine("██████████████████████████████████████████████████████████");
            Console.WriteLine("█─▄─▄─██▀▄─██▄─▄███▄─▄▄─█▄─▀█▄─▄█─▄─▄─███─█─█▄─██─▄█▄─▄─▀█");
            Console.WriteLine("███─████─▀─███─██▀██─▄█▀██─█▄▀─████─█████─▄─██─██─███─▄─▀█");
            Console.WriteLine("▀▀▄▄▄▀▀▄▄▀▄▄▀▄▄▄▄▄▀▄▄▄▄▄▀▄▄▄▀▀▄▄▀▀▄▄▄▀▀▀▀▄▀▄▀▀▄▄▄▄▀▀▄▄▄▄▀▀");
            Console.WriteLine(new string('*', 58));//llamo al constructor de la clase string y tomo un caracter y un numero entero como argumentos, repitiendo el caracter 80 veces.
            Console.WriteLine();
        }

        private static void resumenOperaciones()
        {
            Console.WriteLine($"Su resumen de las operaciones ingresadas es:");
            Console.WriteLine($"INSERT: {countIns}");
            Console.WriteLine($"DELETE: {countDel}");
            Console.WriteLine($"PATCH: {countPat}");
            Console.WriteLine();
        }

        private static void todosregistros(string[] FileData)
        {
            Console.Clear();
            Mostrardisplay();
            int rowNumber = 1;
            foreach (var line in FileData)
            {
                string[] fila = line.Split(';');
                // Mostrando el tipo de accion y el contenido JSON.
                Console.WriteLine($"{rowNumber} - {fila[0]} - {fila[1]}");
                rowNumber++;
            }
            Console.WriteLine("Presione cualquier tecla para regresar a menu...");
            //  ruta del directorio de entrada y la llave para encriptar/desencriptar
            string inputPath = @"C:\Users\julio\Downloads\LAB01-EDII\inputs3\inputs";
            string encryptedPath = @"C:\Users\julio\Downloads\LAB01-EDII\encriptado";
            string decryptedPath = @"C:\Users\julio\Downloads\LAB01-EDII\desencriptado";
            string llave = "miLlaveSuperSecreta";

            // verifico que los directorios existen
            if (!Directory.Exists(encryptedPath))
            {
                Directory.CreateDirectory(encryptedPath);
            }

            if (!Directory.Exists(decryptedPath))
            {
                Directory.CreateDirectory(decryptedPath);
            }

            // Obtener todos los archivos en el directorio de entrada
            string[] files = Directory.GetFiles(inputPath, "REC-*.txt");

            foreach (string file in files)
            {
                // Leer el contenido del archivo
                string content = File.ReadAllText(file);

                // Encriptar el contenido
                string encryptedContent = DES.encriptar(content, llave);

                // Guardar el contenido encriptado en un nuevo archivo en el directorio de encriptados
                string encryptedFileName = Path.GetFileNameWithoutExtension(file) + "-encriptado.txt";
                string encryptedFilePath = Path.Combine(encryptedPath, encryptedFileName);
                File.WriteAllText(encryptedFilePath, encryptedContent);

                // Desencriptar el contenido
                string decryptedContent = DES.desencriptar(encryptedContent, llave);

                // Guardar el contenido desencriptado en un nuevo archivo en el directorio de desencriptados
                string decryptedFileName = Path.GetFileNameWithoutExtension(file) + "-desencriptado.txt";
                string decryptedFilePath = Path.Combine(decryptedPath, decryptedFileName);
                File.WriteAllText(decryptedFilePath, decryptedContent);
            }

            Console.WriteLine("----Proceso completado!----");
        }

        private static void buscarregistros(AVL<Persona> arbolPersonas)
        {
            char flag2;
            do
            {
                Console.Clear();
                Mostrardisplay();
                resumenOperaciones();

                personas.Clear();
                //resumenOperaciones();
                Console.WriteLine("Ingrese el nombre de la persona que quiere buscar (Como usted quiera escribirlo): ");
                string? name = Console.ReadLine(); //leo la entrada del usuario y la dicha variable name de tipo nullable ya que podria ser null

                Persona temporal = new Persona(); //creo un nuevo objeto de Persona y lo asigno a temporal 

                temporal.name = name!; //asigno el valor leido en la consola al atributo name del objeto temporal, al valor leido en consola con valor ! de afirmacion de no nulidad.

                arbolPersonas.QueryResults(arbolPersonas.Root!, temporal, Delegates.NameComparison, personas);
                // llamo a mi metodo queryresults para llenar la lista, tambien verifico que la raiz del arbol no este nulla , el objeto persona que tiene el nombre que se quiere buscar, y el respectivo delegado para comparar por el atributo de nombre, y su respectiva lista para almacenar los resultados de la busqueda

                string line = "";
                if (personas.Count() == 0)
                {
                    line = "Sin coincidencias para el nombre:  " + name;
                    Console.WriteLine(line);
                }
                else
                {
                    Console.WriteLine($"Se encontraron en el sistema: {personas.Count} coincidencias para el nombre: {name} ");
                    int counter = 0;
                    foreach (var item in personas)
                    {
                        Console.WriteLine($"name: {item.name} \t dpi: {item.dpi} \t dateBirth: {item.datebirth} \t address: {item.address}");
                        counter++;
                        line += JsonSerializer.Serialize<Persona>(item) + "\n";
                    }
                    string output = @"C:\\Users\\julio\\Downloads\\LAB01-EDII\\packages" + name + ".txt";
                    File.WriteAllText(output, line);
                }

                Console.WriteLine("¿Desea realizar otra búsqueda? Ingrese \'1\' o \'2\'");
                Console.WriteLine("1. Si");
                Console.WriteLine("2. No");
                flag2 = Console.ReadKey().KeyChar;
            } while (flag2 == '1');
        }


        private static void buscarYCodificarDecodificar(AVL<Persona> arbolPersonas)
        {
            Console.Clear();
            Mostrardisplay();

            bool isValiddpi;
            string? dpi;
            do
            {
                Console.WriteLine("Ingrese el DPI de la persona que quiere buscar: ");
                dpi = Console.ReadLine();
                isValiddpi = dpi.Length == 13 && dpi.All(char.IsDigit);//verifico que tenga longitud de 13 y que todos sean digitos
                if (!isValiddpi)
                {
                    Console.WriteLine("El DPI ingresado no es válido. Por favor, intentelo de nuevo.");
                }
            } while (!isValiddpi);//repito hasta que sea un dpi valido 

            Persona personaBusqueda = new Persona { dpi = dpi! };
            Persona resultado = arbolPersonas.Search(personaBusqueda, Delegates.DpiComparison);

            if (resultado != null)
            {
                Console.WriteLine("---------------------------------------------------");
                //codifico los datos en segundo plano
                Dictionary<char, int> diccionarioGlobal = CreateGlobalDictionary(resultado);
                codificadorhuffman.SetDictionary(diccionarioGlobal);

                List<string> codigosHuffman = new List<string>();
                foreach (string company in resultado.companies)
                {
                    string codigoParaCodificar = resultado.dpi + "  " + company;
                    string codigoHuffman = codificadorhuffman.Coding(codigoParaCodificar, diccionarioGlobal);
                    codigosHuffman.Add(codigoHuffman);
                }
                resultado.huffmanCodes = codigosHuffman;
                Console.WriteLine("---------------------------------------------------");

                List<string> companiesDecodificadas = new List<string>();

                for (int i = 0; i < resultado.huffmanCodes.Count; i++)
                {
                    string codigoHuffman = resultado.huffmanCodes[i];
                    string dataDecodificada = codificadorhuffman.Decode(codigoHuffman);
                    string companyDecodificada = dataDecodificada.Substring(13);  //elimino el DPI
                    companiesDecodificadas.Add(companyDecodificada);

                }
                //creo un objeto con los datos
                var datosPersona = new
                {
                    DPI = resultado.dpi,
                    Nombre = resultado.name,
                    Fecha_de_Nacimiento = resultado.datebirth,
                    Direccion = resultado.address,
                    Compañias = companiesDecodificadas
                };
                //convierto el objeto a formato JSON
                string datosEnJson = JsonSerializer.Serialize(datosPersona, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });

                //mustro los datos en formato JSON al usuario
                Console.WriteLine(datosEnJson);

                //guardo DPI + compañias en archivo .txt en formato JSON
                var datosGuardado = new
                {
                    name = resultado.name,
                    dpi = resultado.dpi,
                    datebirth = resultado.datebirth,
                    address = resultado.address,
                    companies = resultado.huffmanCodes
                };
                Console.WriteLine("Presione cualquier tecla para regresar a menu........");

                string outputData = JsonSerializer.Serialize(datosGuardado);
                string ruta = @"C:\Users\julio\Downloads\LAB01-EDII\DPI_Companies.txt";
                File.AppendAllText(ruta, outputData);
            }
            else
            {
                Console.WriteLine("DPI no encontrado.");
                Console.ReadKey();
            }
        }

        //creo mi diccionario de caracteres donde las combinaciones son del DPI + companies y su frecuencia
        private static Dictionary<char, int> CreateGlobalDictionary(Persona resultado)
        {
            //inicializo un nuevo diccionario para caracteres y frecuencias
            Dictionary<char, int> diccionario = new Dictionary<char, int>();
            //recorre sobre cada compania en el objeto persona
            foreach (string company in resultado.companies)
            {
                //combino el dpi + compania actual con un espacio en medio 
                string combined = resultado.dpi + "  " + company;
                //recorro sobre la combinacion 
                foreach (char caracter in combined)
                {
                    //comprubo si diccionario ya tiene caracter
                    if (!diccionario.ContainsKey(caracter))
                    {
                        diccionario[caracter] = 0;//sidiccionario no contiene el caracter añade al diccionario con un valor inicial de 0
                    }
                    diccionario[caracter]++;//aumento el valor asociado al caracter en el diccionario en uno
                }
            }
            return diccionario;//devuelvo el diccionario completo con todos los caracteres y las frecuencias de estos
        }


        public static void ProcesoConversacion(string dpi, string llave)
        {
            try
            {
                //FirmaDigital firma = new FirmaDigital(); // Instanciación de la clase de firma

                int counter = 1;
                string filename = $"CONV-{dpi}-{counter}.txt";
                string path = @"C:\\Users\\julio\\Downloads\\LAB01-EDII\\inputslab4\\inputs\\" + filename;
                while (File.Exists(path))
                {
                    string conversation = File.ReadAllText(path);

                    // Generar firma digital usando RSA
                    byte[] signatureBytes = firmaGlobal.GenerarFirma(conversation);
                    string signature = Convert.ToBase64String(signatureBytes);

                    // Concatenar mensaje y firma
                    string combinedMessage = $"{conversation}\nSIGNATURE:{signature}";

                    // Cifrar el mensaje
                    string encryptedMessage = DES.encriptar(combinedMessage, llave);

                    // Comprimir el mensaje cifrado
                    List<int> compressedMessage = LZW.encode(encryptedMessage);

                    // Guardar la lista como bytes en un archivo binario se desea guardar la representacion binaria de la lista de enteros en archivo
                    byte[] byteArray = compressedMessage.SelectMany(BitConverter.GetBytes).ToArray();

                    string outputPath = $@"C:\\Users\\julio\\Downloads\\LAB01-EDII\\inputslab4\\salidalab4\\cifrado\\CONV-{dpi}-{counter}-comprimir.txt";
                    Console.WriteLine($"Archivo procesado con éxito y guardado en: {outputPath}");
                    File.WriteAllBytes(outputPath, byteArray);

                    counter++;
                    filename = $"CONV-{dpi}-{counter}.txt";
                    path = @"C:\\Users\\julio\\Downloads\\LAB01-EDII\\inputslab4\\inputs\\" + filename;
                }

                Console.WriteLine($"Proceso completado. Se procesaron {counter - 1} archivos.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Se produjo un error: {e.Message}");
                Console.WriteLine(e.StackTrace);
            }
        }

        public static void DisplayConversacionesDPI(string dpi, string llave)
        {
            try
            {
                //FirmaDigital firma = new FirmaDigital(); // Instanciación de la clase de firma

                int conversationCounter = 1;
                string filename = $"CONV-{dpi}-{conversationCounter}.txt";
                string path = $@"C:\\Users\\julio\\Downloads\\LAB01-EDII\\inputslab4\\salidalab4\\cifrado\\CONV-{dpi}-{conversationCounter}-comprimir.txt";

                while (File.Exists(path))
                {
                    Console.WriteLine($"Leyendo y descifrando el archivo: {path}");

                    byte[] byteArray = File.ReadAllBytes(path);//lo almaceno en un array de bytes
                    //de arreglo de bytes a lista de enteros
                    List<int> compressedMessage = Enumerable.Range(0, byteArray.Length / sizeof(int)).Select(offset => BitConverter.ToInt32(byteArray, offset * sizeof(int))).ToList(); //enumerable -> (()) secuencia de numeros enteros crea una secuencia de índices que representan las posiciones iniciales de cada entero en el arreglo de bytes, el archivo binario guarda los datos en forma de bytes, pero para descomprimir la conversación, necesitas recuperar la lista original de enteros que representan la conversacion comprimida. 

                    // Descomprimir el mensaje
                    string encryptedMessage = LZW.Decompress(compressedMessage);

                    // Descifrar el mensaje
                    string combinedMessage = DES.desencriptar(encryptedMessage, llave);

                    // Separar el mensaje de la firma
                    var parts = combinedMessage.Split(new string[] { "\nSIGNATURE:" }, StringSplitOptions.None);
                    string message = parts[0];
                    byte[] signatureBytes = Convert.FromBase64String(parts[1]);

                    StringBuilder sb = new StringBuilder();

                    // Validar firma con RSA
                    if (firmaGlobal.ValidarFirma(message, signatureBytes))
                    {
                        Console.WriteLine("---------------------------------------------");
                        sb.AppendLine($"Conversación {conversationCounter}:");
                        sb.AppendLine(message);
                        sb.AppendLine("-------------------------------------------------");
                    }
                    else
                    {
                        sb.AppendLine("La firma del mensaje es inválida o ha sido alterada.");
                    }

                    Console.WriteLine(sb.ToString());

                    string outputPath = $@"C:\\Users\\julio\\Downloads\\LAB01-EDII\\inputslab4\\salidalab4\\descifrado\\CONV-{dpi}-{conversationCounter}.txt";
                    File.WriteAllText(outputPath, sb.ToString());

                    conversationCounter++;
                    filename = $"CONV-{dpi}-{conversationCounter}.txt";
                    path = $@"C:\\Users\\julio\\Downloads\\LAB01-EDII\\inputslab4\\salidalab4\\cifrado\\CONV-{dpi}-{conversationCounter}-comprimir.txt";
                }

                Console.WriteLine($"Proceso completado. Se desplegaron {conversationCounter - 1} conversaciones.");
                Console.WriteLine("Presione Cualquier tecla para continuar");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Se produjo un error: {e.Message}");
                Console.WriteLine(e.StackTrace);
            }
        }






    }
}
