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
    public class DetalleTareasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DetalleTareasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DetalleTareas
        public async Task<IActionResult> Index(int pagina = 1)
        {
            Paginador paginas = new Paginador();
            paginas.PaginaActual = pagina;
            paginas.RegistrosPorPagina = 3;

            var applicationDbContext = _context.detallesTareas;

            paginas.TotalRegistros = applicationDbContext.Count();

            var registrosMostrar = applicationDbContext
            .Skip((pagina - 1) * paginas.RegistrosPorPagina)
            .Take(paginas.RegistrosPorPagina);

            DetalleTareaVM datos = new DetalleTareaVM()
            {
                detallesTarea = registrosMostrar.ToList(),
                paginador = paginas
            };

            return View(datos);
        }

        // GET: DetalleTareas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.detallesTareas == null)
            {
                return NotFound();
            }

            var detalleTarea = await _context.detallesTareas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (detalleTarea == null)
            {
                return NotFound();
            }

            return View(detalleTarea);
        }

        // GET: DetalleTareas/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: DetalleTareas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Descripcion,FechaInicio,FechaFin")] DetalleTarea detalleTarea)
        {
            if (ModelState.IsValid)
            {
                _context.Add(detalleTarea);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(detalleTarea);
        }

        // GET: DetalleTareas/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.detallesTareas == null)
            {
                return NotFound();
            }

            var detalleTarea = await _context.detallesTareas.FindAsync(id);
            if (detalleTarea == null)
            {
                return NotFound();
            }
            return View(detalleTarea);
        }

        // POST: DetalleTareas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion,FechaInicio,FechaFin")] DetalleTarea detalleTarea)
        {
            if (id != detalleTarea.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(detalleTarea);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DetalleTareaExists(detalleTarea.Id))
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
            return View(detalleTarea);
        }

        // GET: DetalleTareas/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.detallesTareas == null)
            {
                return NotFound();
            }

            var detalleTarea = await _context.detallesTareas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (detalleTarea == null)
            {
                return NotFound();
            }

            return View(detalleTarea);
        }

        // POST: DetalleTareas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.detallesTareas == null)
            {
                return Problem("Entity set 'ApplicationDbContext.detallesTareas'  is null.");
            }
            var detalleTarea = await _context.detallesTareas.FindAsync(id);
            if (detalleTarea != null)
            {
                _context.detallesTareas.Remove(detalleTarea);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DetalleTareaExists(int id)
        {
          return (_context.detallesTareas?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
