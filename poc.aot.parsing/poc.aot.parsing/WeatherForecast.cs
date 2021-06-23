using System;
using System.ComponentModel.DataAnnotations;

namespace poc.aot.parsing
{
    public class WeatherForecast
    {
        [Required]
        public int TemperatureC { get; set; }

        public string Summary { get; set; }

        public Generated.MetaData Metadata { get; set; }

        public PersonalData PersonalData { get; set; }
    }

    public partial class PersonalData
    {

    }
}
