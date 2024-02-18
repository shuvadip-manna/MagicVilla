using MagicVilla_Web.Models;
using MagicVilla_Web.Services.IServices;
using Newtonsoft.Json;
using System.Text;
using static MagicVilla_Utility.SD;

namespace MagicVilla_Web.Services
{
    public class BaseService : IBaseService
    {
        public APIResponse responseModel { get; set; }
        public IHttpClientFactory httpClient { get; set; }
        public BaseService(IHttpClientFactory httpClient)
        {
            this.responseModel = new();
            this.httpClient = httpClient;
        }

        public async Task<T> SendAsync<T>(APIRequest apiRequest)
        {
            try
            {
                var client = httpClient.CreateClient("MagicVilla");
                HttpRequestMessage message = new HttpRequestMessage();
                message.Headers.Add("Accept", "application/json");
                message.RequestUri = new Uri(apiRequest.Url);
                if (apiRequest.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data), Encoding.UTF8, "application/json");
                }
                switch (apiRequest.apiType)
                {
                    case ApiType.GET:
                        message.Method = HttpMethod.Get;
                        break;
                    case ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Patch;
                        break;
                }
                HttpResponseMessage responseMessage = null;
                responseMessage = await client.SendAsync(message);

                var apiContent = await responseMessage.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<T>(apiContent);
                return response;
            }
            catch (Exception ex)
            {
                var dto = new APIResponse
                {
                    ErrorMessages = new List<string> { ex.Message.ToString() },
                    IsSuccess = false
                };
                var res = JsonConvert.SerializeObject(dto);
                var response = JsonConvert.DeserializeObject<T>(res);
                return response;
            }
        }
    }
}
