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
using System.Net;

namespace MagicVilla_VillaAPI.Controllers
{
    // [Route("api/[controller]")]                   //Commenting this to VillaAPI is the fixed API name as if someone changes the controller
    //name then API name will also get change then we have to change that wherever the API url is used.
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private ILogger _logger;
        protected APIResponse _apiResponse;
        private readonly IVillaRepository _villaRepository;
        private readonly IMapper _mapper;
        public VillaAPIController(ILogger<VillaAPIController> logger
                                  , IMapper mapper
                                  , IVillaRepository villaRepository)
        {
            _logger = logger;
            //_dbContext = dbContext;
            _mapper = mapper;
            _villaRepository = villaRepository;
            _apiResponse = new();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillas()
        {
            try
            {
                _logger.LogInformation(message: " GetVillas : getting all villas !");
                IEnumerable<Villa> villas = await _villaRepository.GetAllAsync();

                _apiResponse.Result = _mapper.Map<List<VillaDTO>>(villas);
                _apiResponse.StatusCode = HttpStatusCode.OK;
                _apiResponse.IsSuccess = true;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess=false;
                _apiResponse.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _apiResponse;
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
        public async Task<ActionResult<APIResponse>> GetVilla(int Id)
        {
            try
            {
                if (Id == 0)
                {
                    _logger.LogError(message: " GetVilla : Id is 0");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.IsSuccess = false;
                    return BadRequest(_apiResponse);
                }
                var Villa = await _villaRepository.GetAsync(u => u.Id == Id);
                if (Villa == null)
                {
                    _logger.LogError(message: " GetVilla : no data is present for Id " + Id);
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.IsSuccess = false;
                    return NotFound(_apiResponse);
                }
                _logger.LogInformation(message: " GetVilla :succesfully get the data ");

                _apiResponse.Result = _mapper.Map<VillaDTO>(Villa);
                _apiResponse.StatusCode = HttpStatusCode.OK;
                _apiResponse.IsSuccess = true;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _apiResponse;
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
        public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreateDTO villa)
        {
            try
            {
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
                
                Villa model = _mapper.Map<Villa>(villa);                

                await _villaRepository.CreateAsync(model);

                _logger.LogInformation(message: " CreateVilla : succesfully created !");

                _apiResponse.Result = _mapper.Map<VillaDTO>(model);
                _apiResponse.StatusCode = HttpStatusCode.Created;
                _apiResponse.IsSuccess = true;
                return CreatedAtRoute("GetVilla", new { Id = model.Id }, _apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _apiResponse;
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
        public async Task<ActionResult<APIResponse>> DeleteVilla(int Id)
        {
            try
            {
                if (Id == 0)
                {
                    _logger.LogError(message: " DeleteVilla : Id is 0 ");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.IsSuccess = false;
                    return BadRequest(_apiResponse);
                }

                var villa = await _villaRepository.GetAsync(u => u.Id == Id);

                if (villa == null)
                {
                    _logger.LogError(message: " DeleteVilla : no data is present for Id " + Id);
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.IsSuccess = false;
                    return NotFound(_apiResponse);
                }

                await _villaRepository.RemoveAsync(villa);
                _logger.LogInformation(message: " DeleteVilla : Delete successfull ");

                _apiResponse.StatusCode = HttpStatusCode.NoContent;
                _apiResponse.IsSuccess = true;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _apiResponse;
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
        public async Task<ActionResult<APIResponse>> UpdateVilla(int Id, VillaUpdateDTO villa)
        {
            try
            {
                if (Id != villa.Id)
                {
                    _logger.LogError(message: "UpdateVilla : Id doesnt match with the model Id ");
                    _apiResponse.StatusCode= HttpStatusCode.BadRequest;
                    _apiResponse.IsSuccess = false;
                    return BadRequest(_apiResponse);
                }
                var data = await _villaRepository.GetAsync(x => x.Id == Id, false);
                if (data == null)
                {
                    _logger.LogError(message: " updatevilla : no data is present for id " + Id);
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.IsSuccess = false;
                    return NotFound(_apiResponse);
                }
                Villa model = _mapper.Map<Villa>(villa);

                await _villaRepository.UpdateAsync(model);
                _logger.LogInformation(message: " UpdateVilla : Update successfull ");

                _apiResponse.StatusCode = HttpStatusCode.NoContent;
                _apiResponse.IsSuccess = true;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _apiResponse;
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

            PatchVilla.ApplyTo(villaDto, ModelState);

            Villa model = _mapper.Map<Villa>(villaDto);

            await _villaRepository.UpdateAsync(model);
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
