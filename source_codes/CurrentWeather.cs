using System.Collections.Generic;

namespace API
{
    public class CurrentWeather
    {
        public ACoord Coord { get; set; }
        public List<AWeather> Weather { get; set; }
        public AMain Main { get; set; }
        public AWind Wind { get; set; }
        public TheClouds Clouds { get; set; }
        public ASys Sys { get; set; }
        public double Timezone { get; set; }

        public class ACoord
        {
            public double Lon { get; set; }
            public double Lat { get; set; }
        }
        
        public class AWeather
        {
            public string Main { get; set; }
            public string Description { get; set; }
        }

        public class AMain
        {
            public double Temp { get; set; }
            public double Feels_like { get; set; }
            public double Pressure{ get; set; }
            public double Humidity{ get; set; }
        }

        public class AWind
        {
            public double Speed { get; set; }
            public double Deg { get; set; }
        }

        public class TheClouds
        {
            public double All { get; set; }
        }

        public class ASys
        {
            public int Sunrise { get; set; }
            public int Sunset { get; set; }
        }
    }
}