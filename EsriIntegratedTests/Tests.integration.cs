using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESRI.ArcGIS.Geodatabase;
using System.Diagnostics;
using ESRI.ArcGIS.Geometry;

namespace Tests
{
    /// <summary>
    /// Summary description for Tests
    /// </summary>
    [TestClass]
    public class Tests
    {
        private static Geodatabase _geodatabase = null;

        public Tests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            try
            {
                _geodatabase = new Geodatabase(null, Geodatabase.InitializeLicense.Intialize);
            }
            catch (Exception ex)
            {
                Assert.Inconclusive("Can't initialize license" + "Trace:" + ex.Message);
            }
        }

        [TestInitialize()]
        public void MyTestInitialize()
        {
            _geodatabase.AbrirGeodatabase();
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            _geodatabase.FecharGeodatabase(true);
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestMethod1()
        {
            #region Abrir FeatureClass (Consultar dados)

            //IFeatureClass myFeatureClass = _geodatabase.GetFeatureClass("PL_MUNICIPIO");
            IFeatureClass myFeatureClass = _geodatabase.GetFeatureClass("PT_PONTO_GEOGRAFICO");

            string tabs = "\t" + "\t";

            Debug.WriteLine("==================================================================================================================================");
            Debug.WriteLine("Nome do Campo" + tabs + "Tipo do Campo" + tabs + "Requerido" + tabs + "Precisão" + tabs + "Domínio");
            Debug.WriteLine("==================================================================================================================================");
            for (int i = 0; i < myFeatureClass.Fields.FieldCount; i++)
            {
                IField fld = myFeatureClass.Fields.get_Field(i);
                Debug.WriteLine(fld.AliasName + tabs + fld.Type + tabs + fld.Required + tabs + fld.Length + "." + fld.Precision + tabs + LerDominio(fld));
            }

            Debug.WriteLine("----------------------------------------------------------------------------------------------------------------------------------");

            #region Dominios
            Debug.WriteLine("Dados de Domínio");
            Debug.WriteLine("----------------------------------------------------------------------------------------------------------------------------------");
            for (int i = 0; i < myFeatureClass.Fields.FieldCount; i++)
            {
                IField fld = myFeatureClass.Fields.get_Field(i);
                string nomeDominio = LerDominio(fld);
                if (nomeDominio != String.Empty)
                {
                    Debug.WriteLine("----------------------------------------");
                    Debug.WriteLine("Nome Domínio: " + nomeDominio);
                    Debug.WriteLine("Valores possíveis:");
                    ArrayList dominios = _geodatabase.Dominios(nomeDominio, Geodatabase.ws);

                    for (int j = 0; j < dominios.Count; j++)
                    {
                        Debug.WriteLine(dominios[j]);
                    }
                }
            }
            #endregion

            Debug.WriteLine("----------------------------------------------------------------------------------------------------------------------------------");
            #endregion

            #region Editar FeatureClass

            _geodatabase.IniciarSessaoDeEdicao(false);
           
            IFeature newFeature = myFeatureClass.CreateFeature();

            #region Definir valores
            newFeature.set_Value(newFeature.Fields.FindField("DE_BARRAMENTO"), "NovaFeicao");

            IPoint point = new PointClass();
            point.X = 181625.978;
            point.Y = 9290283.918;

            newFeature.Shape = point;

            #endregion

            newFeature.Store();

            _geodatabase.PararSessaoDeEdicao(true);

            #endregion           
        }

        private string LerDominio(IField fld)
        {
            string r = String.Empty;

            if (fld.Domain != null)
                return fld.Domain.Name;

            return r;
        }
    }
}
