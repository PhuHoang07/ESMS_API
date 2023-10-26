using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.SecretService
{
    public static class SecretService
    {
        private const string KeyVaultURI = "https://swp-key.vault.azure.net/";
        public static readonly string ConnectionString;
        public static readonly string JwtKey;

        static SecretService()
        {
            ConnectionString = getConnectionString();
            JwtKey = getJwtKey();
        }

        private static string getConnectionString()
        {
            var keyVaultEndPoint = new Uri(KeyVaultURI);
            var secretClient = new SecretClient(keyVaultEndPoint, new DefaultAzureCredential());

            KeyVaultSecret keyVaultSecret = secretClient.GetSecret("ESMS-AzureSQL");

            return keyVaultSecret.Value;
        }

        private static string getJwtKey()
        {
            var keyVaultEndPoint = new Uri(KeyVaultURI);
            var secretClient = new SecretClient(keyVaultEndPoint, new DefaultAzureCredential());

            KeyVaultSecret keyVaultSecret = secretClient.GetSecret("JwtKey");

            return keyVaultSecret.Value;
        }
    }
}
