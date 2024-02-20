using DR.Networking.Core;
using DR.Networking.Models;
using System.Threading.Tasks;

namespace DR.Networking
{
    public class Request
    {
        #region GET
        public static async Task<ResultData> Get(string url) => await Main.RequestBase<string?>(url, RequestTypes.Get, null, null);

        #endregion GET

        #region HEAD
        #endregion HEAD

        #region POST
        #endregion POST

        #region PUT
        #endregion PUT

        #region DELETE
        #endregion DELETE

        #region TRACE
        #endregion TRACE

        #region OPTIONS
        #endregion OPTIONS

        #region CONNECT
        #endregion CONNECT

        #region PATCH
        #endregion PATCH
    }
}
