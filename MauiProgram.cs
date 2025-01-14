using Microsoft.Extensions.Logging;
using ExpenseTracker.Services;

namespace ExpenseTracker
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

            //registering User Services
            builder.Services.AddSingleton<IUserService, UserService>();
            //builder.Services.AddSingleton<ITodoService, TodoService>();
            builder.Services.AddSingleton<AuthenticationStateService>();
            builder.Services.AddSingleton<TransactionService>(); // Add the TransactionService
            builder.Services.AddScoped<TransactionService>();



#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
