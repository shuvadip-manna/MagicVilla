using AutoMapper;
using MagicVilla_VillaAPI.Model;
using MagicVilla_VillaAPI.Model.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/VillaNumberAPI")]
    [ApiController]
    public class VillaNumberAPIController : ControllerBase
    {
        private ILogger _logger;
        protected APIResponse _apiResponse;
        private readonly IVillaNumberRepository _villaNumberRepository;
        private readonly IVillaRepository _villaRepository;
        private readonly IMapper _mapper;
        public VillaNumberAPIController(ILogger<VillaNumberAPIController> logger
                                        , IMapper mapper
                                        , IVillaNumberRepository villaNumberRepository
                                        , IVillaRepository villaRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _villaNumberRepository = villaNumberRepository;
            _apiResponse = new();
            _villaRepository = villaRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillaNumbers()
        {
            try
            {
                _logger.LogInformation(message: " GetVillaNumbers : getting all villa numbers !");
                IEnumerable<VillaNumber> villas = await _villaNumberRepository.GetAllAsync();

                _apiResponse.Result = _mapper.Map<List<VillaNumberDTO>>(villas);
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
        [HttpGet("{Id:int}", Name = "GetVillaNumber")]                           
        [ProducesResponseType(StatusCodes.Status200OK)]                          
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVillaNumber(int Id)
        {
            try
            {
                if (Id == 0)
                {
                    _logger.LogError(message: " GetVillaNumber : Id is 0");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.IsSuccess = false;
                    return BadRequest(_apiResponse);
                }
                var Villa = await _villaNumberRepository.GetAsync(u => u.VillaNo == Id);
                if (Villa == null)
                {
                    _logger.LogError(message: " GetVillaNumber : no data is present for VillaNo " + Id);
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.IsSuccess = false;
                    return NotFound(_apiResponse);
                }
                _logger.LogInformation(message: " GetVillaNumber :succesfully get the data ");

                _apiResponse.Result = _mapper.Map<VillaNumberDTO>(Villa);
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
        [ProducesResponseType(StatusCodes.Status201Created)]                 
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDTO villaNumber)
        {
            try
            {
                if (await _villaNumberRepository.GetAsync(u => u.VillaNo == villaNumber.VillaNo) != null)
                {
                    ModelState.AddModelError("CustomDuplicateNumberError", "VillaNumber Already Exist");
                    _logger.LogError(message: " CreateVillaNumber : Already a villaNumber with similar VillaNo exist !");
                    return BadRequest(ModelState);
                }
                if(await _villaRepository.GetAsync(x => x.Id == villaNumber.VillaID) == null)
                {
                    ModelState.AddModelError("ForeignKeyError", "VillaId does not exist");
                    _logger.LogError(message: "CreateVillaNumber : ForeignKey error VillaId does not Exist !");
                    return BadRequest(ModelState);
                }

                if (villaNumber == null)
                {
                    _logger.LogError(message: " CreateVillaNumber : Model is null !");
                    return BadRequest();
                }

                VillaNumber model = _mapper.Map<VillaNumber>(villaNumber);                

                await _villaNumberRepository.CreateAsync(model);

                _logger.LogInformation(message: " CreateVillaNumber : succesfully created !");

                _apiResponse.Result = _mapper.Map<VillaNumberDTO>(model);
                _apiResponse.StatusCode = HttpStatusCode.Created;
                _apiResponse.IsSuccess = true;
                return CreatedAtRoute("GetVillaNumber", new { Id = model.VillaNo }, _apiResponse);
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
        [HttpDelete("{Id:int}", Name = "DeleteVillaNumber")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int Id)
        {
            try
            {
                if (Id == 0)
                {
                    _logger.LogError(message: "DeleteVillaNumber : VillaNo is 0 ");
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.IsSuccess = false;
                    return BadRequest(_apiResponse);
                }

                var villa = await _villaNumberRepository.GetAsync(u => u.VillaNo == Id);

                if (villa == null)
                {
                    _logger.LogError(message: "DeleteVillaNumber : no data is present for VillaNo " + Id);
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.IsSuccess = false;
                    return NotFound(_apiResponse);
                }

                await _villaNumberRepository.RemoveAsync(villa);
                _logger.LogInformation(message: "DeleteVillaNumber : Delete successfull ");

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
        [HttpPut("{Id:int}", Name = "UpdateVillaNumber")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int Id, VillaNumberUpdateDTO villaNumber)
        {
            try
            {
                if (Id != villaNumber.VillaNo)
                {
                    _logger.LogError(message: "UpdateVillaNumber : Id doesnt match with the model VillaNo ");
                    _apiResponse.StatusCode= HttpStatusCode.BadRequest;
                    _apiResponse.IsSuccess = false;
                    return BadRequest(_apiResponse);
                }
                var data = await _villaNumberRepository.GetAsync(x => x.VillaNo == Id, false);
                if (data == null)
                {
                    _logger.LogError(message: "updatevillaNumber : no data is present for VillaNo " + Id);
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.IsSuccess = false;
                    return NotFound(_apiResponse);
                }
                VillaNumber model = _mapper.Map<VillaNumber>(villaNumber);

                await _villaNumberRepository.UpdateAsync(model);
                _logger.LogInformation(message: " UpdateVillaNumber : Update successfull ");

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
        [HttpPatch("{Id:int}", Name = "UpdatePartialVillaNumber")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdatePartialVillaNumber(int Id, JsonPatchDocument<VillaNumberUpdateDTO> PatchVillaNumber)
        {
            if (Id == 0 || PatchVillaNumber == null)
            {
                _logger.LogError(message: "UpdatePartialVillaNumber : Id is 0 or the model is null");
                return BadRequest();
            }
            var villaNumber = await _villaNumberRepository.GetAsync(x => x.VillaNo == Id,false);

            if (villaNumber == null)
            {
                _logger.LogError(message: " UpdatePartialVillaNumber : no data is present for VillaNo " + Id);
                return NotFound();
            }

            VillaNumberUpdateDTO villaNumberDto = _mapper.Map<VillaNumberUpdateDTO>(villaNumber);

            PatchVillaNumber.ApplyTo(villaNumberDto, ModelState);

            VillaNumber model = _mapper.Map<VillaNumber>(villaNumberDto);

            await _villaNumberRepository.UpdateAsync(model);
            if (!ModelState.IsValid)
            {
                _logger.LogError(message: " UpdatePartialVillaNumber : Model is Invalid");
                return BadRequest();
            }

            _logger.LogInformation(message: " UpdatePartialVillaNumber : Update Successfull");
            return NoContent();
        }
    }
}
