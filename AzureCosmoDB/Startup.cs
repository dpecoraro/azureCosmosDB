using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureCosmoDB.BusinessLogic;
using AzureCosmoDB.Model;
using AzureCosmoDB.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AzureCosmoDB
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public static string databaseName { get; set; }
        public static string containerName { get; set; }
        public static string account { get; set; }
        public static string key { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            var settings = Configuration.GetSection("AppSettings").Get<AppSettings>();

            //services.AddSingleton(typeof(ICosmosDBService<>), InitializeCosmosClientInstanceAsync(settings).GetAwaiter().GetResult());

            services.AddSingleton<ICosmosDBService>(InitializeCosmosClientInstanceAsync(settings).GetAwaiter().GetResult());

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
        }

        private static async Task<CosmosDBService> InitializeCosmosClientInstanceAsync(AppSettings settings)
        {     
            
            databaseName = settings.CosmosDb.DatabaseName;
            containerName = settings.CosmosDb.ContainerName;
            account = settings.CosmosDb.Account;
            key = settings.CosmosDb.Key;

            
            CosmosClient client = new CosmosClient(account, key);

            CosmosDBService cosmosDBService = new CosmosDBService(client, databaseName, containerName);
            DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);

            await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

            return cosmosDBService;
        }
    }
}
