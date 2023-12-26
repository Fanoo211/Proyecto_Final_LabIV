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
    public class TareasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TareasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tareas
        public async Task<IActionResult> Index(int? busqPrioridadId, int? busqEstadoId, int? busqUsuarioId, int pagina = 1)
        {
            Paginador paginas = new Paginador();
            paginas.PaginaActual = pagina;
            paginas.RegistrosPorPagina = 3;

            var applicationDbContext = _context.tareas
                .Include(p => p.Prioridad)
                .Include(e => e.Estado)
                .Include(u => u.Usuario)
                .Include(d => d.DetalleTarea)
                .Select(s => s);

            if (busqPrioridadId != null && busqPrioridadId > 0)
            {
                applicationDbContext = applicationDbContext.Where(s => s.PrioridadId.Equals(busqPrioridadId));
                paginas.ValoresQueryString.Add("PrioridadId", busqPrioridadId.ToString());
            }
            if (busqEstadoId != null && busqEstadoId > 0)
            {
                applicationDbContext = applicationDbContext.Where(s => s.EstadoId.Equals(busqEstadoId));
                paginas.ValoresQueryString.Add("EstadoId", busqEstadoId.ToString());
            }
            if (busqUsuarioId != null && busqUsuarioId > 0)
            {
                applicationDbContext = applicationDbContext.Where(s => s.UsuarioId.Equals(busqUsuarioId));
                paginas.ValoresQueryString.Add("UsuarioId", busqUsuarioId.ToString());
            }

            paginas.TotalRegistros = applicationDbContext.Count();

            var registrosMostrar = applicationDbContext
                        .Skip((pagina - 1) * paginas.RegistrosPorPagina)
                        .Take(paginas.RegistrosPorPagina);


            TareaVM datos = new TareaVM()
            {
                tareas = registrosMostrar.ToList(),
                busqPrioridadId = busqPrioridadId,
                busqEstadoId = busqEstadoId,
                busqUsuarioId = busqUsuarioId,
                ListPrioridad = new SelectList(_context.prioridades, "Id", "Nombre"),
                ListEstado = new SelectList(_context.estados, "Id", "Nombre"),
                ListUsuario = new SelectList(_context.usuarios, "Id", "Apellido"),
                paginador = paginas
            };

            return View(datos);
        }

        // GET: Tareas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.tareas == null)
            {
                return NotFound();
            }

            var tarea = await _context.tareas
                .Include(t => t.DetalleTarea)
                .Include(t => t.Estado)
                .Include(t => t.Prioridad)
                .Include(t => t.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tarea == null)
            {
                return NotFound();
            }

            return View(tarea);
        }

        // GET: Tareas/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["DetalleTareaId"] = new SelectList(_context.detallesTareas, "Id", "Descripcion");
            ViewData["EstadoId"] = new SelectList(_context.estados, "Id", "Nombre");
            ViewData["PrioridadId"] = new SelectList(_context.prioridades, "Id", "Nombre");

            var usuarios = _context.usuarios.ToList();
            ViewData["UsuarioId"] = new SelectList(usuarios, "Id", "NombreCompleto");

            return View();
        }

        // POST: Tareas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Titulo,PrioridadId,EstadoId,UsuarioId,DetalleTareaId")] Tarea tarea)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tarea);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DetalleTareaId"] = new SelectList(_context.detallesTareas, "Id", "Id", tarea.DetalleTareaId);
            ViewData["EstadoId"] = new SelectList(_context.estados, "Id", "Id", tarea.EstadoId);
            ViewData["PrioridadId"] = new SelectList(_context.prioridades, "Id", "Id", tarea.PrioridadId);
            ViewData["UsuarioId"] = new SelectList(_context.usuarios, "Id", "Id", tarea.UsuarioId);
            return View(tarea);
        }

        // GET: Tareas/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.tareas == null)
            {
                return NotFound();
            }

            var tarea = await _context.tareas.FindAsync(id);
            if (tarea == null)
            {
                return NotFound();
            }
            ViewData["DetalleTareaId"] = new SelectList(_context.detallesTareas, "Id", "Descripcion", tarea.DetalleTareaId);
            ViewData["EstadoId"] = new SelectList(_context.estados, "Id", "Nombre", tarea.EstadoId);
            ViewData["PrioridadId"] = new SelectList(_context.prioridades, "Id", "Nombre", tarea.PrioridadId);

            var usuarios = _context.usuarios.ToList();
            ViewData["UsuarioId"] = new SelectList(usuarios, "Id", "NombreCompleto");

            return View(tarea);
        }

        // POST: Tareas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,PrioridadId,EstadoId,UsuarioId,DetalleTareaId")] Tarea tarea)
        {
            if (id != tarea.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tarea);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TareaExists(tarea.Id))
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
            ViewData["DetalleTareaId"] = new SelectList(_context.detallesTareas, "Id", "Id", tarea.DetalleTareaId);
            ViewData["EstadoId"] = new SelectList(_context.estados, "Id", "Id", tarea.EstadoId);
            ViewData["PrioridadId"] = new SelectList(_context.prioridades, "Id", "Id", tarea.PrioridadId);
            ViewData["UsuarioId"] = new SelectList(_context.usuarios, "Id", "Id", tarea.UsuarioId);
            return View(tarea);
        }

        // GET: Tareas/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.tareas == null)
            {
                return NotFound();
            }

            var tarea = await _context.tareas
                .Include(t => t.DetalleTarea)
                .Include(t => t.Estado)
                .Include(t => t.Prioridad)
                .Include(t => t.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tarea == null)
            {
                return NotFound();
            }

            return View(tarea);
        }

        // POST: Tareas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.tareas == null)
            {
                return Problem("Entity set 'ApplicationDbContext.tareas'  is null.");
            }
            var tarea = await _context.tareas.FindAsync(id);
            if (tarea != null)
            {
                _context.tareas.Remove(tarea);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TareaExists(int id)
        {
          return (_context.tareas?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
