﻿using MagicVilla_Utilidad;
using MagicVilla_Web.Models;
using Newtonsoft.Json;
using System.Text;

namespace MagicVilla_Web.Services.IServices
{
    public class BaseService : IBaseService
    {
        public APIResponse responseModel { get; set; }
        public IHttpClientFactory _httpClient { get; set; }


        public BaseService(IHttpClientFactory httpClient)
        {
            this.responseModel = new();
            _httpClient = httpClient;
            
        }

        public async Task<T> SendAsync<T>(APIRequest apiResquest)
        {
            try
            {
                var client = _httpClient.CreateClient("MagicAPI");
                HttpRequestMessage message = new HttpRequestMessage();
                message.Headers.Add("Accept", "application/json");
                message.RequestUri = new Uri(apiResquest.Url);

                //POST or PUT
                if (apiResquest.Datos != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(apiResquest.Datos),
                        Encoding.UTF8, "application/json");

                }

                switch (apiResquest.APITipo)
                {
                    case DS.APITipo.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case DS.APITipo.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case DS.APITipo.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:                        
                        message.Method = HttpMethod.Get;
                        break;
                }

                HttpResponseMessage apiResponse = null;
                apiResponse = await client.SendAsync(message);
                var apiContent = await apiResponse.Content.ReadAsStringAsync();
                var APIResponse = JsonConvert.DeserializeObject<T>(apiContent);
                return APIResponse;

            }
            catch (Exception ex)
            {

                var dto = new APIResponse
                {
                    ErrorMessages = new List<string> { Convert.ToString(ex.Message) },
                    IsExitoso = false
                };
                var res = JsonConvert.SerializeObject(dto);
                var APIResponseERR = JsonConvert.DeserializeObject<T>(res);
                return APIResponseERR;
            }
        }
    }
}
