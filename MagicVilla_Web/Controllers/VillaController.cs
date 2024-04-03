using AutoMapper;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MagicVilla_Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly IVillaService _villaService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        public VillaController(IVillaService villaService
                               , IMapper mapper
                               , ILogger<VillaController> logger)
        {
            _mapper = mapper;
            _villaService = villaService;
            _logger = logger;
        }
        public async Task<IActionResult> IndexVilla()
        {
            List<VillaDTO> list= new List<VillaDTO>();

            _logger.LogDebug("IndexVilla: Calling the get all API.");
            var response = await _villaService.GetAllAsync<APIResponse>();
            _logger.LogDebug("IndexVilla: recieved the response from the api : " + response.Result.ToString());
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result)); 
            }
            return View(list);
        }
    }
}
