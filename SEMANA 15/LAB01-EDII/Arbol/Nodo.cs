using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB01_EDII.Arbol
{
    public class Nodo<T>
    {
        public T Value { get; set; }
        public Nodo<T>? Left { get; set; }  // significa que la propiedad left puede contener un valor null o una instancia de Nodo<T>
        public Nodo<T>? Right { get; set; }
        public int Height { get; set; }

        public Nodo(T value) //defino el constructor de la clase Nodo<T>
        {
            this.Value = value;
            Left = null;
            Right = null;
            Height = 1;
            //FE = altura subarbol derecho - altura subarbolizquierdo por definicion para un arbol AVL este valor debe ser -1, 0 , 1 si el factor equilibrio de un Nodo es 0 -> el Nodo esta equilibrado y sus subarboles tienen exactamente la misma altura.

        }
    }
}
