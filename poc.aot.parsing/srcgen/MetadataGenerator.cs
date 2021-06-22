using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Schema;

namespace poc.aot.srcgen
{
    [Generator]
    public class MetadataGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for this one
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // read file into a string and parse JsonSchema from the string
            //var schema1 = Json.Parse(File.ReadAllText());
            var jsonString = File.ReadAllText(@"C:\Projects\SourceGeneratedCsharp\poc.aot.parsing\srcgen\schema.json");

            var schema = JsonSchema.Parse(jsonString);

            // begin creating the source we'll inject into the users compilation
            var sourceBuilder = new StringBuilder(@"using System;

namespace Generated
{
    public class MetaData
    {
        public string Description { get; set; }

        public DateTime LocalTime { get; set; }
");
            //sourceBuilder.AppendLine("/*" + jsonString.ToString() + "*/");
            foreach (var item in schema.Properties)
            {
                sourceBuilder.AppendLine($"public string {item.Key.ToUpperInvariant()}" + "{ get; set; }");
            }
            sourceBuilder.Append(@"
        }
}
");

            // inject the created source into the users compilation
            context.AddSource("GeneratedMetadata", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }
    }
}
