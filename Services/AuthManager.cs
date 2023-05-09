using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Newtonsoft.Json;


public class AuthManager
{
    private readonly HttpClient _httpClient;

    private readonly string _loginUrl = "https://faceapi.700t.com/api/UserLogin/UserLogin";
    private readonly string _selectUserInfoUrl = "https://faceapi.700t.com/api/UserLogin/SelectUserInfo";


    public AuthManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<BasicResult> LoginAsync(string username, string password)
    {
        var data = new { loginName = username, loginpwd = password };
        var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_loginUrl, content);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<BasicResult>(json);

            if (result.Code == 0)
            {
                var successResult = JsonConvert.DeserializeObject<LoginSuccessResponse>(json);

                return successResult;
            }
            return result;
        }
        else
        {
            return new BasicResult
            {
                Code = (int)response.StatusCode,
                Message = response.ReasonPhrase,
            };
        }
    }

    public async Task<BasicResult> SelectUserInfoAsync(string token)
    {
        var tokenWithoutPrefix = token.Replace("Bearer ", "");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",tokenWithoutPrefix);

        HttpResponseMessage response = await _httpClient.PostAsync(_selectUserInfoUrl, null);

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<BasicResult>(json);

            if (result.Code == 0)
            {
                var successResult = JsonConvert.DeserializeObject<SelectUserInfoSucessResponse>(json);

                return successResult;
            }
            return result;
        }
        else
        {
            return new BasicResult
            {
                Code = (int)response.StatusCode,
                Message = response.ReasonPhrase,
            };
        }
    }

    public class BasicResult
    {
        public int Code { get; set; }
        public string? Message { get; set; }
    }

    public class LoginSuccessResponse : BasicResult
    {
        public LoginSuccessResponseData Data { get; set; }

        public int Count { get; set; }
    }

    public class LoginSuccessResponseData
    {
        public string Token { get; set; }

    }

    public class LoginFailureResponse : BasicResult
    {
        public int Data { get; set; }
    }

    public class SelectUserInfoSucessResponse : BasicResult
    {
        public User Data { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LoginName { get; set; }
        public string LoginPwd { get; set; }
        public string PhoneNumber { get; set; }
        public string CompanyName { get; set; }
        public string TeamName { get; set; }
        public string UserPhoto { get; set; }
        public string UserRole { get; set; }
        public int CompanyId { get; set; }
    }
}
