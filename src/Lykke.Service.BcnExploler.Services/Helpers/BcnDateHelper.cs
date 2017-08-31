using System;

namespace Lykke.Service.BcnExploler.Services.Helpers
{
    public static class BcnDateHelper
    {
        public static string ToStringBcnExplolerFormat(this DateTime date)
        {
            if (DateTime.Now.Date - date.Date > TimeSpan.FromDays(6))
            {
                return date.ToString("MMMM dd, yyyy h:mm tt");
            }

            return date.ToString("dddd, h:mm tt");
        }
    }
}