using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Model;
using MagicVilla_VillaAPI.Model.DTO;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MagicVilla_VillaAPI.Controllers
{
    // [Route("api/[controller]")]                                                //Commenting this to VillaAPI is the fixed API name as if someone changes the controller
                                                                                  //name then API name will also get change then we have to change that wherever the API url is used.
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private ILogger _logger;
        public VillaAPIController(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            _logger.LogInformation(" GetVillas : getting all villas !");
            return Ok(VillaStore.VillaList);
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
        public ActionResult<VillaDTO> GetVilla(int Id)
        {
            if (Id == 0)
            {
                _logger.LogError(" GetVilla : Id is 0");
                return BadRequest();
            }
            var Villa = VillaStore.VillaList.FirstOrDefault(u => u.Id == Id);
            if(Villa == null)
            {
                _logger.LogError(" GetVilla : no data is present for Id " + Id);
                return NotFound();
            }
            _logger.LogInformation(" GetVilla :succesfully get the data ");
            return Ok(Villa);
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
        public ActionResult CreateVilla([FromBody]VillaDTO villa)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest();
            //}
            if(VillaStore.VillaList.FirstOrDefault(u=>u.Name.ToLower() == villa.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomDuplicateNameError", "Villa Already Exist");
                _logger.LogError(" CreateVilla : Already a villa with similar name exist !");
                return BadRequest(ModelState);
            }    
            if(villa == null)
            {
                _logger.LogError(" CreateVilla : Model is null !");
                return BadRequest();
            }
            if(villa.Id > 0)
            {
                _logger.LogError(" CreateVilla: model Id is not 0 ");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            villa.Id = VillaStore.VillaList.OrderByDescending(x => x.Id)
                                           .FirstOrDefault().Id + 1;
            VillaStore.VillaList.Add(villa);

            _logger.LogInformation(" CreateVilla : succesfully created !");
            //return Ok(villa);
            return CreatedAtRoute("GetVilla", new { Id = villa.Id },villa);
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
        public IActionResult DeleteVilla(int Id)
        {
            if (Id == 0)
            {
                _logger.LogError(message: " DeleteVilla : Id is 0 ");
                return BadRequest();
            }

            var villa = VillaStore.VillaList.FirstOrDefault(u => u.Id == Id);

            if (villa == null)
            {
                _logger.LogError(message: " DeleteVilla : no data is present for Id " + Id);
                return NotFound();
            }

            VillaStore.VillaList.Remove(villa);
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
        public IActionResult UpdateVilla(int Id, VillaDTO villa)
        {
            if (Id != villa.Id)
            {
                _logger.LogError(message: "UpdateVilla : Id doesnt match with the model Id ");
                return BadRequest();
            }
            var data = VillaStore.VillaList.FirstOrDefault(x => x.Id == Id);
            if (data == null)
            {
                _logger.LogError(message: " UpdateVilla : no data is present for Id " + Id);
                return NotFound();
            }
            data.Name = villa.Name;
            data.Sqtft = villa.Sqtft;
            data.Occupancy = villa.Occupancy;
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
        public IActionResult UpdatePartialVilla(int Id, JsonPatchDocument<VillaDTO> PatchVilla)
        {
            if (Id == 0 || PatchVilla == null)
            {
                _logger.LogError(message: "UpdatePartialVilla : Id is 0 or the model is null");
                return BadRequest();
            }
            var villa = VillaStore.VillaList.FirstOrDefault(x => x.Id == Id);
            if (villa == null)
            {
                _logger.LogError(message: " UpdatePartialVilla : no data is present for Id " + Id);
                return NotFound();
            }
            PatchVilla.ApplyTo(villa, ModelState);
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
