using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Zhalobobot.Bot.Services;
using Zhalobobot.Common.Clients.Core;

namespace Zhalobobot.Bot
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private BotConfiguration BotConfig { get; }
        private Settings Settings { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            BotConfig = configuration.GetSection("BotConfiguration").Get<BotConfiguration>();
            Settings = configuration.GetSection("Settings").Get<Settings>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<ConfigureWebhook>();

            services.AddHttpClient("tgwebhook")
                    .AddTypedClient<ITelegramBotClient>(httpClient
                        => new TelegramBotClient(BotConfig.TelegramBotToken, httpClient));

            services.AddSingleton(Settings);
            services.AddSingleton<IPollService, PollService>();
            services.AddSingleton<IConversationService, ConversationService>();
            services.AddScoped<HandleUpdateService>();
            
            services.AddSingleton<IZhalobobotApiClient>(
                new ZhalobobotApiClient(Settings.ServerAddress));

            services.AddControllers()
                    .AddNewtonsoftJson();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "tgwebhook",
                    $"bot/{BotConfig.TelegramBotToken}",
                    new { controller = "Webhook", action = "Post" });
                endpoints.MapControllers();
            });
        }
    }
}
