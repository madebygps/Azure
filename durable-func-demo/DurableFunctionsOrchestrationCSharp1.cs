using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Company.Function
{
    public static class DurableFunctionsOrchestrationCSharp1
    {
        [FunctionName("DurableFunctionsOrchestrationCSharp1")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();
            EmployeesRequest data = context.GetInput<EmployeesRequest>();

            foreach(var employee in data.Employees)
            {
                outputs.Add(await context.CallActivityAsync<string>("DurableFunctionsOrchestrationCSharp1_Hello", employee.name));

            }           

            return outputs;
        }

        [FunctionName("DurableFunctionsOrchestrationCSharp1_Hello")]
        public static string SayHello([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"Saying hello to {name}.");
            return $"Hello {name}!";
        }

        [FunctionName("DurableFunctionsOrchestrationCSharp1_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {

            var data = await req.Content.ReadAsAsync<EmployeesRequest>();

            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("DurableFunctionsOrchestrationCSharp1", data);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}