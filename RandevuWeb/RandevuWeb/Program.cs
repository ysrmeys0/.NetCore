using RandevuWeb.Data.Models;
using Microsoft.EntityFrameworkCore;
using RandevuWeb.Data; // RoleConstants için ekledik

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<RandevuContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RandevuDB")));

var app = builder.Build();

// ----- İLK ADMİN KULLANICISI OLUŞTURMA MANTIĞI BAŞLANGICI -----
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<RandevuContext>();
    
    // Veritabanının var olduğundan emin ol ve varsa oluştur
    // Bu, migrations kullanmadığınız için gerekli
    context.Database.EnsureCreated();

    context.SaveChanges();
    
    // Admin hesabı yoksa oluştur
    if (!context.Kisilers.Any(k => k.Email == "admin@hospital.com"))
    {
        var admin = new Kisiler
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Sistem",
            Surname = "Yöneticisi",
            Email = "admin@hospital.com",
            PhoneNumber = "00000000000",
            CreatedDate = DateTime.Now,
            Gender = null
        };
        context.Kisilers.Add(admin);
        
        // Admin kullanıcısına admin rolünü ata
        var adminRolu = new KisilerinRolleri
        {
            UserId = admin.Id,
            RoleId = "655f965d-dc73-4429-aa34-ec9cce5be6df"
        };
        context.KisilerinRolleris.Add(adminRolu);
        context.SaveChanges();
    }
}

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
