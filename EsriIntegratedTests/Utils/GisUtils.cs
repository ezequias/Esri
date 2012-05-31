using System;
using System.Collections.Generic;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using System.Drawing;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.ConversionTools;
using System.Windows.Forms;
using ESRI.ArcGIS.Geoprocessing;
using System.Threading;
using ESRI.ArcGIS.DataSourcesFile;
using System.Collections;
using System.IO;

namespace Tests
{
    public class GisUtils
    {

        #region IGisUtils Members
        private static IApplication application = null;
        public static IMap mapa = null;
        public static IMapEvents_VersionChangedEventHandler pmapEvents;
        private static string timestampFormat = "yyyy-MM-dd HH:mm:ss";

        public static string TimestampFormat
        {
            get { return GisUtils.timestampFormat; }
        }


        public static IApplication Aplicacao
        {
            get { return GisUtils.application; }
            set { GisUtils.application = value; }
        }

        public static bool ValorValido(IField field, object valor)
        {
            return field.CheckValue(valor);
        }

        public static List<IRow> GetRows(ITable table, string colunaChave, object valor)
        {
            List<IRow> rows = new List<IRow>();

            IQueryFilter qf = new QueryFilterClass();
            qf.WhereClause = colunaChave + "=" + QuotedStr(valor);

            ICursor cursor = table.Search(qf, false);
            IRow row = cursor.NextRow();

            while (row != null)
            {
                rows.Add(row);

                row = cursor.NextRow();
            }

            return rows;
        }

        /// <summary>
        /// Retorna uma featureClass a partir do seu nome no geodatabase
        /// </summary>
        /// <param name="nomeLayer">Nome da classe</param>
        /// <param name="mapa">Mana onde se encontra a classe</param>
        /// <returns></returns>
        public static IFeatureClass GetFeatureClass(String nomeDaClasse, IMap mapa)
        {
            IFeatureClass fclassReturn = null;
            UID uid = new UID();
            uid.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}"; // FeatureLayers
            IEnumLayer enumLayers = mapa.get_Layers(uid, false);
            ILayer layer = enumLayers.Next();

            IFeatureClass fClass = null;
            while (layer != null)
            {
                fClass = (layer as IFeatureLayer).FeatureClass;
                string[] className = (fClass as IDataset).Name.Split('.');
                if (className.Length > 1)
                {
                    if (className[1] == nomeDaClasse)
                    {
                        return fClass;
                    }
                }
                else
                {
                    if ((fClass as IDataset).Name == nomeDaClasse)
                    {
                        return fClass;
                    }
                }
                layer = enumLayers.Next();
            }

            return fclassReturn;
        }

        /// <summary>
        /// Retorna uma enumeração de FeatureLayers a partir do seu nome no geodatabase
        /// </summary>
        /// <param name="activeView">View onde o mapa se encontra a classe</param>
        /// <returns></returns>
        public static IEnumLayer GetFeatureLayers(IActiveView activeView)
        {
            UID uid = new UID();
            uid.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}"; // FeatureLayers
            return mapa.get_Layers(uid, false);
        }

        /// <summary>
        /// Retorna o total de feature layers existentes no mapa
        /// </summary>
        /// <param name="mapa">Mapa onde se encontra a classe</param>
        /// <returns></returns>
        public static int GetNumFeatureLayers(IActiveView activeView)
        {
            int totalLayers = 0;
            UID uid = new UID();
            uid.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}"; // FeatureLayers
            IEnumLayer enLayer = activeView.FocusMap.get_Layers(uid, false);
            ILayer layer = enLayer.Next();
            while (layer != null)
            {
                if (layer.Visible)
                    totalLayers++;

                layer = enLayer.Next();
            }

            return totalLayers;
        }
        /// <summary>
        /// Retorna uma FeatureLayer a partir do seu nome no geodatabase
        /// </summary>
        /// <param name="nomeLayer">Nome da classe</param>
        /// <param name="mapa">Mapa onde se encontra a classe</param>
        /// <returns></returns>
        public static IFeatureLayer GetFeatureLayer(String nomeDaClasse, IActiveView activeView)
        {
            IFeatureLayer fclassReturn = null;
            UID uid = new UID();
            uid.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}"; // FeatureLayers
            IEnumLayer enumLayers = activeView.FocusMap.get_Layers(uid, false);
            ILayer layer = enumLayers.Next();

            IFeatureClass fClass = null;
            while (layer != null)
            {
                fClass = (layer as IFeatureLayer).FeatureClass;
                string nomeClasseCorrente = (fClass as IDataset).Name;
                if (nomeClasseCorrente.Split('.').Length > 1)
                {
                    if (nomeClasseCorrente.Split('.')[1].Trim() == nomeDaClasse.Trim())
                    {
                        return layer as IFeatureLayer;
                    }
                }
                else if (nomeClasseCorrente.Trim() == nomeDaClasse.Trim())
                {
                    return layer as IFeatureLayer;
                }
                layer = enumLayers.Next();
            }

            return fclassReturn;
        }



        private static object QuotedStr(object valor)
        {
            if (valor is String)
            {
                return "'" + valor + "'";
            }
            else
            {
                return valor;
            }
        }
        #endregion

        private static bool UltimoItemAdicionado(ITable tabela, int i)
        {
            return (tabela.Fields.FieldCount == i);

        }

        private static string ExtractUserFromFeatureLayer(ILayer layer)
        {
            var r = String.Empty;
            if (layer != null && layer is IFeatureLayer)
            {
                var fClass = (layer as IFeatureLayer).FeatureClass;
                var dataset = (IDataset)fClass;
                r = dataset.Workspace.ConnectionProperties.GetProperty("USER").ToString();
            }

            return r;
        }
        /// <summary>
        /// Seleciona uma feature no Mapa
        /// </summary>
        /// <param name="feature">Feature que deve ser selecionada</param>        
        ///<param name="activeView">Visão de mapa em uso</param>
        public static void SelecionarFeature(IFeature feature, IActiveView activeView)
        {
            LimparSelecoes(activeView);

            esriFeatureType featType = ((IFeatureClass)feature.Table).FeatureType;
            if (featType == esriFeatureType.esriFTSimpleJunction | featType == esriFeatureType.esriFTSimpleEdge | featType == esriFeatureType.esriFTSimple | featType == esriFeatureType.esriFTComplexEdge)
            {
                IDataset ds = feature.Table as IDataset;
                string[] className = ds.Name.Split('.');

                IFeatureLayer flayer = GetFeatureLayer(className[1], activeView);

                if (flayer != null)
                {
                    if (className.Length > 1)
                    {
                        activeView.FocusMap.SelectFeature(flayer, feature);
                    }
                    else
                    {
                        activeView.FocusMap.SelectFeature(flayer, feature);
                    }
                    activeView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);

                    if (feature.Shape.GeometryType == esriGeometryType.esriGeometryPoint)
                    {
                        IEnvelope env = activeView.Extent.Envelope;
                        env.CenterAt(feature.Shape as IPoint);
                        activeView.Extent = env;
                    }
                }

                activeView.Refresh();
            }
        }
        /// <summary>
        /// Limpa seleções ativas no mapa em um dado momento
        /// </summary>
        public static void LimparSelecoes(IActiveView activeView)
        {
            activeView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
            activeView.FocusMap.ClearSelection();
            activeView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
        }
       
        /// <summary>
        /// Transforma um Color em um IColor
        /// </summary>
        /// <param name="cor">Cor solicitada</param>
        /// <returns>Cor na interface IColor</returns>
        public static IColor ColorToIColor(Color cor)
        {
            RgbColorClass icolor = new RgbColorClass();
            icolor.Red = cor.R;
            icolor.Green = cor.G;
            icolor.Blue = cor.B;

            return (IColor)icolor;
        }

        /// <summary>
        /// Limpa todos os gráfios do Graphic container
        /// </summary>
        /// <param name="activeView"></param>
        public static void LimparGraficosDoContainer(IActiveView activeView)
        {
            activeView.GraphicsContainer.DeleteAllElements();
        }

        public static IEnvelope GetGridBagEnvelope(List<GisDataAdapter> lstRows)
        {
            IEnvelope envelopeTotal = new EnvelopeClass();
            IGeometryCollection geomCollection = new GeometryBagClass();

            object before = Type.Missing;
            object after = Type.Missing;
            for (int i = 0; i < lstRows.Count; i++)
            {
                IGeometry geometry = lstRows[i].GetFieldValue((lstRows[i].DataObjectClass as IFeatureClass).ShapeFieldName) as IGeometry;
                geomCollection.AddGeometry(geometry, ref before, ref after);
            }
            envelopeTotal = (geomCollection as GeometryBag).Envelope;
            return envelopeTotal;
        }

        /// <summary>
        /// Cria um shapefile a partir de um featureCursor
        /// </summary>
        /// <param name="feature">Feature desejada</param>
        public static void CriarShapefile(IFeatureLayer fLayer,
                                        IFeature feature,
                                        string pasta,
                                        IActiveView activeView,
                                        bool adicionarSaidaNoMapa,
                                        IFeatureWorkspace pFeatureWorkspaceShp)
        {
            if (feature != null)
            {
                #region Deletando shapefiles que possam existir já no diretório
                DirectoryInfo dirInf = new DirectoryInfo(pasta);
                FileInfo[] fi = dirInf.GetFiles();
                for (int i = 0; i < fi.Length; i++)
                {
                    if (fi[i].Name.Contains(fLayer.FeatureClass.AliasName) && !fi[i].Name.Contains(".lock"))
                    {
                        fi[i].Delete();
                    }
                }
                #endregion

                IWorkspace pScratchWorkspace;
                IScratchWorkspaceFactory pScratchWorkspaceFactory;
                pScratchWorkspaceFactory = new ScratchWorkspaceFactoryClass();
                pScratchWorkspace = pScratchWorkspaceFactory.DefaultScratchWorkspace;
                IFeatureSelection pNewSelSet = fLayer as IFeatureSelection;
                IEnvelope pEnv = activeView.Extent.Envelope;
                IGeometry pEnvGeo = pEnv as IEnvelope;
                ISpatialFilter pSF = new SpatialFilterClass();
                pSF.Geometry = pEnvGeo;
                pSF.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

                IQueryFilter qf = new QueryFilterClass();

                qf.WhereClause = (fLayer as IFeatureLayerDefinition).DefinitionExpression;
                pNewSelSet.SelectFeatures(qf, esriSelectionResultEnum.esriSelectionResultNew, false);

                Geoprocessor gp = new Geoprocessor();
                gp.AddOutputsToMap = adicionarSaidaNoMapa;

                FeatureClassToShapefile fcToShapeFile = new FeatureClassToShapefile();

                fcToShapeFile.Input_Features = fLayer;
                fcToShapeFile.Output_Folder = pasta;

                IGeoProcessorResult result = gp.Execute(fcToShapeFile, null) as IGeoProcessorResult;
                string nomeArquivoSaida = gp.GetMessage(3);

                IFeatureClass pFeatureClassShp = pFeatureWorkspaceShp.OpenFeatureClass(fLayer.FeatureClass.AliasName);

                GisUtils.TratarSubtiposDominiosShapefile(fLayer, pasta, pFeatureWorkspaceShp, pFeatureClassShp);

                pFeatureClassShp = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public static string GetChaveDataHora()
        {
            string data = DateTime.Now.ToShortDateString().Replace("/", String.Empty);
            string hora = DateTime.Now.ToLongTimeString().Replace(":", String.Empty);
            return (data + hora).Replace(" ", String.Empty);
        }

        public static IFeatureWorkspace GetFeatureWoskspacePasta(string pasta)
        {
            IFeatureWorkspace pFeatureWorkspace;

            IPropertySet pPropset = new PropertySetClass();
            IWorkspace pWorkspace = null;

            pPropset.SetProperty("DATABASE", pasta);

            IWorkspaceFactory pFact = new ShapefileWorkspaceFactory();
            pWorkspace = pFact.Open(pPropset, 0);

            pFeatureWorkspace = pWorkspace as IFeatureWorkspace;

            return pFeatureWorkspace;
        }

        private static IFeatureLayer GetFeatureLayerByAlias(string p, IActiveView activeView)
        {
            throw new NotImplementedException();
        }

        private static void TratarSubtiposDominiosShapefile(IFeatureLayer fLayer, string pasta, IFeatureWorkspace pFeatureWorkspace, IFeatureClass pFeatureClassShp)
        {            
            List<String> camposAdicionados = new List<string>();            
            IWorkspace ws = (fLayer.FeatureClass as IDataset).Workspace;

            IQueryFilter qf = new QueryFilterClass();
            IFeatureCursor fcur = null;
            IFeature ftr = null;

            #region
            for (int i = 0; i < fLayer.FeatureClass.Fields.FieldCount; i++)
            {
                IDomain domain = fLayer.FeatureClass.Fields.get_Field(i).Domain;
                int totLinhas = (fLayer.FeatureClass as ITable).RowCount(qf);

                if (domain != null)
                {
                    AdicionarColuna(fLayer.FeatureClass.Fields.get_Field(i), fLayer, pFeatureClassShp, camposAdicionados);
                    #region Setando valores
                    fcur = pFeatureClassShp.Update(qf, true);

                    ftr = fcur.NextFeature();
                    while (ftr != null)
                    {
                        int idxNovoCampo = pFeatureClassShp.Fields.FieldCount - 1;

                        ftr.set_Value(idxNovoCampo, GetDescricaoDominio(domain.Name, ftr.get_Value(i + 1), ws));
                        fcur.UpdateFeature(ftr);
                        ftr = fcur.NextFeature();
                    }
                    #endregion
                }
                else
                {
                    #region Tratamento para subtipos
                    if (CampoEhSubtipo(fLayer.FeatureClass.Fields.get_Field(i).Name, fLayer.FeatureClass))
                    {
                        AdicionarColuna(fLayer.FeatureClass.Fields.get_Field(i), fLayer, pFeatureClassShp, camposAdicionados);

                        ISubtypes subtipos = fLayer.FeatureClass as ISubtypes;
                        IEnumSubtype enumSub = subtipos.Subtypes;
                        int subtypeCode;
                        String descSubtipo = enumSub.Next(out subtypeCode);

                        fcur = pFeatureClassShp.Update(qf, true);
                        ftr = fcur.NextFeature();
                        while (ftr != null)
                        {
                            int idxNovoCampo = pFeatureClassShp.Fields.FieldCount - 1;

                            ftr.set_Value(idxNovoCampo, GetDescricaoSubtipo(subtipos, ftr.get_Value(i + 1)));
                            fcur.UpdateFeature(ftr);
                            ftr = fcur.NextFeature();
                        }
                    }
                    #endregion
                }
            }

            pFeatureClassShp = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            #endregion
        }

        public static ICursor GetSelecoesNoMapa(IActiveView activeView, String layerName)
        {
            IFeatureLayer flayer = GisUtils.GetFeatureLayer(layerName, activeView);
            IFeatureSelection featureSelection = flayer as IFeatureSelection;
            ISelectionSet selectionSet = featureSelection.SelectionSet;

            ICursor cursor;
            selectionSet.Search(null, false, out cursor);

            return cursor;
        }

        private static string GetDescricaoSubtipo(ISubtypes subtipos, object valor)
        {
            String r = String.Empty;

            int result;
            if (Int32.TryParse(valor.ToString(), out result) == false)
                return r;

            IEnumSubtype enumSubtp = subtipos.Subtypes;
            int SubtypeCode;
            string descSubtipo = enumSubtp.Next(out SubtypeCode);
            while (descSubtipo != null)
            {
                if (SubtypeCode == result)
                {
                    r = descSubtipo;
                }

                descSubtipo = enumSubtp.Next(out SubtypeCode);
            }

            return r;
        }

        private static void AdicionarColuna(IField field, IFeatureLayer fLayer, IFeatureClass pFeatureClassShp, List<String> camposAdicionados)
        {
            IFieldEdit fldDomain = new FieldClass();
            string aliasName = field.AliasName;
            string fieldName = field.Name;
            fldDomain.AliasName_2 = GetNomeNovo(aliasName, pFeatureClassShp, camposAdicionados);
            fldDomain.Name_2 = GetNomeNovo(fieldName, pFeatureClassShp, camposAdicionados);
            fldDomain.Editable_2 = true;
            fldDomain.Length_2 = 254;

            fldDomain.Type_2 = esriFieldType.esriFieldTypeString;

            pFeatureClassShp.AddField(fldDomain);
            camposAdicionados.Add(fldDomain.Name);
        }

        private static bool CampoEhSubtipo(String fieldName, IFeatureClass classe)
        {
            bool r = false;
            ISubtypes subtipo = classe as ISubtypes;
            if (subtipo != null && subtipo.HasSubtype)
            {
                if (fieldName.ToUpper() == subtipo.SubtypeFieldName.ToUpper())
                    r = true;
            }

            return r;
        }

        /// <summary>
        /// Retorna todos os elementos do domínio na ordem em que aparecem na sua definição
        /// </summary>
        /// <param name="nomeDominio">Nome do domínio que se deseja ler os dados</param>
        /// <returns>Lista com os elementos do tipo Cod,Descrição para utilização no sistema em geral</returns>
        public static ArrayList GetDominios(String nomeDominio, IWorkspace ws)
        {
            IWorkspaceDomains workSpaceDomains = (IWorkspaceDomains)ws;
            System.Collections.IList valores = new System.Collections.ArrayList();

            try
            {
                if (workSpaceDomains.get_DomainByName(nomeDominio) is ICodedValueDomain)
                {
                    ICodedValueDomain codedDomain = (ICodedValueDomain)workSpaceDomains.get_DomainByName(nomeDominio);
                    for (int i = 0; i < codedDomain.CodeCount; i++)
                    {
                        valores.Add(new ListItem(codedDomain.get_Value(i).ToString(), codedDomain.get_Name(i).ToString()));
                    }
                }
                else // Domínio de Range
                {
                    IRangeDomain rangeDomain = (IRangeDomain)workSpaceDomains.get_DomainByName(nomeDominio);
                    for (int i = Convert.ToInt32(rangeDomain.MinValue); i < Convert.ToInt32(rangeDomain.MaxValue); i++)
                    {
                        valores.Add(new ListItem(i.ToString(), i.ToString()));
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("Erro ao tentar capturar domínio " + nomeDominio);
            }

            //ordendando a lista
            System.Collections.ArrayList listaOrdenada = new System.Collections.ArrayList((System.Collections.ICollection)valores);
            System.Collections.IComparer ordernador = new OrdenaListItem();
            listaOrdenada.Sort(ordernador);
            return listaOrdenada;
        }

        public static object GetCodigoDominio(String nomeDominio, object descricao)
        {
            if (descricao == null || descricao.ToString() == String.Empty)
            {
                return null;
            }

            ArrayList dominio = GetDominios(nomeDominio, Geodatabase.ws);
            foreach (ListItem item in dominio)
            {
                if (item.Valor.ToString().ToUpper().Equals(descricao.ToString().ToUpper()))
                    return Convert.ToInt32(item.Chave);
            }
            throw new Exception("Valor (" + descricao + ")" + " para domínio (" + nomeDominio + ")" + " é inválido.");
        }

        public static String GetDescricaoDominio(String nomeDominio, object id, IWorkspace ws)
        {
            String nomeDom = nomeDominio;
            object idDom = id;

            try
            {
                IWorkspaceDomains workSpaceDomains = (IWorkspaceDomains)ws;
                ICodedValueDomain codedDomain = workSpaceDomains.get_DomainByName(nomeDominio) as ICodedValueDomain;
                IList valores = new ArrayList();
                if (codedDomain != null)
                {
                    for (int i = 0; i < codedDomain.CodeCount; i++)
                    {
                        if (codedDomain.get_Value(i).ToString() == id.ToString())
                            return codedDomain.get_Name(i).ToString();
                    }
                }
                else
                {
                    return id.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Não foi possível capturar descrição do domínio" + ":" + ex.Message);
            }

            return null;
        }

        private static string GetNomeNovo(string nome, IFeatureClass fclass, List<String> camposAdicionados)
        {
            string nomeNovo = nome;

            if (nome.Length > 9)
            {
                nomeNovo = nome.Substring(0, 9) + "1";
            }
            else
            {
                nomeNovo = nome.Substring(0, nome.Length) + "1";
            }


            while (CampoJaExisteOuFoiAdicionado(fclass, nomeNovo, camposAdicionados))
            {
                int numeralNome = Convert.ToInt32(nomeNovo.Substring(nomeNovo.Length - 1, 1));
                nomeNovo = nome.Substring(0, nomeNovo.Length - 1) + (numeralNome + 1);
            }

            return nomeNovo;
        }

        private static bool CampoJaExisteOuFoiAdicionado(IFeatureClass fclass, string nomeNovo, List<string> camposAdicionados)
        {
            bool r = false;

            if (fclass.FindField(nomeNovo) >= 0)
            {
                r = true;
            }
            else
            {
                foreach (var item in camposAdicionados)
                {
                    if (item == nomeNovo)
                    {
                        r = true;
                    }
                }
            }


            return r;
        }

        public static void FlashGeometria(IGeometry pGeo, IActiveView activeView)
        {
            ILineSymbol pSimpleLineSymbol = default(ILineSymbol);
            ISimpleFillSymbol pSimpleFillSymbol = default(ISimpleFillSymbol);
            ISimpleMarkerSymbol pSimpleMarkersymbol = default(ISimpleMarkerSymbol);
            IActiveView pActive = default(IActiveView);
            ISymbol pSymbol = default(ISymbol);
            IScreenDisplay pDisplay = default(IScreenDisplay);
            IRgbColor pColor = default(IRgbColor);

            int sleepValue = 500;
            pColor = new ESRI.ArcGIS.Display.RgbColorClass();
            pColor.Red = 255;
            pActive = activeView;
            pDisplay = activeView.ScreenDisplay;
            pDisplay.StartDrawing(0, (short)esriScreenCache.esriNoScreenCache);
            switch (pGeo.GeometryType)
            {
                case esriGeometryType.esriGeometryPolyline:
                    pSimpleLineSymbol = new SimpleLineSymbol();
                    pSymbol = pSimpleLineSymbol as ISymbol;
                    pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;
                    pSimpleLineSymbol.Width = 4;
                    pSimpleLineSymbol.Color = pColor;
                    pDisplay.SetSymbol(pSimpleLineSymbol as ISymbol);
                    pDisplay.DrawPolyline(pGeo);
                    Thread.Sleep(sleepValue);
                    break;
                case esriGeometryType.esriGeometryPolygon:
                    pSimpleFillSymbol = new SimpleFillSymbol();
                    pSymbol = pSimpleFillSymbol as ISymbol;
                    pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;
                    pSimpleFillSymbol.Color = pColor;
                    pDisplay.SetSymbol(pSimpleFillSymbol as ISymbol);
                    pDisplay.DrawPolygon(pGeo);
                    Thread.Sleep(sleepValue);
                    break;
                case esriGeometryType.esriGeometryPoint:
                    pSimpleMarkersymbol = new SimpleMarkerSymbol();
                    pSymbol = pSimpleMarkersymbol as ISymbol;
                    pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;
                    pSimpleMarkersymbol.Color = pColor;
                    pSimpleMarkersymbol.Size = 12;
                    pDisplay.SetSymbol(pSimpleMarkersymbol as ISymbol);
                    pDisplay.DrawPoint(pGeo);
                    Thread.Sleep(sleepValue);
                    break;
                case esriGeometryType.esriGeometryMultipoint:
                    pSimpleMarkersymbol = new SimpleMarkerSymbol();
                    pSymbol = pSimpleMarkersymbol as ISymbol;
                    pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;
                    pSimpleMarkersymbol.Color = pColor;
                    pSimpleMarkersymbol.Size = 12;
                    pDisplay.SetSymbol(pSimpleMarkersymbol as ISymbol);
                    pDisplay.DrawMultipoint(pGeo);
                    pDisplay.DrawMultipoint(pGeo);
                    Thread.Sleep(sleepValue);
                    break;
                default:
                    break;
            }
            pDisplay.Invalidate(null, true, 0);

            pDisplay.FinishDrawing();
        }
        /// <summary>
        /// Realiza uma busca espacial retornando um cursor apenas
        /// no layer específico. A relação é "Está contido"
        /// </summary>
        /// <param name="polyBuffer">Geometria do buffer que circunda a pesquisa</param>
        /// <param name="flayer">FeatureLayer que se deseja selecionar</param>
        /// <returns>Cursor com o resultado da busca espacial</returns>
        public static IFeatureCursor BuscaEspacial(IGeometry polyBuffer, IFeatureLayer flayer)
        {
            ISpatialFilter sf = new SpatialFilter();
            sf.Geometry = polyBuffer;
            sf.GeometryField = "Shape";
            sf.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
            IFeatureCursor fc = null;
            try
            {
                fc = flayer.FeatureClass.Search(sf, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return fc;
        }

        /// <summary>
        /// Realiza uma busca espacial retornando um cursor apenas
        /// no layer específico. A relação é "Está contido"
        /// </summary>
        /// <param name="polyBuffer">Geometria do buffer que circunda a pesquisa</param>
        /// <param name="flayer">Feature Class que se deseja selecionar</param>
        /// <param name="spatialEnum">Relação espacial de consulta</param>
        /// <param name="whereClause">Condition beyond the spatial filter</param>
        /// <returns>Cursor com o resultado da busca espacial</returns>
        public static IFeatureCursor BuscaEspacial(IGeometry polyBuffer,
                                                IFeatureLayer flayer,
                                                esriSpatialRelEnum spatialEnum)
        {
            ISpatialFilter sf = new SpatialFilter();
            sf.Geometry = polyBuffer;
            sf.GeometryField = "Shape";
            sf.SpatialRel = spatialEnum;
            IFeatureCursor fc = null;
            //sf.WhereClause = (flayer as IFeatureLayerDefinition).DefinitionExpression;
            try
            {
                fc = flayer.FeatureClass.Search(sf, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return fc;
        }

        /// <summary>
        /// Realiza uma busca espacial retornando um cursor apenas
        /// no layer específico. A relação é "Está contido"
        /// </summary>
        /// <param name="polyBuffer">Geometria do buffer que circunda a pesquisa</param>
        /// <param name="flayer">Feature Class que se deseja selecionar</param>
        /// <param name="spatialEnum">Relação espacial de consulta</param>
        /// <param name="whereClause">Condition beyond the spatial filter</param>
        /// <returns>Cursor com o resultado da busca espacial</returns>
        public static IFeatureCursor BuscaEspacial(IGeometry polyBuffer,
                                                IFeatureClass fClass,
                                                esriSpatialRelEnum spatialEnum)
        {
            ISpatialFilter sf = new SpatialFilter();
            sf.Geometry = polyBuffer;
            sf.GeometryField = "Shape";
            sf.SpatialRel = spatialEnum;
            IFeatureCursor fc = null;
            try
            {
                fc = fClass.Search(sf, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return fc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ObjectIdPonto"></param>
        /// <param name="nomeClasseObjetoInterceptado">Ponto de referência</param>
        /// <param name="nomeDaFeicaoPoligonal">Nome da feição que conterá o ponto passado</param>
        /// <returns>Retorna objectid da feição poligonal que contém o ponto passado</returns>
        /// <param name="ws">Workspace desejado</param>
        public static int BuscaEspacialPontoEmPoligono(int ObjectIdPonto, string nomeClasseObjetoInterceptado, string nomeDaFeicaoPoligonal)
        {
            int r = -1;

            IFeatureClass fclass = GisUtils.GetFeatureClass(nomeClasseObjetoInterceptado, Geodatabase.ws);
            IFeatureClass fclassPoligono = GisUtils.GetFeatureClass(nomeDaFeicaoPoligonal, Geodatabase.ws);

            if (fclass != null)
            {
                IFeature ftrPonto = fclass.GetFeature(ObjectIdPonto);

                if (ftrPonto != null)
                {
                    IFeatureCursor fcur = GisUtils.BuscaEspacial(ftrPonto.Shape, fclassPoligono, esriSpatialRelEnum.esriSpatialRelIntersects);
                    IFeature ftr = fcur.NextFeature();

                    if (ftr != null)
                    {
                        r = ftr.OID;
                    }
                }
            }
            return r;
        }

        public static ESRI.ArcGIS.Geometry.IPoint GetPontoPorXY(double x, double y, ISpatialReference referenciaEspacialOriginal, IDataset dataset)
        {
            IPoint pontoInformado = new PointClass();

            ISpatialReference spatialReferenceDataset = (dataset as IGeoDataset).SpatialReference;

            pontoInformado.X = x;
            pontoInformado.Y = y;
            pontoInformado.SpatialReference = referenciaEspacialOriginal;

            pontoInformado.Project(spatialReferenceDataset);
            return pontoInformado;
        }              

        public static void LayerDefinitionUpdate(ref IFeatureLayerDefinition layerDefinition, ref IFeatureLayer flayerHistorico, string condicao)
        {
            layerDefinition = (IFeatureLayerDefinition)flayerHistorico;
            layerDefinition.DefinitionExpression = condicao;
        }

        public static string GetUsuarioCorrente(IActiveView activeView)
        {
            string userName = String.Empty;

            IEnumLayer enumLayer = GetFeatureLayers(activeView);
            ILayer layer = enumLayer.Next();
            if (layer != null)
            {
                userName = ExtractUserFromFeatureLayer(layer);
            }

            return userName;
        }

        public static IWorkspace GetCurrentWorkspace(IActiveView activeView)
        {
            IWorkspace ws = null;

            UID uid = new UID();
            uid.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}"; // FeatureLayers
            IEnumLayer enumLayers = activeView.FocusMap.get_Layers(uid, false);
            ILayer layer = enumLayers.Next();
            IDataset dataset = null;
            if (layer != null)
            {
                IFeatureLayer flayer = layer as IFeatureLayer;
                dataset = flayer as IDataset;
                if (dataset != null)
                    ws = dataset.Workspace;
            }
            else
            {
                throw new Exception("Não há layers");
            }

            return ws;
        }

        public static void SetChangeVersionEvent(IMap map)
        {
            mapa = map;
            IMapEvents_Event mapEvents = mapa as IMapEvents_Event;
            pmapEvents = new IMapEvents_VersionChangedEventHandler(OnVersionChangedFunction);

            mapEvents.VersionChanged -= pmapEvents;
            mapEvents.VersionChanged += pmapEvents;
        }

        public static void OnVersionChangedFunction(IVersion oldVersion, IVersion newVersion)
        {
            UID uid = new UID();
            uid.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}"; // FeatureLayers
            IEnumLayer enumLayers = mapa.get_Layers(uid, false);
            ILayer layer = enumLayers.Next();
            if (layer != null)
            {
                Geodatabase.ws = ((layer as IFeatureLayer).FeatureClass as IDataset).Workspace;
            }
        }

        public static void TornarTodosNaoSelecionaveis(IActiveView activeView)
        {
            IEnumLayer enumFeatLayers = GisUtils.GetFeatureLayers(activeView);
            IFeatureLayer flayer = enumFeatLayers.Next() as IFeatureLayer;

            while (flayer != null)
            {
                (flayer as IFeatureLayer).Selectable = false;
                flayer = enumFeatLayers.Next() as IFeatureLayer; ;
            }
        }

        internal static IFeatureClass GetFeatureClass(string nomeClasseObjetoInterceptado, IWorkspace ws)
        {
            IFeatureClass fclass = null;

            if (ws != null && ws is IFeatureWorkspace)
            {
                fclass = (ws as IFeatureWorkspace).OpenFeatureClass(nomeClasseObjetoInterceptado);
            }

            return fclass;
        }

        /// <summary>
        /// Retorna o OID da classe do tipo Linha informado no parâmetro nomeClasseTipoLinha
        /// </summary>
        /// <param name="oidDaClassePontual">OID da feição da classe que contém o ponto de referência</param>
        /// <param name="nomeClassePontual">Nome da Feature Class de ponto</param>
        /// <param name="nomeClasseTipoLinha"></param>
        /// <param name="geodatabase"></param>
        /// <returns></returns>
        public static int GetLinhaMaisProximaDoPonto(int oidDaClassePontual, string nomeClassePontual, string nomeClasseTipoLinha, Geodatabase geodatabase)
        {
            int r = -1;

            IFeatureClass classePonto = geodatabase.GetFeatureClass(nomeClassePontual);
            IFeatureClass classeLinha = geodatabase.GetFeatureClass(nomeClasseTipoLinha);

            IFeature ftrPonto = classePonto.GetFeature(oidDaClassePontual);

            if (ftrPonto != null)
            {
                IFeature ftrLinha = GetLinhaMaisProxima(ftrPonto.Shape as IPoint, classeLinha);
                r = ftrLinha.OID;
            }

            return r;
        }

        private static IFeature GetLinhaMaisProxima(IPoint ponto, IFeatureClass classeLinha)
        {
            IFeature ftrMaisProx = null;

            IFeatureCursor fcursor = BuscaEspacial(ponto, classeLinha, esriSpatialRelEnum.esriSpatialRelIntersects);
            IFeature ftrLinha = fcursor.NextFeature();
            double taxaBusca = 10; //10m
            int tentativas = 0;

            if (ftrLinha != null)
            {
                ftrMaisProx = LinhaMaisProxima(ponto, ref fcursor, ref ftrLinha, ref ftrMaisProx);
            }
            else
            {
                while (ftrLinha == null)
                {
                    IEnvelope env = new EnvelopeClass();
                    env = ponto.Envelope;
                    tentativas++;
                    env.Expand(taxaBusca * tentativas, taxaBusca * tentativas, false);

                    fcursor = GisUtils.BuscaEspacial(env, classeLinha, esriSpatialRelEnum.esriSpatialRelIntersects);

                    ftrLinha = fcursor.NextFeature();
                }

                ftrMaisProx = LinhaMaisProxima(ponto, ref fcursor, ref ftrLinha, ref ftrMaisProx);
            }
            return ftrMaisProx;
        }

        private static IFeature LinhaMaisProxima(IPoint ponto, ref IFeatureCursor fcursor, ref IFeature ftrLinha, ref IFeature ftrMaisProx)
        {
            IProximityOperator proxOper = ponto as IProximityOperator;
            Double distancia = double.MaxValue;

            while (ftrLinha != null)
            {
                //verifica se está mais próximo
                if (proxOper.ReturnDistance(ftrLinha.Shape) < distancia)
                {
                    ftrMaisProx = ftrLinha;
                    distancia = proxOper.ReturnDistance(ftrLinha.Shape);
                }

                ftrLinha = fcursor.NextFeature();
            }

            return ftrMaisProx;
        }

        public static int GetIndexCombo(ComboBox cmbBox, string valor)
        {
            int r = 0;
            if (cmbBox.DataSource is List<ListItem>)
            {
                List<ListItem> lista = cmbBox.DataSource as List<ListItem>;

                for (int i = 0; i < lista.Count; i++)
                {
                    if ((lista[i] as ListItem).Valor.Equals(valor))
                    {
                        r = i;
                    }
                }
            }

            return r;
        }

        public static int GetTotalLayersAtivos(IMap iMap)
        {
            UID uid = new UID();
            uid.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}"; // FeatureLayers
            IEnumLayer enumLayers = mapa.get_Layers(uid, false);
            ILayer lay = enumLayers.Next();
            int tot = 0;

            while (lay != null)
            {
                if (lay.Visible)
                {
                    tot++;
                }

                lay = enumLayers.Next();
            }

            return tot;

        }

        public static string GetPastaTemp(string pasta, string dataHora)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(pasta + "\\Temp" + dataHora);
            dirInfo.Create();

            return pasta + "\\Temp" + dataHora;
        }
    }
}
