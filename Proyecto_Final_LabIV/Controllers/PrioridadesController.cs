using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_Final_LabIV.Data;
using Proyecto_Final_LabIV.Models;
using Proyecto_Final_LabIV.ModelView;

namespace Proyecto_Final_LabIV.Controllers
{
    public class PrioridadesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PrioridadesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Prioridades
        public async Task<IActionResult> Index(int pagina = 1)
        {
            Paginador paginas = new Paginador();
            paginas.PaginaActual = pagina;
            paginas.RegistrosPorPagina = 3;

            var applicationDbContext = _context.prioridades;

            paginas.TotalRegistros = applicationDbContext.Count();

            var registrosMostrar = applicationDbContext
            .Skip((pagina - 1) * paginas.RegistrosPorPagina)
            .Take(paginas.RegistrosPorPagina);

            PrioridadVM datos = new PrioridadVM()
            {
                prioridades = registrosMostrar.ToList(),
                paginador = paginas
            };

            return View(datos);
        }

        // GET: Prioridades/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.prioridades == null)
            {
                return NotFound();
            }

            var prioridad = await _context.prioridades
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prioridad == null)
            {
                return NotFound();
            }

            return View(prioridad);
        }

        // GET: Prioridades/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Prioridades/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Nombre")] Prioridad prioridad)
        {
            if (ModelState.IsValid)
            {
                _context.Add(prioridad);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(prioridad);
        }

        // GET: Prioridades/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.prioridades == null)
            {
                return NotFound();
            }

            var prioridad = await _context.prioridades.FindAsync(id);
            if (prioridad == null)
            {
                return NotFound();
            }
            return View(prioridad);
        }

        // POST: Prioridades/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre")] Prioridad prioridad)
        {
            if (id != prioridad.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prioridad);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrioridadExists(prioridad.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(prioridad);
        }

        // GET: Prioridades/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.prioridades == null)
            {
                return NotFound();
            }

            var prioridad = await _context.prioridades
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prioridad == null)
            {
                return NotFound();
            }

            return View(prioridad);
        }

        // POST: Prioridades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.prioridades == null)
            {
                return Problem("Entity set 'ApplicationDbContext.prioridades'  is null.");
            }
            var prioridad = await _context.prioridades.FindAsync(id);
            if (prioridad != null)
            {
                _context.prioridades.Remove(prioridad);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PrioridadExists(int id)
        {
          return (_context.prioridades?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
