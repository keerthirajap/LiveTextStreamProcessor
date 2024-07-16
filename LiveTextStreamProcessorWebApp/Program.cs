namespace LiveTextStreamProcessorWebApp
{
    using LiveTextStreamProcessorService.Concrete;
    using LiveTextStreamProcessorService.Interface;
    using LiveTextStreamProcessorWebApp.ActionFilter;
    using LiveTextStreamProcessorWebApp.Cache;
    using LiveTextStreamProcessorWebApp.Hubs;
    using LiveTextStreamProcessorWebApp.Services;
    using NLog;
    using NLog.Web;

    public class Program
    {
        public static void Main(string[] args)
        {
            // Configure NLog
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.
                builder.Services.AddControllersWithViews(options =>
                {
                    options.Filters.Add(typeof(LogPageRequestAttribute)); // Add globally
                    options.Filters.Add(typeof(LogExceptionAttribute)); // Add globally
                });

                builder.Services.AddSignalR();
                builder.Services.AddSingleton<StreamHub>();
                builder.Services.AddSingleton<IWordStreamReaderService, WordStreamReaderService>();
                builder.Services.AddSingleton<StreamProcessingService>();

                builder.Host.UseNLog();

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Home/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();

                app.UseRouting();

                app.UseAuthorization();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}");

                    endpoints.MapHub<StreamHub>("/streamHub");
                });

                // Start the stream processing service
                var streamProcessingService = app.Services.GetRequiredService<StreamProcessingService>();
                streamProcessingService.StartProcessing();

                app.Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }
    }
}