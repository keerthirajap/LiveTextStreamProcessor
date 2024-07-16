namespace LiveTextStreamProcessorWebApp
{
    using Hangfire;
    using Hangfire.MemoryStorage;
    using LiveTextStreamProcessorWebApp.Hubs;
    using LiveTextStreamProcessorWebApp.Services;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSignalR();
            //builder.Services.AddHangfire(config => config.UseMemoryStorage());
            builder.Services.AddSingleton<StreamHub>();
            builder.Services.AddSingleton<StreamProcessingService>();

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

            //app.UseHangfireDashboard();
            //app.UseHangfireServer();

            // Resolve the StreamProcessingService instance
            //var serviceProvider = app.Services;
            //var streamProcessingService = serviceProvider.GetRequiredService<StreamProcessingService>();

            //// Schedule the recurring job to run immediately at startup and then every 5 seconds
            //RecurringJob.AddOrUpdate(() => streamProcessingService.ProcessStream(), Cron.Minutely);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapHub<StreamHub>("/streamHub"); // Make sure this path matches the URL used in JavaScript
            });

            var streamProcessingService = app.Services.GetRequiredService<StreamProcessingService>();
            streamProcessingService.StartProcessing(); // Start the stream processing service


            app.Run();
        }
    }
}