using System.Web;
using ClauseLibrary.Common;

namespace ClauseLibrary.Web.Services
{
    /// <summary>
    /// Manages refresh token access.
    /// </summary>
    public interface IRefreshTokenManager
    {
        /// <summary>
        /// Retrieves the token.
        /// </summary>
        string RetrieveToken(HttpRequestBase request);

        /// <summary>
        /// Stores the access token.
        /// </summary>
        void StoreAccessToken(SharePointAccessInfo accessInfo, HttpContextBase context);
    }
}