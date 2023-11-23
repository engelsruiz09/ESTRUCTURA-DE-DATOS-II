using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB01_EDII.Modelo
{
    public class DES
    {
        // definicion de las permutaciones y S - Boxes segun el estandar DES
        //------------------P-BOX-------------
        private static int[] IP = { 58, 50, 42, 34, 26, 18, 10, 2,
                     60, 52, 44, 36, 28, 20, 12, 4,
                     62, 54, 46, 38, 30, 22, 14, 6,
                     64, 56, 48, 40, 32, 24, 16, 8,
                     57, 49, 41, 33, 25, 17, 9, 1,
                     59, 51, 43, 35, 27, 19, 11, 3,
                     61, 53, 45, 37, 29, 21, 13, 5,
                     63, 55, 47, 39, 31, 23, 15, 7};

        private static int[] INVP = { 40, 8, 48, 16, 56, 24, 64, 32,
                       39, 7, 47, 15, 55, 23, 63, 31,
                       38, 6, 46, 14, 54, 22, 62, 30,
                       37, 5, 45, 13, 53, 21, 61, 29,
                       36, 4, 44, 12, 52, 20, 60, 28,
                       35, 3, 43, 11, 51, 19, 59, 27,
                       34, 2, 42, 10, 50, 18, 58, 26,
                       33, 1, 41, 9, 49, 17, 57, 25};

        private static int[] PC1 = { 57, 49, 41, 33, 25, 17, 9,
                      1, 58, 50, 42, 34, 26, 18,
                      10, 2, 59, 51, 43, 35, 27,
                      19, 11, 3, 60, 52, 44, 36,
                      63, 55, 47, 39, 31, 23, 15,
                      7, 62, 54, 46, 38, 30, 22,
                      14, 6, 61, 53, 45, 37, 29,
                      21, 13, 5, 28, 20, 12, 4};

        private static int[] PC2 =
        {
                      14, 17, 11, 24, 1, 5,
                      3, 28, 15, 6, 21, 10,
                      23, 19, 12, 4, 26, 8,
                      16, 7, 27, 20, 13, 2,
                      41, 52, 31, 37, 47, 55,
                      30, 40, 51, 45, 33, 48,
                      44, 49, 39, 56, 34, 53,
                      46, 42, 50, 36, 29, 32
        };

        private static int[] E =
        {
                    32, 1, 2, 3, 4, 5,
                    4, 5, 6, 7, 8, 9,
                    8, 9, 10, 11, 12, 13,
                    12, 13, 14, 15, 16, 17,
                    16, 17, 18, 19, 20, 21,
                    20, 21, 22, 23, 24, 25,
                    24, 25, 26, 27, 28, 29,
                    28, 29, 30, 31, 32, 1
        };

        private static int[] P =
        {
            16, 7, 20, 21, 29, 12, 28, 17,
            1, 15, 23, 26, 5, 18, 31, 10,
            2, 8, 24, 14, 32, 27, 3, 9,
            19, 13, 30, 6, 22, 11, 4, 25
        };

        //-----------------S-BOX--------------
        private static int[,] S1 = {
            {14, 4, 13, 1,  2,  15, 11, 8,  3,  10, 6,  12, 5,  9,  0,  7},
            {0, 15, 7,  4,  14, 2,  13, 1,  10, 6,  12, 11, 9,  5,  3,  8},
            {4, 1,  14, 8,  13, 6,  2,  11, 15, 12, 9,  7,  3,  10, 5,  0},
            {15, 12, 8,  2,  4,  9,  1,  7,  5, 11, 3, 14, 10, 0, 6, 13}
        };

        private static int[,] S2 =
        {
            {15,1,  8,  14, 6,  11, 3,  4,  9,  7,  2,  13, 12, 0,  5,  10},
            {3, 13, 4,   7,  15, 2,  8,  14, 12, 0,  1,  10, 6,  9,  11, 5},
            {0, 14, 7,  11, 10, 4,  13, 1,  5,  8,  12, 6,  9,  3,  2,  15},
            {13, 8,  10, 1,  3,  15, 4,  2,  11, 6,  7,  12, 0,  5,  14, 9}
        };

        private static int[,] S3 =
        {
            {10,    0,  9,  14, 6,  3,  15, 5,  1,  13, 12, 7,  11, 4,  2,  8},
            {13,    7,  0,  9,  3,  4,  6,  10, 2,  8,  5,  14, 12, 11, 15, 1},
            {13,    6,  4,  9,  8,  15, 3,  0,  11, 1,  2,  12, 5,  10, 14, 7},
            {1, 10, 13, 0,  6,  9,  8,  7,  4,  15, 14, 3,  11, 5,  2,  12}
        };

        private static int[,] S4 =
        {
            {7, 13, 14, 3,  0,  6,  9,  10, 1,  2,  8,  5,  11, 12, 4,  15},
            {13,    8,  11, 5,  6,  15, 0,  3,  4,  7,  2,  12, 1,  10, 14, 9},
            {10,    6,  9,  0,  12, 11, 7,  13, 15, 1,  3,  14, 5,  2,  8,  4},
            {3, 15, 0,  6,  10, 1,  13, 8,  9,  4,  5,  11, 12, 7,  2,  14}
        };

        private static int[,] S5 =
        {
            {2, 12, 4,  1,  7,  10, 11, 6,  8,  5,  3,  15, 13, 0,  14, 9},
            {14,    11, 2,  12, 4,  7,  13, 1,  5,  0,  15, 10, 3,  9,  8,  6},
            {4, 2,  1,  11, 10, 13, 7,  8,  15, 9,  12, 5,  6,  3,  0,  14},
            {11,    8,  12, 7,  1,  14, 2,  13, 6,  15, 0,  9,  10, 4,  5,  3}
        };

        private static int[,] S6 =
        {
            {12,    1,  10, 15, 9,  2,  6,  8,  0,  13, 3,  4,  14, 7,  5,  11},
            {10,    15, 4,  2,  7,  12, 9,  5,  6,  1,  13, 14, 0,  11, 3,  8},
            {9, 14, 15, 5,  2,  8,  12, 3,  7,  0,  4,  10, 1,  13, 11, 6},
            {4, 3,  2,  12, 9,  5,  15, 10, 11, 14, 1,  7,  6,  0,  8,  13}
        };

        private static int[,] S7 =
        {
            {4, 11, 2,  14, 15, 0,  8,  13, 3,  12, 9,  7,  5,  10, 6,  1},
            {13,0,  11, 7,  4,  9,  1,  10, 14, 3,  5,  12, 2,  15, 8,  6},
            {1, 4,  11, 13, 12, 3,  7,  14, 10, 15, 6,  8,  0,  5,  9,  2},
            {6, 11, 13, 8,  1,  4,  10, 7,  9,  5,  0,  15, 14, 2,  3,  12}
        };

        private static int[,] S8 =
        {
            {13, 2,  8,  4,  6,  15, 11, 1,  10, 9,  3,  14, 5,  0,  12, 7},
            {1, 15, 13, 8,  10, 3,  7,  4,  12, 5,  6,  11, 0,  14, 9,  2},
            {7, 11, 4,  1,  9,  12, 14, 2,  0,  6,  10, 13, 15, 3,  5,  8},
            {2, 1,  14, 7,  4,  10, 8,  13, 15, 12, 9,  0,  3,  5,  6,  11}
        };

        // funcion que cifra el mensaje proporcionado usando el algoritmo DES

        // Define una funcion para encriptar un mensaje usando el algoritmo DES.
        public static Func<string, string, string> encriptar = delegate (string mensaje, string llave)
        {
            // Si la longitud del mensaje no es un multiplo de 8bytes, 64 bits se añaden espacios hasta que lo sea.
            while (mensaje.Length % 8 != 0)
            {
                mensaje = mensaje + " ";
            }

            // Utiliza un StringBuilder para construir el mensaje encriptado.
            StringBuilder msjEncriptado = new StringBuilder();

            //mensaje = AddPadding(mensaje, 8);

            // Convierte el mensaje a su representacion en bits.
            string bits = StringToBinary(mensaje);
            int posicion = 0;

            // Genera las llaves necesarias para el proceso DES.
            string[] keys = generateKeys(llave);

            // Procesa el mensaje en bloques de 64 bits.
            while (posicion < bits.Length)
            {
                // Extrae un bloque de 64 bits del mensaje.
                string block = bits.Substring(posicion, 64);
                posicion = posicion + 64;

                // Aplica una permutacion inicial al bloque.
                string initialP = operacionesVectores(IP, block);

                // Realiza el proceso principal de DES al bloque.
                string procesoDES = ProcesoDES(initialP, keys);

                // Divide el resultado en dos mitades: izquierda y derecha.
                string left = procesoDES.Substring(0, 32);
                string right = procesoDES.Substring(32, 32);

                // Combina las dos mitades en orden inverso.
                string temp = right + left;

                // Aplica una permutacion inversa al bloque combinado.
                string permINV = operacionesVectores(INVP, temp);

                // Convierte la representacion en bits a texto y lo añade al mensaje encriptado.
                msjEncriptado.Append(BinaryToString(permINV));
            }

            // Retorna el mensaje encriptado.
            return msjEncriptado.ToString();
        };


        // funcion que desencripta el mensaje proporcionado usando el algoritmo DES
        // Define un delegado para la funcion de desencriptacion.
        public static Func<string, string, string> desencriptar = delegate (string msjCifrado, string llave)
        {
            while (msjCifrado.Length % 8 != 0)//Se llena de espacio en blanco si es necesario
            {
                msjCifrado = msjCifrado + " ";
            }
            StringBuilder msjDescifrado = new StringBuilder();
            int posicion = 0;
            string bits = StringToBinary(msjCifrado);//Se pasa a bits el mensaje

            string[] keys = generateKeys(llave); //Se generan las llaves
            keys = invLlaves(keys);

            while (posicion < bits.Length)
            {
                string block = bits.Substring(posicion, 64);
                string initialP = operacionesVectores(IP, block); //Permutación inicial                                

                string procesoDES = ProcesoDES(initialP, keys);
                string left = procesoDES.Substring(0, 32);
                string right = procesoDES.Substring(32, 32);
                string temp = right + left;

                string permINV = operacionesVectores(INVP, temp);
                msjDescifrado.Append(BinaryToString(permINV));
                posicion = posicion + 64;
            }

            return msjDescifrado.ToString();
        };

        // Define un metodo para generar las llaves usadas en DES.
        private static string[] generateKeys(String key)
        {
            // Crea un arreglo para almacenar las 16 llaves generadas.
            string[] keys = new string[16];

            // Convierte la llave dada (en formato de cadena) a su representacion en bits.
            string keyBits = StringToBinary(key);

            // Aplica una permutacion especifica (PC1) a los bits de la llave.
            string keySinP = operacionesVectores(PC1, keyBits);

            // Divide la llave en dos mitades: izquierda y derecha.
            string izq = keySinP.Substring(0, 28);
            string der = keySinP.Substring(28, 28);

            // Utiliza un bucle para generar 16 llaves.
            for (int round = 1; round <= 16; round++)
            {
                // Dependiendo de la ronda actual, se desplaza la llave izquierda y derecha 1 o 2 veces.
                if (round == 1 || round == 2 || round == 9 || round == 16)
                {
                    izq = shiftLeft(izq, 1);
                    der = shiftLeft(der, 1);
                }
                else
                {
                    izq = shiftLeft(izq, 2);
                    der = shiftLeft(der, 2);
                }

                // Combina las dos mitades de la llave.
                string txt = izq + der;

                // Aplica una segunda permutacion (PC2) a la llave combinada.
                string ki = operacionesVectores(PC2, txt);

                // Almacena la llave generada en el arreglo de llaves.
                keys[round - 1] = ki;
            }

            // Retorna el arreglo de llaves generadas.
            return keys;
        }

        // Define un metodo para desplazar una cadena de bits hacia la izquierda.
        private static string shiftLeft(string texto, int cantidad)
        {
            int contador = 0;
            StringBuilder resultado = new StringBuilder();
            char[] caracteres = texto.ToCharArray();
            while (contador < cantidad)
            {
                char primero = caracteres[0];
                for (int i = 1; i < caracteres.Length; i++)
                {
                    caracteres[i - 1] = caracteres[i];
                }
                caracteres[caracteres.Length - 1] = primero;
                contador++;
            }

            foreach (var c in caracteres)
            {
                resultado.Append(c);
            }

            return resultado.ToString();
        }

        // Define el proceso principal de DES.
        private static string ProcesoDES(string message, string[] keys)
        {
            string left = message.Substring(0, 32);
            string right = message.Substring(32, 32);
            for (int i = 0; i < 16; i++)
            {
                string temp = right;
                string resultF = funcCompleja(right, keys[i]);
                string resultFXOR = XOR(left, resultF);
                right = resultFXOR;
                left = temp;
            }
            return left + right;
        }

        // Define una funcion compleja usada en el proceso DES.
        private static string funcCompleja(string right, string key)
        {
            // Expande la mitad derecha del mensaje usando una matriz E.
            string rightExp = operacionesVectores(E, right);

            // Aplica una operacion XOR entre la mitad derecha expandida y la llave.
            string rightExpXOR = XOR(rightExp, key);

            // Realiza una sustitucion usando cajas SBOX.
            string sbox = SBOX(rightExpXOR);

            // Aplica una permutacion usando una matriz P.
            return operacionesVectores(P, sbox);
        }

        // Define el proceso de sustitucion usando cajas SBOX.
        private static string SBOX(string data)//procesa bloques de 6 bits de la cadena data
        {
            int posicion = 0;//posicion en data
            int cont = 1; // contador para saber que sbox usar 1 - 8
            int row, col; // fila y columna en la sbox
            StringBuilder sb = new StringBuilder();

            // Procesa bloques de 6 bits del mensaje.
            while (posicion < data.Length)
            {
                // Extrae un bloque de 6 bits.
                string temporal = data.Substring(posicion, 6);

                // Determina la fila y columna en la caja SBOX correspondiente.
                row = Convert.ToByte(temporal[0].ToString() + temporal[5], 2);
                col = Convert.ToByte(temporal.Substring(1, 4), 2);

                // Realiza la sustitucion usando la caja SBOX correspondiente. esta representacion puede no ser de 4 bits 
                switch (cont)
                {
                    //para cada bloque de 6 bits, determina que caja SBOX y encuentra un valor en esa caja usando row y col que se derivan del bloque de 6 bits 
                    case 1: sb.Append(auxiliarFunc(Convert.ToString(S1[row, col], 2), 4)); break;//auxiliar aseegura que la representacion binaria del valor obtenido del sbox tenga logi de 4 bits si es menor se añaden 0 al inicio para alcanzar longitud
                    case 2: sb.Append(auxiliarFunc(Convert.ToString(S2[row, col], 2), 4)); break;
                    case 3: sb.Append(auxiliarFunc(Convert.ToString(S3[row, col], 2), 4)); break;
                    case 4: sb.Append(auxiliarFunc(Convert.ToString(S4[row, col], 2), 4)); break;
                    case 5: sb.Append(auxiliarFunc(Convert.ToString(S5[row, col], 2), 4)); break;
                    case 6: sb.Append(auxiliarFunc(Convert.ToString(S6[row, col], 2), 4)); break;
                    case 7: sb.Append(auxiliarFunc(Convert.ToString(S7[row, col], 2), 4)); break;
                    case 8: sb.Append(auxiliarFunc(Convert.ToString(S8[row, col], 2), 4)); break;
                    default: break;
                }

                // Avanza la posicion para procesar el siguiente bloque.
                posicion += 6;

                // Incrementa el contador para usar la siguiente caja SBOX.
                cont++;
            }

            // Retorna la cadena resultante despues de la sustitucion.
            return sb.ToString();
        }

        // Define un metodo para aplicar operaciones con vectores en DES.
        private static string operacionesVectores(int[] vector, string cadenaBits)//Función para operar con vectores
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < vector.Length; i++)
            {
                char c = cadenaBits[vector[i] - 1];
                sb.Append(c);
            }
            return sb.ToString();
        }

        // Define una operacion XOR para dos cadenas de bits.
        private static string XOR(string txt1, string txt2)
        {
            // Crea un StringBuilder para construir el resultado.
            StringBuilder resultado = new StringBuilder();

            // Aplica la operación XOR bit a bit entre las dos cadenas.
            for (int i = 0; i < txt1.Length; i++)
            {
                resultado.Append(txt1[i] == txt2[i] ? "0" : "1");
            }

            // Retorna la cadena resultante de la operación XOR.
            return resultado.ToString();
        }

        // Convierte una cadena de texto a su representacion en bits.
        private static string StringToBinary(string data)
        {
            // Crea un StringBuilder para construir el resultado.
            StringBuilder sb = new StringBuilder();

            // Convierte cada caracter de la cadena a su representacion en bits.
            foreach (char c in data.ToCharArray())
            {
                sb.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
            }

            // Retorna la representacion en bits de la cadena.
            return sb.ToString();
        }

        // Convierte una representacion en bits a su correspondiente cadena de texto.
        private static string BinaryToString(string data)
        {
            // Crea una lista para almacenar los bytes.
            List<Byte> byteList = new List<Byte>();

            // Convierte cada bloque de 8 bits a su correspondiente byte.
            for (int i = 0; i < data.Length; i += 8)
            {
                byteList.Add(Convert.ToByte(data.Substring(i, 8), 2));
            }

            // Decodifica la lista de bytes a una cadena de texto usando la codificación ISO-8859-1 LATIN-1 representa el alfabeto latino.
            System.Text.Encoding iso_8859_1 = System.Text.Encoding.GetEncoding("ISO-8859-1");
            return iso_8859_1.GetString(byteList.ToArray());
        }

        // Define una funcion auxiliar para completar una cadena de bits con ceros hasta alcanzar una longitud maxima.
        private static string auxiliarFunc(string bits, int max)
        {
            // Agrega ceros al inicio de la cadena hasta alcanzar la longitud maxima.
            while (bits.Length < max)
            {
                bits = "0" + bits;
            }

            // Retorna la cadena con la longitud maxima.
            return bits;
        }

        // Invierte el orden de un arreglo de llaves para el proceso de descifrado.
        private static string[] invLlaves(string[] keys)
        {
            // Crea un arreglo para almacenar las llaves invertidas.
            string[] result = new string[keys.Length];
            int c = keys.Length - 1;

            // Invierte el orden de las llaves.
            for (int i = 0; i < keys.Length; i++)
            {
                result[c] = keys[i];
                c--;
            }

            // Retorna el arreglo de llaves invertidas.
            return result;
        }


        private static string AddPadding(string message, int blockSize = 8)
        {
            int paddingLength = blockSize - (message.Length % blockSize);
            char paddingChar = (char)paddingLength;
            return message + new string(paddingChar, paddingLength);
        }


        private static string RemovePadding(string msjCifrado)
        {
            int paddingLength = (int)msjCifrado[msjCifrado.Length - 1];
            return msjCifrado.Substring(0, msjCifrado.Length - paddingLength);
        }


    }
}
