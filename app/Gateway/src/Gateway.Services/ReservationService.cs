using System.Net;
using System.Text;
using System.Text.Json;
using Common.CircuitBreaker;
using Common.Models.DTO;
using Gateway.RequestQueueService;
using Microsoft.Extensions.Logging;

namespace Gateway.Services;

public class ReservationService : BaseHttpService, IReservationService, IRequestQueueUser
{
    public string Name => "reservation";
    
    private readonly IRequestQueueService _queueService;

    public ReservationService(
        IHttpClientFactory httpClientFactory,
        string baseUrl,
        ICircuitBreaker circuitBreaker,
        ILogger<ReservationService> logger,
        IRequestQueueService queueService)
        : base(httpClientFactory, baseUrl, circuitBreaker, logger)
    {
        _queueService = queueService;
    }

    public async Task<List<RawBookReservationResponse>?> GetUserReservationsAsync(string xUserName)
    {
        var method = $"/api/v1/reservations";
        var request = new HttpRequestMessage(HttpMethod.Get, method);
        request.Headers.Add("X-User-Name", xUserName);

        return await circuitBreaker.ExecuteCommandAsync(
            async () => await SendAsync<List<RawBookReservationResponse>>(request)
        );
    }

    public async Task<RawBookReservationResponse?> TakeBook(string xUserName, TakeBookRequest body)
    {
        var method = $"/api/v1/reservations";
        var request = new HttpRequestMessage(HttpMethod.Post, method);
        request.Headers.Add("X-User-Name", xUserName);
        string jsonBody = JsonSerializer.Serialize(body);
        request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        
        return await circuitBreaker.ExecuteCommandAsync(
            async () => await SendAsync<RawBookReservationResponse>(request)
        );
    }
    
    public async Task<RawBookReservationResponse?> ReturnBook(Guid reservationUid, DateOnly date)
    {
        var method = $"/api/v1/reservations/{reservationUid}/return";
        var request = new HttpRequestMessage(HttpMethod.Patch, method);
        
        return await circuitBreaker.ExecuteCommandAsync(
            async () => await SendAsync<RawBookReservationResponse>(request)
        );
    }

    public async Task SendRequestAsync(HttpRequestMessage request)
    {
        await circuitBreaker.ExecuteCommandAsync<object>(
            async () =>
            {
                await SendAsync(request);
                return null;
            },
            fallback: async () =>
            {
                await _queueService.EnqueueRequestAsync(this, request);
                return null;
            });
    }
}