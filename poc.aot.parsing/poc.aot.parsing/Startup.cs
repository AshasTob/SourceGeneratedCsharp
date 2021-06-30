using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;
using poc.njsonschema.gen;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace poc.aot.parsing
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
           // GenerateAdditionalModels();
            MetadataJsonSchema.Init("json_schema.json").GetAwaiter().GetResult();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "poc.aot.parsing", Version = "v1" });
                c.DocumentFilter<LocationModelDocumentFilter>();
                var filePath = Path.Combine(System.AppContext.BaseDirectory, "poc.aot.parsing.xml");
                c.IncludeXmlComments(filePath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "poc.aot.parsing v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        public class LocationModelDocumentFilter : IDocumentFilter
        {
            public void Apply(OpenApiDocument openapiDoc, DocumentFilterContext context)
            {
                var schema = JsonSchema.FromFileAsync("json_schema.json").GetAwaiter().GetResult();
                context.SchemaRepository.AddDefinition("Metadata", new OpenApiSchema());
                
                //context.SchemaGenerator.GenerateSchema(schema.ActualSchema.GetType(), context.SchemaRepository);
            }
        }

        private void GenerateAdditionalModels()
        {
            var personalDataGenerator = new MetaDataRuntimeGenerator();
            var code = personalDataGenerator.GenerateSharpCode().GetAwaiter().GetResult();
            personalDataGenerator.CompileAssembly(code);
        }
    }
}
