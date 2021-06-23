using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
//#if DEBUG
//            if (!Debugger.IsAttached)
//            {
//                Debugger.Launch();
//            }
//#endif
        }

        public void Execute(GeneratorExecutionContext context)
        {
            //context.AddSource(identifier, generatedCode);
            // read file into a string and parse JsonSchema from the string
            //var schema1 = Json.Parse(File.ReadAllText());
            var schemaFile = context.AdditionalFiles
                .First(f => f.Path.EndsWith("aot_schema.json"));

            var schema = JsonSchema.Parse(schemaFile.GetText(context.CancellationToken).ToString());

            // begin creating the source we'll inject into the users compilation
            var sourceBuilder = new StringBuilder(@"using System;
using System.ComponentModel.DataAnnotations;

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
                if (item.Value.Required != null && item.Value.Required.Value)
                {
                    sourceBuilder.AppendLine("[Required]");
                }
                sourceBuilder.AppendLine($"public {SchemaTypeToSharpType(item.Value.Type.ToString())} {item.Key.ToUpperInvariant()}" + "{ get; set; }");
            }

            sourceBuilder.Append(@"
        }
}
");

            // inject the created source into the users compilation
            context.AddSource("GeneratedMetadata", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }

        private string SchemaTypeToSharpType(string type)
        {
            switch (type.ToLower())
            {
                case "integer":
                    return "int";
                case "string":
                    return "string";
                default:
                    return "string";
                    throw new InvalidEnumArgumentException("Uknonwn type in the schema");
            }
        }
    }
}
