using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PFM.Core.Models.Products;
using PFM.Core.Interfaces.Products;
using PFM.Core.Interfaces.Balance;
using PFM.Core.Models.Balance;

namespace PFM.Core
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var productsEndpoint = Configuration["PFM:Endpoints:Products"];
            services.AddSingleton<IProductManager>(new ProductManager(productsEndpoint));

            var balanceEndpoint = Configuration["PFM:Endpoints:Balance"];
            services.AddSingleton<IBalanceManager>(new BalanceManager(balanceEndpoint));
            services.AddSingleton<IPredictionManager>(new PredictionManager(balanceEndpoint));
            services.AddSingleton<ISummaryManager>(new SummaryManager(balanceEndpoint));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
