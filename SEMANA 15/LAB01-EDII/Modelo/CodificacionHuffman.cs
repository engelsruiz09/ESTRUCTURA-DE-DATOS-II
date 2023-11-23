using LAB01_EDII.Arbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB01_EDII.Modelo
{
    public class CodificacionHuffman
    {
        //raiz del arbol para el arbol de huffman
        private Huffmannode Raiz;

        //mi metodo para construir el arbol de Huffman
        //diccionario con caracteres y las respectivas frecuencias, devuelve la raiz del arbol construido
        //char cualquier caracter o simbolo, letra, espacio, numero del texto, int cuantas veces aparece el caracter en el texto
        private Huffmannode ConstruirArbol(Dictionary<char, int> diccionario)
        {
            // una lista de nodos, inicialmente con todos los caracteres y sus frecuencias
            List<Huffmannode> nodos = diccionario.Select(item => new Huffmannode
            {
                Character = item.Key,
                Frecuencia = item.Value
            }).ToList();

            while (nodos.Count > 1)//mientras hay mas de un nodo en la lista
            {
                //ordeno nodos por frecuencia en orden ascendente
                nodos = nodos.OrderBy(node => node.Frecuencia).ToList();

                //tomo los nodos con menor frecuencia
                Huffmannode izquierda = nodos[0];
                Huffmannode derecha = nodos[1];

                //creo nuevo nodo que combine los dos nodos de menor frecuencia
                Huffmannode combinado = new Huffmannode
                {
                    Frecuencia = izquierda.Frecuencia + derecha.Frecuencia,
                    Izquierda = izquierda,
                    Derecha = derecha
                };

                //elimino los nodos que ya fueron combinados
                //elimino dos veces ya que despues de la primera llamada el nodo que estaba en el indice 1 y se desplaza al indice 0
                nodos.RemoveAt(0);
                nodos.RemoveAt(0);
                nodos.Add(combinado);//agrego el nodo combinado a la lista
            }

            return nodos[0]; //el ultimo nodo es la raiz del arbol
        }

        //codifica el texto usando el huffman
        //devuelve el texto codificado en huffman
        public string Coding(string texto, Dictionary<char, int> diccionario)
        {
            {
                //se arma el arbol de huffman
                Raiz = ConstruirArbol(diccionario);
                //creo un diccionario para guardar el codigohuffman de cada caracter
                Dictionary<char, string> huffmanCodes = new Dictionary<char, string>();
                //genero los codigos huffman para cada caracter
                GenerarCodigos(Raiz, "", huffmanCodes);

                //muestro mi diccionario de huffman
                //Console.WriteLine("Diccionario de Huffman:");//descomentar para ver la clave y su frecuencia
                //foreach (var item in huffmanCodes)//descomentar para ver la clave y su frecuencia
                //{
                //    Console.WriteLine($"{item.Key}: {item.Value}");
                //}

                string codigoHuffman = "";//codifico el texto usando los codifos de huffman
                foreach (char c in texto)
                {
                    codigoHuffman += huffmanCodes[c];
                }

                //Console.WriteLine($"Texto original: {texto}");//descomentar para ver la clave y su frecuencia
                //Console.WriteLine($"Texto codificado: {codigoHuffman}");//descomentar para ver la clave y su frecuencia
                return codigoHuffman;
            }
        }

        //creo un metodo recursivo para generar los codigos de huffman
        private void GenerarCodigos(Huffmannode nodo, string codigo, Dictionary<char, string> huffmanCodes)
        {
            if (nodo == null)//si el nodo es nullo me salgo
                return;

            //es para comprobar si nodol.character tiene un valor real de caracter o si es null
            if (!nodo.Character.HasValue)//si el nodo no tiene caracter asociado, es un nodo intermedio
            {
                //genero recursivamente codigos para el hijo izquiero y derecho
                GenerarCodigos(nodo.Izquierda, codigo + "0", huffmanCodes);
                GenerarCodigos(nodo.Derecha, codigo + "1", huffmanCodes);
            }
            else
            {
                //si nodo presenta un caracter añado el codigo al diccionario
                huffmanCodes[nodo.Character.Value] = codigo;
            }
        }

        //aca decodifico el codigo de huffman 
        public string Decode(string codigoHuffman)
        {
            string textoDecodificado = "";
            Huffmannode nodoActual = Raiz;

            foreach (char bit in codigoHuffman)
            {
                if (bit == '0')
                {//muevo a la izquierda o derecha en el arbol 
                    nodoActual = nodoActual.Izquierda;
                }
                else
                {
                    nodoActual = nodoActual.Derecha;
                }
                    
                if (nodoActual.Character.HasValue)//si el nodo tiene un caracter , añado el caracter al texto decodificado y regreso a la raiz
                {
                    textoDecodificado += nodoActual.Character.Value;
                    nodoActual = Raiz;
                }
            }
            //Console.WriteLine($"Texto decodificado: {textoDecodificado}");
            return textoDecodificado;
        }

        public void SetDictionary(Dictionary<char, int> diccionario)
        {
            Raiz = ConstruirArbol(diccionario);
        }


        //// Método de prueba
        //public void Test()
        //{
        //    string testString = "7686671562353holacomoestasbienytujulioanthonyengelsruizcoto";
        //    Dictionary<char, Letra> diccionario = CreateDictionary(testString); // Asume que tienes este método
        //    string encoded = Coding(testString, diccionario);
        //    string decoded = Decode(encoded);
        //    Console.WriteLine($"Original: {testString}");
        //    Console.WriteLine($"Decoded: {decoded}");
        //}

        //private Dictionary<char, Letra> CreateDictionary(string texto)
        //{
        //    Dictionary<char, Letra> diccionario = new Dictionary<char, Letra>();

        //    foreach (char c in texto)
        //    {
        //        if (diccionario.ContainsKey(c))
        //        {
        //            diccionario[c].frecuencia++;
        //        }
        //        else
        //        {
        //            diccionario[c] = new Letra { frecuencia = 1 };
        //        }
        //    }

        //    return diccionario;
        //}


    }
}
