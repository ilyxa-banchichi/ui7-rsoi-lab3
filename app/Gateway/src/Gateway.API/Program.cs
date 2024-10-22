using Common.CircuitBreaker;
using Common.Models.Serialization;
using Gateway.Services;

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

builder.Services.AddSingleton<ICircuitBreaker<LibraryService>, CircuitBreaker<LibraryService>>();
builder.Services.AddSingleton<ICircuitBreaker<ReservationService>, CircuitBreaker<ReservationService>>();
builder.Services.AddSingleton<ICircuitBreaker<RatingService>, CircuitBreaker<RatingService>>();

builder.Services.AddTransient<ILibraryService, LibraryService>(provider =>
{
    return new LibraryService(
        httpClientFactory: provider.GetRequiredService<IHttpClientFactory>(), 
        baseUrl: builder.Configuration.GetConnectionString("LibraryService"),
        circuitBreaker: provider.GetRequiredService<ICircuitBreaker<LibraryService>>(),
        logger: provider.GetRequiredService<ILogger<LibraryService>>()
    );
});

builder.Services.AddTransient<IReservationService, ReservationService>(provider =>
{
    return new ReservationService(
        httpClientFactory: provider.GetRequiredService<IHttpClientFactory>(),
        baseUrl: builder.Configuration.GetConnectionString("ReservationService"),
        circuitBreaker: provider.GetRequiredService<ICircuitBreaker<ReservationService>>(),
        logger: provider.GetRequiredService<ILogger<ReservationService>>()
    );
});

builder.Services.AddTransient<IRatingService, RatingService>(provider =>
{
    return new RatingService(
        httpClientFactory: provider.GetRequiredService<IHttpClientFactory>(), 
        baseUrl: builder.Configuration.GetConnectionString("RatingService"),
        circuitBreaker: provider.GetRequiredService<ICircuitBreaker<RatingService>>(),
        logger: provider.GetRequiredService<ILogger<RatingService>>()
    );
});

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