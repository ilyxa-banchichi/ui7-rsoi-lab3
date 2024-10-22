using Common.CircuitBreaker;
using Common.Models.DTO;
using Common.Models.Enums;
using Gateway.RequestQueueService;
using Microsoft.Extensions.Logging;

namespace Gateway.Services;

public class LibraryService : BaseHttpService, ILibraryService, IRequestQueueUser
{
    public string Name => "library";
    
    private readonly IRequestQueueService _queueService;
    
    public LibraryService(
        IHttpClientFactory httpClientFactory,
        string baseUrl,
        ICircuitBreaker circuitBreaker,
        ILogger<LibraryService> logger,
        IRequestQueueService queueService) 
        : base(httpClientFactory, baseUrl, circuitBreaker, logger)
    {
        _queueService = queueService;
    }

    public async Task<LibraryPaginationResponse?> GetLibrariesInCityAsync(
        string city, int page, int size)
    {
        var method = $"/api/v1/libraries?city={city}&page={page}&size={size}";
        return await GetAsync<LibraryPaginationResponse>(method);
    }

    public async Task<LibraryBookPaginationResponse?> GetBooksInLibraryAsync(
        string libraryUid, int page, int size, bool showAll = false)
    {
        var method = $"/api/v1/libraries/{libraryUid}/books?page={page}&size={size}&showAll={showAll}";
        return await GetAsync<LibraryBookPaginationResponse>(method);
    }

    public async Task<List<LibraryResponse>?> GetLibrariesListAsync(IEnumerable<Guid> librariesUid)
    {
        var method = $"/api/v1/libraries/list";
        return await GetAsync<List<LibraryResponse>>(method,
            new Dictionary<string, string>()
            {
                { "librariesUid", string.Join(", ", librariesUid) }
            });
    }

    public async Task<List<BookInfo>?> GetBooksListAsync(IEnumerable<Guid> booksUid)
    {
        var method = $"/api/v1/libraries/books/list";
        return await GetAsync<List<BookInfo>>(method,
            new Dictionary<string, string>()
            {
                { "booksUid", string.Join(", ", booksUid) }
            });
    }
    
    public async Task<bool> TakeBookAsync(Guid libraryUid, Guid bookUid)
    {
        var method = $"/api/v1/libraries/{libraryUid}/books/{bookUid}";
        return await PatchAsync<bool>(method);
    }

    public async Task<UpdateBookConditionResponse?> ReturnBookAsync(Guid libraryUid, Guid bookUid, BookCondition condition)
    {
        var method = $"/api/v1/libraries/{libraryUid}/books/{bookUid}/return";
        return await PatchAsync<UpdateBookConditionResponse>(method, body: condition);
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