using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using Proyecto_Final_LabIV.Data;
using Proyecto_Final_LabIV.Models;
using Proyecto_Final_LabIV.ModelView;

namespace Proyecto_Final_LabIV.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly object hostingEnvironment;

        public UsuariosController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpPost]
        public async Task<IActionResult> ImportarUsuariosDesdeExcel(IFormFile excel)
        {
            try
            {
                //LOS VIEWBAG NO FUNCIONAN POR LOS REDIRECTTOACTION
                if (excel == null || excel.Length == 0)
                {
                    ViewBag.Mensaje = "Error: No se ha seleccionado ningún archivo para importar.";
                }
                else
                {
                    // Ruta de la carpeta wwwroot/impo
                    var pathDestino = Path.Combine(_env.WebRootPath, "impo");

                    // Crea la carpeta si no existe
                    if (!Directory.Exists(pathDestino))
                    {
                        Directory.CreateDirectory(pathDestino);
                    }

                    // Ruta del archivo en la carpeta imps
                    string filePath = Path.Combine(pathDestino, excel.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await excel.CopyToAsync(stream);
                    }

                    var workbook = new XLWorkbook(excel.OpenReadStream());
                    var hoja = workbook.Worksheet(1);
                    var primeraFilaUsada = hoja.FirstRowUsed().RangeAddress.FirstAddress.RowNumber;
                    var ultimaFilaUsada = hoja.LastRowUsed().RangeAddress.FirstAddress.RowNumber;
                    var usuarios = new List<Usuario>();

                    for (int i = primeraFilaUsada + 1; i <= ultimaFilaUsada; i++)
                    {
                        var fila = hoja.Row(i);
                        var usuario = new Usuario();

                        if (fila.Cell(1).IsEmpty() || fila.Cell(2).IsEmpty() || fila.Cell(3).IsEmpty() || fila.Cell(4).IsEmpty() || fila.Cell(5).IsEmpty() || fila.Cell(6).IsEmpty())
                        {
                            ViewBag.Mensaje = $"Error: La fila {i} contiene celdas vacías.";
                            return RedirectToAction("Index", await _context.usuarios.ToListAsync());
                        }

                        usuario.Nombre = fila.Cell(1).GetString();
                        usuario.Apellido = fila.Cell(2).GetString();
                        usuario.Correo = fila.Cell(3).GetString();
                        usuario.DNI = fila.Cell(4).GetValue<int>();
                        usuario.FechaNacimiento = fila.Cell(5).GetDateTime();
                        usuario.Foto = fila.Cell(6).GetString();

                        usuarios.Add(usuario);
                    }

                    if (usuarios.Count > 0)
                    {
                        _context.AddRange(usuarios);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        ViewBag.Mensaje = "Error: No se importó ningún usuario. Asegúrate de que el archivo Excel contenga datos válidos.";
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = "Error: La importación ha fallado. Verifica el formato del archivo o consulta los registros del servidor para obtener más detalles.";
                Console.WriteLine("Error en la importación: " + ex.Message);

                if (ex.InnerException != null)
                {
                    Console.WriteLine("Excepción interna: " + ex.InnerException.Message);
                }
            }

            return RedirectToAction("Index", await _context.usuarios.ToListAsync());
        }

        [HttpGet]
        public async Task<FileResult> ExportarUsuariosAExcel()
        {
            var usuarios = await _context.usuarios.ToListAsync();
            var nombreArchivo = $"Usuarios.xlsx";
            return GenerarExcel(nombreArchivo, usuarios);
        }


        private FileResult GenerarExcel(string nombreArchivo, IEnumerable<Usuario> usuarios)
        {
            DataTable dataTable = new DataTable("Usuarios");
            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Nombre"),
                new DataColumn("Apellido"),
                new DataColumn("Correo"),
                new DataColumn("DNI"),
                new DataColumn("FechaNacimiento"),
            });

            foreach (var usuario in usuarios)
            {
                dataTable.Rows.Add(usuario.Nombre,
                    usuario.Apellido,
                    usuario.Correo,
                    usuario.DNI,
                    usuario.FechaNacimiento);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dataTable);

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        nombreArchivo);
                }
            }

        }

        // GET: Usuarios
        public async Task<IActionResult> Index(string? busqNombre, string? busqApellido, int? busqDNI, int pagina = 1)
        {
            Paginador paginas = new Paginador();
            paginas.PaginaActual = pagina;
            paginas.RegistrosPorPagina = 3;

            var applicationDbContext = _context.usuarios.Select(e => e);

            if (!string.IsNullOrEmpty(busqNombre))
            {
                applicationDbContext = applicationDbContext.Where(e => e.Nombre.Contains(busqNombre));
                paginas.ValoresQueryString.Add("busqNombre", busqNombre);
            }
            if (!string.IsNullOrEmpty(busqApellido))
            {
                applicationDbContext = applicationDbContext.Where(e => e.Apellido.Contains(busqApellido));
                paginas.ValoresQueryString.Add("busqApellido", busqApellido);
            }
            if (busqDNI != null && busqDNI > 0)
            {
                applicationDbContext = applicationDbContext.Where(s => s.DNI.Equals(busqDNI));
                paginas.ValoresQueryString.Add("DNI", busqDNI.ToString());
            }

            paginas.TotalRegistros = applicationDbContext.Count();

            var registrosMostrar = applicationDbContext
            .Skip((pagina - 1) * paginas.RegistrosPorPagina)
            .Take(paginas.RegistrosPorPagina);

            UsuarioVM datos = new UsuarioVM()
            {
                usuarios = registrosMostrar.ToList(),
                busqNombre = busqNombre,
                busqApellido = busqApellido,
                busqDNI = busqDNI,
                paginador = paginas
            };

            return View(datos);
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Apellido,Correo,DNI,FechaNacimiento,Foto")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                usuario.Foto = cargarFoto("");

                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Apellido,Correo,DNI,FechaNacimiento,Foto")] Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string nuevaFoto = cargarFoto(string.IsNullOrEmpty(usuario.Foto) ? "" : usuario.Foto);
                    if (!string.IsNullOrEmpty(nuevaFoto))
                    {
                        usuario.Foto = nuevaFoto;
                    }

                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Id))
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
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.usuarios == null)
            {
                return Problem("Entity set 'ApplicationDbContext.usuarios'  is null.");
            }
            var usuario = await _context.usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.usuarios.Remove(usuario);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
          return (_context.usuarios?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private string cargarFoto(string fotoAnterior)
        {
            var archivos = HttpContext.Request.Form.Files;
            if (archivos != null && archivos.Count > 0)
            {
                var archivoFoto = archivos[0];
                if (archivoFoto.Length > 0)
                {
                    var pathDestino = Path.Combine(_env.WebRootPath, "fotos");

                    fotoAnterior = Path.Combine(pathDestino, fotoAnterior);
                    if (System.IO.File.Exists(fotoAnterior))
                        System.IO.File.Delete(fotoAnterior);

                    var archivoDestino = Guid.NewGuid().ToString().Replace("-", "");
                    archivoDestino += Path.GetExtension(archivoFoto.FileName);

                    using (var filestream = new FileStream(Path.Combine(pathDestino, archivoDestino), FileMode.Create))
                    {
                        archivoFoto.CopyTo(filestream);
                        return archivoDestino;
                    };
                }
            }
            return "";
        }
    }
}