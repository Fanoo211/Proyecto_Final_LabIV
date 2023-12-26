namespace Proyecto_Final_LabIV.Models
{
    public class Tarea
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int PrioridadId { get; set; }
        public Prioridad? Prioridad { get; set; }
        public int EstadoId { get; set; }
        public Estado? Estado { get; set; }
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public int DetalleTareaId { get; set; }
        public DetalleTarea? DetalleTarea { get; set; }
    }
}
