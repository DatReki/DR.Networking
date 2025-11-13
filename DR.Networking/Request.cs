using DR.Networking.Core;
using DR.Networking.Models;
using System.Threading.Tasks;

namespace DR.Networking
{
    public class Request
    {
        #region GET
        /// <summary>
        /// Make a get request.
        /// </summary>
        /// <param name="url">The url you want to make the request to.</param>
        /// <returns></returns>
        public static async Task<ResultData> Get(string url) => await Main.RequestBase<string?>(url, RequestTypes.Get, null, null);

        /// <summary>
        /// Make a get request with a named HttpClient.
        /// </summary>
        /// <param name="url">The url you want to make the request to.</param>
        /// <param name="clientName">
        /// The name of the HttpClient you want to use. <br />
        /// Make sure you added this named client in <see cref="Configuration"/>
        /// </param>
        /// <returns></returns>
        public static async Task<ResultData> Get(string url, string clientName) => await Main.RequestBase<string?>(url, RequestTypes.Get, null, null, clientName);

        /// <summary>
        /// Make a get request with a named HttpClient. <br />
        /// Make sure you added this named client in <see cref="Configuration"/>
        /// </summary>
        /// <param name="url">The url you want to make the request to.</param>
        /// <returns></returns>
        public static async Task<ResultData> Get<T>(string url) => await Main.RequestBase<string?>(url, RequestTypes.Get, null, null, typeof(T).GetClientName());

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
