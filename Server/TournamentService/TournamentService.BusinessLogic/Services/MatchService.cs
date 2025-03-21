using System;
using System.Text;
using System.Text.Json;
using TournamentService.BusinessLogic.Models.Match;
using TournamentService.BusinessLogic.Services.Interfaces;

namespace TournamentService.BusinessLogic.Services;

public class MatchService : IMatchService
{
    private HttpClient _httpClient = new HttpClient();
    public async void CreateMatches(List<MatchDto> matches)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"http://microservice3:5008/api/");//Написать адрес эндпоинта
        request.Content = new StringContent(JsonSerializer.Serialize(matches), Encoding.UTF8, "application/json");
        var response = await _httpClient.SendAsync(request);
    }

    public async Task<MatchDto> GetMatchById(string matchId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"http://microservice3:5008/api/");//Написать адрес эндпоинта
        var response = await _httpClient.SendAsync(request);
        string strResponse = await response.Content.ReadAsStringAsync();
        var tasks = JsonSerializer.Deserialize<MatchDto>(strResponse);
        return tasks;
    }

    public async Task<MatchDto> GetMatchByName(string name)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"http://microservice3:5008/api/");//Написать адрес эндпоинта
        var response = await _httpClient.SendAsync(request);
        string strResponse = await response.Content.ReadAsStringAsync();
        var tasks = JsonSerializer.Deserialize<MatchDto>(strResponse);
        return tasks;
    }

    public async Task UpdateMatch(string matchId, MatchDto match)
    {
        match.Id = matchId;
        var request = new HttpRequestMessage(HttpMethod.Get, $"http://microservice3:5008/api/");//Написать адрес эндпоинта
        request.Content = new StringContent(JsonSerializer.Serialize(match), Encoding.UTF8, "application/json");
        var response = await _httpClient.SendAsync(request);
    }
}
