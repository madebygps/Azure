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

namespace MadeByGPS.Function
{
    public static class Lights {
        [FunctionName ("Lights")]
        public static async Task<IActionResult> Run (
            [HttpTrigger (AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log) {

// docs https://developers.meethue.com/develop/get-started-2/
                
            log.LogInformation ("C# HTTP trigger function processed a request.");
            // Bridge IP = 192.168.1.124
            // username = sUQTmsDzmAJbqsbI3ZXbRp1W6AKPb8BTslQJfdLj
            // url https://192.168.1.124/api/sUQTmsDzmAJbqsbI3ZXbRp1W6AKPb8BTslQJfdLj/groups/1/action
            /*
                device 
                {"devicetype":"my_hue_app#iphone gps"}

                to change all lights 
                
            */

            HttpClientHandler clientHandler = new HttpClientHandler();
clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

// Pass the handler to httpclient(from you are calling api)
HttpClient client = new HttpClient(clientHandler);

            string color = req.Query["color"];

            string requestBody = await new StreamReader (req.Body).ReadToEndAsync ();
            dynamic data = JsonConvert.DeserializeObject (requestBody);
            color = color ?? data?.color;

            Jsoncall jsoncall = new Jsoncall ();
            string json = jsoncall.make_call (color);

            //var response = await client.PostAsync("https://192.168.1.124/api/sUQTmsDzmAJbqsbI3ZXbRp1W6AKPb8BTslQJfdLj/groups/1/action", json);
            var response = await client.PutAsync("https://192.168.1.124/api/sUQTmsDzmAJbqsbI3ZXbRp1W6AKPb8BTslQJfdLj/groups/1/action", new StringContent(json, Encoding.UTF8, "application/json"));
            string responseContent = "";
            if (response.Content != null) {
         responseContent = await response.Content.ReadAsStringAsync();

        // From here on you could deserialize the ResponseContent back again to a concrete C# type using Json.Net
    }

            string responseMessage = string.IsNullOrEmpty (json) ?
                "This HTTP triggered function executed successfully. Pass a color to change the lights!." :
                $"Setting color to: {json}. This HTTP triggered function executed successfully.";

            return new OkObjectResult (responseMessage);
        }
    }
}