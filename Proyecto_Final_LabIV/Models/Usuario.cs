namespace Proyecto_Final_LabIV.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public int DNI { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string? Foto { get; set; }
        public string NombreCompleto => $"{Nombre} {Apellido}";
    }
}
