using System.ComponentModel.DataAnnotations;

namespace BibliotecaMVC.Models
{
    public class Libro
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Titulo { get; set; }

        [Required]
        [StringLength(100)]
        public string Autor { get; set; }

        [StringLength(50)]
        public string Categoria { get; set; }

        [Range(0, 9999)]
        public decimal Precio { get; set; }

        public bool Disponible { get; set; }

        public string ImagenUrl { get; set; }
    }
}
