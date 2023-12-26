using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Proyecto_Final_LabIV.Data;
using Proyecto_Final_LabIV.Models;
using System.Diagnostics;

namespace Proyecto_Final_LabIV.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompletarTarea(int tareaId)
        {
            var tarea = await _context.tareas.FindAsync(tareaId);

            if (tarea == null)
            {
                return NotFound();
            }

            _context.tareas.Remove(tarea);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public IActionResult Index()
        {
            var vehiculos = _context.tareas.Include(v => v.Prioridad).Include(v => v.Estado).Include(v => v.Usuario).Include(v => v.DetalleTarea).ToList();

            return View(vehiculos);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}