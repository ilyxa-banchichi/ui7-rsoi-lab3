using System.Net.Http.Json;
using Common.CircuitBreaker;
using Common.Models.DTO;
using Common.Models.Enums;
using Gateway.RequestQueueService;
using Gateway.Services.Exceptions;
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
        var request = new HttpRequestMessage(HttpMethod.Get, method);
        
        return await circuitBreaker.ExecuteCommandAsync(
            async () => await SendAsync<LibraryPaginationResponse>(request)
        );
    }

    public async Task<LibraryBookPaginationResponse?> GetBooksInLibraryAsync(
        string libraryUid, int page, int size, bool showAll = false)
    {
        var method = $"/api/v1/libraries/{libraryUid}/books?page={page}&size={size}&showAll={showAll}";
        var request = new HttpRequestMessage(HttpMethod.Get, method);
        
        return await circuitBreaker.ExecuteCommandAsync(
            async () => await SendAsync<LibraryBookPaginationResponse>(request)
        );
    }

    public async Task<List<LibraryResponse>?> GetLibrariesListAsync(IEnumerable<Guid> librariesUid)
    {
        var method = $"/api/v1/libraries/list";
        var request = new HttpRequestMessage(HttpMethod.Get, method);
        request.Headers.Add("librariesUid", string.Join(", ", librariesUid));
        
        return await circuitBreaker.ExecuteCommandAsync(
            async () => await SendAsync<List<LibraryResponse>>(request),
            fallback: async () =>
            {
                var fallback = new List<LibraryResponse>(librariesUid.Count());
                foreach (var guid in librariesUid)
                    fallback.Add(new LibraryResponse() { LibraryUid = guid.ToString() });
                
                return fallback;
            }
        );
    }

    public async Task<List<BookInfo>?> GetBooksListAsync(IEnumerable<Guid> booksUid)
    {
        var method = $"/api/v1/libraries/books/list";
        var request = new HttpRequestMessage(HttpMethod.Get, method);
        request.Headers.Add("booksUid", string.Join(", ", booksUid));
        
        return await circuitBreaker.ExecuteCommandAsync(
            async () => await SendAsync<List<BookInfo>>(request),
            fallback: async () =>
            {
                var fallback = new List<BookInfo>(booksUid.Count());
                foreach (var guid in booksUid)
                    fallback.Add(new BookInfo() { BookUid = guid });
                
                return fallback;
            }
        );
    }
    
    public async Task TakeBookAsync(Guid libraryUid, Guid bookUid)
    {
        var method = $"/api/v1/libraries/{libraryUid}/books/{bookUid}";
        var request = new HttpRequestMessage(HttpMethod.Patch, method);
        
        await circuitBreaker.ExecuteCommandAsync<object?>(
            async () =>
            {
                await SendAsync<object>(request);
                return null;
            },
            fallback: () => throw new LibraryServiceUnavailableException());
    }

    public async Task<UpdateBookConditionResponse?> ReturnBookAsync(
        Guid libraryUid, Guid bookUid, BookCondition condition)
    {
        var method = $"/api/v1/libraries/{libraryUid}/books/{bookUid}/return";
        var request = new HttpRequestMessage(HttpMethod.Patch, method);
        request.Content = JsonContent.Create(condition);
        
        return await circuitBreaker.ExecuteCommandAsync(
            async () => await SendAsync<UpdateBookConditionResponse?>(request),
            fallback: async () =>
            {
                await _queueService.EnqueueRequestAsync(this, request);
                return new UpdateBookConditionResponse()
                {
                    LibraryUid = libraryUid,
                    BookUid = bookUid,
                    NewCondition = condition,
                    OldCondition = condition,
                };
            }
        );  
    }

    public async Task SendRequestAsync(HttpRequestMessage request)
    {
        await circuitBreaker.ExecuteCommandAsync<object?>(
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