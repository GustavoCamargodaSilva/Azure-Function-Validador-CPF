using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace httpValidaCpf
{
    public static class fnvalidacpf
    {
        [FunctionName("fnvalidacpf")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Iniciando a validacao do CPF.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            
            if(data == null)
            {
                return new BadRequestObjectResult("Por favor, informe o CPF.");
            }

            string cpf = data?.cpf;

            if (ValidaCPF(cpf))
            {
                return new OkObjectResult("CPF válido.");
            }
            else
            {
                return new BadRequestObjectResult("CPF inválido.");
            }
        }

        public static bool ValidaCPF(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            cpf = new string(cpf.Where(char.IsDigit).ToArray());
            if (cpf.Length != 11 || cpf.Distinct().Count() == 1)
                return false;

            int soma = 0, resto;
            for (int i = 1; i <= 9; i++)
                soma += int.Parse(cpf[i - 1].ToString()) * (11 - i);
            resto = (soma * 10) % 11;
            if (resto == 10) resto = 0;
            if (resto != int.Parse(cpf[9].ToString()))
                return false;

            soma = 0;
            for (int i = 1; i <= 10; i++)
                soma += int.Parse(cpf[i - 1].ToString()) * (12 - i);
            resto = (soma * 10) % 11;
            if (resto == 10) resto = 0;
            return resto == int.Parse(cpf[10].ToString());
        }
    }
}