using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace Company.Function
{
    public static class SayHello
    {

        [FunctionName("index")]
        public static IActionResult GetHomePage([HttpTrigger(AuthorizationLevel.Anonymous)]HttpRequest req, ExecutionContext context)
        {
            var path = Path.Combine(context.FunctionAppDirectory, "content", "index.html");
            
            return new ContentResult
            {
                Content = File.ReadAllText(path),
                ContentType = "text/html",
            };
        }

        [FunctionName("SayHello")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [SignalR(HubName = "helloHub")] IAsyncCollector<SignalRMessage> messages,
            ILogger log)
        {
            string name = req.Query["name"];

            if (string.IsNullOrEmpty(name))
            {
                return new BadRequestObjectResult("Parmeters not given.");
            }

            await messages.AddAsync(new SignalRMessage()
            {
                Target = "helloClient",
                Arguments = new object[] { name }
            });

            return new OkResult();
        }

        [FunctionName("negotiate")]
        public static SignalRConnectionInfo Negotiate( 
            [HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req,
            [SignalRConnectionInfo(HubName = "helloHub")] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }
    }
}
