using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.TraceExtensions;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MsTeamsBot.Flows;
using MsTeamsBot.Middlewares;
using MsTeamsBot.State;

namespace MsTeamsBot
{
    public class Startup : IStartup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _env;
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            // Setup flows
            var dialogSet = new DialogSet();
            services.AddSingleton(dialogSet);
            
            var flows = new List<IFlow>();
            flows.Add(new HelpFlow());
            flows.Add(new LoginFlow(_configuration));
            flows.Add(new LogoutFlow());

            foreach (var flow in flows)
                flow.AddDialogs(dialogSet);

            services.AddSingleton<ICollection<IFlow>>(flows.AsReadOnly());
            
            // Setup bot
            services.AddBot<Bots.MsTeamsBot>(options =>
            {
                options.CredentialProvider = new ConfigurationCredentialProvider(_configuration);
                
                IStorage dataStore = new MemoryStorage();    
                
                options.Middleware.Add(new CatchExceptionMiddleware<Exception>(async (context, exception) =>
                {
                    Console.WriteLine($"Exception:\n{exception}");
                    
                    await context.TraceActivity("EchoBot Exception", exception);
                    await context.SendActivity(new Activity
                    {
                        Type = ActivityTypes.Message,
                        Text = "Sorry, it looks like something went wrong!\n" +
                               "\n" +
                               "```\n" +
                               $"{exception}\n" +
                               $"```"
                    });
                }));

                options.Middleware.Add(new ConversationState<Dictionary<string, object>>(dataStore));
                options.Middleware.Add(new UserState<UserState>(dataStore));
                options.Middleware.Add(new AuthenticationMiddleware(dialogSet));
            });

            return services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseDefaultFiles()
                .UseStaticFiles()
                .UseMvc()
                .UseBotFramework(bot =>
                {
                    bot.BasePath = _configuration["BotBasePath"];
                    bot.MessagesPath = _configuration["BotMessagesPath"];
                });
        }
    }
}
