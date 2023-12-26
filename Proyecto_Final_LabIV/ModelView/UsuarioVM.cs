using Microsoft.AspNetCore.Mvc.Rendering;
using Proyecto_Final_LabIV.Models;

namespace Proyecto_Final_LabIV.ModelView
{
    public class UsuarioVM
    {
        public List<Usuario> usuarios { get; set; }
        public string? busqNombre { get; set; }
        public string? busqApellido { get; set; }
        public int? busqDNI { get; set; }
        public Paginador paginador { get; set; }
    }
}
