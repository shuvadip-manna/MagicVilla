using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Model.DTO
{
    public class VillaDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
        public float Sqtft { get; set; }
        public int Occupancy { get; set; }
    }
}
