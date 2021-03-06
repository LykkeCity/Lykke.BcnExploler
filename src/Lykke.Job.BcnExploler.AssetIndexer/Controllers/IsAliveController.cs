﻿using System;
using System.Linq;
using Lykke.Job.BcnExploler.AssetIndexer.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Job.BcnExploler.AssetIndexer.Controllers
{
    // NOTE: See https://lykkex.atlassian.net/wiki/spaces/LKEWALLET/pages/35755585/Add+your+app+to+Monitoring
    [Route("api/[controller]")]
    public class IsAliveController : Controller
    {
        /// <summary>
        /// Checks service is alive
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            // NOTE: Feel free to extend IsAliveResponse, to display job-specific indicators
            return Ok(new IsAliveResponse
            {
                Name = Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationName,
                Version = Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationVersion,
                Env = Environment.GetEnvironmentVariable("ENV_INFO"),
#if DEBUG
                IsDebug = true,
#else
                IsDebug = false,
#endif
                IssueIndicators =Enumerable.Empty<IsAliveResponse.IssueIndicator>()
            });
        }
    }
}