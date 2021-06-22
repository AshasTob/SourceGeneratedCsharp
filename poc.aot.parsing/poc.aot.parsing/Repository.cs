using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace poc.aot.parsing
{
    public static class Repository
    {
        private static List<WeatherForecast> _forecasts = new List<WeatherForecast>();

        public static void Add(WeatherForecast forecast)
        {
            _forecasts.Add(forecast);
        }

        public static List<WeatherForecast> List()
        {
            return _forecasts;
        }
    }
}
