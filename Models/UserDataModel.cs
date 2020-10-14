using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

using KintaiSystem.Infrastructure;
using KintaiSystem.Infrastructure.Json;

using Microsoft.AspNetCore.Http;

namespace KintaiSystem.Models
{
    public class UserDataModel
    {
        #region properties
        public string TenantId { get; set; }

        public string ClientId { get; set; }

        public Uri RedirectUri { get; set; }

        public HttpResponseMessage Response { get; set; }
        #endregion

        public UserDataModel(HostString host)
        {
            TenantId = "common";
            ClientId = string.Empty;

            var redirectUri = new UriBuilder()
            {
                Scheme = Uri.UriSchemeHttps,
                Host = host.Host,
                Path = $"/.auth/aad-login/callback",
            };
            // ポート番号は値があるときだけ設定
            if (host.Port.HasValue)
            {
                redirectUri.Port = host.Port.Value;
            }
            RedirectUri = redirectUri.Uri;
        }

        /// <summary>
        /// <para>認証コードを取得するためのURIを取得</para>
        /// <para>cf. <seealso href="https://docs.microsoft.com/ja-jp/azure/active-directory/develop/v2-oauth2-auth-code-flow">Microsoft ID プラットフォームと OAuth 2.0 認証コード フロー</seealso></para>
        /// </summary>
        /// <param name="scope">ユーザーに同意を求めるスコープの、スペースで区切られたリスト。</param>
        /// <param name="state">
        /// <para>CSRF対策に使用する、ランダムに生成されたコード。</para>
        /// <para>この値を使用すると、認証要求の前にアプリ内でユーザーの状態 (表示中のページやビューなど) に関する情報をエンコードすることもできます。</para>
        /// </param>
        /// <returns>認証コードを取得するためのURI（ログインページ）</returns>
        public Uri GetRequestAuthenticationCodeUri(string scope, string state)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query.Add("client_id", ClientId);
            query.Add("response_type", "code");
            query.Add("redirect_uri", RedirectUri.OriginalString);
            query.Add("response_mode", "query");
            query.Add("scope", scope);
            query.Add("state", state);


            var returnUri = new UriBuilder()
            {
                Scheme = Uri.UriSchemeHttps,
                Host = "login.microsoftonline.com",
                Path = $"/{TenantId}/oauth2/v2.0/authorize",
                Query = query.ToString(),
            };

            return returnUri.Uri;
        }

        /// <summary>
        /// <para>認証コードを元にアクセストークンを取得する</para>
        /// <para>i.e. https://login.microsoftonline.com/{TenantId}/oauth2/v2.0/token</para>
        /// </summary>
        /// <param name="authenticationCode">認証コード</param>
        /// <param name="clientSecret">ClientSecret</param>
        /// <returns>アクセストークン</returns>
        public async Task<string> GetAccessTokenAsync(string authenticationCode, string clientSecret)
        {
            var requestAccessTokenUri = new UriBuilder()
            {
                Scheme = Uri.UriSchemeHttps,
                Host = "login.microsoftonline.com",
                Path = $"/{TenantId}/oauth2/v2.0/token"
            }.Uri.AbsoluteUri;

            using var client = new HttpClient();
            using var accessTokenRequest = new HttpRequestMessage(HttpMethod.Post, requestAccessTokenUri);

            var parameters = new Dictionary<string, string>()
                {
                    { "client_id", ClientId },
                    { "scope", Constants.Scope.UserRead },
                    { "code", authenticationCode },
                    { "redirect_uri", RedirectUri.OriginalString },
                    { "grant_type", "authorization_code" },
                    { "client_secret", clientSecret },
                };
            accessTokenRequest.Content = new FormUrlEncodedContent(parameters);

            Response = await client.SendAsync(accessTokenRequest);

            var getTokenResult = await Response.Content.ReadAsStringAsync();

            var deserializeObj = JsonSerializer.Deserialize<AuthenticationTokenJsonData>(getTokenResult);
            var accessToken = deserializeObj.AccessToken;

            return accessToken;
        }
    }
}
