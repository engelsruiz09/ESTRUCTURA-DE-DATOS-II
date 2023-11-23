using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB01_EDII.Arbol
{
    public class Huffmannode
    {
        public char? Character { get; set; } //el caracter representado por este nodo (si existe).
        public int Frecuencia { get; set; } //frecuencia del caracter.
        public Huffmannode Izquierda { get; set; } //nodo hijo izquierdo.
        public Huffmannode Derecha { get; set; } //nodo hijo derecho.
    }
}
