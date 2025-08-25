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
    public class DoctorController : Controller
    {
        private readonly RandevuContext _context;

        public DoctorController(RandevuContext context)
        {
            _context = context;
        }

        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            // Bu, 'Views/Doctor/doctorDashboard.cshtml' dosyasını açar.
            return View("doctorDashboard");
        }
    }
}