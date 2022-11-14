using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace UserCetrificateAutoEnrollment.BL.Windows
{
    public static class WindowsCertificateFactory
    {

        // Consistent key usage bits: DIGITAL_SIGNATURE
        private const string OID_PKIX_KP_CLIENT_AUTH = "1.3.6.1.5.5.7.3.2";
        private const int CERT_ENHKEY_USAGE_PROP_ID = 9;

        [DllImport("Crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern Boolean CertSetCertificateContextProperty(
            [In] IntPtr pCertContext,
            [In] UInt32 dwPropId,
            [In] UInt32 dwFlags,
            [In] IntPtr pvData);


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct CRYPT_OBJID_BLOB
        {
            public uint cbData;
            public IntPtr pbData;
        }

        [DllImport("Crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern Boolean CertAddEnhancedKeyUsageIdentifier(
            [In] IntPtr pCertContext,
            [In] string pszUsageIdentifier);

        private static bool ChangeEKU(X509Certificate2 cert, OidCollection oids)
        {
            X509EnhancedKeyUsageExtension eku = new X509EnhancedKeyUsageExtension(oids, true);

            CRYPT_OBJID_BLOB objID = new CRYPT_OBJID_BLOB();

            //allocate space in memory
            IntPtr pbData = Marshal.AllocHGlobal(eku.RawData.Length);
            IntPtr pvData = Marshal.AllocHGlobal(Marshal.SizeOf(objID));

            //copy eku raw data into pbData
            Marshal.Copy(eku.RawData, 0, pbData, eku.RawData.Length);

            //set CRYPT_OBJECT value with ekuRaw data and Length
            objID.pbData = pbData;
            objID.cbData = (uint)eku.RawData.Length;

            //copy CRYPT OBJECT into IntPtr
            Marshal.StructureToPtr(objID, pvData, false);

            var result = CertSetCertificateContextProperty(cert.Handle, CERT_ENHKEY_USAGE_PROP_ID, 0, pvData);

            Marshal.FreeHGlobal(objID.pbData);
            Marshal.FreeHGlobal(pvData);

            return result;
        }

        public static bool SetClientAuthEKU(X509Certificate2 cert)
        {
            OidCollection oids = new OidCollection();

            oids.Add(new Oid
            {
                Value = OID_PKIX_KP_CLIENT_AUTH
            });

            return ChangeEKU(cert, oids);
        }

        public static bool HasCertificateClientAuthEKU(this X509Certificate2 cert)
        {
            var certificateExtentions = cert.Extensions.OfType<X509EnhancedKeyUsageExtension>().FirstOrDefault()?.EnhancedKeyUsages;

            if(certificateExtentions == null)
            {
                return false;
            }

            foreach (var eku in certificateExtentions)
            {
                if (eku.Value == OID_PKIX_KP_CLIENT_AUTH)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool RemoveClientAuthEKU(X509Certificate2 cert)
        {
            var certificateExtentions = cert.Extensions.OfType<X509EnhancedKeyUsageExtension>().First().EnhancedKeyUsages;

            OidCollection oids = new OidCollection();

            foreach (var eku in certificateExtentions)
            {
                if (eku.Value != OID_PKIX_KP_CLIENT_AUTH)
                {
                    oids.Add(new Oid
                    {
                        Value = eku.Value
                    });
                }
            }

            return ChangeEKU(cert, oids);
        }
    }
}
