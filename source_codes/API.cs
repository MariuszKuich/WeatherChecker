using System;
using System.Threading.Tasks;

namespace API
{
    class API
    {
        static async Task Main(string[] args)
        {
            DisplayHeader();
            SetClient.InitializeClient();
            while (true)
            {
                string city = Dialog();
                if (String.IsNullOrWhiteSpace(city))
                {
                    Console.WriteLine("Field 'Enter city name: ' cannot be empty.");
                    Console.WriteLine();
                    continue;
                }

                if (city.ToLower() == "exit") break;
                CurrentWeather weather = null;
                try
                {
                    weather = await LoadWeather.Load(city);
                }
                catch (Exception e)
                {
                    switch (e.Message)
                    {
                        case "Resource temporarily unavailable":
                            Console.WriteLine(
                                "openweathermap.org servers are down or your Internet connection is not working.");
                            break;
                        case "Bad Request":
                            Console.WriteLine("Field 'Enter city name: ' cannot be empty.");
                            break;
                        case "Not Found":
                            Console.WriteLine("City name incorrect or not present in database.");
                            break;
                        case "Unauthorized":
                            Console.WriteLine(
                                "Incorrect API key for openweathermap.org request. Check your API key in located in 'api_key' file.");
                            break;
                    }

                    Console.WriteLine("Error message: " + e.Message);
                    Console.WriteLine();
                    continue;
                }

                DisplayInfo(weather, city);
            }

            SetClient.Client.Dispose();
        }

        static void DisplayHeader()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("# Weather Checker #");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("# Author: Mariusz #");
            Console.WriteLine("#   Version 1.0   #");
            Console.WriteLine();
            Console.WriteLine("Application checks current weather in given city.");
            Console.WriteLine();
            Console.ResetColor();
        }

        static string Dialog()
        {
            Console.Write("Enter city name(type 'exit' to terminate the application): ");
            return Console.ReadLine();
        }

        static void DisplayInfo(CurrentWeather weather, string city)
        {
            Console.WriteLine();
            OneWordInColor(city, ConsoleColor.Green, false);
            Console.WriteLine(" - info:");
            Console.Write("\tLongitude:\t");
            OneWordInColor("" + weather.Coord.Lon, ConsoleColor.Green, true);
            Console.Write("\tLatitude:\t");
            OneWordInColor("" + weather.Coord.Lat, ConsoleColor.Green, true);
            int time = TimeSpan.FromSeconds(weather.Timezone).Hours;
            Console.Write("\tTimezone:\t");
            OneWordInColor("UTC ", ConsoleColor.Green, false);
            OneWordInColor(time >= 0 ? "+" + time : "" + time, ConsoleColor.Green, true);
            int sunriseHour, sunriseMinute, sunsetHour, sunsetMinute;
            string sunriseHourS, sunriseMinuteS, sunsetHourS, sunsetMinuteS;
            SetSunriseSunset(out sunriseHour, out sunriseMinute, out sunsetHour, out sunsetMinute, out sunriseHourS,
                out sunriseMinuteS, out sunsetHourS, out sunsetMinuteS, weather, time);
            Console.Write("\tSunrise:\t");
            OneWordInColor("" + sunriseHourS + ":" + sunriseMinuteS, ConsoleColor.Green, true);
            Console.Write("\tSunset:\t\t");
            OneWordInColor("" + sunsetHourS + ":" + sunsetMinuteS, ConsoleColor.Green, true);
            Console.WriteLine();

            OneWordInColor(city, ConsoleColor.Green, false);
            Console.WriteLine(" - weather:");
            Console.Write("\tWeather type:\t");
            OneWordInColor(weather.Weather[0].Main, ConsoleColor.Green, true);
            char firstLetter = Char.ToUpper(weather.Weather[0].Description[0]);
            string desc = weather.Weather[0].Description.Substring(1, weather.Weather[0].Description.Length - 1);
            desc = firstLetter + desc;
            Console.Write("\tDescription:\t");
            OneWordInColor(desc, ConsoleColor.Green, true);
            Console.Write("\tTemperature:\t");
            double temp = Math.Round(weather.Main.Temp - 273.15, 1);
            OneWordInColor(temp + " *C", ConsoleColor.Green, true);
            Console.Write("\tSensed temp.:\t");
            temp = Math.Round(weather.Main.Feels_like - 273.15, 1);
            OneWordInColor(temp + " *C", ConsoleColor.Green, true);
            Console.Write("\tPressure:\t");
            OneWordInColor(weather.Main.Pressure + " hPa", ConsoleColor.Green, true);
            Console.Write("\tHumidity:\t");
            OneWordInColor(weather.Main.Humidity + " %", ConsoleColor.Green, true);
            Console.Write("\tWind speed:\t");
            OneWordInColor(Math.Round(weather.Wind.Speed / 1000 * 3600, MidpointRounding.AwayFromZero) + " km/h", ConsoleColor.Green, true);
            string direction = GetWindDirection(weather.Wind.Deg);
            Console.Write("\tWind direction:\t");
            OneWordInColor(direction, ConsoleColor.Green, true);
            Console.Write("\tCloudiness:\t");
            OneWordInColor(weather.Clouds.All + " %", ConsoleColor.Green, true);
            Console.WriteLine();
        }

        static void OneWordInColor(string word, ConsoleColor color, bool eol)
        {
            Console.ForegroundColor = color;
            if (eol) Console.WriteLine(word);
            else Console.Write(word);
            Console.ResetColor();
        }

        static void SetSunriseSunset(out int sunriseHour, out int sunriseMinute, out int sunsetHour,
            out int sunsetMinute, out string sunriseHourS, out string sunriseMinuteS, out string sunsetHourS,
            out string sunsetMinuteS, CurrentWeather weather, int time)
        {
            sunriseHour = TimeSpan.FromSeconds(weather.Sys.Sunrise).Hours + time;
            if (sunriseHour >= 24) sunriseHour -= 24;
            sunriseHourS = sunriseHour < 10 ? "0" + sunriseHour : "" + sunriseHour;
            sunriseMinute = TimeSpan.FromSeconds(weather.Sys.Sunrise).Minutes;
            sunriseMinuteS = sunriseMinute < 10 ? "0" + sunriseMinute : "" + sunriseMinute;
            sunsetHour = TimeSpan.FromSeconds(weather.Sys.Sunset).Hours + time;
            if (sunsetHour >= 24) sunsetHour -= 24;
            sunsetHourS = sunsetHour < 10 ? "0" + sunsetHour : "" + sunsetHour;
            sunsetMinute = TimeSpan.FromSeconds(weather.Sys.Sunset).Minutes;
            sunsetMinuteS = sunsetMinute < 10 ? "0" + sunsetMinute : "" + sunsetMinute;
        }

        static string GetWindDirection(double deg)
        {
            if (deg >= 348.75 && deg < 11.25) return "S";
            else if (deg >= 11.25 && deg < 33.75) return "SSW";
            else if (deg >= 33.75 && deg < 56.25) return "SW";
            else if (deg >= 56.25 && deg < 78.75) return "WSW";
            else if (deg >= 78.75 && deg < 101.25) return "W";
            else if (deg >= 101.25 && deg < 123.75) return "WNW";
            else if (deg >= 123.75 && deg < 146.25) return "NW";
            else if (deg >= 146.25 && deg < 168.75) return "NNW";
            else if (deg >= 168.75 && deg < 191.25) return "N";
            else if (deg >= 191.25 && deg < 213.75) return "NNE";
            else if (deg >= 213.75 && deg < 236.25) return "NE";
            else if (deg >= 236.25 && deg < 258.75) return "ENE";
            else if (deg >= 258.75 && deg < 281.25) return "E";
            else if (deg >= 281.25 && deg < 303.75) return "ESE";
            else if (deg >= 303.75 && deg < 326.25) return "SE";
            else return "SSE";
        }
    }
}