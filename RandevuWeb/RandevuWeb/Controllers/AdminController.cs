using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RandevuWeb.Data;
using RandevuWeb.Data.Models;

namespace RandevuWeb.Controllers
{
    public class AdminController : Controller
    {
        private readonly RandevuContext _context;

        public AdminController(RandevuContext context)
        {
            _context = context;
        }

        // URL: /Admin/Login
        public IActionResult Login()
        {
            return View();
        }

        // URL: /Admin/Dashboard
        public IActionResult Dashboard()
        {
            // Bu sayfaya sadece giriş yapmış adminlerin erişmesi sağlanacak
            return View();
        }

        public class LoginModel
        {
            public string? Email { get; set; }
            public string? PhoneNumber { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> AdminGirisYap([FromBody] LoginModel loginModel)
        {
            if (string.IsNullOrEmpty(loginModel.Email) && string.IsNullOrEmpty(loginModel.PhoneNumber))
            {
                return BadRequest(new { success = false, message = "E-posta veya telefon numarası girilmesi zorunludur." });
            }

            var kisi = await _context.Kisilers.FirstOrDefaultAsync(k => k.Email == loginModel.Email || k.PhoneNumber == loginModel.PhoneNumber);

            if (kisi == null)
            {
                return Unauthorized(new { success = false, message = "Kullanıcı bulunamadı." });
            }

            var isAuthorized = await _context.KisilerinRolleris
                                             .AnyAsync(kr => kr.UserId == kisi.Id && kr.RoleId == "655f965d-dc73-4429-aa34-ec9cce5be6df");

            if (!isAuthorized)
            {
                return Unauthorized(new { success = false, message = "Yetkisiz erişim. Bu hesap bir yönetici veya asistana ait değildir." });
            }

            return Ok(new { success = true, message = "İdari giriş başarılı." });
        }

        // URL: /Admin/GetAllAppointments
        // Bu metot, admin panelindeki tüm randevuları döndürür.
        [HttpGet]
        public async Task<IActionResult> GetAllAppointments()
        {
            try
            {
                // Randevuları ilgili Hasta, Doktor ve Randevu Tipi verileriyle birlikte çekme
                var allAppointments = await _context.Randevulars
                    .Include(r => r.Patient)     // Hasta verilerini yükle
                    .Include(r => r.Doctor)      // Doktor verilerini yükle
                    .Include(r => r.AppointmentType) // Randevu Tipi verilerini yükle
                    .OrderByDescending(r => r.AppointmentStartDate) // En yeni randevuyu üste getir
                    .Select(r => new // Sadece gerekli verileri içeren yeni bir nesne oluşturma
                    {
                        id = r.Id,
                        patientName = r.Patient.Name + " " + r.Patient.Surname,
                        doctorName = r.Doctor.Name + " " + r.Doctor.Surname,
                        appointmentStartDate = r.AppointmentStartDate,
                        startHour = r.StartHour,
                        endHour = r.EndHour,
                        appointmentTypeName = r.AppointmentType.TypeName,
                        isCancelled = r.IsCancelled, // İptal bilgisini ekle
                        isAttended = r.IsPatientCome,   // Katılım bilgisini ekle (IsPatientCome olarak güncellendi)
                        cancellationReason = r.CancellReason // İptal nedenini ekle (CancellReason olarak güncellendi)
                    })
                    .ToListAsync();

                // Verileri JSON formatında döndürme
                return Ok(allAppointments);
            }
            catch (Exception ex)
            {
                // Hata durumunda konsola logla ve istemciye hata mesajı dön
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
