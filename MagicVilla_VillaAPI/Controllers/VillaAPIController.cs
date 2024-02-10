using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Model;
using MagicVilla_VillaAPI.Model.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MagicVilla_VillaAPI.Controllers
{
    // [Route("api/[controller]")]                   //Commenting this to VillaAPI is the fixed API name as if someone changes the controller
    //name then API name will also get change then we have to change that wherever the API url is used.
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private ILogger _logger;
        //private ILogging _logger;
        //private readonly ApplicationDbContext _dbContext;
        private readonly IVillaRepository _villaRepository;
        private readonly IMapper _mapper;
        public VillaAPIController(ILogger<VillaAPIController> logger
                                 // , ApplicationDbContext dbContext
                                  , IMapper mapper
                                  , IVillaRepository villaRepository)
        {
            _logger = logger;
            //_dbContext = dbContext;
            _mapper = mapper;
            _villaRepository = villaRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            _logger.LogInformation(message: " GetVillas : getting all villas !");
            IEnumerable<Villa> villas = await _villaRepository.GetAllAsync();
            return Ok(_mapper.Map<List<VillaDTO>>(villas));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("{Id:int}", Name = "GetVilla")]                                  //Name over here is for routing.
        [ProducesResponseType(StatusCodes.Status200OK)]                            //A better way to define the status code return by the endpoint
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(200, Type = typeof(VillaDTO))]                     //this will define what are the status code will get return by this Endpoint
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        public async Task<ActionResult<VillaDTO>> GetVilla(int Id)
        {
            if (Id == 0)
            {
                _logger.LogError(message: " GetVilla : Id is 0");
                return BadRequest();
            }
            var Villa = await _villaRepository.GetAsync(u => u.Id == Id);
            if (Villa == null)
            {
                _logger.LogError(message: " GetVilla : no data is present for Id " + Id);
                return NotFound();
            }
            _logger.LogInformation(message: " GetVilla :succesfully get the data ");
            return Ok(_mapper.Map<VillaDTO>(Villa));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="villa"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]                            //A better way to define the status code return by the endpoint
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateVilla([FromBody] VillaCreateDTO villa)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest();
            //}
            if (await _villaRepository.GetAsync(u => u.Name.ToLower() == villa.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomDuplicateNameError", "Villa Already Exist");
                _logger.LogError(message: " CreateVilla : Already a villa with similar name exist !");
                return BadRequest(ModelState);
            }
            if (villa == null)
            {
                _logger.LogError(message: " CreateVilla : Model is null !");
                return BadRequest();
            }
            //if (villa.Id > 0)
            //{
            //    _logger.LogError(message: " CreateVilla: model Id is not 0 ");
            //    return StatusCode(StatusCodes.Status500InternalServerError);
            //}
            //villa.Id = VillaStore.VillaList.OrderByDescending(x => x.Id)
            //                               .FirstOrDefault().Id + 1;
            //VillaStore.VillaList.Add(villa);

            Villa model = _mapper.Map<Villa>(villa);
            //Villa model = new()
            //{
            //    Amenity = villa.Amenity,
            //    Name = villa.Name,
            //    Sqft = villa.Sqft,
            //    Occupancy = villa.Occupancy,
            //    Rate = villa.Rate,
            //    ImageUrl = villa.ImageUrl,
            //    Details = villa.Details,
            //    //Id = villa.Id,
            //};

            await _villaRepository.CreateAsync(model);
            //await _dbContext.SaveChangesAsync();

            _logger.LogInformation(message: " CreateVilla : succesfully created !");
            //return Ok(villa);
            return CreatedAtRoute("GetVilla", new { Id = model.Id }, model);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("{Id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]                            //A better way to define the status code return by the endpoint
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteVilla(int Id)
        {
            if (Id == 0)
            {
                _logger.LogError(message: " DeleteVilla : Id is 0 ");
                return BadRequest();
            }

            var villa = await _villaRepository.GetAsync(u => u.Id == Id);

            if (villa == null)
            {
                _logger.LogError(message: " DeleteVilla : no data is present for Id " + Id);
                return NotFound();
            }

            await _villaRepository.RemoveAsync(villa);
           // await _dbContext.SaveChangesAsync();
            _logger.LogInformation(message: " DeleteVilla : Delete successfull ");
            return NoContent();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="villa"></param>
        /// <returns></returns>
        [HttpPut("{Id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]                            //A better way to define the status code return by the endpoint
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateVilla(int Id, VillaUpdateDTO villa)
        {
            if (Id != villa.Id)
            {
                _logger.LogError(message: "UpdateVilla : Id doesnt match with the model Id ");
                return BadRequest();
            }
            var data = await _villaRepository.GetAsync(x => x.Id == Id,false);
            if (data == null)
            {
                _logger.LogError(message: " updatevilla : no data is present for id " + Id);
                return NotFound();
            }
            //data.Name = villa.Name;
            //data.Sqft = villa.Sqft;
            //data.Occupancy = villa.Occupancy;
            Villa model = _mapper.Map<Villa>(villa); 
            //Villa model = new()
            //{
            //    Amenity = villa.Amenity,
            //    Name = villa.Name,
            //    Sqft = villa.Sqft,
            //    Occupancy = villa.Occupancy,
            //    Rate = villa.Rate,
            //    ImageUrl = villa.ImageUrl,
            //    Details = villa.Details,
            //    Id = villa.Id,
            //};

            await _villaRepository.UpdateAsync(model);
            //await _dbContext.SaveChangesAsync();
            _logger.LogInformation(message: " UpdateVilla : Update successfull ");
            return NoContent();
        }
        /// <summary>
        /// take notes/details from the website https://jsonpatch.com/
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="villa"></param>
        /// <returns></returns>
        [HttpPatch("{Id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]                            //A better way to define the status code return by the endpoint
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdatePartialVilla(int Id, JsonPatchDocument<VillaUpdateDTO> PatchVilla)
        {
            if (Id == 0 || PatchVilla == null)
            {
                _logger.LogError(message: "UpdatePartialVilla : Id is 0 or the model is null");
                return BadRequest();
            }
            var villa = await _villaRepository.GetAsync(x => x.Id == Id,false);

            if (villa == null)
            {
                _logger.LogError(message: " UpdatePartialVilla : no data is present for Id " + Id);
                return NotFound();
            }

            VillaUpdateDTO villaDto = _mapper.Map<VillaUpdateDTO>(villa);
            //VillaUpdateDTO villaDto = new()
            //{
            //    Amenity = villa.Amenity,
            //    Name = villa.Name,
            //    Sqft = villa.Sqft,
            //    Occupancy = villa.Occupancy,
            //    Rate = villa.Rate,
            //    ImageUrl = villa.ImageUrl,
            //    Details = villa.Details,
            //    Id = villa.Id,
            //};

            PatchVilla.ApplyTo(villaDto, ModelState);

            Villa model = _mapper.Map<Villa>(villaDto);

            //Villa model = new()
            //{
            //    Amenity = villaDto.Amenity,
            //    Name = villaDto.Name,
            //    Sqft = villaDto.Sqft,
            //    Occupancy = villaDto.Occupancy,
            //    Rate = villaDto.Rate,
            //    ImageUrl = villaDto.ImageUrl,
            //    Details = villaDto.Details,
            //    Id = villaDto.Id,
            //};

            await _villaRepository.UpdateAsync(model);
            //await _dbContext.SaveChangesAsync();
            if (!ModelState.IsValid)
            {
                _logger.LogError(message: " UpdatePartialVilla : Model is Invalid");
                return BadRequest();
            }

            _logger.LogInformation(message: " UpdatePartialVilla : Update Successfull");
            return NoContent();
        }
    }
}
