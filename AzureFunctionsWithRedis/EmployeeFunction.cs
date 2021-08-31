using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;


namespace AzureFunctionsWithRedis
{
    public class EmployeeFunction
    {

        private readonly IDistributedCache _cache;
        public EmployeeFunction(IDistributedCache cache)
        {
            this._cache = cache;
        }

        [FunctionName("GetEmployeeByID")]
        public IActionResult GetEmployeeById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Employee/{Id}")] HttpRequest req,
            ILogger log, string Id)
        {
            var employee = this._cache.GetString(Id);
            if (employee == null)
                return (IActionResult)new NotFoundResult();
            return (IActionResult)new OkObjectResult(employee);
        }

        [FunctionName("CreateEmployee")]
        public async Task<IActionResult> CreateEmployee([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Employee")] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var emp = JsonConvert.DeserializeObject<Employee>(requestBody);
            
            if (string.IsNullOrEmpty(emp.Id)) return new BadRequestResult();
            
            if (SaveEmployee(emp))
            {
                return new OkObjectResult(emp);
            }
            else
            {
                return new BadRequestResult();
            }

        }

        private bool SaveEmployee(Employee emp)
        {
            try
            {
                this._cache.SetString(emp.Id, JsonConvert.SerializeObject(emp));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}

