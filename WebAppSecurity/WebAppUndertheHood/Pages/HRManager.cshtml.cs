using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using WebAppUndertheHood.Authorization;
using WebAppUndertheHood.DTO;
using WebAppUndertheHood.Pages.Account;

namespace WebAppUndertheHood.Pages
{
    [Authorize(Policy = "HRManagerOnly")]
    public class HRManagerModel : PageModel
    {
        private readonly IHttpClientFactory httpClientFactory;

        [BindProperty]
        public List<WeatherForecastDTO> weatherForecastItems { get; set; } = new List<WeatherForecastDTO>();

        public HRManagerModel(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }


        public async Task OnGetAsync()
        {
            var httpClient = httpClientFactory.CreateClient("OurWebAPI");
            var response = await httpClient.PostAsJsonAsync("auth", new Credential { UserName = "admin", Password = "password" });
            response.EnsureSuccessStatusCode();
            string strJwt = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<JwtToken>(strJwt);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken ?? string.Empty);
            weatherForecastItems = await httpClient.GetFromJsonAsync<List<WeatherForecastDTO>>("WeatherForecast") ?? 
                                                                                                new List<WeatherForecastDTO>();          
        }
    }
}
