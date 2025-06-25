using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace YingCaiAiWin.Helpers
{
    public class HttpClientHelper
    {
        private  HttpClient _httpClient;
        private readonly string _url = "http://113.105.116.171:8000/";
        public HttpClientHelper()
        {
           // _httpClient = new HttpClient();
        }

        public async Task<string> GetDataAsync(int id)
        {
            _httpClient = new HttpClient();
            var response = await _httpClient.GetAsync(_url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task<string> PostDataAsync(string url,object data)
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(360);  // 10秒超时
            var response = await _httpClient.PostAsync(_url+url, content);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
    }
}
