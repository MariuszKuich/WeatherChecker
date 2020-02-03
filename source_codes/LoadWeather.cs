using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace API
{
    public static class LoadWeather
    {
        public static async Task<CurrentWeather> Load(string cityName)
        {
            string url = $"http://api.openweathermap.org/data/2.5/weather?q={cityName}&APPID={SetClient.Apid}";

            using (HttpResponseMessage response = await SetClient.Client.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    CurrentWeather weather = await response.Content.ReadAsAsync<CurrentWeather>();
                    return weather;
                }
                throw new Exception(response.ReasonPhrase);
            }
        }
    }
}