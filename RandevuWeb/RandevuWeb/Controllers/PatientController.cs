using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RandevuWeb.Data; // Your RandevuContext's namespace
using RandevuWeb.Data.Models; // Your Kisiler model's namespace
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;

namespace RandevuWeb.Controllers
{
    [Route("[controller]")]
    public class PatientController : Controller
    {
        private readonly RandevuContext _context;

        public PatientController(RandevuContext context)
        {
            _context = context;
        }

        [HttpGet("patientDashboard")]
        public IActionResult PatientDashboard()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetUserInfo()
        {
            // We get the email address of the logged-in user.
            // This method works if the logged-in user identity is configured correctly.
            var userEmail = User.Identity?.Name;

            if (string.IsNullOrEmpty(userEmail))
            {
                // The user is not logged in or has no identity information.
                return Unauthorized(new { message = "Unauthorized access. Please log in." });
            }

            // We find the user in the database by email address.
            var user = await _context.Kisilers.FirstOrDefaultAsync(k => k.Email == userEmail);

            if (user == null)
            {
                return NotFound(new { message = "User information not found." });
            }

            return Json(new
            {
                name = user.Name,
                surname = user.Surname,
                email = user.Email,
                phoneNumber = user.PhoneNumber
            });
        }

        [HttpGet("getDoctors")]
        public async Task<IActionResult> GetDoctors()
        {
            // Define the doctor role ID
            var doctorRoleId = "532a11cc-2031-472b-b3f0-e8ad471b11bf";

            // Find the user IDs with the doctor role
            var doctorUserIds = await _context.KisilerinRolleris
                                            .Where(kr => kr.RoleId == doctorRoleId)
                                            .Select(kr => kr.UserId)
                                            .ToListAsync();

            // Find the doctors with these IDs from the Kisiler table
            // Null check is added to ensure k.Id is not null.
            var doctors = await _context.Kisilers
                                    .Where(k => k.Id != null && doctorUserIds.Contains(k.Id))
                                    .Select(k => new { k.Id, k.Name, k.Surname })
                                    .ToListAsync();

            if (doctors == null || !doctors.Any())
            {
                return NotFound(new { message = "No doctors found." });
            }

            return Ok(doctors);
        }

        [HttpGet("getAppointmentTypes")]
        public async Task<IActionResult> GetAppointmentTypes()
        {
            // We only bring "examination" (Id=1) and "check-up" (Id=2) types for patients.
            // And we rename the "TypeName" property to "name".
            var appointmentTypes = await _context.RandevuTipleris
                .Where(rt => rt.Id == 1 || rt.Id == 2)
                .Select(rt => new { rt.Id, name = rt.TypeName }) // Correction here!
                .ToListAsync();

            if (appointmentTypes == null || !appointmentTypes.Any())
            {
                return NotFound(new { message = "Appointment types not found." });
            }

            return Ok(appointmentTypes);
        }

        [HttpPost("updateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] Kisiler updatedUser)
        {
            // We get the Id from the body of the request.
            if (string.IsNullOrEmpty(updatedUser.Id))
            {
                return Unauthorized(new { success = false, message = "Unauthorized access. User ID not found." });
            }

            // We find the user in the database by ID.
            var userToUpdate = await _context.Kisilers.FirstOrDefaultAsync(k => k.Id == updatedUser.Id);

            if (userToUpdate == null)
            {
                return NotFound(new { success = false, message = "User to be updated not found." });
            }

            // We update using the data from the incoming model
            userToUpdate.Name = updatedUser.Name;
            userToUpdate.Surname = updatedUser.Surname;
            userToUpdate.Email = updatedUser.Email;
            userToUpdate.PhoneNumber = updatedUser.PhoneNumber;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Information updated successfully." });
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, new { success = false, message = "Concurrency update error occurred." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Server error: " + ex.Message });
            }
        }

        [HttpPost("createAppointment")]
        public async Task<IActionResult> CreateAppointment([FromBody] Randevular newAppointment)
        {
            // Check if required fields in the incoming data are empty
            if (string.IsNullOrEmpty(newAppointment.PatientId) ||
                string.IsNullOrEmpty(newAppointment.DoctorId) ||
                newAppointment.AppointmentTypeId == 0)
            {
                return BadRequest(new { success = false, message = "Missing appointment information." });
            }

            // Check if there is an existing appointment for the same doctor, on the same date, at the same time
            var existingAppointment = await _context.Randevulars.FirstOrDefaultAsync(r =>
                r.DoctorId == newAppointment.DoctorId &&
                r.AppointmentStartDate.Date == newAppointment.AppointmentStartDate.Date &&
                r.StartHour == newAppointment.StartHour);

            if (existingAppointment != null)
            {
                return BadRequest(new { success = false, message = "Se\u00E7ti\u011Finiz randevu saati doludur." });
            }

            // Create a new appointment object to be saved to the database
            var appointment = new Randevular
            {
                CreatedDate = DateTime.Now,
                PatientId = newAppointment.PatientId,
                DoctorId = newAppointment.DoctorId,
                AppointmentTypeId = newAppointment.AppointmentTypeId,
                AppointmentStartDate = newAppointment.AppointmentStartDate,
                StartHour = newAppointment.StartHour,
                AppointmentEndDate = newAppointment.AppointmentEndDate,
                EndHour = newAppointment.EndHour,
                // Initial values as requested by the user
                IsPatientCome = false,
                IsCancelled = false,
                CancellReason = null
            };

            try
            {
                _context.Randevulars.Add(appointment);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Randevu ba\u015Far\u0131yla olu\u015Fturuldu." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while saving the appointment: " + ex.Message });
            }
        }

        [HttpGet("getAppointments")]
        public async Task<IActionResult> GetAppointments(string patientId)
        {
            if (string.IsNullOrEmpty(patientId))
            {
                return BadRequest(new { message = "Patient ID is required." });
            }

            try
            {
                var appointments = await _context.Randevulars
                    .Where(r => r.PatientId == patientId)
                    .Include(r => r.Doctor) // Include doctor information
                    .Include(r => r.AppointmentType) // Include appointment type information
                    .OrderByDescending(r => r.AppointmentStartDate) // En yeni tarihten en eskiye doğru sırala
                    .Select(r => new
                    {
                        r.Id,
                        DoctorName = r.Doctor.Name + " " + r.Doctor.Surname,
                        r.AppointmentStartDate,
                        r.StartHour,
                        r.EndHour,
                        AppointmentTypeName = r.AppointmentType.TypeName,
                        r.IsCancelled
                    })
                    .ToListAsync();

                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving appointments: " + ex.Message });
            }
        }

        // Yeni model: Randevu iptali için gerekli verileri taşır
        public class CancelAppointmentRequest
        {
            public int AppointmentId { get; set; }
            public string? CancellationReason { get; set; }
        }

        [HttpPost("cancelAppointment")]
        public async Task<IActionResult> CancelAppointment([FromBody] CancelAppointmentRequest request)
        {
            if (request == null || request.AppointmentId <= 0)
            {
                return BadRequest(new { message = "Geçersiz randevu bilgisi." });
            }

            try
            {
                var appointment = await _context.Randevulars.FindAsync(request.AppointmentId);

                if (appointment == null)
                {
                    return NotFound(new { message = "Belirtilen randevu bulunamadı." });
                }

                // Randevunun tarihini kontrol et, geçmiş randevular iptal edilemez.
                if (appointment.AppointmentStartDate < DateTime.Now)
                {
                     return BadRequest(new { message = "Geçmiş tarihli randevular iptal edilemez." });
                }

                // Randevu bilgilerini güncelle
                appointment.IsCancelled = true;
                appointment.CancellReason = request.CancellationReason;

                // Değişiklikleri veritabanına kaydet
                await _context.SaveChangesAsync();

                return Ok(new { message = "Randevu başarıyla iptal edildi." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Randevu iptal edilirken bir hata oluştu: " + ex.Message });
            }
        }
    }
}
