using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TelegramBot;

class Program
{
    static void Main(string[] args)
    {
        // Create a host with DI and Configuration setup
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                // Load configuration from appsettings.json
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<TelegrammApp>();
                services.AddSingleton<ITelegrammAction, SendSticker>();
                services.AddSingleton<ITelegrammAction, ImageConverter>();
                services.AddSingleton<ITelegrammAction, EnhanceImage>();
                services.AddSingleton<ITelegrammAction, FindMusic>();
                services.AddSingleton<ITelegrammAction, FindVidioInYoutube>();
            })
            .Build();


        var telegrammApp = host.Services.GetRequiredService<TelegrammApp>();
        telegrammApp.Start();
        Console.ReadLine();
    }
}