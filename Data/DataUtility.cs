using CrucibleBlog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using CrucibleBlog.Data;

namespace CrucibleBlog.Data
{
	public static class DataUtility
	{
		private const string? _adminRole = "Admin";
		private const string? _moderatorRole = "Moderator";
		public static string GetConnectionString(IConfiguration configuration)
		{
			string? connectionString = configuration.GetConnectionString("DefaultConnection");
			string? databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

			return string.IsNullOrEmpty(databaseUrl) ? connectionString! : BuildConnectionString(databaseUrl);

		}
		private static string BuildConnectionString(string databaseUrl)
		{
			var databaseUri = new Uri(databaseUrl);
			var userInfo = databaseUri.UserInfo.Split(':');
			var builder = new NpgsqlConnectionStringBuilder()
			{
				Host = databaseUri.Host,
				Port = databaseUri.Port,
				Username = userInfo[0],
				Password = userInfo[1],
				Database = databaseUri.LocalPath.TrimStart('/'),
				SslMode = SslMode.Require,
				TrustServerCertificate = true
			};
			return builder.ToString();
		}

		public static async Task ManageDataAsync(IServiceProvider svcProvider)
		{

			// Obtaining the necessary services based on the IServiceProvider parameter
			var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();
			var userManagerSvc = svcProvider.GetRequiredService<UserManager<BlogUser>>();
			var roleManagerSvc = svcProvider.GetRequiredService<RoleManager<IdentityRole>>();
			var configurationSvc = svcProvider.GetRequiredService<IConfiguration>();

			//Align the database by checking Migrations
			await dbContextSvc.Database.MigrateAsync();



			//Seed Demo User(s)
			await SeedDemoUsersAsync(userManagerSvc, configurationSvc);

		}


		private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
		{
			if (!await roleManager.RoleExistsAsync(_adminRole!))
			{
				await roleManager.CreateAsync(new IdentityRole(_adminRole!));
			}

			if (!await roleManager.RoleExistsAsync(_moderatorRole!))
			{
				await roleManager.CreateAsync(new IdentityRole(_moderatorRole!));
			}
		}


		// Demo Users Seed Methods
		private static async Task SeedDemoUsersAsync(UserManager<BlogUser> userManagerSvc, IConfiguration configuration)
		{

			string? adminEmail = configuration["AdminLoginEmail"] ?? Environment.GetEnvironmentVariable("AdminLoginEmail");
			string? adminPassword = configuration["AdminLoginPassword"] ?? Environment.GetEnvironmentVariable("AdminLoginPassword");

			string? moderatorEmail = configuration["ModeratorLoginEmail"] ?? Environment.GetEnvironmentVariable("ModeratorLoginEmail");
			string? moderatorPassword = configuration["ModeratorLoginPassword"] ?? Environment.GetEnvironmentVariable("ModeratorLoginPassword");

			try
			{
				BlogUser? adminUser = new BlogUser()
				{
					UserName = adminEmail,
					Email = adminEmail,
					FirstName = "Bryant",
					LastName = "Gates",
					EmailConfirmed = true
				};

				/*BlogUser modUser = new BlogUser()
				{
					UserName = moderatorEmail,
					Email = moderatorEmail,
					FirstName = "Dylan",
					LastName = "Taylor",
					EmailConfirmed = true
				};*/

				BlogUser? user = await userManagerSvc.FindByEmailAsync(adminUser.Email!);

				if (user == null)
				{
					// TODO: Use Enviroment Variable
					await userManagerSvc.CreateAsync(adminUser, adminPassword!);
					await userManagerSvc.AddToRoleAsync(adminUser, _adminRole!);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("*************  ERROR  *************");
				Console.WriteLine("Error Seeding Admin Login User.");
				Console.WriteLine(ex.Message);
				Console.WriteLine("***********************************");
				throw;
			}
		}

	}
}
