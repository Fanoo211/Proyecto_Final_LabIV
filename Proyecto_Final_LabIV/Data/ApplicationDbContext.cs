using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Proyecto_Final_LabIV.Models;

namespace Proyecto_Final_LabIV.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Tarea> tareas { get; set; }
        public DbSet<DetalleTarea> detallesTareas { get; set; }
        public DbSet<Estado> estados { get; set; }
        public DbSet<Usuario> usuarios { get; set; }
        public DbSet<Prioridad> prioridades { get; set; }
    }
}