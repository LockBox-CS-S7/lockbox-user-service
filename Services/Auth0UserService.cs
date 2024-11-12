using System.Net.Http.Headers;
using System.Text.Json;
using dotenv.net;
using lockbox_user_service.Models;

namespace lockbox_user_service.Services;

public class Auth0UserService : IUserService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private string? _accessToken;
    private DateTime? _tokenExpiry;

    public Auth0UserService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }
    
    // Define a class to deserialize token response JSON
    private class TokenResponse
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        // public string token { get; set; }
    }
    
    private async Task<string> GetAccessTokenAsync()
    {
        if (_accessToken != null && DateTime.UtcNow < _tokenExpiry)
        {
            return _accessToken; // Use cached token if not expired
        }

        var domain = _configuration["AUTH0_DOMAIN"];
        var clientId = _configuration["CLIENT_ID"];
        var clientSecret = _configuration["CLIENT_SECRET"];
        var audience = _configuration["AUTH0_AUDIENCE"];

        var tokenRequest = new HttpRequestMessage(HttpMethod.Post, $"https://{domain}/oauth/token");
        tokenRequest.Content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret),
            new KeyValuePair<string, string>("audience", audience),
            new KeyValuePair<string, string>("grant_type", "client_credentials")
        });

        var response = await _httpClient.SendAsync(tokenRequest);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var tokenData = JsonSerializer.Deserialize<TokenResponse>(json);
        _accessToken = tokenData.access_token;
        _tokenExpiry = DateTime.UtcNow.AddSeconds(tokenData.expires_in);

        return _accessToken;
    }
    
    
    private async Task<string> GetUserDetailsAsync(string userId)
    {
        var token = await GetAccessTokenAsync();
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://{_configuration["AUTH0_DOMAIN"]}/api/v2/users/{userId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request); // TODO: Response is still 'unauthorised'
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
    
    private async Task DeleteUserAsync(string userId)
    {
        var token = await GetAccessTokenAsync();
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_configuration["Auth0:Audience"]}users/{userId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
    
    
    public async Task<string?> GetUserById(string id)
    {
        var userDetails = await GetUserDetailsAsync(id);
        return userDetails;
    }

    public async Task<bool> DeleteUserById(string id)
    {
        try
        {
            await DeleteUserAsync(id);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}
