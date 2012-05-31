using System;
using System.Collections;
using System.Collections.Generic;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace Tests
{
    public interface IGeodatabase
    {
        /// <summary>
        /// Retorna todos os elementos do domínio na ordem em que aparecem na sua definição
        /// </summary>
        /// <param name="nomeDominio">Nome do domínio que se deseja ler os dados</param>
        /// <returns>Lista com os elementos do tipo Cod,Descrição para utilização no sistema em geral</returns>
        ArrayList Dominios(String nomeDominio, IWorkspace ws);
        void AbrirGeodatabase();
        void FecharGeodatabase(bool saveEdits);
        int InserirRegistroGis(List<ListItem> itens, string nomeTabela);
        int InserirFeatureGis(List<ListItem> itens, string nomeClasse, IGeometry geometria);
        int CriarRegistroGis(List<ListItem> itens, string nomeTabela);
        int AtualizarRegistroGis(List<ListItem> itens, string nomeTabela);
        bool AtualizarRegistroGis(List<ListItem> itens, IRow row);
        bool WorkspaceEmEdicao();
        bool ExisteRegistro(string nomeTabela, string campoChave, int valor);
        ITable GetTable(string tableName);
        IFeatureClass GetFeatureClass(string className);
        List<ListItem> GetRow(int oid, string tableName);
        List<ListItem> GetRow(int id, string tableName, string fieldName);        
        List<ListItem> GetRow(string id, string tableName, string fieldName);      
        List<ListItem> GetFeature(string id, string className, string fieldName);
        List<ListItem> GetARow(string tableName);      
        void DeleteRow(int objectId, string tableName);
        bool IsBeenEdit();
        void PararSessaoDeEdicao(bool saveEdits);
        void IniciarSessaoDeEdicao(bool undoRedo);
        List<GisDataAdapter> GetListaDeRows(string tableName, string condicao, string orderingFields);
        List<GisDataAdapter> GetRowsPorQuery(string tableName, string whereClause, string orderingFields);
        int CriarFeatureDePonto(string featureClassName, double x, double y, List<ListItem> lstValores);
        void DeleteFeatures(string nomeFeatureClasse, string condicao);
        void CriarRelationShipPorObjecto(String tableNameOrign, int oidOrign, String tableNameDestination, int oidDestination, string relName, Geodatabase geodatabase);
        List<GisDataAdapter> GetRows(string nometabela, string whereClause, string fields);
    }
}