using System;

using System.Text;
using System.Collections;


namespace Tests
{
    class OrdenaListItem : IComparer
    {
        /// <summary>
        /// Construtor da classe OrdenaListView
        /// </summary>
        public OrdenaListItem() { }

        /// <summary>
        /// Método de Comparação de Collections
        /// </summary>
        /// <param name="x">Objeto LisItem A</param>
        /// <param name="y">Objeto LisItem B</param>
        /// <returns></returns>
        public int Compare(object x, object y)
        {
            int indice = 0;

            ListItem objetoA = (ListItem)x;
            ListItem objetoB = (ListItem)y;

            //Tratamento para nao fazer comparação de objetos nulos
            if (objetoA.Valor != null && objetoB.Valor != null)
                indice = objetoA.Valor.ToString().CompareTo(objetoB.Valor.ToString());

            return indice;

        }

    }
}