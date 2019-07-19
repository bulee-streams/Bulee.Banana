using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;

namespace API
{
    public static class Secrets
    {
        public static async Task<string> Get(string secretName)
        {
            try
            {
                AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
                KeyVaultClient keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

                var secret = await keyVaultClient.GetSecretAsync("https://bulee-keys.vault.azure.net/", secretName)
                    .ConfigureAwait(false);

                return secret.Value;
            }
            catch (KeyVaultErrorException keyVaultException) {
                return keyVaultException.Message;
            }
        }
    }
}
