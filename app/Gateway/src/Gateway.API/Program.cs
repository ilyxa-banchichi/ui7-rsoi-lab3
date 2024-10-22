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

builder.Services.AddTransient<ILibraryService, LibraryService>(provider =>
{
    return new LibraryService(
        httpClientFactory: provider.GetRequiredService<IHttpClientFactory>(), 
        baseUrl: builder.Configuration.GetConnectionString("LibraryService"),
        circuitBreaker: provider.GetRequiredService<ICircuitBreaker<ILibraryService>>(),
        logger: provider.GetRequiredService<ILogger<LibraryService>>()
    );
});

builder.Services.AddTransient<IReservationService, ReservationService>(provider =>
{
    return new ReservationService(
        httpClientFactory: provider.GetRequiredService<IHttpClientFactory>(),
        baseUrl: builder.Configuration.GetConnectionString("ReservationService"),
        circuitBreaker: provider.GetRequiredService<ICircuitBreaker<IReservationService>>(),
        logger: provider.GetRequiredService<ILogger<ReservationService>>()
    );
});

builder.Services.AddTransient<IRatingService, RatingService>(provider =>
{
    return new RatingService(
        httpClientFactory: provider.GetRequiredService<IHttpClientFactory>(), 
        baseUrl: builder.Configuration.GetConnectionString("RatingService"),
        circuitBreaker: provider.GetRequiredService<ICircuitBreaker<IRatingService>>(),
        logger: provider.GetRequiredService<ILogger<RatingService>>(),
        queueService: provider.GetRequiredService<IRequestQueueService>()
    );
});

var redisConnection = builder.Configuration.GetConnectionString("RedisQueue");
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnection));

builder.Services.AddTransient<IRequestQueueService, RequestQueueService>();
builder.Services.AddHostedService<RequestQueueJob>();

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