﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Moonglade.IndexNow.Client;

public class IndexNowClient(ILogger<IndexNowClient> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory) : IIndexNowClient
{
    public Task SendRequestAsync(Uri uri)
    {
        string[] pingTargets = configuration.GetSection("IndexNow:PingTargets").Get<string[]>();
        var apiKey = configuration["IndexNow:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            logger.LogWarning("IndexNow:ApiKey is not configured.");
            return Task.CompletedTask;
        }

        if (pingTargets == null || !pingTargets.Any())
        {
            throw new InvalidOperationException("IndexNow:PingTargets is not configured.");
        }

        foreach (var pingTarget in pingTargets)
        {
            var client = httpClientFactory.CreateClient(pingTarget);

            var requestBody = new IndexNowRequest
            {
                Host = uri.Host,
                Key = apiKey,
                KeyLocation = $"https://{uri.Host}/indexnowkey.txt",
                UrlList = [uri.ToString()]
            };
        }

        throw new NotImplementedException();
    }
}