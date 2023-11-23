using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LAB01_EDII.Arbol;

namespace LAB01_EDII.Modelo
{
    public class Persona
    {
        public string name { get; set; }
        public string dpi { get; set; }
        public DateTime datebirth { get; set; }
        public string address { get; set; }

        //agregando lo nuevo que piden en practica 2
        public string[] companies { get; set; }//mi lista de compañias que tiene cada registro

        public string recluiter { get; set; }
        public List<string> huffmanCodes { get; set; } //lista de codigos Huffman para cada compañía
        public List<Dictionary<char, string>> huffmanDictionaries { get; set; } //lista de diccionarios Huffman utilizados para la codificacion


        //public string[] companies { get; set; }
        //public List<Dictionary<char, Letra>> dictionaries { get; set; }
        //public List<decimal> codes { get; set; }
    }
}
