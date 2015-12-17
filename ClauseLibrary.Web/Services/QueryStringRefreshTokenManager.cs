using System.Web;
using ClauseLibrary.Common;

namespace ClauseLibrary.Web.Services
{
    /// <summary>
    /// Manages the refresh token via the query string.
    /// </summary>
    public class QueryStringRefreshTokenManager : IRefreshTokenManager
    {
        /// <summary>
        /// Retrieves the token.
        /// </summary>
        public string RetrieveToken(HttpRequestBase request)
        {
            string refreshTokenEncrypted = request.Form["refreshToken"];
            return refreshTokenEncrypted.Decrypt();
        }

        /// <summary>
        /// Stores the access token.
        /// </summary>
        public void StoreAccessToken(SharePointAccessInfo accessInfo, HttpContextBase context)
        {
            accessInfo.RefreshToken = accessInfo.RefreshToken.Encrypt();
        }
    }
}