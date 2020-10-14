using System;
using System.Threading.Tasks;

using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;

namespace KintaiSystem.Helpers
{
    public static class KeyVaultHelper
    {
        /// <summary>
        /// KeyVaultからシークレットを取得する
        /// </summary>
        /// <param name="targetUri">取得するシークレットのUri</param>
        /// <returns>(Tuple)取得結果、プリンシパル情報</returns>
        public static async Task<(string Result, string PrincipalInfo)> GetSecretFromKeyVaultAsync(string targetUri)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();

            try
            {
                var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

                var secret = await keyVaultClient.GetSecretAsync(targetUri).ConfigureAwait(false);

                return (Result: secret.Value, PrincipalInfo: azureServiceTokenProvider.PrincipalUsed?.ToString() ?? string.Empty);

            }
            catch (Exception exp)
            {
                return (Result: exp.Message, PrincipalInfo: azureServiceTokenProvider.PrincipalUsed?.ToString() ?? string.Empty);
            }
        }

    }
}
