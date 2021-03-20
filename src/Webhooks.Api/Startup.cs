using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Webhooks.Api.Swagger;
using Webhooks.Application.Validations;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Webhooks.Application.Extensions;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using Webhooks.Api.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Serilog;
using Webhooks.Api.Extensions;

namespace Webhooks.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(Configuration)
            .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region Logging
            services.AddLogging(c => c.AddSerilog());
            #endregion

            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                    {
                        options.SerializerSettings.ContractResolver =
                            new CamelCasePropertyNamesContractResolver();
                        options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    })
                .AddFluentValidation(options =>
                    {
                        options.RegisterValidatorsFromAssemblyContaining<SubscriptionAddModelValidator>();
                        options.ImplicitlyValidateChildProperties = true;
                    })
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var validations = new List<ValidationModel>();
                        foreach (var error in context.ModelState)
                        {
                            validations.AddRange(error.Value.Errors.Select(e => new ValidationModel(error.Key, e.ErrorMessage)));
                        }
                        return new BadRequestObjectResult(validations);
                    };
                });

            #region Swagger and versioning services

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
            });

            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
            });

            services.AddSwaggerGen(options =>
            {
                options.CustomSchemaIds(type => type.FullName);
                options.EnableAnnotations();
                // resolve the IApiVersionDescriptionProvider service
                // note: that we have to build a temporary service provider here because one has not been created yet
                using (var serviceProvider = services.BuildServiceProvider())
                {
                    var provider = serviceProvider.GetRequiredService<IApiVersionDescriptionProvider>();

                    // add a swagger document for each discovered API version
                    // note: you might choose to skip or document deprecated API versions differently
                    String assemblyDescription = typeof(Startup).Assembly.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerDoc(description.GroupName, new Microsoft.OpenApi.Models.OpenApiInfo()
                        {
                            Title = $"{typeof(Startup).Assembly.GetCustomAttribute<AssemblyProductAttribute>().Product} {description.ApiVersion}",
                            Version = description.ApiVersion.ToString(),
                            Description = description.IsDeprecated ? $"{assemblyDescription} - DEPRECATED" : $"{assemblyDescription}"
                            //$"<p><img src='/api/swagger-ui/healthcheck-30x30.png' valign='middle'/> <a href='/api/health' target='_blank'>Healthchecks</a></p>" +
                            //$"<p><img src='/api/swagger-ui/build-30x30.png' valign='middle'/> Build #{typeof(Startup).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}</p>"
                        });
                    }
                }

                // integrate xml comments
                var currentAssembly = Assembly.GetExecutingAssembly();
                var xmlDocs = currentAssembly.GetReferencedAssemblies()
                .Union(new AssemblyName[] { currentAssembly.GetName() })
                .Select(a => Path.Combine(Path.GetDirectoryName(currentAssembly.Location), $"{a.Name}.xml"))
                .Where(f => File.Exists(f)).ToArray();

                Array.ForEach(xmlDocs, (d) =>
                {
                    options.IncludeXmlComments(d);
                });

                options.DocumentFilter<RemoveDefaultApiVersionRouteDocumentFilter>();
                options.ParameterFilter<EventNameParameterFilter>();
                options.OperationFilter<RemoveQueryApiVersionParamOperationFilter>();
                options.DocumentFilter<EventPublishDocumentFilter>();
            });

            #endregion

            services.AddApplicationServices(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseExceptionHandling(env);
            loggerFactory.AddSerilog();

            app.UseStaticFiles();
            app.UseApplicationServices();

            #region Configure Swagger
            app.UseSwagger(c =>
            {
                    // camelCase all parameters
                    c.PreSerializeFilters.Add((document, request) =>
                {
                    var parameters = document.Paths.SelectMany(p => p.Value.Operations.Values).SelectMany(v => v.Parameters);
                    if (parameters != null && parameters.Any())
                        foreach (var p in parameters) p.Name = Char.ToLowerInvariant(p.Name[0]) + p.Name.Substring(1);
                });
            });

            var provider = app.ApplicationServices.GetService<IApiVersionDescriptionProvider>();
            app.UseSwaggerUI(
                options =>
                {
                    options.InjectStylesheet(@"/swagger-ui/style.css");
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    }
                });
            #endregion

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });           
        }
    }
}
