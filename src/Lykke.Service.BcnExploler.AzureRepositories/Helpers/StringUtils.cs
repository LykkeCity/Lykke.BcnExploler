using System;
using Common;

namespace Lykke.Service.BcnExploler.AzureRepositories.Helpers
{
    public static class StringUtils
    {
        public static bool IsBase64(this string src)
        {
            try
            {
                src.Base64ToString();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
