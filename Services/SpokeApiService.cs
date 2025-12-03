using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NYR.API.Models.Configuration;
using NYR.API.Models.DTOs;
using NYR.API.Services.Interfaces;
using System.Net.Http.Headers;
using System.Text;

namespace NYR.API.Services
{
    public class SpokeApiService : ISpokeApiService
    {
        private readonly HttpClient _httpClient;
        private readonly SpokeApiSettings _settings;
        private readonly ILogger<SpokeApiService> _logger;

        public SpokeApiService(
            HttpClient httpClient,
            IOptions<SpokeApiSettings> settings,
            ILogger<SpokeApiService> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;

            // Configure HttpClient
            _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds);
        }

        public async Task<SpokeRouteOptimizationResponse> OptimizeRouteAsync(string planId)
        {
            try
            {
                _logger.LogInformation("Optimizing plan {PlanId}", planId);

                var apiKey = "cpMWZhCWoyfUH4YV2O0w"; // ?? replace with actual key
                var basicAuth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{apiKey}:"));

                using var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"https://api.getcircuit.com/public/v0.2b/plans/{planId}:optimize");
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await _httpClient.SendAsync(requestMessage);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<SpokeRouteOptimizationResponse>(responseContent);
                    if (result != null)
                    {
                        result.Success = true;
                        _logger.LogInformation("Route optimization successful - {NumStops} stops optimized", result.Result?.NumOptimizedStops ?? 0);
                    }
                    return result ?? new SpokeRouteOptimizationResponse { Success = false, Message = "Empty response" };
                }
                else
                {
                    _logger.LogError("Route optimization failed: {StatusCode} - {Response}", response.StatusCode, responseContent);
                    return new SpokeRouteOptimizationResponse
                    {
                        Success = false,
                        Message = $"API Error: {response.StatusCode} - {responseContent}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during route optimization");
                return new SpokeRouteOptimizationResponse
                {
                    Success = false,
                    Message = $"Exception: {ex.Message}"
                };
            }
        }

        public async Task<SpokeTrackingResponse> UpdateTrackingAsync(SpokeTrackingUpdate update)
        {
            try
            {
                _logger.LogInformation("Updating tracking for route {RouteId}, stop {StopId}", update.RouteId, update.StopId);

                var json = JsonConvert.SerializeObject(update);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"/routes/{update.RouteId}/tracking", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<SpokeTrackingResponse>(responseContent);
                    _logger.LogInformation("Tracking update successful");
                    return result ?? new SpokeTrackingResponse { Success = false, Message = "Empty response" };
                }
                else
                {
                    _logger.LogError("Tracking update failed: {StatusCode} - {Response}", response.StatusCode, responseContent);
                    return new SpokeTrackingResponse
                    {
                        Success = false,
                        Message = $"API Error: {response.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during tracking update");
                return new SpokeTrackingResponse
                {
                    Success = false,
                    Message = $"Exception: {ex.Message}"
                };
            }
        }

        public async Task<SpokeOptimizedRoute?> GetRouteDetailsAsync(string routeId)
        {
            try
            {
                _logger.LogInformation("Getting route details for {RouteId}", routeId);

                var response = await _httpClient.GetAsync($"/routes/{routeId}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<SpokeOptimizedRoute>(responseContent);
                    _logger.LogInformation("Route details retrieved successfully");
                    return result;
                }
                else
                {
                    _logger.LogError("Failed to get route details: {StatusCode} - {Response}", response.StatusCode, responseContent);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while getting route details");
                return null;
            }
        }

        public async Task<bool> CancelRouteAsync(string routeId)
        {
            try
            {
                _logger.LogInformation("Cancelling route {RouteId}", routeId);

                var response = await _httpClient.DeleteAsync($"/routes/{routeId}");

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Route cancelled successfully");
                    return true;
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to cancel route: {StatusCode} - {Response}", response.StatusCode, responseContent);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while cancelling route");
                return false;
            }
        }

        public async Task<SpokePlanResponse> CreatePlanAsync(CreateSpokePlanRequest request)
        {
            try
            {
                _logger.LogInformation("Creating plan: {Title} for {StartDate}", request.Title, request.StartDate);

                var json = JsonConvert.SerializeObject(new
                {
                    title = request.Title,
                    starts = new
                    {
                        day = request.StartDate.Day,
                        month = request.Month ?? request.StartDate.Month,
                        year = request.Year ?? request.StartDate.Year
                    },
                    drivers = new[] { "drivers/QOHOmwlwieBQMdYLKwmX" }
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                // API Key Authentication  (Basic Auth: key as username, blank password)
                var apiKey = "cpMWZhCWoyfUH4YV2O0w"; // ?? replace with actual key
                var basicAuth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{apiKey}:")); ;
                // NOTE: prefer per-request headers rather than DefaultRequestHeaders when using a shared HttpClient.
                using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.getcircuit.com/public/v0.2b/plans")
                {
                    Content = content
                };
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await _httpClient.SendAsync(requestMessage);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<SpokePlanResponse>(responseContent);
                    result.Success = true;
                    _logger.LogInformation("Plan created successfully");
                    return result ?? new SpokePlanResponse { Success = false, Message = "Empty response" };
                }
                else
                {
                    _logger.LogError("Plan creation failed: {StatusCode} - {Response}", response.StatusCode, responseContent);
                    return new SpokePlanResponse
                    {
                        Success = false,
                        Message = $"API Error: {response.StatusCode} - {responseContent}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during plan creation");
                return new SpokePlanResponse
                {
                    Success = false,
                    Message = $"Exception: {ex.Message}"
                };
            }
        }

        public async Task<SpokeStopsBatchImportResponse> ImportStopsAsync(string planId, List<SpokeStopImport> stops)
        {
            try
            {
                _logger.LogInformation("Importing {Count} stops to plan {PlanId}", stops.Count, planId);

                // Format stops according to Spoke API structure
                var stopsData = stops.Select(s => new
                {
                    address = new
                    {
                        addressLineOne = s.Address.AddressLineOne,
                        //addressLineTwo = s.Address.AddressLineTwo,
                        //city = s.Address.City,
                        //state = s.Address.State,
                        //zipCode = s.Address.ZipCode,
                        //country = s.Address.Country
                    },
                    //customer_name = s.CustomerName,
                    //contact_phone = s.ContactPhone,
                    //notes = s.Notes,
                    //service_time = s.ServiceTime,
                    //time_window_start = s.TimeWindowStart,
                    //time_window_end = s.TimeWindowEnd
                }).ToList();

                // Serialize as array directly (not wrapped in object)
                var json = JsonConvert.SerializeObject(stopsData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var apiKey = "cpMWZhCWoyfUH4YV2O0w"; // ?? replace with actual key
                var basicAuth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{apiKey}:")); ;
                // NOTE: prefer per-request headers rather than DefaultRequestHeaders when using a shared HttpClient.
                using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.getcircuit.com/public/v0.2b/plans")
                {
                    Content = content
                };
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Use the correct endpoint format: /plans/{planId}/stops:import
                var response = await _httpClient.PostAsync($"https://api.getcircuit.com/public/v0.2b/plans/{planId}/stops:import", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Stops imported successfully");
                    var result = JsonConvert.DeserializeObject<ImportStopsResult>(responseContent);
                    return new SpokeStopsBatchImportResponse { stops = result.success, Success = true, Message = "Empty response" };
                }
                else
                {
                    _logger.LogError("Stops import failed: {StatusCode} - {Response}", response.StatusCode, responseContent);
                    return new SpokeStopsBatchImportResponse
                    {
                        Success = false,
                        Message = $"API Error: {response.StatusCode}",
                        Errors = new List<string> { responseContent }
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during stops import");
                return new SpokeStopsBatchImportResponse
                {
                    Success = false,
                    Message = $"Exception: {ex.Message}",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<GetDriversResponse> GetDriversAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving list of drivers from Circuit");

                var apiKey = "cpMWZhCWoyfUH4YV2O0w"; // ?? replace with actual key
                var basicAuth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{apiKey}:"));

                using var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://api.getcircuit.com/public/v0.2b/drivers");
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await _httpClient.SendAsync(requestMessage);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<CircuitDriversApiResponse>(responseContent);
                    _logger.LogInformation("Retrieved {Count} drivers successfully", apiResponse?.Drivers?.Count ?? 0);
                    return new GetDriversResponse
                    {
                        Success = true,
                        Drivers = apiResponse?.Drivers ?? new List<CircuitDriver>(),
                        NextPageToken = apiResponse?.NextPageToken
                    };
                }
                else
                {
                    _logger.LogError("Failed to retrieve drivers: {StatusCode} - {Response}", response.StatusCode, responseContent);
                    return new GetDriversResponse
                    {
                        Success = false,
                        Message = $"API Error: {response.StatusCode} - {responseContent}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while retrieving drivers");
                return new GetDriversResponse
                {
                    Success = false,
                    Message = $"Exception: {ex.Message}"
                };
            }
        }

        public async Task<GetPlanStopsResponse> GetPlanStopsAsync(string planId)
        {
            try
            {
                _logger.LogInformation("Retrieving stops for plan {PlanId}", planId);

                var apiKey = "cpMWZhCWoyfUH4YV2O0w"; // ?? replace with actual key
                var basicAuth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{apiKey}:"));

                using var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"https://api.getcircuit.com/public/v0.2b/plans/{planId}/stops");
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await _httpClient.SendAsync(requestMessage);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<PlanStopsApiResponse>(responseContent);
                    _logger.LogInformation("Retrieved {Count} stops successfully", apiResponse?.Stops?.Count ?? 0);
                    return new GetPlanStopsResponse
                    {
                        Success = true,
                        Stops = apiResponse?.Stops ?? new List<PlanStop>(),
                        NextPageToken = apiResponse?.NextPageToken
                    };
                }
                else
                {
                    _logger.LogError("Failed to retrieve plan stops: {StatusCode} - {Response}", response.StatusCode, responseContent);
                    return new GetPlanStopsResponse
                    {
                        Success = false,
                        Message = $"API Error: {response.StatusCode} - {responseContent}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while retrieving plan stops");
                return new GetPlanStopsResponse
                {
                    Success = false,
                    Message = $"Exception: {ex.Message}"
                };
            }
        }
    }
}
