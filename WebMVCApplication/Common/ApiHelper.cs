using Microsoft.Extensions.Options;
using System.Net.Http;
using System;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;

namespace WebMVCApplication.Common;

public interface IApiHelper
{
    Task<HttpResponseMessage> CallGetApi(string endpoint, string? bearerToken);
    Task<HttpResponseMessage> CallPostApi(string endpoint, object? data, string? bearerToken);
    Task<HttpResponseMessage> CallPutApi(string endpoint, object? data, string? bearerToken);
    Task<HttpResponseMessage> CallDeleteApi(string endpoint, string? bearerToken);
}

public class ApiHelper : IApiHelper
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ApiHelper(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<HttpResponseMessage> CallGetApi(string endpoint, string? bearerToken)
    {
        var client = _httpClientFactory.CreateClient("MyApi");
        if (bearerToken != null && bearerToken != string.Empty)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }
       return await client.GetAsync(endpoint);
    }

    public async Task<HttpResponseMessage> CallPostApi(string endpoint, object? data, string? bearerToken)
    {
        var client = _httpClientFactory.CreateClient("MyApi");
        if(bearerToken != null && bearerToken != string.Empty)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }
        var jsonContent = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
        return await client.PostAsync(endpoint, jsonContent);
    }

    public async Task<HttpResponseMessage> CallPutApi(string endpoint, object? data, string? bearerToken)
    {
        var client = _httpClientFactory.CreateClient("MyApi");
        if (bearerToken != null && bearerToken != string.Empty)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }
        var jsonContent = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
        return await client.PutAsync(endpoint, jsonContent);
    }

    public async Task<HttpResponseMessage> CallDeleteApi(string endpoint, string? bearerToken)
    {
        var client = _httpClientFactory.CreateClient("MyApi");
        if (bearerToken != null && bearerToken != string.Empty)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }
        return await client.DeleteAsync(endpoint);
    }
}
