using Microsoft.EntityFrameworkCore;
using OauthServer.Models;
using System;

namespace OauthServer.Services
{
    public class DBMigrate
    {
        public static void Migration(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<AuthDbContext>();
                try
                {
                    Console.WriteLine($"---> Migration Started");
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"---> Error Occured During Migration {ex.Message}");
                }
            }
        }
    }
}
