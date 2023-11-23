using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB01_EDII.Arbol
{
    public class AVL<T>
    {
        public Nodo<T>? Root;
        public AVL()
        {
            Root = null;
        }

        public bool IsEmpty() //Función para verificar si el árbol esta vacío
        {
            return Root == null;
        }

        public int GetBalance(Nodo<T> n1) //Función para obtener el factor de balance de un nodo
        {
            if (n1.Left == null && n1.Right == null)
            {
                return 0;
            }

            else if (n1.Left == null)
            {
                return -n1.Right!.Height;
            }

            else if (n1.Right == null)
            {
                return n1.Left!.Height;
            }

            else
            {
                return n1.Left!.Height - n1.Right!.Height;
            }
        }

        private void SetHeight(Nodo<T> nodo) //Procedimiento para asignar la altura de un nodo
        {
            //Se toma el nodo hijo con mayor altura
            if (nodo.Left == null || nodo.Right == null)
            {
                if (nodo.Left == null && nodo.Right == null)
                {
                    nodo.Height = 1;
                }

                else if (nodo.Left == null)
                {
                    nodo.Height = 1 + nodo.Right!.Height;
                }

                else
                {
                    nodo.Height = 1 + nodo.Left!.Height;
                }
            }

            else if (nodo.Left!.Height > nodo.Right!.Height) //Nodo 'One' mayor?
            {
                nodo.Height = 1 + nodo.Left!.Height; //Si
            }
            else
            {
                nodo.Height = 1 + nodo.Right!.Height; //No
            }
        }

        //si la comparacion mediante condition1 no resuelve la ubicacion del elemento, es decir, los elementos son considerados iguales con el primer criterio, entonces se utiliza condition2 como un criterio secundario.
        public void Add(T item, Delegate condition1) //Procedimiento para añadir elementos al árbol
        {
            Root = AddInAVL(Root!, item, condition1);
        }

        //funcion agrega un nuevo nodo al arbol, utiliza condition1 y condition2 para determinar la posicion correcta del nodo.Si el arbol se desequilibra despues de la insercion, se llama a una funcion para equilibrarlo de nuevo.
        private Nodo<T> AddInAVL(Nodo<T>? nodo, T item, Delegate condition1)
        {
            if (nodo == null)
            {
                nodo = new Nodo<T>(item);
            }
            else
            {
                if ((int)condition1.DynamicInvoke(item, nodo!.Value) < 0) //El valor a agregar es menor al valor del nodo (condicion 1)
                {
                    nodo.Left = AddInAVL(nodo.Left!, item, condition1);
                }

                else if ((int)condition1.DynamicInvoke(item, nodo!.Value!) > 0)
                {
                    nodo.Right = AddInAVL(nodo.Right!, item, condition1);
                }
            }

            nodo = ReBalance(nodo);
            return nodo;

        }

        //elimina un nodo del arbol. utiliza condition1 y condition2 para encontrar el nodo a eliminar. Si el arbol se desequilibra despues de la eliminacion, se llama a una funcion para equilibrarlo.
        public void Delete(T item, Delegate condition1) //Procedimiento para eliminar elementos del árbol
        {
            Root = DeleteInAVL(Root!, item, condition1);
        }

        private Nodo<T> DeleteInAVL(Nodo<T>? nodo, T item, Delegate condition1)
        {
            if (nodo == null)
            {
                return nodo;
            }

            else if ((int)condition1.DynamicInvoke(item, nodo.Value) < 0)
            {
                nodo.Left = DeleteInAVL(nodo.Left!, item, condition1);
            }

            else if ((int)condition1.DynamicInvoke(item, nodo.Value) > 0)
            {
                nodo.Right = DeleteInAVL(nodo.Right!, item, condition1);
            }

            else
            {
                if (nodo.Left == null && nodo.Right == null) //Es una hoja
                {
                    nodo = null;
                    return nodo;
                }

                else if (nodo.Left == null && nodo.Right != null)
                {
                    Nodo<T> temp = nodo.Right;
                    nodo.Right = null;
                    nodo = temp;
                }

                else if (nodo.Left != null && nodo.Right == null)
                {
                    Nodo<T> temp = nodo.Left;
                    nodo.Left = null;
                    nodo = temp;
                }

                else
                {
                    Nodo<T>? temp = RightestfromLeft(nodo.Left!);
                    nodo.Value = temp!.Value;
                    nodo.Left = DeleteInAVL(nodo.Left!, temp.Value, condition1);
                }
            }

            nodo = ReBalance(nodo);
            return nodo;
        }


        //Esta funcion busca un nodo en el arbol y, si lo encuentra, actualiza su valor. Utiliza condition1 y condition2 para localizar el nodo.
        public void Patch(T item, Delegate condition1)
        {
            if (item == null)
            {
                return;
            }
            if (IsEmpty())
            {
                return;
            }
            bool flag = false;
            Nodo<T>? temporal = this.Root;
            while (temporal != null && flag != true)
            {
                if ((int)condition1.DynamicInvoke(item, temporal.Value) < 0)
                {
                    temporal = temporal.Left;
                }
                else if ((int)condition1.DynamicInvoke(item, temporal.Value) > 0)
                {
                    temporal = temporal.Right;
                }
                else
                {
                    flag = true;
                }
            }
            if (temporal == null)
            {
                return;
            }
            temporal.Value = item;
        }


        //Esta funcion busca en el árbol todos los nodos que coincidan con un criterio específico (usando condition1) y agrega esos nodos a una lista de resultados.
        public void QueryResults(Nodo<T> temporal, T item, Delegate condition1, List<T> Results) //Procedimiento que llena una lista con todos los registros que sean iguales al elemento ingresado
        {
            if (temporal == null)
            {
                return;
            }
            if (temporal.Left != null)
            {
                QueryResults(temporal.Left, item, condition1, Results);
            }
            if ((int)condition1.DynamicInvoke(item, temporal.Value) == 0)
            {
                Results.Add(temporal.Value);
            }
            if (temporal.Right != null)
            {
                QueryResults(temporal.Right, item, condition1, Results);
            }
        }


        //esta funcion busca un elemento en arbol AVL utilizando una condicion especifica proporcionada por el delegado. Si encuentra el elemento, lo devuelve de lo contrario, devolvera el ultimo nodo que visito antes de salir del bucle.
        public T? Search(T item, Delegate condition)//item el elemento a buscar en el arbol, y condition un delegado que defino para la condicion de busqueda y que devuelve un entero 
        {
            Nodo<T>? temporal = Root; //inicializo mi nodo temporal con la raiz del arbol es la posicion inicio de busqueda
            while (temporal != null && (int)condition.DynamicInvoke(item, temporal!.Value) != 0) //aseguro que el nodo temporal no este null, uso el delegado condition para comparar el item buscado con el valor actual de dicho nodo
            {
                if ((int)condition.DynamicInvoke(item, temporal.Value) > 0)//item buscado es mayor que el valor del nodo actual
                {
                    temporal = temporal.Right;//se mueve para el nodo derecho 
                }
                else
                {
                    temporal = temporal.Left;//caso contrario el item a buscar es menor que el valor del nodo actual y se mueve hacia nodo izquierdo 
                }   
            }
            if (temporal == null)
            {
                Console.WriteLine("El elemento no se encuentra en el arbol."); // Informa al usuario que el DPI no se encontró.
                return default(T);
            }

            return temporal!.Value;//cuando termina el bucle si encontro item o llego al fin del arbol me devuelve el valor del nodo temporal 
        }

        private Nodo<T> ReBalance(Nodo<T> nodo)
        {
            SetHeight(nodo); //Se actualiza la altura del nodo
            int FE = GetBalance(nodo); //Se obtiene el factor de equilibrio del nodo
            if (FE < -1)
            {
                if (GetBalance(nodo.Right!) < 0)
                {
                    nodo = LeftRotation(nodo);//Rotación izquierda
                }
                else
                {
                    nodo = DoubleLeftRotation(nodo);//Rotación doble a la izquierda
                }
            }

            else if (FE > 1)
            {
                if (GetBalance(nodo.Left!) > 0)
                {
                    nodo = RightRotation(nodo);//Rotación derecha
                }
                else
                {
                    nodo = DoubleRightRotation(nodo);//Rotación doble a la derecha
                }
            }

            return nodo;
        }
        private Nodo<T> RightRotation(Nodo<T> nodo) //Rotación simple a la derecha
        {
            Nodo<T> temp = nodo.Left!;
            nodo.Left = temp.Right!;
            temp.Right = nodo;
            SetHeight(nodo);
            SetHeight(temp);
            return temp;
        }

        private Nodo<T> LeftRotation(Nodo<T> nodo) //Rotación simple a la izquierda
        {
            Nodo<T> temp = nodo.Right!;
            nodo.Right = temp.Left!;
            temp.Left = nodo;
            SetHeight(nodo);
            SetHeight(temp);
            return temp;
        }

        private Nodo<T> DoubleRightRotation(Nodo<T> nodo) //Rotación doble a la derecha
        {
            Nodo<T> temp = nodo.Left!;
            temp = LeftRotation(temp);
            nodo.Left = temp;
            nodo = RightRotation(nodo);
            return nodo;
        }

        private Nodo<T> DoubleLeftRotation(Nodo<T> nodo) //Rotación doble a la izquierda
        {
            Nodo<T> temp = nodo.Right!;
            temp = RightRotation(temp);
            nodo.Right = temp;
            nodo = LeftRotation(nodo);
            return nodo;
        }

        private Nodo<T>? RightestfromLeft(Nodo<T>? nodo) //Función que devuelve el nodo más grande del subárbol izquierdo
        {
            Nodo<T> nodoTemp = nodo;
            while (nodoTemp!.Right != null)
            {
                nodoTemp = nodoTemp.Right;
            }
            return nodoTemp;
        }

        public void InOrder(Action<T> action)
        {
            InOrderTraversal(Root, action);
        }

        private void InOrderTraversal(Nodo<T>? nodo, Action<T> action)
        {
            if (nodo != null)
            {
                InOrderTraversal(nodo.Left, action);
                action(nodo.Value);
                InOrderTraversal(nodo.Right, action);
            }
        }

    }
}
