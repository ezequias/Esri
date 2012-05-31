using System;
using System.Collections;
using System.Text;

namespace Tests
{

    /// <summary>
    /// Lista simples com elementos comumente utilizados em controles do tipo combobox
    /// ou listas do tipo COD, Descrição comumente utilizadas na aplicação
    /// </summary>
    public class ListItem
    {
        private string chave;

        /// <summary>
        /// Atributo valor (comumente utilizado como código na lista)
        /// </summary>
        private object codigo;

        public object Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }
        /// <summary>
        /// Atributo nome
        /// </summary>
        private object descricao;
        /// <summary>
        /// Construtor da classe ListItem
        /// </summary>
        /// <param name="valor">Valor [codigo] da lista</param>
        public ListItem(object codigo)
        {
            this.codigo = codigo;
        }

        /// <summary>
        /// Construtor da lista com passagem parcial de parâmetros
        /// </summary>
        /// <param name="valor">Valor na lista</param>
        /// <param name="nome">Nome na lista</param>
        public ListItem(String chave, object valor)
        {
            this.Chave = chave;
            this.Valor = valor;
        }

        /// <summary>
        /// Construtor da lista com passagem parcial de parâmetros
        /// </summary>
        /// <param name="codigo">Código na lista</param>
        /// <param name="valor">Valor na lista</param>
        public ListItem(object codigo, object valor)
        {
            this.Codigo = codigo;
            this.Valor = valor;
        }

        /// <summary>
        /// Atributo chave da lista
        /// </summary>
        public string Chave
        {
            get { return chave; }
            set { chave = value; }
        }

        /// <summary>
        /// Atributo descrição da lista
        /// </summary>
        public object Valor
        {
            get { return descricao; }
            set { descricao = value; }
        }

        public static ListItem GetItem(System.Collections.Generic.List<ListItem> list, object chave)
        {
            ListItem lstRetun = null;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Chave.Equals(chave))
                {
                    lstRetun = list[i];
                    break;
                }
            }

            if (lstRetun.Valor is DBNull)
                return null;
            else
                return lstRetun;
        }
        /// <summary>
        /// Converte o valor para String
        /// </summary>
        /// <returns>O valor no formato String</returns>
        public override String ToString()
        {
            return Valor.ToString();
        }
    }
}