using System;
using System.Collections.Generic;
using System.Text;
using Common.Log;

namespace Lykke.Service.BcnExploler.Core.Helpers
{
    public static class ConsoleHelper
    {
	    public static void Write(this IConsole console, string component, string process, string context, string info)
	    {
		    console.WriteLine($"{DateTime.UtcNow:T} {component}.{process} : {info} : {context}");
	    }
    }
}
