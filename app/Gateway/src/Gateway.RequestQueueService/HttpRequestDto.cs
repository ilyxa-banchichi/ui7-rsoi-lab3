namespace Gateway.RequestQueueService;

public class HttpRequestDto
{
    public string Method { get; set; }
    public string RequestUri { get; set; }
    public string Headers { get; set; }
    public string Content { get; set; }

    public static HttpRequestDto FromHttpRequestMessage(HttpRequestMessage request)
    {
        return new HttpRequestDto
        {
            Method = request.Method.ToString(),
            RequestUri = request.RequestUri.ToString(),
            Headers = request.Headers.ToString(),
            Content = request.Content != null ? request.Content.ReadAsStringAsync().Result : null
        };
    }

    public static HttpRequestMessage FromDto(HttpRequestDto requestDto)
    {
        var requestMessage = new HttpRequestMessage(new HttpMethod(requestDto.Method), requestDto.RequestUri);
        
        foreach (var header in requestDto.Headers.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = header.Split(new[] { ": " }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                requestMessage.Headers.TryAddWithoutValidation(parts[0], parts[1]);
            }
        }

        if (!string.IsNullOrEmpty(requestDto.Content))
        {
            requestMessage.Content = new StringContent(requestDto.Content);
        }

        return requestMessage;
    }
}