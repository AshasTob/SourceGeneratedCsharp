using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.OpenApi.Validations.Rules;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace poc.aot.parsing
{
    public class WeatherForecast: IValidatableObject
    {
        [Required]
        public int TemperatureC { get; set; }

        public string Summary { get; set; }

        //Built in compile time
        public Generated.MetaDataCompileTime MetadataCompileTime { get; set; }

        //Built in runtime
        //TODO:: was not able to implement this approach
        public MetadataRunTime MetadataRunTime { get; set; }

        //Built with JObject
        /// <summary>
        /// This is a dynamic object
        /// </summary>
        /// <example>{ "quantity":13, "age":25 }</example>
        //TODO:: i was not able to implement
        public object MetaDataPureJson { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var jsonValidationErrors = MetadataJsonSchema.Schema.Validate(MetaDataPureJson.ToString());

            foreach (var schemaValidationError in jsonValidationErrors)
            {
                yield return new ValidationResult(schemaValidationError.ToString());
            }

        }
    }

    public static class MetadataJsonSchema
    {
        public static JsonSchema Schema { get; set; }

        public static async Task Init(string fileLocation)
        {
            Schema = await JsonSchema.FromFileAsync(fileLocation);
        }
    }

    public partial class MetadataRunTime
    {

    }
}
