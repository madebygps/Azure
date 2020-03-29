using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

// https://blog.vjirovsky.cz/azure-functions-and-forbidden-socket-exception/

namespace MadeByGPS.Function {
    public static class Lights {

        public static HttpClient client = new HttpClient();
        public static HttpClientHandler clientHandler = new HttpClientHandler ();
        [FunctionName ("Lights")]
        public static async Task<IActionResult> Run (
            [HttpTrigger (AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log) {

            // docs https://developers.meethue.com/develop/get-started-2/

            log.LogInformation ("C# HTTP trigger function processed a request.");
        

            string bridge_ip = System.Environment.GetEnvironmentVariable("bridge_ip");
            string bridge_username = System.Environment.GetEnvironmentVariable("bridge_username");
            string url = ($"https://{bridge_ip}/api/{bridge_username}/groups/1/action");


            
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
        

            client = new HttpClient (clientHandler);

            string color = req.Query["color"];

            string requestBody = await new StreamReader (req.Body).ReadToEndAsync ();
            dynamic data = JsonConvert.DeserializeObject (requestBody);
            color = color ?? data?.color;

            LightSetting lightSetting = new LightSetting ();
            string json = lightSetting.make_call (color);

            
            var response = await client.PutAsync (url, new StringContent (json, Encoding.UTF8, "application/json"));

            client.Dispose();
            clientHandler.Dispose();
            

            string responseMessage = string.IsNullOrEmpty (json) ?
                "This HTTP triggered function executed successfully. Pass a color to change the lights!." :
                $"Setting color to: {json}. This HTTP triggered function executed successfully.";

            return new OkObjectResult (responseMessage);
        }

    
    
    }
}

