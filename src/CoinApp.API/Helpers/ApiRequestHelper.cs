using CoinApp.API.Extentions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CoinApp.API.Helpers
{
    public class ApiRequestHelper<T>
    {
        public static readonly string URL = Startup.StaticConfig.GetSection("ApiSettings").GetSection("URL").Value;
        public static async Task<ApiResponse<T>> Get(string url, AuthenticationHeader authHeader, bool isAuthorize = false)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            if (isAuthorize)
                SetRequestAuthenticationHeader(ref client, authHeader);

            HttpResponseMessage response = await client.GetAsync(url);

            var jsonString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ApiResponse<T>>(jsonString);
        }

        public static async Task<ApiResponse2<T>> GetSingle(string url, AuthenticationHeader authHeader, bool isAuthorize = false)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            if (isAuthorize)
                SetRequestAuthenticationHeader(ref client, authHeader);

            HttpResponseMessage response = await client.GetAsync(url);

            var jsonString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ApiResponse2<T>>(jsonString);
        }

        public static async Task<ApiResponse<T>> Post(string url, string parameters, AuthenticationHeader authHeader, bool isAuthorize = false, KeyValuePair<string, string> headerParam = (default(KeyValuePair<string, string>)))
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            if (isAuthorize)
                SetRequestAuthenticationHeader(ref client, authHeader);

            if (!headerParam.Equals(default(KeyValuePair<string, string>)))
                client.DefaultRequestHeaders.Add(headerParam.Key, headerParam.Value);

            var content = new StringContent(parameters, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(url, content);

            var jsonString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ApiResponse<T>>(jsonString);
        }

        public static async Task<ApiResponse<T>> Post(string url, MultipartFormDataContent content, AuthenticationHeader authHeader, bool isAuthorize = false)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            if (isAuthorize)
                SetRequestAuthenticationHeader(ref client, authHeader);

            HttpResponseMessage response = await client.PostAsync(url, content);

            var jsonString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ApiResponse<T>>(jsonString);
        }

        public static async Task<ApiResponse<T>> Put(string url, string parameters, AuthenticationHeader authHeader, bool isAuthorize = false)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            if (isAuthorize)
                SetRequestAuthenticationHeader(ref client, authHeader);

            var content = new StringContent(parameters, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PutAsync(url, content);

            var jsonString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ApiResponse<T>>(jsonString);
        }

        public static async Task<ApiResponse<T>> Put(string url, MultipartFormDataContent content, AuthenticationHeader authHeader, bool isAuthorize = false)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            if (isAuthorize)
                SetRequestAuthenticationHeader(ref client, authHeader);

            HttpResponseMessage response = await client.PutAsync(url, content);

            var jsonString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ApiResponse<T>>(jsonString);
        }

        public static async Task<ApiResponse<T>> Delete(string url, AuthenticationHeader authHeader, bool isAuthorize = false)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            if (isAuthorize)
                SetRequestAuthenticationHeader(ref client, authHeader);

            HttpResponseMessage response = await client.DeleteAsync(url);

            var jsonString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ApiResponse<T>>(jsonString);
        }

        public static void SetRequestAuthenticationHeader(ref HttpClient httpClient, AuthenticationHeader authenticationHeader)
        {
            if (authenticationHeader.IsBasic)
            {
                var authenticationString = $"{authenticationHeader.Username}:{authenticationHeader.Password}";
                var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);

                return;
            }

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", authenticationHeader.Token);

        }
    }
}
