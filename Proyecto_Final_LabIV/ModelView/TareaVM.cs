using Microsoft.AspNetCore.Mvc.Rendering;
using Proyecto_Final_LabIV.Models;

namespace Proyecto_Final_LabIV.ModelView
{
    public class TareaVM
    {
        public List<Tarea> tareas { get; set; }
        public SelectList ListPrioridad { get; set; }
        public SelectList ListEstado { get; set; }
        public SelectList ListUsuario { get; set; }
        //public SelectList ListDetalleTarea { get; set; }
        public int? busqPrioridadId { get; set; }
        public int? busqEstadoId { get; set; }
        public int? busqUsuarioId { get; set; }
        public Paginador paginador { get; set; }
    }
}
