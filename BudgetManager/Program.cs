using BudgetManager.Models;
using BudgetManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

var policyUsers = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();


// Add services to the container.
builder.Services.AddControllersWithViews(options => {
    options.Filters.Add(new AuthorizeFilter(policyUsers));

});
builder.Services.AddTransient<IRepositoryAccountTypes, AccountTypeRepository>();
builder.Services.AddTransient<IRepositoryUsers, UsersRepository>();
builder.Services.AddTransient<IRepositoryAccounts, AccountRepository>();
builder.Services.AddTransient<IRepositoryCategories, CategoriesRepository>();
builder.Services.AddTransient<IRepositoryTransactions, TransactionsRepository>();
builder.Services.AddTransient<IReportService, ReportService>();
builder.Services.AddTransient<SignInManager<User>>();
builder.Services.AddTransient<IUserStore<User>, UserStore>();
builder.Services.AddIdentityCore<User>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
});
builder.Services.AddAuthentication(options=> {
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignOutScheme = IdentityConstants.ApplicationScheme;
}).AddCookie(IdentityConstants.ApplicationScheme, options => {
    options.LoginPath = "/Users/Login/";
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddMvc().AddViewOptions(options =>
{
    options.HtmlHelperOptions.ClientValidationEnabled = true;
});


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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Transactions}/{action=Index}/{id?}");

app.Run();
