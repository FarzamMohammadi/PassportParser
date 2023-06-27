using System;
using System.IO;
using System.Threading.Tasks;
using Azure;
using Azure.AI.FormRecognizer;
using Azure.AI.FormRecognizer.Models;

namespace PassportParser
{
    class Program
    {
        private const string endpoint = "<your-form-recognizer-endpoint>";
        private const string apiKey = "<your-form-recognizer-api-key>";

        private static void Main(string[] args)
        {
            var passportFilePath = args[0]; //Assume the first argument is the passport image file path

            var client = AuthenticateClient(endpoint, apiKey);
            RecognizePassportAsync(client, passportFilePath).Wait();
        }

        static FormRecognizerClient AuthenticateClient(string endpoint, string apiKey)
        {
            var credential = new AzureKeyCredential(apiKey);
            var client = new FormRecognizerClient(new Uri(endpoint), credential);
            return client;
        }

        static async Task RecognizePassportAsync(FormRecognizerClient client, string passportFilePath)
        {
            using var stream = new FileStream(passportFilePath, FileMode.Open);
            var options = new RecognizeContentOptions();
            var operation = await client.StartRecognizeContentAsync(stream, options);
            var operationResponse = await operation.WaitForCompletionAsync();
            var forms = operationResponse.Value;

            //Print recognized details in JSON format
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(forms));
        }
    }
}