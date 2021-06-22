using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace poc.aot.srcgen
{
    [Generator]
    public class MetadataGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for this one
            Console.WriteLine("Initialized");
        }

        public void Execute(GeneratorExecutionContext context)
        {
            Console.WriteLine("Executed");
            // begin creating the source we'll inject into the users compilation
            var sourceBuilder = new StringBuilder(@"
using System;
namespace Generated
{
    public class MetaData
    {
        public string Description { get; set; }
    }
}
");

            // inject the created source into the users compilation
            context.AddSource("MetadataGenerator", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }
    }
}
