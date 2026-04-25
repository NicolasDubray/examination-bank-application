using Bank.Application.Services;
using Bank.Domain.Interfaces;
using Bank.Infrastructure.Factories;
using Bank.Infrastructure.Repositories;
using Bank.Infrastructure.Seed;
using Bank.Presentation.Components;

namespace Bank.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddScoped<IAccountRuleFactory, AccountRuleFactory>();

            builder.Services.AddSingleton<ICustomerRepository, InMemoryCustomerRepository>();
            builder.Services.AddSingleton<IAccountRepository, InMemoryAccountRepository>();
            builder.Services.AddSingleton<ITransactionRepository, InMemoryTransactionRepository>();

            builder.Services.AddScoped<AccountRuleService>();
            builder.Services.AddScoped<AccountService>();
            builder.Services.AddScoped<CustomerService>();
            builder.Services.AddScoped<TransactionService>();

            var app = builder.Build();

            var customerRepo = app.Services.GetRequiredService<ICustomerRepository>();
            var accountRepo = app.Services.GetRequiredService<IAccountRepository>();
            var transactionRepo = app.Services.GetRequiredService<ITransactionRepository>();

            Seeder.SeedAll(customerRepo, accountRepo, transactionRepo);

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
            app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
