using Microsoft.EntityFrameworkCore;
using simple_todo_web_app.Common.Constants;
using simple_todo_web_app.Data;
using simple_todo_web_app.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
	// メール認証が必要
	options.SignIn.RequireConfirmedAccount = true;

	// パスワード規則
	options.Password.RequiredLength = PasswordConstants.RequiredLength;
	options.Password.RequireDigit = PasswordConstants.RequireDigit;
	options.Password.RequireLowercase = PasswordConstants.RequireLowercase;
	options.Password.RequireUppercase = PasswordConstants.RequireUppercase;
	options.Password.RequireNonAlphanumeric = PasswordConstants.RequireNonAlphanumeric;

}).AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options => 
{
	options.LoginPath = "/account/login";
	// 認証の有効期限を14日に設定
	options.ExpireTimeSpan = TimeSpan.FromDays(14);
	// アクセスの度に有効期限を更新する
	options.SlidingExpiration = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
}
else
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}")
	.WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
