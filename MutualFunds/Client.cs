using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MutualFunds
{
    public class Client
    {
        private readonly HttpClient client = new HttpClient();

        public async Task<T> GetPriceHistory<T>(string url)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("*/*"));

            using(var httpResponse = 
            await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
{
            httpResponse.EnsureSuccessStatusCode(); // throws if not 200-299

            try
            {
                var stringResult = await httpResponse.Content.ReadAsStringAsync();

                Newtonsoft.Json.JsonSerializer serializer = new JsonSerializer();

                return JsonConvert.DeserializeObject<T>(stringResult);
            }
            catch // Could be ArgumentNullException or UnsupportedMediaTypeException
            {
                Console.WriteLine("HTTP Response was invalid or could not be deserialised.");
            }

            return default(T);
}
            

            // using var httpResponse = await client.GetAsync(url,
            //     HttpCompletionOption.ResponseHeadersRead);
            // {
            //     var contentStream = httpResponse.Content.ReadAsStreamAsync();

            //     using (var streamReader = new StreamReader(contentStream.Result))
            //     {
            //         using var jsonReader = new JsonTextReader(streamReader);

            //         Newtonsoft.Json.JsonSerializer serializer = new JsonSerializer();

            //         return serializer.Deserialize<T>(jsonReader);
            //     }
            // }
        }
    }
}
