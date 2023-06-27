using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Azure;
using Azure.AI.FormRecognizer;

namespace PassportParser
{
    class Program
    {
        private const string Endpoint = "YOUR_ENDPOINT";
        private const string ApiKey = "YOUR_API_KEY";

        private static void Main()
        {
            const string passportFilePath = @"C:\Users\image.jpg"; 

            var client = AuthenticateClient();
            RecognizePassportAsync(client, passportFilePath).Wait();
        }

        private static FormRecognizerClient AuthenticateClient()
        {
            var credential = new AzureKeyCredential(ApiKey);
            var client = new FormRecognizerClient(new Uri(Endpoint), credential);
            return client;
        }

        private static async Task RecognizePassportAsync(FormRecognizerClient client, string passportFilePath)
        {
            using var stream = new FileStream(passportFilePath, FileMode.Open);
            var options = new RecognizeContentOptions();
            var operation = await client.StartRecognizeContentAsync(stream, options);
            var operationResponse = await operation.WaitForCompletionAsync();
            var forms = operationResponse.Value;

            Console.WriteLine(JsonSerializer.Serialize(forms));
        }
    }
}