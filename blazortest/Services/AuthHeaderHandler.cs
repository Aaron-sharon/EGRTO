using System.Net.Http.Headers;
using Microsoft.JSInterop;

public class AuthHeaderHandler
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;

    public AuthHeaderHandler(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
    }

    public async Task AttachToken()
    {
        try
        {
            var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

            if (!string.IsNullOrEmpty(token))
            {
                Console.WriteLine($"[INFO] Token found in localStorage: {token}");

                if (_httpClient.DefaultRequestHeaders.Authorization == null ||
                    _httpClient.DefaultRequestHeaders.Authorization?.Parameter != token)
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    Console.WriteLine("[INFO] Token successfully attached to headers.");
                }
                else
                {
                    Console.WriteLine("[INFO] Token already present in headers.");
                }
            }
            else
            {
                Console.WriteLine("[WARNING] Token not found in localStorage.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to attach token: {ex.Message}");
        }
    }

}
