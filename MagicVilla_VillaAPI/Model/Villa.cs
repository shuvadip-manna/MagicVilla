namespace MagicVilla_VillaAPI.Model
{
    public class Villa
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public float Sqtft { get; set; }
        public int Occupancy { get; set; }
    }
}
