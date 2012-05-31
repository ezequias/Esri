using System;
using Microsoft.Win32;
using System.Reflection;



namespace Tests
{

    public class ArcFMUtil
    {
        private static Type pType;
        private static object _pInstance;
        private static readonly object[] Obj = new object[1];
        public static bool LoadArcFmLicense()
        {

            try
            {

                if (_pInstance == null)

                    GetArcFmAssembly();



                MethodInfo pMetodoinitialize = pType.GetMethod("Initialize");

                object retornoInitialize = pMetodoinitialize.Invoke(_pInstance, Obj);



                if (!retornoInitialize.ToString().ToUpper().Equals("mmLicenseCheckedOut".ToUpper()))

                    return false;

                else

                    return true;

            }

            catch (Exception)
            {

                return false;

            }

        }
        public static bool ShutDownArcFmLicense()
        {

            if (_pInstance != null)
            {

                MethodInfo pMetodoinitialize = pType.GetMethod("Shutdown");

                pMetodoinitialize.Invoke(_pInstance, null);

            }

            return true;

        }

        private static void GetArcFmAssembly()
        {
            Obj[0] = 5; //= mmLicensedProductCode.mmLPArcFM

            RegistryKey registry = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Miner and Miner\\", RegistryKeyPermissionCheck.ReadSubTree);
            if (registry != null)
            {
                string arcFmPath = registry.GetValue("LastInstallDir").ToString();
                Assembly pAssembly =
                    Assembly.LoadFile(arcFmPath +
                                      "ArcFM Solution\\DotNet\\Assemblies\\v9.2.0.0\\Miner.Interop.System.DLL");

                pType = pAssembly.GetType("Miner.Interop.MMAppInitializeClass");
                MethodInfo pMetodo = pType.GetMethod("IsProductCodeAvailable");
                _pInstance = pAssembly.CreateInstance(pType.FullName);
                object retorno = pMetodo.Invoke(_pInstance, Obj);
            }
        }
    }
}