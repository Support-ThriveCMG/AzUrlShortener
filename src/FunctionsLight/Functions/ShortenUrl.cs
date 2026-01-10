using Cloud5mins.ShortenerTools.Core.Services;
using Cloud5mins.ShortenerTools.Core.Service;
using Cloud5mins.ShortenerTools.Core.Messages;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Azure.Data.Tables;
using System.Net;
using System.Text.Json;
using System.IO;

namespace Cloud5mins.ShortenerTools.Functions
{
    public class CreateShortUrl
    {
        private readonly ILogger _logger;
        private readonly TableServiceClient _tblClient;

        public CreateShortUrl(
            ILoggerFactory loggerFactory,
            TableServiceClient tblClient)
        {
            _logger = loggerFactory.CreateLogger<CreateShortUrl>();
            _tblClient = tblClient;
        }

        [Function("CreateShortUrl")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")]
            HttpRequestData req)
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonSerializer.Deserialize<CreateShortUrlRequest>(body);

            if (string.IsNullOrWhiteSpace(request?.Url))
            {
                var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                await bad.WriteStringAsync("Url is required");
                return bad;
            }

            var urlService = new UrlServices(
                _logger,
                new AzStrorageTablesService(_tblClient));

            var host = $"{req.Url.Scheme}://{req.Url.Host}";

            ShortResponse result = await urlService.Create(
                request.Url,
                host);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result);
            return response;
        }
    }

    public record CreateShortUrlRequest(string Url);
}
