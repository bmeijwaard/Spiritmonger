﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spiritmonger.Cmon.Settings;
using Spiritmonger.Core.Contracts.Services;
using Spiritmonger.Core.Services;
using Spiritmonger.Persistence;
using Spiritmonger.Persistence.Contracts;
using Spiritmonger.Persistence.Providers;
using System;

namespace Spiritmonger.Mapping.Modules
{
    public static class CoreModule
    {
        public static IServiceCollection Load(IServiceCollection services, IConfiguration config)
        {
            // load options from config
            services.AddOptions();
            services.Configure<ConnectionStrings>(opt => config.GetSection("ConnectionStrings").Bind(opt));

            // setup EF
            services.AddEntityFrameworkSqlServer()
                .AddDbContext<SqlContext>(options =>
                    options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            // register dependencies
            services.AddScoped<ISqlContext>(provider => provider.GetService<SqlContext>());
            services.AddScoped<IDapperProvider>(provider => new DapperProvider(config.GetConnectionString("DefaultConnection")));
            services.AddScoped<ISqlContextProvider, SqlContextProvider>();
            services.AddScoped(provider => new Func<SqlContext>(provider.GetService<SqlContext>));

            services.AddScoped<ICardService, CardService>();
            services.AddScoped<ICardNameService, CardNameService>();
            services.AddScoped<IMetaCardService, MetaCardService>();

            return services;
        }
    }
}
