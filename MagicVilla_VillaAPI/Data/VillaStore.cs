using MagicVilla_VillaAPI.Model;
using MagicVilla_VillaAPI.Model.DTO;

namespace MagicVilla_VillaAPI.Data
{
    public static class VillaStore
    {
        public static List<VillaDTO> VillaList = new List<VillaDTO> {
                new VillaDTO
                {
                    Id = 1,
                    Name = "Villa1",
                    Sqft = 1200,
                    Occupancy = 10
                },
                new VillaDTO
                {
                    Id = 2,
                    Name = "Villa2",
                    Sqft = 1800,
                    Occupancy = 20
                }
            };
    }
}
