/// I, Manh Truong Nguyen, student number 000893836, certify that this material is my
/// original work. No other person's work has been used without due
/// acknowledgement and I have not made my work available to anyone else.

namespace Assignment1.Data
{
    /// <summary>
    /// Provides startup seeding for Identity roles/users and sample Company data.
    /// </summary>
    public static class Seed
    {
        /// <summary>
        /// Ensures the required roles (Supervisor, Employee) exist and creates one user for each role.
        /// </summary>
        public static async Task SeedAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            static string GetRequired(IConfiguration cfg, string key) =>
                cfg[key] ?? throw new InvalidOperationException($"Missing configuration value: '{key}'.");

            string[] roleNames = { "Supervisor", "Employee" };
            foreach (var roleName in roleNames)
                if (!await roleManager.RoleExistsAsync(roleName))
                    await roleManager.CreateAsync(new IdentityRole(roleName));

            // Supervisor
            var supervisorEmail = GetRequired(configuration, "SeedUsers:Supervisor:Email");
            var supervisorPassword = GetRequired(configuration, "SeedUsers:Supervisor:Password");

            if (await userManager.FindByEmailAsync(supervisorEmail) is null)
            {
                var supervisor = new ApplicationUser
                {
                    UserName = supervisorEmail,
                    Email = supervisorEmail,
                    FirstName = "Super",
                    LastName = "Visor",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(supervisor, supervisorPassword);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(supervisor, "Supervisor");
            }

            // Employee
            var employeeEmail = GetRequired(configuration, "SeedUsers:Employee:Email");
            var employeePassword = GetRequired(configuration, "SeedUsers:Employee:Password");

            if (await userManager.FindByEmailAsync(employeeEmail) is null)
            {
                var employee = new ApplicationUser
                {
                    UserName = employeeEmail,
                    Email = employeeEmail,
                    FirstName = "Emp",
                    LastName = "Loyee",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(employee, employeePassword);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(employee, "Employee");
            }
        }

        /// <summary>
        /// Seeds sample Company records if they do not already exist.
        /// </summary>
        public static async Task SeedCompaniesAsync(ApplicationDbContext context)
        {
            await context.Database.MigrateAsync();

            if (!await context.Companies.AnyAsync(c => c.Name == "Company 1"))
            {
                context.Companies.Add(new Company
                {
                    Name = "Company 1",
                    YearsInBusiness = 15,
                    Website = "https://www.company-1.com",
                    Province = "ON"
                });
            }

            if (!await context.Companies.AnyAsync(c => c.Name == "Google"))
            {
                context.Companies.Add(new Company
                {
                    Name = "Google",
                    YearsInBusiness = 20,
                    Website = "https://www.google.com",
                    Province = "CA"
                });
            }

            if (!await context.Companies.AnyAsync(c => c.Name == "Company 2"))
            {
                context.Companies.Add(new Company
                {
                    Name = "Company 2",
                    YearsInBusiness = 22,
                    Website = "https://www.company-2.com"
                });
            }

            await context.SaveChangesAsync();
        }
    }
}
