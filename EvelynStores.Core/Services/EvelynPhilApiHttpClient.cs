namespace EvelynStores.Core.Services;

using Blazored.LocalStorage;
using Blazored.SessionStorage;
using EvelynStores.Core.DTOs;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

public interface IEvelynPhilApiHttpClient
{
    Task<EvelynPhilApiResponse<T>> GetAsync<T>(string endpoint);
    Task<EvelynPhilApiResponse<T>> PostAsync<T>(string endpoint, object? data = null);
    Task<EvelynPhilApiResponse<T>> PutAsync<T>(string endpoint, object data);
    Task<EvelynPhilApiResponse<T>> PatchAsync<T>(string endpoint, object data);
    Task<EvelynPhilApiResponse<T>> DeleteAsync<T>(string endpoint);
    Task<EvelynPhilApiResponse> PostAsync(string endpoint, object? data = null);
    Task<EvelynPhilApiResponse> DeleteAsync(string endpoint);
    void SetAuthToken(string token);
    Task SetAuthTokenAsync(string token, bool rememberMe);
    Task<string?> GetRememberMeEmailAsync();
    Task SetRememberMeEmailAsync(string? email);
    void ClearAuthToken();
    string? GetAuthToken();
}

public class EvelynPhilApiHttpClient : IEvelynPhilApiHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<EvelynPhilApiHttpClient> _logger;
    private readonly ILocalStorageService _localStorage;
    private readonly ISessionStorageService _sessionStorage;
    private const string TokenStorageKey = "auth_token";
    private const string TokenExpiryKey = "token_expiry";
    private const string RefreshTokenStorageKey = "refresh_token";
    private const string RememberMeKey = "remember_me";
    private const string RememberMeEmailKey = "remember_me_email";

    // In-memory cache (for current request cycle)
    private string? _authToken;
    private bool _rememberMe = true;

    // Prevent concurrent refresh attempts
    private static readonly SemaphoreSlim _refreshLock = new(1, 1);
    private bool _isRefreshing;

    public EvelynPhilApiHttpClient(HttpClient httpClient, ILogger<EvelynPhilApiHttpClient> logger, ILocalStorageService localStorage, ISessionStorageService sessionStorage)
    {
        _httpClient = httpClient;
        _logger = logger;
        _localStorage = localStorage;
        _sessionStorage = sessionStorage;
    }

    public async Task<EvelynPhilApiResponse<T>> GetAsync<T>(string endpoint)
    {
        try
        {
            // Restore token from storage on each request
            await RestoreTokenFromStorage();

            AddAuthHeader();

            // LOG THE FULL URL
            var fullUrl = _httpClient.BaseAddress?.ToString().TrimEnd('/') + "/" + endpoint.TrimStart('/');
            _logger.LogInformation($"🔗 REQUEST URL: {fullUrl}");

            var response = await _httpClient.GetAsync(fullUrl);

            _logger.LogInformation($"📊 RESPONSE STATUS: {response.StatusCode}");

            // If 401, attempt token refresh and retry once
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var refreshed = await TryRefreshTokenAsync();
                if (refreshed)
                {
                    AddAuthHeader();
                    response = await _httpClient.GetAsync(fullUrl);
                    _logger.LogInformation($"📊 RETRY RESPONSE STATUS: {response.StatusCode}");
                }
            }

            return await HandleResponse<T>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error calling GET {endpoint}");
            return EvelynPhilApiResponse<T>.ErrorResponse($"Error: {ex.Message}");
        }
    }

    public async Task<EvelynPhilApiResponse<T>> PostAsync<T>(string endpoint, object? data = null)
    {
        try
        {
            // Restore token from storage on each request
            await RestoreTokenFromStorage();

            AddAuthHeader();

            // LOG THE FULL URL
            var fullUrl = _httpClient.BaseAddress?.ToString().TrimEnd('/') + "/" + endpoint.TrimStart('/');
            _logger.LogInformation($"🔗 REQUEST URL: {fullUrl}");

            HttpContent? content = null;
            if (data != null)
            {
                content = JsonContent.Create(data);
            }

            var response = await _httpClient.PostAsync(fullUrl, content);

            // If 401, attempt token refresh and retry once
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var refreshed = await TryRefreshTokenAsync();
                if (refreshed)
                {
                    AddAuthHeader();
                    content = data != null ? JsonContent.Create(data) : null;
                    response = await _httpClient.PostAsync(fullUrl, content);
                }
            }

            return await HandleResponse<T>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error calling POST {endpoint}");
            return EvelynPhilApiResponse<T>.ErrorResponse($"Error: {ex.Message}");
        }
    }

    public async Task<EvelynPhilApiResponse<T>> PatchAsync<T>(string endpoint, object data)
    {
        try
        {
            // Restore token from storage on each request
            await RestoreTokenFromStorage();

            AddAuthHeader();

            // LOG THE FULL URL
            var fullUrl = _httpClient.BaseAddress?.ToString().TrimEnd('/') + "/" + endpoint.TrimStart('/');
            _logger.LogInformation($"🔗 REQUEST URL: {fullUrl}");

            var content = JsonContent.Create(data);
            var response = await _httpClient.PatchAsync(fullUrl, content);

            // If 401, attempt token refresh and retry once
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var refreshed = await TryRefreshTokenAsync();
                if (refreshed)
                {
                    AddAuthHeader();
                    content = JsonContent.Create(data);
                    response = await _httpClient.PatchAsync(fullUrl, content);
                }
            }

            return await HandleResponse<T>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error calling PATCH {endpoint}");
            return EvelynPhilApiResponse<T>.ErrorResponse($"Error: {ex.Message}");
        }
    }


    public async Task<EvelynPhilApiResponse<T>> PutAsync<T>(string endpoint, object data)
    {
        try
        {
            // Restore token from storage on each request
            await RestoreTokenFromStorage();

            AddAuthHeader();

            // LOG THE FULL URL
            var fullUrl = _httpClient.BaseAddress?.ToString().TrimEnd('/') + "/" + endpoint.TrimStart('/');
            _logger.LogInformation($"🔗 REQUEST URL: {fullUrl}");

            var content = JsonContent.Create(data);
            var response = await _httpClient.PutAsync(fullUrl, content);

            // If 401, attempt token refresh and retry once
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var refreshed = await TryRefreshTokenAsync();
                if (refreshed)
                {
                    AddAuthHeader();
                    content = JsonContent.Create(data);
                    response = await _httpClient.PutAsync(fullUrl, content);
                }
            }

            return await HandleResponse<T>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error calling PUT {endpoint}");
            return EvelynPhilApiResponse<T>.ErrorResponse($"Error: {ex.Message}");
        }
    }

    public async Task<EvelynPhilApiResponse<T>> DeleteAsync<T>(string endpoint)
    {
        try
        {
            // Restore token from storage on each request
            await RestoreTokenFromStorage();

            AddAuthHeader();

            // LOG THE FULL URL
            var fullUrl = _httpClient.BaseAddress?.ToString().TrimEnd('/') + "/" + endpoint.TrimStart('/');
            _logger.LogInformation($"🔗 REQUEST URL: {fullUrl}");

            var response = await _httpClient.DeleteAsync(fullUrl);

            // If 401, attempt token refresh and retry once
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var refreshed = await TryRefreshTokenAsync();
                if (refreshed)
                {
                    AddAuthHeader();
                    response = await _httpClient.DeleteAsync(fullUrl);
                }
            }

            return await HandleResponse<T>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error calling DELETE {endpoint}");
            return EvelynPhilApiResponse<T>.ErrorResponse($"Error: {ex.Message}");
        }
    }

    public async Task<EvelynPhilApiResponse> PostAsync(string endpoint, object? data = null)
    {
        try
        {
            // Restore token from storage on each request
            await RestoreTokenFromStorage();

            AddAuthHeader();

            // LOG THE FULL URL
            var fullUrl = _httpClient.BaseAddress?.ToString().TrimEnd('/') + "/" + endpoint.TrimStart('/');
            _logger.LogInformation($"🔗 REQUEST URL: {fullUrl}");

            HttpContent? content = null;
            if (data != null)
            {
                content = JsonContent.Create(data);
            }

            var response = await _httpClient.PostAsync(fullUrl, content);

            // If 401, attempt token refresh and retry once
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var refreshed = await TryRefreshTokenAsync();
                if (refreshed)
                {
                    AddAuthHeader();
                    content = data != null ? JsonContent.Create(data) : null;
                    response = await _httpClient.PostAsync(fullUrl, content);
                }
            }

            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error calling POST {endpoint}");
            return EvelynPhilApiResponse.ErrorResponse($"Error: {ex.Message}");
        }
    }

    public async Task<EvelynPhilApiResponse> DeleteAsync(string endpoint)
    {
        try
        {
            // Restore token from storage on each request
            await RestoreTokenFromStorage();

            AddAuthHeader();

            // LOG THE FULL URL
            var fullUrl = _httpClient.BaseAddress?.ToString().TrimEnd('/') + "/" + endpoint.TrimStart('/');
            _logger.LogInformation($"🔗 REQUEST URL: {fullUrl}");

            var response = await _httpClient.DeleteAsync(fullUrl);

            // If 401, attempt token refresh and retry once
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var refreshed = await TryRefreshTokenAsync();
                if (refreshed)
                {
                    AddAuthHeader();
                    response = await _httpClient.DeleteAsync(fullUrl);
                }
            }

            return await HandleDeleteResponse(response, fullUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error calling DELETE {endpoint}");
            return EvelynPhilApiResponse.ErrorResponse($"Error: {ex.Message}");
        }
    }

    public async Task SetAuthTokenAsync(string token, bool rememberMe)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Attempted to set null or empty token");
                return;
            }

            _authToken = token;
            _rememberMe = rememberMe;

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            await StoreTokenInStorageAsync(token);
            await StoreRememberMePreferenceAsync(rememberMe);

            _logger.LogInformation("Auth token set with RememberMe={RememberMe}", rememberMe);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting auth token");
        }
    }

    public void SetAuthToken(string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Attempted to set null or empty token");
                return;
            }

            _authToken = token;
            _logger.LogInformation("Auth token set successfully");

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            _ = StoreTokenInStorageAsync(token);

            _logger.LogInformation("Authorization header set with Bearer token");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting auth token");
        }
    }

    public void ClearAuthToken()
    {
        try
        {
            _authToken = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;

            _ = RemoveAllTokensFromStorageAsync();

            _logger.LogInformation("Auth token cleared");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing auth token");
        }
    }

    public string? GetAuthToken()
    {
        return _authToken;
    }

    public async Task<string?> GetRememberMeEmailAsync()
    {
        try
        {
            return await _localStorage.GetItemAsStringAsync(RememberMeEmailKey);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogDebug($"Storage not available: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving remember me email");
            return null;
        }
    }

    public async Task SetRememberMeEmailAsync(string? email)
    {
        try
        {
            if (string.IsNullOrEmpty(email))
            {
                await _localStorage.RemoveItemAsync(RememberMeEmailKey);
            }
            else
            {
                await _localStorage.SetItemAsStringAsync(RememberMeEmailKey, email);
            }
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogDebug($"Storage not available: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing remember me email");
        }
    }

    /// <summary>
    /// Attempt to refresh the access token using the stored refresh token.
    /// Uses a semaphore to prevent concurrent refresh requests.
    /// </summary>
    private async Task<bool> TryRefreshTokenAsync()
    {
        if (_isRefreshing)
            return false;

        await _refreshLock.WaitAsync();
        try
        {
            _isRefreshing = true;

            var currentAccessToken = _authToken;
            var refreshToken = await GetFromStorageAsync(RefreshTokenStorageKey);

            if (string.IsNullOrEmpty(currentAccessToken) || string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogWarning("Cannot refresh token: missing access token or refresh token");
                return false;
            }

            _logger.LogInformation("🔄 Attempting token refresh...");

            var refreshRequest = new
            {
                AccessToken = currentAccessToken,
                RefreshToken = refreshToken
            };

            var fullUrl = _httpClient.BaseAddress?.ToString() + "/auth/refresh-token";

            // Clear auth header for the refresh call to avoid sending the expired token
            _httpClient.DefaultRequestHeaders.Authorization = null;

            var response = await _httpClient.PostAsync(fullUrl, JsonContent.Create(refreshRequest));

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<EvelynPhilApiResponse<RefreshTokenResponse>>(content, options);

                if (result?.Success == true && result.Data != null)
                {
                    _authToken = result.Data.AccessToken;

                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", _authToken);

                    await StoreTokenInStorageAsync(_authToken);
                    await SetInStorageAsync(RefreshTokenStorageKey, result.Data.RefreshToken);

                    _logger.LogInformation("✅ Token refreshed successfully");
                    return true;
                }
            }

            _logger.LogWarning("❌ Token refresh failed — clearing tokens");
            ClearAuthToken();
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return false;
        }
        finally
        {
            _isRefreshing = false;
            _refreshLock.Release();
        }
    }

    /// <summary>
    /// Restore token from storage before each request.
    /// Checks both localStorage and sessionStorage to determine the active storage mode.
    /// </summary>
    private async Task RestoreTokenFromStorage()
    {
        try
        {
            await RestoreRememberMePreferenceAsync();

            var token = await GetTokenFromStorageAsync();

            if (!string.IsNullOrEmpty(token) && _authToken != token)
            {
                _authToken = token;
                _logger.LogDebug("Token restored from storage (RememberMe={RememberMe})", _rememberMe);
            }
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogDebug($"Storage not available: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring token from storage");
        }
    }

    private void AddAuthHeader()
    {
        try
        {
            if (!string.IsNullOrEmpty(_authToken))
            {
                _logger.LogDebug("Adding authorization header with token");
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _authToken);
            }
            else
            {
                _logger.LogDebug("No auth token available for this request");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding auth header");
        }
    }

    private async Task<EvelynPhilApiResponse<T>> HandleResponse<T>(HttpResponseMessage response)
    {
        try
        {
            var content = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(content))
            {
                return EvelynPhilApiResponse<T>.ErrorResponse(
                    "Empty response from server",
                    (int)response.StatusCode);
            }

            // Check if response is JSON before attempting deserialization
            if (!IsJsonContent(response.Content.Headers.ContentType?.MediaType))
            {
                _logger.LogError($"Received non-JSON response (Content-Type: {response.Content.Headers.ContentType?.MediaType}). Status: {response.StatusCode}. Response: {content.Substring(0, Math.Min(content.Length, 200))}");
                return EvelynPhilApiResponse<T>.ErrorResponse(
                    $"Server returned an error: {response.StatusCode}",
                    (int)response.StatusCode);
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<EvelynPhilApiResponse<T>>(content, options);

            return result ?? EvelynPhilApiResponse<T>.ErrorResponse(
                "Failed to deserialize response",
                (int)response.StatusCode);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing response");
            return EvelynPhilApiResponse<T>.ErrorResponse(
                "Error parsing server response",
                (int)response.StatusCode);
        }
    }

    private async Task<EvelynPhilApiResponse> HandleResponse(HttpResponseMessage response)
    {
        try
        {
            var content = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(content))
            {
                return EvelynPhilApiResponse.ErrorResponse(
                    "Empty response from server",
                    (int)response.StatusCode);
            }

            // Check if response is JSON before attempting deserialization
            if (!IsJsonContent(response.Content.Headers.ContentType?.MediaType))
            {
                _logger.LogError($"Received non-JSON response (Content-Type: {response.Content.Headers.ContentType?.MediaType}). Status: {response.StatusCode}. Response: {content.Substring(0, Math.Min(content.Length, 200))}");
                return EvelynPhilApiResponse.ErrorResponse(
                    $"Server returned an error: {response.StatusCode}",
                    (int)response.StatusCode);
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<EvelynPhilApiResponse>(content, options);

            return result ?? EvelynPhilApiResponse.ErrorResponse(
                "Failed to deserialize response",
                (int)response.StatusCode);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing response");
            return EvelynPhilApiResponse.ErrorResponse(
                "Error parsing server response",
                (int)response.StatusCode);
        }
    }

    /// <summary>
    /// Handle response and deserialize
    /// </summary>
    private async Task<EvelynPhilApiResponse<T>> HandleResponse<T>(HttpResponseMessage response, string endpoint)
    {
        try
        {
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogDebug($"Response from {endpoint}: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                if (string.IsNullOrEmpty(content))
                {
                    return EvelynPhilApiResponse<T>.SuccessResponse(default!, "Success");
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var EvelynPhilApiResponse = JsonSerializer.Deserialize<EvelynPhilApiResponse<T>>(content, options);
                return EvelynPhilApiResponse ?? EvelynPhilApiResponse<T>.SuccessResponse(default!, "Success");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _logger.LogWarning($"Unauthorized request to {endpoint} - clearing token");
                ClearAuthToken();
                return EvelynPhilApiResponse<T>.ErrorResponse("Unauthorized - please login again");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                _logger.LogWarning($"Forbidden request to {endpoint}");
                return EvelynPhilApiResponse<T>.ErrorResponse("Access denied");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning($"Not found: {endpoint}");
                return EvelynPhilApiResponse<T>.ErrorResponse("Resource not found");
            }
            else
            {
                _logger.LogError($"Error response from {endpoint}: {response.StatusCode} - {content}");
                return EvelynPhilApiResponse<T>.ErrorResponse($"Error: {response.StatusCode} - {content}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error handling response from {endpoint}");
            return EvelynPhilApiResponse<T>.ErrorResponse($"Error parsing response: {ex.Message}");
        }
    }

    /// <summary>
    /// Handle delete response
    /// </summary>
    private async Task<EvelynPhilApiResponse> HandleDeleteResponse(HttpResponseMessage response, string endpoint)
    {
        try
        {
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogDebug($"Response from {endpoint}: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                if (string.IsNullOrEmpty(content))
                {
                    return EvelynPhilApiResponse.SuccessResponse("Success");
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var apiResponse = JsonSerializer.Deserialize<EvelynPhilApiResponse>(content, options);
                return apiResponse ?? EvelynPhilApiResponse.SuccessResponse("Success");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _logger.LogWarning($"Unauthorized request to {endpoint}");
                ClearAuthToken();
                return EvelynPhilApiResponse.ErrorResponse("Unauthorized - please login again");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                _logger.LogWarning($"Forbidden request to {endpoint}");
                return EvelynPhilApiResponse.ErrorResponse("Access denied");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning($"Not found: {endpoint}");
                return EvelynPhilApiResponse.ErrorResponse("Resource not found");
            }
            else
            {
                _logger.LogError($"Error response from {endpoint}: {response.StatusCode} - {content}");
                return EvelynPhilApiResponse.ErrorResponse($"Error: {response.StatusCode} - {content}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error handling response from {endpoint}");
            return EvelynPhilApiResponse.ErrorResponse($"Error parsing response: {ex.Message}");
        }
    }


    // ========== RememberMe-Aware Storage ==========

    private async Task StoreTokenInStorageAsync(string token)
    {
        try
        {
            var expiry = DateTime.UtcNow.AddHours(2).ToString("O");

            if (_rememberMe)
            {
                await _localStorage.SetItemAsStringAsync(TokenStorageKey, token);
                await _localStorage.SetItemAsStringAsync(TokenExpiryKey, expiry);
            }
            else
            {
                await _sessionStorage.SetItemAsStringAsync(TokenStorageKey, token);
                await _sessionStorage.SetItemAsStringAsync(TokenExpiryKey, expiry);
            }

            _logger.LogDebug("Token stored (RememberMe={RememberMe})", _rememberMe);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogDebug($"Storage not available: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing token");
        }
    }

    private async Task<string?> GetTokenFromStorageAsync()
    {
        try
        {
            string? token;
            string? expiryStr;

            if (_rememberMe)
            {
                token = await _localStorage.GetItemAsStringAsync(TokenStorageKey);
                expiryStr = await _localStorage.GetItemAsStringAsync(TokenExpiryKey);
            }
            else
            {
                token = await _sessionStorage.GetItemAsStringAsync(TokenStorageKey);
                expiryStr = await _sessionStorage.GetItemAsStringAsync(TokenExpiryKey);
            }

            if (string.IsNullOrEmpty(token))
                return null;

            if (!string.IsNullOrEmpty(expiryStr) && DateTime.TryParse(expiryStr, out var expiry))
            {
                if (DateTime.UtcNow >= expiry)
                {
                    _logger.LogInformation("Token in storage has expired");
                    await RemoveAllTokensFromStorageAsync();
                    return null;
                }
            }

            return token;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogDebug($"Storage not available: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving token from storage");
            return null;
        }
    }

    private async Task SetInStorageAsync(string key, string value)
    {
        try
        {
            if (_rememberMe)
                await _localStorage.SetItemAsStringAsync(key, value);
            else
                await _sessionStorage.SetItemAsStringAsync(key, value);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogDebug($"Storage not available: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing to storage");
        }
    }

    private async Task<string?> GetFromStorageAsync(string key)
    {
        try
        {
            if (_rememberMe)
                return await _localStorage.GetItemAsStringAsync(key);
            else
                return await _sessionStorage.GetItemAsStringAsync(key);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogDebug($"Storage not available: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading from storage");
            return null;
        }
    }

    private async Task RemoveAllTokensFromStorageAsync()
    {
        try
        {
            // Clear from both storages to ensure clean state
            await _localStorage.RemoveItemAsync(TokenStorageKey);
            await _localStorage.RemoveItemAsync(TokenExpiryKey);
            await _localStorage.RemoveItemAsync(RefreshTokenStorageKey);
            await _localStorage.RemoveItemAsync(RememberMeKey);

            await _sessionStorage.RemoveItemAsync(TokenStorageKey);
            await _sessionStorage.RemoveItemAsync(TokenExpiryKey);
            await _sessionStorage.RemoveItemAsync(RefreshTokenStorageKey);

            _logger.LogInformation("All tokens removed from storage");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogDebug($"Storage not available: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing tokens from storage");
        }
    }

    private async Task StoreRememberMePreferenceAsync(bool rememberMe)
    {
        try
        {
            // Always store in localStorage so it survives session close
            await _localStorage.SetItemAsStringAsync(RememberMeKey, rememberMe.ToString());
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogDebug($"Storage not available: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing RememberMe preference");
        }
    }

    private async Task RestoreRememberMePreferenceAsync()
    {
        try
        {
            var value = await _localStorage.GetItemAsStringAsync(RememberMeKey);
            if (!string.IsNullOrEmpty(value) && bool.TryParse(value, out var rememberMe))
            {
                _rememberMe = rememberMe;
            }
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogDebug($"Storage not available: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring RememberMe preference");
        }
    }

    /// <summary>
    /// Checks if the content type indicates JSON
    /// </summary>
    private static bool IsJsonContent(string? mediaType)
    {
        if (string.IsNullOrEmpty(mediaType))
            return true; // Assume JSON if not specified

        return mediaType.Contains("application/json", StringComparison.OrdinalIgnoreCase) ||
               mediaType.Contains("text/json", StringComparison.OrdinalIgnoreCase);
    }
}
