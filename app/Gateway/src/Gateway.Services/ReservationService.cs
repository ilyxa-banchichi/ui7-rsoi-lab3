using System.Net;
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
        return await GetAsync<List<RawBookReservationResponse>>(method,
            new Dictionary<string, string>()
            {
                { "X-User-Name", xUserName }
            });
    }

    public async Task<RawBookReservationResponse?> TakeBook(string xUserName, TakeBookRequest body)
    {
        var method = $"/api/v1/reservations";
        return await PostAsync<RawBookReservationResponse>(method, body,
            new Dictionary<string, string>()
            {
                { "X-User-Name", xUserName }
            });
    }
    
    public async Task<RawBookReservationResponse?> ReturnBook(Guid reservationUid, DateOnly date)
    {
        try
        {
            var method = $"/api/v1/reservations/{reservationUid}/return";
            return await PatchAsync<RawBookReservationResponse>(method, date);
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == HttpStatusCode.NotFound)
                return null;
            
            throw;
        }
    }

    public async Task SendRequestAsync(HttpRequestMessage request)
    {
        await circuitBreaker.ExecuteCommandAsync<object>(
            async () =>
            {
                await base.SendAsync(request);
                return null;
            },
            fallback: async () =>
            {
                await _queueService.EnqueueRequestAsync(this, request);
                return null;
            });
    }
}