using Miner.Interop;

namespace Tests
{
    public class Helper
    {
        private mmLicenseStatus licenseStatus;
        private MMAppInitialize m_MMAppInitialize;

        public mmLicenseStatus LicenseStatus
        {
            get { return licenseStatus; }
            set { licenseStatus = value; }
        }

        public mmLicenseStatus InicializaLicencaArcFm(mmLicensedProductCode minerLicense)
        {
            this.licenseStatus = CheckOutLicensesArcFM(minerLicense);
            return licenseStatus;
        }

        public void FinalizaLicencaArcFM()
        {
            m_MMAppInitialize.Shutdown();
        }

        //ArcFM License Initializer
        private mmLicenseStatus CheckOutLicensesArcFM(mmLicensedProductCode productCode)
        {
            m_MMAppInitialize = new MMAppInitializeClass();
            mmLicenseStatus licenseStatus = default(mmLicenseStatus);
            licenseStatus = m_MMAppInitialize.IsProductCodeAvailable(productCode);

            if (licenseStatus == mmLicenseStatus.mmLicenseAvailable)
            {
                licenseStatus = m_MMAppInitialize.Initialize(productCode);
            }
            return licenseStatus;
        }
    }
}
