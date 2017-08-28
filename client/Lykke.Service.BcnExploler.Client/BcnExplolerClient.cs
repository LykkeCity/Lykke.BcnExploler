using System;
using Common.Log;

namespace Lykke.Service.BcnExploler.Client
{
    public class BcnExplolerClient : IBcnExplolerClient, IDisposable
    {
        private readonly ILog _log;

        public BcnExplolerClient(string serviceUrl, ILog log)
        {
            _log = log;
        }

        public void Dispose()
        {
            //if (_service == null)
            //    return;
            //_service.Dispose();
            //_service = null;
        }
    }
}
