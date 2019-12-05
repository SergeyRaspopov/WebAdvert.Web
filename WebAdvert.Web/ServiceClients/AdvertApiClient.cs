using AdvertApi.Models;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebAdvert.Web.ServiceClients.Models;

namespace WebAdvert.Web.ServiceClients
{
    public class AdvertApiClient : IAdvertApiClient
    {
        private HttpClient _httpClient;
        private IMapper _mapper;
        private IConfiguration _configuration;

        public AdvertApiClient(HttpClient httpClient, IMapper mapper, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _mapper = mapper;
            _configuration = configuration;

            _httpClient.BaseAddress = new Uri(_configuration.GetSection("AdvertApi").GetValue<string>("BaseUrl"));
            _httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
        }

        public async Task<bool> Confirm(ApiConfirmAdvert model)
        {
            var confirmAdvert = _mapper.Map<ConfirmAdvertModel>(model);
            var reqJson = JsonConvert.SerializeObject(confirmAdvert);
            var response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/confirm", new StringContent(reqJson));
            return response.IsSuccessStatusCode;
        }

        public async Task<ApiCreateAdvertResponse> Create(ApiAdvert model)
        {
            var advert = _mapper.Map<AdvertModel>(model);
            var reqJson = JsonConvert.SerializeObject(advert);
            var response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/create", new StringContent(reqJson));
            var resJson = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<CreateAdvertResponse>(resJson);
            return _mapper.Map<ApiCreateAdvertResponse>(result);
        }
    }
}
