using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB01_EDII.Modelo
{
    public class LZW
    {
        public static List<int> encode(string text) // retorno una lista de enteros representando el texto codificado 
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            for (int i = 0; i < 256; i++) // inicializo el diccionario con todos los caracteres ASCII
            {
                dictionary.Add(((char)i).ToString(), i);
            }
            string w = string.Empty;//variable para almacenar la secuencia actual de caracteres
            List<int> resul = new List<int>();// lista para almacenar los codigos resultantes

            foreach (var c in text)//itero sobre cada caracter del texto
            {
                string temp = w + c; //concateno el caracter actual a la secuencia
                if (dictionary.ContainsKey(temp))//si el diccionario ya contiene la secuencia, sigue acumulandola
                {   
                    w = temp;
                }
                else
                {
                    resul.Add(dictionary[w]); //si no se encuentra la secuencia se agrega al resultado y al diccionario
                    dictionary.Add(temp, dictionary.Count);
                    w = c.ToString();
                }
            }

            if (!string.IsNullOrEmpty(text))//si el texto no esta vacio agrego el ultimo valor codificado
            {
                resul.Add(dictionary[w]);
            }

            return resul;
        }

        public static string Decompress(List<int> compressed)
        {
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            for (int i = 0; i < 256; i++)
            {
                dictionary.Add(i, ((char)i).ToString());
            }
               
            string w = dictionary[compressed[0]];//se obtiene el primer caracter del texto descomprimido
            compressed.RemoveAt(0);//elimino el primer elemento de la lista comprimida
            StringBuilder decompressed = new StringBuilder(w);//stringbuilder para construir el texto comprimido

            foreach (int k in compressed)//itero sobre cada codigo de la lista comprimida
            {
                string entry = "";
                if (dictionary.ContainsKey(k))
                {
                    entry = dictionary[k];
                }     
                else if (k == dictionary.Count)
                {
                    entry = w + w[0];
                }
                decompressed.Append(entry);//añado la secuencia comprimida al resultado 

                //la nueva secuencia se añade al diccionario 
                dictionary.Add(dictionary.Count, w + entry[0]);

                w = entry;
            }

            return decompressed.ToString();
        }

    }
}
