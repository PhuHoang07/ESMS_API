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
        public static readonly string ConnectionString;
        public static readonly string JwtKey;
        public static readonly string EmailPassword;

        private const string KeyVaultURI = "https://swp-key.vault.azure.net/";
        private static readonly Uri keyVaultEndPoint;
        private static readonly SecretClient secretClient;

        static SecretService()
        {
            keyVaultEndPoint = new Uri(KeyVaultURI);
            secretClient = new SecretClient(keyVaultEndPoint, new DefaultAzureCredential());

            ConnectionString = getConnectionString();
            JwtKey = getJwtKey();
            EmailPassword = getEmailPassword();
        }

        private static string getConnectionString()
        {
            KeyVaultSecret keyVaultSecret = secretClient.GetSecret("ESMS-AzureSQL");

            return keyVaultSecret.Value;
        }

        private static string getJwtKey()
        {
            KeyVaultSecret keyVaultSecret = secretClient.GetSecret("JwtKey");

            return keyVaultSecret.Value;
        }

        private static string getEmailPassword()
        {
            KeyVaultSecret keyVaultSecret = secretClient.GetSecret("EmailPassword");

            return keyVaultSecret.Value;
        }
    }
}
