using System;
using DotnetApiComplet.constants;
using DotnetApiComplet.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace DotnetApiComplet.Data;

public class DbSender
{
    public static async Task SeedDataAsync(IApplicationBuilder app)
    {//using will take of disposal of the "scope"
        using var scope = app.ApplicationServices.CreateScope();
        // Applying the pending migration (Not reccomended for prduction)
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if ((await dbContext.Database.GetPendingMigrationsAsync()).Count() > 0)
        {
            await dbContext.Database.MigrateAsync();
        }
        var logger = scope.ServiceProvider.GetRequiredService<Logger<DbSender>>();
        try
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            //check if any user exist
            if (!userManager.Users.Any())
            {
                var user = new User
                {
                    Name = "Admin",
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
              //create an admin role if does not exits
               if (!await roleManager.RoleExistsAsync(Roles.Admin))
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole(Roles.Admin));

                    if (roleResult.Succeeded == false)
                    {
                        var roleErrors = roleResult.Errors.Select(e => e.Description);
                        logger.LogError($"Failed to create admin role. Errors : {string.Join(",", roleErrors)}");
                        return;
                    }
                    logger.LogInformation("Admin role is created");
                }
                 // Attempt to create admin user
                var createUserResult = await userManager
                      .CreateAsync(user: user, password: "Admin@123");
                   // Validate user creation
                if (createUserResult.Succeeded == false)
                {
                    var errors = createUserResult.Errors.Select(e => e.Description);
                    logger.LogError(
                        $"Failed to create admin user. Errors: {string.Join(", ", errors)}"
                    );
                    return;
                }

                // adding role to user
                var addUserToRoleResult = await userManager
                                .AddToRoleAsync(user: user, role: Roles.Admin);

                if (addUserToRoleResult.Succeeded == false)
                {
                    var errors = addUserToRoleResult.Errors.Select(e => e.Description);
                    logger.LogError($"Failed to add admin role to user. Errors : {string.Join(",", errors)}");
                }
                logger.LogInformation("Admin user is created");
            }   
            
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
        }
    }
}
