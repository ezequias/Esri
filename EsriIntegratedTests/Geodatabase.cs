using System;
using ESRI.ArcGIS;
using ESRI.ArcGIS.Geodatabase;
using System.Collections;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;

using System.Collections.Generic;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;

namespace Tests 
{    
    /// <summary>
    /// Summary description for Geodatabase
    /// </summary>
    public class Geodatabase : IGeodatabase
    {
        public enum InitializeLicense
        {
            Intialize,
            NotInitialize
        }

        const string TimestampFormat = "yyyy-MM-dd HH:mm:ss";
        public static IWorkspace ws { get; set; }

        public Geodatabase(object pWorkspace, InitializeLicense initialize)
        {
            if (pWorkspace != null)
            {
                ws = pWorkspace as IWorkspace;
            }
            else
            {
                if (initialize == InitializeLicense.Intialize)
                {
                    Geodatabase.InitializeLicenseEditor();
                }

            }
        }

        #region IGeodatabase

        public int CriarFeatureDePonto(string featureClassName, double x, double y, List<ListItem> lstValores)
        {
            int r = -1;

            if (IsBeenEdit())
            {
                IFeatureClass featureClass = ((IFeatureWorkspace)ws).OpenFeatureClass(featureClassName);
                IFeature ftr = featureClass.CreateFeature();
                for (int i = 0; i < lstValores.Count; i++)
                {
                    ftr.set_Value(ftr.Fields.FindField(lstValores[i].Chave.ToString()), lstValores[i].Valor);
                }

                IPoint pt = new PointClass();
                pt.X = x;
                pt.Y = y;
                ftr.Shape = pt;
                ftr.Store();
                r = ftr.OID;
            }
            else
            {
                throw new Exception("Não há sessão de edição");
            }

            return r;
        }

        /// <summary>
        /// Retorna todos os elementos do domínio na ordem em que aparecem na sua definição
        /// </summary>
        /// <param name="nomeDominio">Nome do domínio que se deseja ler os dados</param>
        /// <returns>Lista com os elementos do tipo Cod,Descrição para utilização no sistema em geral</returns>
        public ArrayList Dominios(String nomeDominio, IWorkspace ws)
        {
            var workSpaceDomains = (IWorkspaceDomains)ws;
            System.Collections.IList valores = new System.Collections.ArrayList();

            if (workSpaceDomains.get_DomainByName(nomeDominio) is ICodedValueDomain)
            {
                var codedDomain = (ICodedValueDomain)workSpaceDomains.get_DomainByName(nomeDominio);
                for (int i = 0; i < codedDomain.CodeCount; i++)
                {
                    valores.Add(new ListItem(codedDomain.get_Value(i).ToString(), codedDomain.get_Name(i).ToString()));
                }
            }
            else // Domínio de Range
            {
                var rangeDomain = (IRangeDomain)workSpaceDomains.get_DomainByName(nomeDominio);
                for (var i = Convert.ToInt32(rangeDomain.MinValue); i < Convert.ToInt32(rangeDomain.MaxValue); i++)
                {
                    valores.Add(new ListItem(i.ToString(), i.ToString()));
                }
            }

            //ordendando a lista
            var listaOrdenada = new ArrayList(valores);
            IComparer ordernador = new OrdenaListItem();
            listaOrdenada.Sort(ordernador);

            return listaOrdenada;
        }

        public void AbrirGeodatabase()
        {
            IPropertySet propSet = null;

            propSet = new PropertySetClass();

            propSet.SetProperty("SERVER", ConnectionProperties.server);
            propSet.SetProperty("INSTANCE", ConnectionProperties.instance);
            propSet.SetProperty("USER", ConnectionProperties.user);
            propSet.SetProperty("PASSWORD", ConnectionProperties.password);
            propSet.SetProperty("VERSION", ConnectionProperties.version);

            IWorkspaceFactory pSdeFact = new SdeWorkspaceFactory();

            ws = pSdeFact.Open(propSet, 0);

            if (ws != null)
            {
                (ws as IWorkspaceEdit).StartEditing(false);
                (ws as IWorkspaceEdit).StartEditOperation();
            }
        }

        public void FecharGeodatabase(bool saveEdits)
        {
            if (ws != null && IsBeenEdit())
            {
                (ws as IWorkspaceEdit).StopEditOperation();
                (ws as IWorkspaceEdit).StopEditing(saveEdits);
            }
        }

        public int InserirRegistroGis(List<ListItem> itens, string nomeTabela)
        {
            ITable tabela = (ws as IFeatureWorkspace).OpenTable(nomeTabela);

            //Assume-se que o ObjectID é a primeira coluna da tabela
            IRow row = tabela.CreateRow();
            for (int i = 1; i < itens.Count; i++)
            {
                int fieldID = tabela.Fields.FindField(itens[i].Chave.ToString());
                if (!GisUtils.ValorValido(tabela.Fields.get_Field(fieldID), itens[i].Valor))
                    throw new Exception("Valor inválido " + "[" + tabela.Fields.get_Field(i).AliasName + "]");

                row.set_Value(fieldID, itens[i].Valor);
            }

            row.Store();

            return row.OID;
        }

        public int InserirFeatureGis(List<ListItem> itens, string nomeClasse, IGeometry geometria)
        {
            IFeatureClass fclass = (ws as IFeatureWorkspace).OpenFeatureClass(nomeClasse);

            //Assume-se que o ObjectID é a primeira coluna da tabela
            IFeature ftr = fclass.CreateFeature();
            for (int i = 1; i < itens.Count; i++)
            {
                int fieldID = fclass.Fields.FindField(itens[i].Chave.ToString());
                if (!GisUtils.ValorValido(fclass.Fields.get_Field(fieldID), itens[i].Valor))
                    throw new Exception("Valor inválido " + "[" + fclass.Fields.get_Field(i).AliasName + "]");

                ftr.set_Value(fieldID, itens[i].Valor);
            }

            ftr.Shape = geometria;
            ftr.Store();

            return ftr.OID;
        }

        public int CriarRegistroGis(List<ListItem> itens, string nomeTabela)
        {
            ITable tabela = (ws as IFeatureWorkspace).OpenTable(nomeTabela);

            IRow newRow = tabela.CreateRow();

            if (newRow != null)
            {
                if (IsBeenEdit())
                {
                    for (int i = 0; i < itens.Count; i++)
                    {
                        int fieldID = tabela.Fields.FindField(itens[i].Chave.ToString());
                        if (!GisUtils.ValorValido(tabela.Fields.get_Field(fieldID), itens[i].Valor))
                            throw new Exception("Valor inválido " + "[" + tabela.Fields.get_Field(i).AliasName + "]");

                        newRow.set_Value(fieldID, itens[i].Valor);
                    }
                }
                else
                {
                    throw new Exception("Não há sessão de edição");
                }
            }

            newRow.Store();

            return newRow.OID;
        }

        public int AtualizarRegistroGis(List<ListItem> itens, string nomeTabela)
        {
            ITable tabela = (ws as IFeatureWorkspace).OpenTable(nomeTabela);

            IRow row = tabela.GetRow(Convert.ToInt32(ListItem.GetItem(itens, "OBJECTID").Valor));
            if (row != null)
            {
                if (IsBeenEdit())
                {
                    //Assume-se que o ObjectID é a primeira coluna da tabela
                    for (int i = 1; i < itens.Count; i++)
                    {
                        if (itens[i].Valor == null || itens[i].Chave.ToString() == "SHAPE" || itens[i].Chave.ToString() == "SHAPE.AREA" || itens[i].Chave.ToString() == "SHAPE.LEN")
                            continue;
                        int fieldID = tabela.Fields.FindField(itens[i].Chave.ToString());
                        if (!GisUtils.ValorValido(tabela.Fields.get_Field(fieldID), itens[i].Valor))
                            throw new Exception("Valor inválido " + "[" + tabela.Fields.get_Field(i).AliasName + "]");

                        if (!(itens[i].Valor is System.DBNull) && itens[i].Valor != String.Empty)
                            row.set_Value(fieldID, itens[i].Valor);
                    }
                }
                else
                {
                    throw new Exception("Não há sessão de edição");
                }
            }

            row.Store();

            return row.OID;
        }

        public bool AtualizarRegistroGis(List<ListItem> itens, IRow row)
        {
            bool r = false;

            //Assume-se que o ObjectID é a primeira coluna da tabela
            for (int i = 1; i < itens.Count; i++)
            {
                int fieldID = row.Fields.FindField(itens[i].Valor.ToString());
                if (itens[i].Valor == null || itens[i].Chave.ToString() == "SHAPE" || itens[i].Chave.ToString() == "SHAPE.AREA" || itens[i].Chave.ToString() == "SHAPE.LEN")
                    continue;
                if (!GisUtils.ValorValido(row.Fields.get_Field(fieldID), itens[i].Chave))
                    throw new Exception("Valor inválido " + "[" + row.Fields.get_Field(i).AliasName + "]");

                row.set_Value(fieldID, itens[i].Chave);
            }

            row.Store();

            return r;
        }

        public bool WorkspaceEmEdicao()
        {
            bool r = false;

            if (ws != null)
            {
                IWorkspaceEdit wsEdit = ws as IWorkspaceEdit;

                if (wsEdit != null)
                {
                    r = wsEdit.IsBeingEdited();
                }
            }

            return r;
        }
        #endregion

        #region Inicialização de Licenças

        //Editor
        public static void InitializeLicenseEditor()
        {
            ESRI.ArcGIS.RuntimeManager.BindLicense(ProductCode.Desktop, LicenseLevel.Standard);
            ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Desktop);
        }
        #endregion
        public bool ExisteRegistro(string nomeTabela, string campoChave, int valor)
        {
            bool retorno = false;

            ITable tabela = (ws as IFeatureWorkspace).OpenTable(nomeTabela);
            IQueryFilter qf = new QueryFilterClass();
            qf.SubFields = campoChave;

            if (campoChave == "OBJECTID")
            {
                IRow row = tabela.GetRow(valor);
                retorno = row != null;
            }
            else
            {
                int idCampo = tabela.Fields.FindField(campoChave);
                if (idCampo >= 0)
                {
                    qf.WhereClause = campoChave + "=" + valor;
                    retorno = tabela.Search(qf, false).NextRow() != null;
                }
            }

            return retorno;
        }

        public ITable GetTable(string tableName)
        {
            return (ws as IFeatureWorkspace).OpenTable(tableName);
        }

        public IFeatureClass GetFeatureClass(string className)
        {
            return (ws as IFeatureWorkspace).OpenFeatureClass(className);
        }

        public List<ListItem> GetRow(int oid, string tableName)
        {
            List<ListItem> lstItens = new List<ListItem>();
            IRow row = (ws as IFeatureWorkspace).OpenTable(tableName).GetRow(oid);

            if (row == null)
                return null;

            for (int i = 0; i < row.Fields.FieldCount; i++)
            {
                lstItens.Add(new ListItem(row.Fields.get_Field(i).Name, row.get_Value(i)));
            }

            return lstItens;
        }

        /// <summary>
        /// Retorna uma row baseada em um campo específico
        /// </summary>
        /// <param name="id">Valor de ID</param>
        /// <param name="tableName">Nome da Tabela</param>
        /// <param name="fieldName">Nome do campo a ser pesquisado</param>
        /// <returns>Lista de campos e seus valores</returns>
        public List<ListItem> GetRow(int id, string tableName, string fieldName)
        {
            List<ListItem> lstItens = new List<ListItem>();
            ITable table = (ws as IFeatureWorkspace).OpenTable(tableName);
            IQueryFilter qf = new QueryFilterClass();

            qf.WhereClause = fieldName + "=" + id;


            IRow row = table.Search(qf, false).NextRow();

            if (row == null)
                return null;

            for (int i = 0; i < row.Fields.FieldCount; i++)
            {
                lstItens.Add(new ListItem(row.Fields.get_Field(i).Name, row.get_Value(i)));
            }

            return lstItens;
        }

        /// <summary>
        /// Retorna uma row baseada em um campo específico
        /// </summary>
        /// <param name="id">Valor de ID</param>
        /// <param name="tableName">Nome da Tabela</param>
        /// <param name="fieldName">Nome do campo a ser pesquisado</param>
        /// <returns>Lista de campos e seus valores</returns>
        public List<ListItem> GetRow(string id, string tableName, string fieldName)
        {
            List<ListItem> lstItens = new List<ListItem>();
            ITable table = (ws as IFeatureWorkspace).OpenTable(tableName);
            IQueryFilter qf = new QueryFilterClass();

            qf.WhereClause = "TRIM(" + fieldName + ")=" + "'" + id + "'";


            IRow row = table.Search(qf, false).NextRow();

            if (row == null)
                return null;

            for (int i = 0; i < row.Fields.FieldCount; i++)
            {
                lstItens.Add(new ListItem(row.Fields.get_Field(i).Name, row.get_Value(i)));
            }

            return lstItens;
        }

        /// <summary>
        /// Retorna uma feature baseada em um campo específico
        /// </summary>
        /// <param name="id">Valor de ID</param>
        /// <param name="tableName">Nome da Feature Class</param>
        /// <param name="fieldName">Nome do campo a ser pesquisado</param>
        /// <param name="className">Nome da classe pesquisada</param>
        /// <returns>Lista de campos e seus valores</returns>
        public List<ListItem> GetFeature(string id, string className, string fieldName)
        {
            List<ListItem> lstItens = new List<ListItem>();
            IFeatureClass featClass = (ws as IFeatureWorkspace).OpenFeatureClass(className);
            IQueryFilter qf = new QueryFilterClass();

            qf.WhereClause = "TRIM(" + fieldName + ")=" + "'" + id + "'";


            IFeature feature = featClass.Search(qf, false).NextFeature();

            if (feature == null)
                return null;

            for (int i = 0; i < feature.Fields.FieldCount; i++)
            {
                lstItens.Add(new ListItem(feature.Fields.get_Field(i).Name, feature.get_Value(i)));
            }

            return lstItens;
        }

        public List<ListItem> GetARow(string tableName)
        {
            List<ListItem> lstItens = new List<ListItem>();
            ITable table = (ws as IFeatureWorkspace).OpenTable(tableName);

            IQueryFilter qf = new QueryFilterClass();
            ICursor cur = table.Search(qf, false);

            IRow row = cur.NextRow();

            if (row != null)
            {
                for (int i = 0; i < row.Fields.FieldCount; i++)
                {
                    lstItens.Add(new ListItem(row.Fields.get_Field(i).Name, row.get_Value(i)));

                }
            }

            return lstItens;
        }    

        public void DeleteRow(int objectId, string tableName)
        {
            IRow row = (ws as IFeatureWorkspace).OpenTable(tableName).GetRow(objectId);

            if (IsBeenEdit())
            {
                row.Delete();
            }
            else
            {
                throw new Exception("Não há sessão de edição");
            }
        }

        public bool IsBeenEdit()
        {
            bool r = false;

            if ((ws as IWorkspaceEdit).IsBeingEdited())
                r = true;

            return r;
        }

        public void PararSessaoDeEdicao(bool saveEdits)
        {
            (ws as IWorkspaceEdit).StopEditing(saveEdits);
        }

        public void IniciarSessaoDeEdicao(bool undoRedo)
        {
            (ws as IWorkspaceEdit).StartEditing(undoRedo);
        }

        public List<GisDataAdapter> GetListaDeRows(string tableName, string condicao, string orderingFields)
        {
            return GetRowsPorQuery(tableName, condicao, orderingFields);
        }

        public List<GisDataAdapter> GetRowsPorQuery(string tableName, string whereClause, string orderingFields)
        {
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)ws;
            ITable table = featureWorkspace.OpenTable(tableName);

            ITableSort tableSort = new TableSortClass();
            tableSort.Table = table;
            tableSort.Fields = orderingFields;
            tableSort.set_Ascending(orderingFields, true);

            ICursor cursor;
            IRow row;
            List<GisDataAdapter> adapters = new List<GisDataAdapter>();

            if (string.IsNullOrEmpty(whereClause))
            {
                tableSort.Sort(null);
                cursor = tableSort.Rows;
            }
            else
            {
                IQueryFilter queryFilter = new QueryFilterClass();
                queryFilter.WhereClause = whereClause;
                tableSort.QueryFilter = queryFilter;
                tableSort.Sort(null);
                cursor = tableSort.Rows;
            }

            while ((row = cursor.NextRow()) != null)
            {
                adapters.Add(new GisDataAdapter((IObject)row));
            }

            return adapters;
        }

        /// <summary>
        /// Retorna o objectid com valor no campo passado 
        /// </summary>
        /// <param name="className">Nome da classe (Table ou FeatureClass)</param>
        /// <param name="fieldName">Nome do campo</param>
        /// <param name="condicaoExtra">Caso haja uma condição extra posicionar nesta variável</param>
        /// <returns>Objectid o registro que atende a máxima condição em relação ao fieldName passado</returns>
        public static int GetLastObjectID(string className, string fieldName, string condicaoExtra)
        {
            if (string.IsNullOrEmpty(className) && string.IsNullOrEmpty(fieldName))
                throw new Exception("Parâmetros não preenchidos");

            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)ws;
            (featureWorkspace as IVersion).RefreshVersion();
            ITable table = featureWorkspace.OpenTable(className);

            if (table == null)
                throw new Exception("Classe não existe");

            //            ESRI.ArcGIS.Carto.ITableHistogram tableHistogram = new 
            BasicTableHistogram bh = new BasicTableHistogram();
            ITableHistogram th = bh as ITableHistogram;
            th.Table = table;
            th.Field = fieldName;

            IStatisticsResults statisticResult = th as IStatisticsResults;
            IQueryFilter qf = new QueryFilterClass();
            DateTime dateTime = DateTime.FromOADate(statisticResult.Maximum);

            string extraCondition = String.Empty;
            if (condicaoExtra != String.Empty)
                extraCondition = " AND " + condicaoExtra;
            qf.SubFields = "*";
            qf.WhereClause = fieldName + "=" + "timestamp '" + dateTime.ToString(TimestampFormat) + "'" + extraCondition;
            ICursor cursor = table.Search(qf, false);
            IRow row = cursor.NextRow();
            if (row != null)
            {
                return Convert.ToInt32(row.OID);
            }
            else
            {
                throw new Exception("Tabela sem registros");
            }
        }        

        public void DeleteFeatures(string nomeFeatureClasse, string condicao)
        {
            if (IsBeenEdit())
            {
                IQueryFilter qf = new QueryFilterClass();
                qf.WhereClause = condicao;
                IFeatureCursor cur = (ws as IFeatureWorkspace).OpenFeatureClass(nomeFeatureClasse).Search(qf, false);
                IFeature ftr = cur.NextFeature();
                while (ftr != null)
                {
                    ftr.Delete();
                    ftr = cur.NextFeature();
                }
            }
            else
            {
                throw new Exception("Não há sessão de edição");
            }
        }

        public static int GetRowCount(string tableName)
        {
            IQueryFilter qf = new QueryFilterClass();
            return (ws as IFeatureWorkspace).OpenTable(tableName).RowCount(qf);
        }

        public static void RefreshVersion(IFeatureLayer flayer, ESRI.ArcGIS.ArcMapUI.IMxDocument doc)
        {
            IWorkspace ws = (flayer.FeatureClass as IDataset).Workspace;
            IVersion2 version = ws as IVersion2;
            version.RefreshVersion();
            doc.UpdateContents();
        }

        public void CriarRelationShipPorObjecto(String tableNameOrign, int oidOrign, String tableNameDestination, int oidDestination, string relName, Geodatabase geodatabase)
        {
            ITable tableOring = GetTable(tableNameOrign);
            ITable tableDestin = GetTable(tableNameDestination);

            IRow rowOrign = tableOring.GetRow(oidOrign);
            IRow rowDestin = tableDestin.GetRow(oidDestination);

            IRelationshipClass relationship = (Geodatabase.ws as IFeatureWorkspace).OpenRelationshipClass(relName);

            relationship.CreateRelationship(rowOrign as IObject, rowDestin as IObject);
        }

        public List<GisDataAdapter> GetRows(string nometabela, string whereClause, string fields)
        {
            List<GisDataAdapter> adapters = new List<GisDataAdapter>();

            IQueryFilter qf = new QueryFilterClass();
            qf.WhereClause = whereClause;
            qf.SubFields = fields;
            ICursor cur = GetTable(nometabela).Search(qf, false);
            IRow row;

            while ((row = cur.NextRow()) != null)
            {
                adapters.Add(new GisDataAdapter((IObject)row));
            }

            return adapters;
        }
    }
}