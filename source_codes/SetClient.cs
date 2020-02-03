using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace API
{
    public static class SetClient
    {
        private static string apid;

        public static string Apid
        {
            get => apid;
        }

        public static HttpClient Client { get; set; }

        public static void InitializeClient()
        {
            Client = new HttpClient();
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                apid = File.ReadAllText("api_key");
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Could not find file 'api_key'. File is necessary for the application to work.");
                Console.WriteLine("Error message: " + e.Message);
                Client.Dispose();
                Console.ReadKey();
                Environment.Exit(1);
            }
        }
    }
}