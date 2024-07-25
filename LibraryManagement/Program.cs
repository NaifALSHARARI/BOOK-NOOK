using LibraryManagement.Models;
using LibraryManagement.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using LibraryManagement.Areas.Identity.Data;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();


builder.Services.AddScoped<IBookRepository, BookRepository>();


builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();


builder.Services.AddDbContext<LibraryDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("LibraryDbContextConnection"));
});


builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<LibraryDbContext>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();


await CreateRolesAndUser(app.Services);

app.Run();

async Task CreateRolesAndUser(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    string[] roleNames = { "Admin", "Editor" };
    IdentityResult roleResult;

    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    var userEmail = "c@c.com";
    var userPassword = "Aa123456@";
    var user = await userManager.FindByEmailAsync(userEmail);

    if (user == null)
    {
        user = new IdentityUser
        {
            UserName = userEmail,
            Email = userEmail
        };
        await userManager.CreateAsync(user, userPassword);
    }

    await userManager.AddToRoleAsync(user, "Admin");
}
