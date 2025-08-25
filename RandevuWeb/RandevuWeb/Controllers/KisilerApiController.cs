using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RandevuWeb.Data.Models;
using RandevuWeb.Models;

namespace RandevuWeb.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KisilerApiController : ControllerBase
{
    private readonly RandevuContext _context;

    public KisilerApiController(RandevuContext context)
    {
        _context = context;
    }

    // --- Kayıt Olma İşlemi ---
    [HttpPost("kayitol")]
    public async Task<IActionResult> KayitOl([FromBody] Kisiler yeniKisi)
    {
        if (!ModelState.IsValid)
        {
            // ModelState hatalarını doğrudan döndür
            var errors = ModelState.Values
                                   .SelectMany(v => v.Errors)
                                   .Select(e => e.ErrorMessage);
            return BadRequest(new { success = false, message = "Geçersiz istek. Lütfen hataları kontrol edin.", errors = errors });
        }

        // Id alanını Guid ile benzersiz bir string olarak oluşturuyoruz
        yeniKisi.Id = Guid.NewGuid().ToString();

        // Aynı e-posta veya telefon numarasına sahip bir kullanıcının olup olmadığını kontrol et
        var mevcutKisi = await _context.Kisilers
                                       .FirstOrDefaultAsync(k => k.Email == yeniKisi.Email || k.PhoneNumber == yeniKisi.PhoneNumber);
        if (mevcutKisi != null)
        {
            return BadRequest(new { success = false, message = "Bu e-posta veya telefon numarası zaten kayıtlı." });
        }

        // Oluşturulma tarihini ayarla
        yeniKisi.CreatedDate = DateTime.Now;

        _context.Kisilers.Add(yeniKisi);

        var hastaRolu = new KisilerinRolleri
        {
            UserId = yeniKisi.Id, // Kişinin yeni oluşturulan Id'sini kullan
            RoleId = "609202f7-bea9-40c2-ad74-024887f26980" // Hasta rolünün Id'si
        };

        _context.KisilerinRolleris.Add(hastaRolu);

        await _context.SaveChangesAsync();

        return Ok(new { success = true, message = "Kayıt işlemi başarıyla tamamlandı." });

    }

    // --- Giriş Yapma İşlemi ---
    public class LoginModel
    {
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }

    [HttpPost("girisyap")]
    public async Task<IActionResult> GirisYap([FromBody] LoginModel loginModel)
    {
        if (string.IsNullOrEmpty(loginModel.Email) && string.IsNullOrEmpty(loginModel.PhoneNumber))
        {
            return BadRequest(new { success = false, message = "E-posta veya telefon numarası girilmesi zorunludur." });
        }

        var kisi = await _context.Kisilers.FirstOrDefaultAsync(k => k.Email == loginModel.Email && k.PhoneNumber == loginModel.PhoneNumber);

        if (kisi == null)
        {
            return BadRequest(new { success = false, message = "Kullanıcı bulunamadı." });
        }

        // KisilerinRolleri tablosundan kişinin rolünü kontrol et
        // Sorguda UserId ve RoleId alanlarını kullanıyoruz
        var kisiyeAitHastaRolu = await _context.KisilerinRolleris
                                                .Where(kr => kr.UserId == kisi.Id && kr.RoleId == "609202f7-bea9-40c2-ad74-024887f26980")
                                                .FirstOrDefaultAsync();

        if (kisiyeAitHastaRolu == null)
        {
            return BadRequest(new { success = false, message = "Giriş yapmaya çalıştığınız hesap bir hasta hesabı değildir." });
        }

        return Ok(new { 
            success = true, 
            message = "Giriş başarılı.", 
            redirectUrl = "/Patient/PatientDashboard",
            user = new { 
                id = kisi.Id,
                name = kisi.Name, 
                surname = kisi.Surname, 
                email = kisi.Email, 
                phoneNumber = kisi.PhoneNumber 
            } 
});
    }
    
    [HttpPost("staffgirisyap")]
    public async Task<IActionResult> StaffGirisYap([FromBody] LoginModel loginModel)
    {
        // E-posta veya telefon numarasının boş olup olmadığını kontrol et
        if (string.IsNullOrEmpty(loginModel.Email) && string.IsNullOrEmpty(loginModel.PhoneNumber))
        {
            return BadRequest(new { success = false, message = "E-posta veya telefon numarası girilmesi zorunludur." });
        }

        // Kullanıcıyı e-posta veya telefon numarası ile bul
        var kisi = await _context.Kisilers.FirstOrDefaultAsync(k => k.Email == loginModel.Email && k.PhoneNumber == loginModel.PhoneNumber);

        if (kisi == null)
        {
            return BadRequest(new { success = false, message = "Kullanıcı bulunamadı." });
        }

        // Kullanıcının rolünü veritabanından getir
        var kullaniciRolu = await _context.KisilerinRolleris
                                            .Where(kr => kr.UserId == kisi.Id && 
                                                         (kr.RoleId == "532a11cc-2031-472b-b3f0-e8ad471b11bf" || kr.RoleId == "d017e6f0-9ab8-43b3-8e9e-8336ff4d29a6"))
                                            .FirstOrDefaultAsync();

        if (kullaniciRolu == null)
        {
            return Unauthorized(new { success = false, message = "Yetkisiz erişim. Bu hesap bir personel hesabına ait değildir." });
        }

        // Rol ID'sine göre yönlendirme URL'ini belirle
        string redirectUrl;
        if (kullaniciRolu.RoleId == "532a11cc-2031-472b-b3f0-e8ad471b11bf")
        {
            // Doktor rolü için yönlendirme
            redirectUrl = "/Doctor/Dashboard"; // Doktor paneli URL'i
        }
        else if (kullaniciRolu.RoleId == "d017e6f0-9ab8-43b3-8e9e-8336ff4d29a6")
        {
            // Asistan rolü için yönlendirme
            redirectUrl = "/Asistan/Dashboard"; // Asistan paneli URL'i
        }
        else
        {
            // Tanımlanmamış bir rol ise hata döndür
            return Unauthorized(new { success = false, message = "Yetkisiz erişim. Geçerli bir rol bulunamadı." });
        }

        return Ok(new { success = true, message = "Giriş başarılı.", redirectUrl = redirectUrl });
    }
}