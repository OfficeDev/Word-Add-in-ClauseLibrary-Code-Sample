using System;
using System.Web;
using ClauseLibrary.Common;

namespace ClauseLibrary.Web.Services
{
    /// <summary>
    /// Manages the refresh token through a server encrypted cookie.
    /// </summary>
    public class ServerCookieRefreshTokenManager : IRefreshTokenManager
    {
        private const string RefreshTokenCookieKey = "RefreshToken";

        /// <summary>
        /// Retrieves the token.
        /// </summary>
        public string RetrieveToken(HttpRequestBase request)
        {
            var httpCookie = request.Cookies[RefreshTokenCookieKey];
            if (httpCookie == null)
            {
                throw new Exception("No refresh token cookie");
            }

            string refreshToken = httpCookie.Value;
            return  refreshToken.Decrypt();
        }

        /// <summary>
        /// Stores the access token.
        /// </summary>
        public void StoreAccessToken(SharePointAccessInfo accessInfo, HttpContextBase context)
        {
            // always store encypted refresh token.
            var cookie = new HttpCookie(RefreshTokenCookieKey, accessInfo.RefreshToken.Encrypt());
            cookie.Expires = DateTime.Now.AddMonths(1);

            context.Response.Cookies.Add(cookie);
            accessInfo.RefreshToken = string.Empty;
        }
    }
}