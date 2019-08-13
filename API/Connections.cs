using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;

namespace API
{
    public static class Connections
    {
        public static async Task<string> Get(string connName)
        {
            var localConn = Startup.StaticConfig[connName];

            if (!string.IsNullOrEmpty(localConn)) {
                return localConn;
            }

            try
            {
                var secretConn = connName.Split(':')[1];

                AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
                KeyVaultClient keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

                var secret = await keyVaultClient.GetSecretAsync("https://bulee-keys.vault.azure.net/", secretConn)
                    .ConfigureAwait(false);

                return secret.Value;
            }
            catch (KeyVaultErrorException keyVaultException) {
                return keyVaultException.Message;
            }
        }
    }
}
