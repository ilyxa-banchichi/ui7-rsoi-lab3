using System.Net.Http.Json;
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
        request.Content = JsonContent.Create(body);
        
        return await circuitBreaker.ExecuteCommandAsync(
            async () => await SendAsync<RawBookReservationResponse>(request)
        );
    }
    
    public async Task TakeBookRollback(Guid reservationGuid)
    {
        var method = $"/api/v1/reservations/{reservationGuid}/rollback";
        var request = new HttpRequestMessage(HttpMethod.Delete, method);
        
        await circuitBreaker.ExecuteCommandAsync<object?>(
            async () =>
            {
                await SendAsync(request);
                return null;
            },
            fallback: async () =>
            {
                await _queueService.EnqueueRequestAsync(this, request);
                return null;
            }
        );
    }
    
    public async Task<RawBookReservationResponse?> ReturnBook(Guid reservationUid, DateOnly date)
    {
        var method = $"/api/v1/reservations/{reservationUid}/return";
        var request = new HttpRequestMessage(HttpMethod.Patch, method);
        request.Content = JsonContent.Create(date);
        
        return await circuitBreaker.ExecuteCommandAsync(
            async () => await SendAsync<RawBookReservationResponse>(request)
        );
    }

    public async Task SendRequestAsync(HttpRequestMessage request)
    {
        await circuitBreaker.ExecuteCommandAsync<object?>(
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