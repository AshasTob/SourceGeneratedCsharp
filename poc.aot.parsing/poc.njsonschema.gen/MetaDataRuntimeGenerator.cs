using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;

namespace poc.njsonschema.gen
{
    public class MetaDataRuntimeGenerator
    {
        public async Task<string> GenerateSharpCode()
        {
            var schema = await JsonSchema.FromFileAsync("njsonschema_schema.json");
            var settings = new CSharpGeneratorSettings();
            var generator = new CSharpGenerator(schema, settings);

            return generator.GenerateFile();
        }

        public Assembly CompileAssembly(string remoteCode)
        {
            if (remoteCode == null)
                throw new ArgumentNullException(nameof(remoteCode));

            var assemblyLocation = typeof(Enumerable).GetTypeInfo().Assembly.Location;
            var coreDir = Directory.GetParent(assemblyLocation);

            CSharpCompilation compilation = CreateCompilerContext(remoteCode, coreDir);

            return CompileAndLoad(compilation);
        }

        private static Assembly CompileAndLoad(CSharpCompilation compilation)
        {
            using var ms = new MemoryStream();
            var emitResult = compilation.Emit(ms);

            if (!emitResult.Success)
            {
                // handle, log errors etc
                Debug.WriteLine("Compilation failed!");
                Debug.WriteLine(emitResult.Diagnostics);
                throw new InvalidOperationException("Could not compile source");
            }

            ms.Seek(0, SeekOrigin.Begin);
            var assembly = Assembly.Load(ms.ToArray());
            return assembly;
        }

        public static CSharpCompilation CreateCompilerContext(string remoteCode, DirectoryInfo coreDir)
        {
            return CSharpCompilation.Create("poc.Models", new[] { CSharpSyntaxTree.ParseText(remoteCode) },
            //var comp = CSharpCompilation.Create("StoreApi.Contracts.Commons", new[] { CSharpSyntaxTree.ParseText(remoteCode) },
            new[] {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(Assembly.GetExecutingAssembly().Location),
                    //MetadataReference.CreateFromFile(typeof(RemoteControllerFeatureProvider).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(System.ComponentModel.DataAnnotations.RequiredAttribute).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Newtonsoft.Json.JsonConverter).Assembly.Location),
                    MetadataReference.CreateFromFile(coreDir.FullName + Path.DirectorySeparatorChar + "System.Runtime.dll"),
                    MetadataReference.CreateFromFile(coreDir.FullName + Path.DirectorySeparatorChar + "netstandard.dll"),
                },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        }
    }
}
