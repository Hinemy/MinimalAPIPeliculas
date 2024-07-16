using System.ComponentModel.DataAnnotations;

namespace MinimalAPIPeliculas.Entidades
{
    /// <summary>
    /// Defines the <see cref="Genero" />
    /// </summary>
    public class Genero
    {
        
        public int Id { get; set; }

        [StringLength(50)]
        public string Nombre { get; set; } = null!;
    }
}
