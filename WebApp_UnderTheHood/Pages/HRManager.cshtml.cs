using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using WebApp_UnderTheHood.Authorization;
using WebApp_UnderTheHood.DTO;

namespace WebApp_UnderTheHood.Pages
{
	[Authorize(Policy = "HRManagerOnly")]
	public class HRManagerModel : PageModel
	{
		private readonly IHttpClientFactory _httpClientFactory;

		[BindProperty]
		public List<WeatherForecastDTO> WeatherForecastItems { get; set; }

		public HRManagerModel(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		public async Task OnGetAsync()
		{
			WeatherForecastItems = await InvokeEndPoint<List<WeatherForecastDTO>>("OurWebAPI", "WeatherForecast");
		}

		private async Task<T> InvokeEndPoint<T>(string clientName, string url)
		{
			JwtToken token = null;

			var strTokenObj = HttpContext.Session.GetString("access_token");

			if (string.IsNullOrWhiteSpace(strTokenObj))
				token = await Authenticate();
			else
				token = JsonConvert.DeserializeObject<JwtToken>(strTokenObj);

			if (token == null || string.IsNullOrWhiteSpace(token.AccessToken) || token.ExpiresAt <= DateTime.UtcNow)
			{
				token = await Authenticate();
			}

			var httpClient = _httpClientFactory.CreateClient(clientName);
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

			return await httpClient.GetFromJsonAsync<T>(url);
		}

		private async Task<JwtToken> Authenticate()
		{
			var httpClient = _httpClientFactory.CreateClient("OurWebAPI");

			var res = await httpClient.PostAsJsonAsync("auth", new Credential { UserName = "admin", Password = "123456" });
			res.EnsureSuccessStatusCode();
			string strJwt = await res.Content.ReadAsStringAsync();
			
			HttpContext.Session.SetString("access_token", strJwt);

			return JsonConvert.DeserializeObject<JwtToken>(strJwt);
		}
	}
}
