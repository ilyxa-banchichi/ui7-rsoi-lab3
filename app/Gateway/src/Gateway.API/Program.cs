using Common.CircuitBreaker;
using Common.Models.Serialization;
using Gateway.RequestQueueService;
using Gateway.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen(options =>
{
    var basePath = AppContext.BaseDirectory;
    var xmlPath = Path.Combine(basePath, "Gateway.API.xml");
    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        corsBuilder => corsBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
    });

builder.Services.AddSingleton<ICircuitBreaker<ILibraryService>, CircuitBreaker<ILibraryService>>();
builder.Services.AddSingleton<ICircuitBreaker<IReservationService>, CircuitBreaker<IReservationService>>();
builder.Services.AddSingleton<ICircuitBreaker<IRatingService>, CircuitBreaker<IRatingService>>();

var GetLibraryService = (IServiceProvider provider) =>
{
    return new LibraryService(
        httpClientFactory: provider.GetRequiredService<IHttpClientFactory>(), 
        baseUrl: "http://library-service:8060",
        circuitBreaker: provider.GetRequiredService<ICircuitBreaker<ILibraryService>>(),
        logger: provider.GetRequiredService<ILogger<LibraryService>>(),
        queueService: provider.GetRequiredService<IRequestQueueService>()
    );
};
builder.Services.AddTransient<ILibraryService, LibraryService>(provider => GetLibraryService(provider));
builder.Services.AddTransient<IRequestQueueUser, LibraryService>(provider => GetLibraryService(provider));

var GetReservationService = (IServiceProvider provider) =>
{
    return new ReservationService(
        httpClientFactory: provider.GetRequiredService<IHttpClientFactory>(),
        baseUrl: "http://reservation-service:8070",
        circuitBreaker: provider.GetRequiredService<ICircuitBreaker<IReservationService>>(),
        logger: provider.GetRequiredService<ILogger<ReservationService>>(),
        queueService: provider.GetRequiredService<IRequestQueueService>()
    );
};
builder.Services.AddTransient<IReservationService, ReservationService>(provider => GetReservationService(provider));
builder.Services.AddTransient<IRequestQueueUser, ReservationService>(provider => GetReservationService(provider));

var GetRatingService = (IServiceProvider provider) =>
{
    return new RatingService(httpClientFactory: provider.GetRequiredService<IHttpClientFactory>(),
        baseUrl: "http://rating-service:8050",
        circuitBreaker: provider.GetRequiredService<ICircuitBreaker<IRatingService>>(),
        logger: provider.GetRequiredService<ILogger<RatingService>>(),
        queueService: provider.GetRequiredService<IRequestQueueService>());
};
builder.Services.AddTransient<IRatingService, RatingService>(provider => GetRatingService(provider));
builder.Services.AddTransient<IRequestQueueUser, RatingService>(provider => GetRatingService(provider));

var redisConnection = "redis-master:6379,password=redis_pass,abortConnect=false";
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnection));

builder.Services.AddTransient<IRequestQueueService, RequestQueueService>();
builder.Services.AddHostedService<RequestQueueJob>();

builder.Services.Configure<RequestQueueConfig>(builder.Configuration.GetSection("RequestQueueConfig"));
builder.Services.Configure<CircuitBreakerConfig>(builder.Configuration.GetSection("CircuitBreakerConfig"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.UseCors("AllowAllOrigins");

app.Run();