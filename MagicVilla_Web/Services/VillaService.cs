using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MagicVilla_Web.Services
{
    public class VillaService : BaseService, IVillaService
    {
        private readonly IHttpClientFactory _httpClient;
        private string VillaUrl;
        public VillaService(IHttpClientFactory httpClient
                            , IConfiguration configuration) : base(httpClient)
        {
            _httpClient = httpClient;
            VillaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
        }

        public async Task<T> CreateAsync<T>(VillaCreateDTO createDTO)
        {
            return await SendAsync<T>(new APIRequest
            {
                apiType = MagicVilla_Utility.SD.ApiType.POST,
                Data = createDTO,
                Url = VillaUrl + "/api/villaAPI"
            });
        }

        public async Task<T> DeleteAsync<T>(int id)
        {
            return await SendAsync<T>(new APIRequest
            {
                apiType = MagicVilla_Utility.SD.ApiType.DELETE,
                Url = VillaUrl + "/api/villaAPI/" + id
            });
        }

        public async Task<T> GetAllAsync<T>()
        {
            return await SendAsync<T>(new APIRequest
            {
                apiType = MagicVilla_Utility.SD.ApiType.GET,
                Url = VillaUrl + "/api/villaAPI"
            });
        }

        public async Task<T> GetAsync<T>(int id)
        {
            return await SendAsync<T>(new APIRequest
            {
                apiType = MagicVilla_Utility.SD.ApiType.GET,
                Url = VillaUrl + "/api/villaAPI/" + id
            });
        }

        public async Task<T> UpdateAsync<T>(VillaUpdateDTO updateDTO)
        {
            return await SendAsync<T>(new APIRequest
            {
                apiType = MagicVilla_Utility.SD.ApiType.PUT,
                Data = updateDTO,
                Url = VillaUrl + "/api/villaAPI/" + updateDTO.Id
            });
        }
    }
}
