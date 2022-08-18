using System.Web.Http;

namespace UserCertificateAutoEnrollment_KeyStoreAPI.Controllers
{
    public class ValuesController : ApiController
    {
        public string GetString(int id)
        {
            return $"We gave you back a string with the provided value: {id}";
        }
    }
}
