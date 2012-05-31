using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ESRI.ArcGIS;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using System.IO;

namespace ClassGeneratorSDE
{ 
    public partial class FrmClassGenerator : Form
    {

        IPropertySet _pPropSet;
        public IWorkspace Ws;
        IWorkspaceFactory _pSdeFact;
        IEnumDatasetName _enumDsFeatureClasses;
        IEnumDatasetName _enumDsTables;
        public const string ident = "    ";

        
        public FrmClassGenerator()
        {
            InitializeComponent();

            try
            {
                InitializeLicenseEditor();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error to check license", "Class Generator", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }

        }

        public void InitializeLicenseEditor()
        {
            RuntimeManager.BindLicense(ProductCode.Desktop, LicenseLevel.Standard);
            RuntimeManager.Bind(ProductCode.Desktop);
        }

        private void BtConnectarClick(object sender, EventArgs e)
        {
            //NEOSDE_PT_PONTO_GEOGRAFICO pg = new NEOSDE_PT_PONTO_GEOGRAFICO();
            


            Ws = null;
            _pPropSet = new PropertySetClass();
            _pSdeFact = new SdeWorkspaceFactory();
            _pPropSet.SetProperty("SERVER", txtBoxServer.Text);
            _pPropSet.SetProperty("INSTANCE", txtBoxInstancia.Text);
            _pPropSet.SetProperty("DATABASE", txtBoxDB.Text);
            _pPropSet.SetProperty("USER", txtBoxUsuario.Text);
            _pPropSet.SetProperty("PASSWORD", txtBoxPassword.Text);
            _pPropSet.SetProperty("VERSION", txtBoxVersao.Text);

            try
            {
                System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
                Ws = _pSdeFact.Open(_pPropSet, 0);

                treeViewDatasets.Nodes.Clear();
                treeViewTables.Nodes.Clear();

                if (chkBoxFeatueClass.Checked)
                    _enumDsFeatureClasses = Ws.get_DatasetNames(esriDatasetType.esriDTFeatureDataset);

                if (chkBoxTables.Checked)
                    _enumDsTables = Ws.get_DatasetNames(esriDatasetType.esriDTTable);

                if (_enumDsFeatureClasses != null)
                {
                    IDatasetName dsName = _enumDsFeatureClasses.Next();
                    while (dsName != null)
                    {
                        treeViewDatasets.Nodes.Add(dsName.Name);
                        dsName = _enumDsFeatureClasses.Next();
                    }
                }

                if (_enumDsTables != null)
                {
                    IDatasetName dsName = _enumDsTables.Next();
                    while (dsName != null)
                    {
                        treeViewTables.Nodes.Add(dsName.Name);
                        dsName = _enumDsTables.Next();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Impossible to Connect" + "\n\n" + ex.Message);
            }
            finally
            {
                System.Windows.Forms.Cursor.Current = Cursors.Default;
            }
        }

        private void BtGerarClick(object sender, EventArgs e)
        {
            try
            {
                #region Verifica se arquivo existe e se não existe pergunta se quer criar
                System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
                var dirInfo = new DirectoryInfo(txtBoxCaminho.Text);
                if (dirInfo.GetFiles().Length == 0)
                {
                    GerarArquivos();
                }
                else
                {
                    var fileInfos = dirInfo.GetFiles();
                    if (fileInfos.Length > 0)
                        for (int i = 0; i < fileInfos.Length; i++)
                        {
                            fileInfos[i].Delete();
                        }


                    GerarArquivos();
                }
                #endregion
            }
            finally
            {
                System.Windows.Forms.Cursor.Current = Cursors.Default;
                MessageBox.Show("Generation Done", "Generator", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void GerarArquivos()
        {
            GerarDominios();
            // Gerando arquivos do tipo FeatureClass
            lstBoxArquivos.Items.Clear();
            for (int i = 0; i < treeViewDatasets.Nodes.Count; i++)
            {
                if (treeViewDatasets.Nodes[i].Checked)
                {
                    GerarArquivoFeatureClass(treeViewDatasets.Nodes[i].Text);
                }
            }


            // Gerando arquivos do tipo Table
            for (var i = 0; i < treeViewTables.Nodes.Count; i++)
            {
                if (treeViewTables.Nodes[i].Checked)
                {
                    GerarArquivoTables(treeViewTables.Nodes[i].Text);
                }
            }

        }

        private void GerarDominios()
        {
            IWorkspaceDomains wsDomains = (IWorkspaceDomains) Ws;
            IEnumDomain enumDomain = wsDomains.Domains;
            enumDomain.Reset();
            IDomain domain = enumDomain.Next();

            if (domain != null)
            {

                while (domain != null)
                {
                    GerarArquivoDominio(domain);
                    domain = enumDomain.Next();
                }
            }
        }

        private void GerarArquivoDominio(IDomain domain)
        {
            var className = "";

            if (domain != null)
                className = domain.Name;


            const string ident = "    ";

            #region CriarSubdir
            DirectoryInfo directoryInfo = new DirectoryInfo(txtBoxCaminho.Text);
            directoryInfo.CreateSubdirectory("domains");
            #endregion

            TextWriter tw = new StreamWriter(txtBoxCaminho.Text + "/" + "domains" + "/" + className + ".cs");

            #region Using
            tw.WriteLine("using System;");
            tw.WriteLine("using System.Collections.Generic;");
            tw.WriteLine("using System.Text;");
            #endregion

            #region Cabeçalho da Classe (nome da classe)
            tw.WriteLine("namespace ClassGeneratorSDE");
            tw.WriteLine("{");
            tw.WriteLine(ident + "class " + className);
            tw.WriteLine(ident + "{");
            #endregion

            #region Atributos/Getters/Setters
            if (domain != null && domain is CodedValueDomain)
            {
                ArrayList arrayListDomains = GetDominios(domain.Name, Ws);

                foreach (var t in arrayListDomains)
                {
                    var itemDominio = (t as ListItem);
                    if (itemDominio != null)
                    {
                        string varName = ChangeCharacters(itemDominio.Valor.ToString());
                        if (varName != itemDominio.Valor)
                        {
                            tw.WriteLine(ident + ident + "/// <summary>");
                            tw.WriteLine(ident + ident + "/// " + itemDominio.Valor);
                            tw.WriteLine(ident + ident + "/// </summary>");
                        }
                        tw.WriteLine(ident + ident + "public const " + EscreveTipo(itemDominio.Chave) + " " + "_" + ChangeCharacters(itemDominio.Valor.ToString()) +
                                     " = " +
                                     DefineValor(itemDominio.Chave) + ";");
                    }
                }
            }

            #endregion

            #region Fim da Classe
            tw.WriteLine(ident + "}");

            tw.WriteLine("}"); //namespace
            #endregion

            tw.Close();
        }

        private static string DefineValor(object p)
        {
            try
            {
                Int32.Parse(p.ToString());
                return p.ToString();
            }
            catch(Exception)
            {
                return "\"" + p + "\"";
            }
        }

        private static string EscreveTipo(object valor)
        {
            try
            {
                Int32.Parse(valor.ToString());
                return "int";
            }
            catch(Exception)
            {
                return "string";
            }
        }

        private void GerarArquivoTables(string tableName)
        {

            IEnumDataset enumDatasetFeatures = Ws.get_Datasets(esriDatasetType.esriDTTable);
            IDataset dataset = enumDatasetFeatures.Next();

            while (dataset != null)
            {
                if (dataset.Name == tableName)
                {
                    gravarArquivo(dataset);
                    lstBoxArquivos.Items.Add(dataset.Name + ".cs");
                    break;
                }
                dataset = enumDatasetFeatures.Next();
            }
        }

        private void GerarArquivoFeatureClass(string featureDatasetName)
        {
            var enumDatasetFeatures = Ws.get_Datasets(esriDatasetType.esriDTFeatureDataset);
            var dataset = enumDatasetFeatures.Next();


            while (dataset != null)
            {
                if (dataset.Name == featureDatasetName)
                {
                    var enumSubDataset = dataset.Subsets;
                    var subDataset = enumSubDataset.Next();
                    while (subDataset != null)
                    {
                        gravarArquivo(subDataset);
                        lstBoxArquivos.Items.Add(subDataset.Name + ".cs");
                        subDataset = enumSubDataset.Next();
                    }
                    break;
                }


                dataset = enumDatasetFeatures.Next();
            }
        }

        private void gravarArquivo(IDataset dataset)
        {
            var fclass =  dataset  as IFeatureClass;
            var table =  dataset  as ITable;
            var className = "";

            if (fclass != null)
                className = ( (IDataset) fclass ).Name;
            else
                className = ( (IDataset) table ).Name;
            
            TextWriter tw = new StreamWriter(txtBoxCaminho.Text + "/" + className + ".cs");

            #region Using
            tw.WriteLine("using System;");
            tw.WriteLine("using System.Collections.Generic;");
            tw.WriteLine("using System.Text;");
            tw.WriteLine("using ESRI.ArcGIS.Geometry;");
            #endregion

            #region Cabeçalho da Classe (nome da classe)
            tw.WriteLine("namespace ClassGeneratorSDE");
            tw.WriteLine("{");
            tw.WriteLine(ident + "class " + className.Replace(".","_"));
            tw.WriteLine(ident + "{");
            #endregion

            #region Atributos/Getters/Setters     
            IDictionary<String, String> subtypes = new Dictionary<String, String>();
            IDictionary<String, String> domains = new Dictionary<String, String>();            
           
            //if (fclass != null)
            //{                             
            //    for (int i = 0; i < fclass.Fields.FieldCount; i++)
            //    {
            //        IField fld = fclass.Fields.get_Field(i);
            //        if (CampoEhSubtipo(fld.Name, fclass))
            //            //subtypes.Add(fld.Name, (fld as ISubtypes).subtype);

            //        if (fld.Domain != null)
            //            domains.Add(fld.Name, fld.Domain.Name);                    

            //        //tw.WriteLine(ident + ident + GetTipo(fclass.Fields.get_Field(i).Type) + " " + fclass.Fields.get_Field(i).Name.ToLower() + ";");
            //    }

            //    // Encapsulamento das variaveis
            //    for (var i = 0; i < fclass.Fields.FieldCount; i++)
            //    {
            //        tw.WriteLine(ident + ident + "public " + GetTipo(fclass.Fields.get_Field(i).Type) + " " + fclass.Fields.get_Field(i).Name.ToUpper() + "{get; set;" + "}");
            //        //tw.WriteLine(ident + ident + ident + "get { return " + fclass.Fields.get_Field(i).Name.ToLower() + "; }");
            //        //tw.WriteLine(ident + ident + ident + "set { " + fclass.Fields.get_Field(i).Name.ToLower() + " = value; }");
            //    }
            //}
            
            if (table != null)
            {
                //for (var i = 0; i < table.Fields.FieldCount; i++)
                //{
                //    //tw.WriteLine(ident + ident + GetTipo(table.Fields.get_Field(i).Type) + " " + table.Fields.get_Field(i).Name.ToLower() + ";");
                //}

                // Encapsulamento das variaveis
                for (var i = 0; i < table.Fields.FieldCount; i++)
                {
                    tw.WriteLine(ident + ident + "public " + GetTipo(table.Fields.get_Field(i).Type) + " " + table.Fields.get_Field(i).Name.ToUpper() + " { get; set; }");
                    //{ return " + table.Fields.get_Field(i).Name.ToLower() + "; }");
                    //tw.WriteLine(ident + ident + ident + "set { " + table.Fields.get_Field(i).Name.ToLower() + " = value; }");
                }
            }

            #endregion

            #region Fim da Classe
            tw.WriteLine();
            GravarDominiosNaClasse(domains, tw);
            tw.WriteLine(ident + "}");
            tw.WriteLine("}");
            #endregion

            tw.Close();
        }

        private void GravarDominiosNaClasse(IDictionary<string, string> domains, TextWriter tw)
        {
            tw.WriteLine("internal class Domains");
            tw.WriteLine(ident + ident + "{");
            
            foreach (var domain in domains)
            {
                tw.WriteLine(ident + ident + ident + "internal class " + domain.Key.ToLower());
                tw.WriteLine(ident + ident + ident + "{");
                var varName = ChangeCharacters(domain.Key.ToString());
                if (varName != domain.Key)
                {
                    tw.WriteLine(ident + ident + ident + ident + "//");
                    tw.WriteLine(ident + ident + ident + ident + varName);
                    tw.WriteLine(ident + ident + ident + ident + "//");
                }
                tw.WriteLine(ident + ident + ident + ident + "public const string " + ChangeCharacters(domain.Key) + " = " + domain.Key);
                tw.WriteLine(ident + ident + ident + ident + "public const string " + domain.Value   + " = " + domain.Value);
                tw.WriteLine(ident + ident + ident + "}");
                tw.WriteLine();
            }

            tw.WriteLine();
            tw.WriteLine(ident + ident + "}");
        }

        private string ChangeCharacters(string key)
        {
            key = key
                .Replace("¹", "1")
                .Replace("²", "2")
                .Replace("³", "3")
                .Replace("ª", "a")
                .Replace("º", "o")

                .Replace("ã", "a")
                .Replace("â", "a")
                .Replace("á", "a")
                .Replace("à", "a")
                .Replace("ä", "a")

                .Replace("Ã", "A")
                .Replace("Â", "A")
                .Replace("Á", "A")
                .Replace("À", "A")
                .Replace("Ä", "A")

                .Replace("ê", "e")
                .Replace("é", "e")
                .Replace("è", "e")
                .Replace("ë", "e")

                .Replace("ê", "e")
                .Replace("é", "e")
                .Replace("è", "e")
                .Replace("ë", "e")

                .Replace("õ", "o");
            return new Regex("[^a-zA-Z0-9_]+").Replace(key, "_");
        }


        private static bool CampoEhSubtipo(String fieldName, IFeatureClass classe)
        {
            var r = false;
            var subtipo = classe as ISubtypes;
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
        public ArrayList GetDominios(String nomeDominio, IWorkspace ws)
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
            var listaOrdenada = new System.Collections.ArrayList((System.Collections.ICollection)valores);
            IComparer ordernador = new OrdenaListItem();
            listaOrdenada.Sort(ordernador);

            return listaOrdenada;
        }

        private static string GetTipo(esriFieldType fieldTipo)
        {
            string retorno = "object";


            try
            {
                System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
                if (fieldTipo == esriFieldType.esriFieldTypeDate)
                    retorno = "DateTime";

                if (fieldTipo == esriFieldType.esriFieldTypeInteger || fieldTipo == esriFieldType.esriFieldTypeOID)
                    retorno = "int";

                if (fieldTipo == esriFieldType.esriFieldTypeDouble)
                    retorno = "double";

                if (fieldTipo == esriFieldType.esriFieldTypeString)
                    retorno = "string";

                if (fieldTipo == esriFieldType.esriFieldTypeGeometry)
                    retorno = "IGeometry";

                if (fieldTipo == esriFieldType.esriFieldTypeGUID)
                    retorno = "GUID";                
            }
            catch (Exception)
            {
                MessageBox.Show("Não foi possível...");
            }
            finally
            {
                System.Windows.Forms.Cursor.Current = Cursors.Default;                
            }

            return retorno;
        }

        private void ChkBoxMarcarTodosFcCheckedChanged(object sender, EventArgs e)
        {
            if (treeViewDatasets.Nodes.Count > 0)
                for (int i = 0; i < treeViewDatasets.Nodes.Count; i++)
                {
                    treeViewDatasets.Nodes[i].Checked = ((CheckBox) sender).Checked;
                }
        }

        private void ChkBoxMarcarTodosTableCheckedChanged(object sender, EventArgs e)
        {
            if (treeViewTables.Nodes.Count > 0)
                for (var i = 0; i < treeViewDatasets.Nodes.Count; i++)
                {
                    treeViewTables.Nodes[i].Checked = ((CheckBox) sender).Checked;
                }
        }
    }
}