using System;
using TournamentService.BusinessLogic.Models.Match;
using TournamentService.BusinessLogic.Services.Interfaces;
using TournamentService.Shared.Constants;
using TournamentService.Shared.Enums;
using TournamentService.Shared.Exceptions;

namespace TournamentService.BusinessLogic.Services.Tournaments;

public class DoubleEliminationBracket
{
    // private readonly IMatchService _matchService;

    // public DoubleEliminationBracket(IMatchService matchService)
    // {
    //     _matchService = matchService;
    // }

    // public async Task GenerateBracket(string tournamentId, List<string> participants)
    // {
    //     if (participants.Count < 2) throw new Exception("Недостаточно участников!");

    //     int totalRounds = (int)Math.Ceiling(Math.Log2(participants.Count)); // Количество раундов в верхней сетке
    //     int totalMatches = (int)(Math.Pow(2, totalRounds) - 1); // Всего матчей в верхней сетке

    //     List<MatchDto> matches = new List<MatchDto>();

    //     // 📌 Генерация первой стадии (Winners Bracket - Верхняя сетка)
    //     for (int i = 0; i < participants.Count / 2; i++)
    //     {
    //         matches.Add(new MatchDto
    //         {
    //             TournamentId = tournamentId,
    //             Round = "1",
    //             Bracket = "Winners",
    //             Participant1Id = participants[i * 2],
    //             Participant2Id = participants[i * 2 + 1],
    //             Status = MatchStatus.Scheduled
    //         });
    //     }

    //     _matchService.CreateMatches(matches);
    // }

    // public async Task HandleMatchResult(string matchId, string winnerId, string loserId)
    // {
    //     var match = await _matchService.GetMatchById(matchId);
    //     if (match == null) throw new NotFoundException(ErrorName.MatchNotFound);

    //     match.Status = MatchStatus.Completed;
    //     match.WinnerId = winnerId;
    //     match.LoserId = loserId;

    //     await _matchService.UpdateMatch(match);

    //     // 📌 Проверяем, куда идет победитель и проигравший
    //     if (match.Bracket == "Winners")
    //     {
    //         await AdvanceInWinners(match, winnerId);
    //         await DropToLosers(match, loserId);
    //     }
    //     else if (match.Bracket == "Losers")
    //     {
    //         await AdvanceInLosers(match, winnerId);
    //     }
    // }

    // private async Task AdvanceInWinners(MatchDto match, string winnerId)
    // {
    //     int nextRound = int.Parse(match.Round) + 1;

    //     var nextMatch = await _matchService.FindMatchByRound(match.TournamentId, nextRound, "Winners");
    //     if (nextMatch != null)
    //     {
    //         nextMatch.Participants.Add(winnerId);
    //         await _matchService.UpdateMatch(nextMatch);
    //     }
    //     else
    //     {
    //         // Создаем новый матч
    //         await _matchService.CreateMatch(new MatchDto
    //         {
    //             TournamentId = match.TournamentId,
    //             Round = nextRound.ToString(),
    //             Bracket = "Winners",
    //             Participant1Id = winnerId,
    //             Status = MatchStatus.Scheduled
    //         });
    //     }
    // }

    // private async Task DropToLosers(MatchDto match, string loserId)
    // {
    //     int round = int.Parse(match.Round) * 2 - 1; // В нижней сетке раунды идут быстрее

    //     var nextMatch = await _matchService.FindMatchByRound(match.TournamentId, round, "Losers");
    //     if (nextMatch != null)
    //     {
    //         nextMatch.Participants.Add(loserId);
    //         await _matchService.UpdateMatch(nextMatch);
    //     }
    //     else
    //     {
    //         await _matchService.CreateMatch(new MatchDto
    //         {
    //             TournamentId = match.TournamentId,
    //             Round = round.ToString(),
    //             Bracket = "Losers",
    //             Participant1Id = loserId,
    //             Status = MatchStatus.Scheduled
    //         });
    //     }
    // }

    // private async Task AdvanceInLosers(MatchDto match, string winnerId)
    // {
    //     int nextRound = int.Parse(match.Round) + 1;

    //     var nextMatch = await _matchService.FindMatchByRound(match.TournamentId, nextRound, "Losers");
    //     if (nextMatch != null)
    //     {
    //         nextMatch.Participants.Add(winnerId);
    //         await _matchService.UpdateMatch(nextMatch);
    //     }
    //     else
    //     {
    //         await _matchService.CreateMatch(new MatchDto
    //         {
    //             TournamentId = match.TournamentId,
    //             Round = nextRound.ToString(),
    //             Bracket = "Losers",
    //             Participant1Id = winnerId,
    //             Status = MatchStatus.Scheduled
    //         });
    //     }
    // }
}
