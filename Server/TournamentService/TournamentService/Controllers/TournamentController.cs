using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TournamentService.BusinessLogic.Models.Request;
using TournamentService.BusinessLogic.Models.Tournament;
using TournamentService.BusinessLogic.Services.Interfaces;
using TournamentService.Shared.Constants;

namespace TournamentService.API.Controllers
{
    [Route("api/tournaments/")]
    [ApiController]
    public class TournamentController : ControllerBase
    {
        private readonly ITournamentService _tournamentService;
        private readonly IParticipantService _participantService;
        public TournamentController(ITournamentService tournamentService, IParticipantService participantService){
            _tournamentService = tournamentService;
            _participantService = participantService;
        }
        [HttpGet]
        [Route("list/")]
        public async Task<IActionResult> GetTournamentsAsync(int page, int pageSize)
        {
            var list = await _tournamentService.GetAllByPageAsync(page, pageSize);

            return Ok(list);
        }

        [HttpPut]
        [Route("{tournamentId}")]
        [Authorize(Roles = RoleName.Organizer)]
        public async Task<IActionResult> UpdateTournamentAsync([FromRoute] string tournamentId, [FromBody] TournamentDto dto)
        {
            var newsDto = await _tournamentService.UpdateAsync(tournamentId, dto, User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value);

            return Ok(newsDto);
        }

        [HttpDelete]
        [Route("{tournamentId}")]
        [Authorize(Roles = RoleName.Organizer)]
        public async Task<IActionResult> DeleteTournamentAsync(string tournamentId)
        {
            var newsDto = await _tournamentService.DeleteAsync(tournamentId, User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value);

            return Ok(newsDto);
        }


        [HttpPost]
        [Route("")]
        [Authorize(Roles = RoleName.Organizer)]
        public async Task<IActionResult> AddTournamentAsync([FromBody] TournamentCreateDto dto)
        {
            var newsDto = await _tournamentService.AddAsync(dto);
            
            return Ok(newsDto);
        }

        [HttpGet]
        [Route("{tournamentId}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] string tournamentId)
        {
            var newsDto = await _tournamentService.GetByIdAsync(tournamentId);
            newsDto.Participants = await _participantService.GetAllByTournamentAsync(tournamentId);
            
            return Ok(newsDto);
        }
        [HttpPut]
        [Route("{tournamentId}/round")]
        [Authorize(Roles = RoleName.Organizer)]
        public async Task<IActionResult> SetNextRound([FromRoute] string tournamentId)
        {
            _tournamentService.SetNextRound(tournamentId);
            return Ok();
        }
        [HttpPut]
        [Route("{tournamentId}/{matchId}/result")]
        [Authorize(Roles = RoleName.Organizer)]
        public async Task<IActionResult> SetWinnerForMatchAsync([FromRoute] string tournamentId, [FromRoute] string matchId, [FromBody]WinRequest request)
        {
            _tournamentService.SetWinnerForMatchAsync(tournamentId, matchId, request.WinnerId, request.LooserId, request.WinPoints, request.LoosePoints);
            return Ok();
        }
        [HttpPut]
        [Route("{tournamentId}/start")]
        [Authorize(Roles = RoleName.Organizer)]
        public async Task<IActionResult> StartTournamentAsync([FromRoute] string tournamentId)
        {
            var res = await _tournamentService.StartTournamentAsync(tournamentId);
            return Ok(res);
        }
        [HttpPut]
        [Route("{tournamentId}/end")]
        [Authorize(Roles = RoleName.Organizer)]
        public async Task<IActionResult> EndTournamentAsync([FromRoute] string tournamentId)
        {
            var res = await _tournamentService.EndTournamentAsync(tournamentId);
            return Ok(res);
        }
        [HttpGet]
        [Route("{tournamentId}/bracket")]
        [Authorize(Roles = RoleName.Organizer)]
        public async Task<IActionResult> GenerateBracketAsync([FromRoute] string tournamentId)
        {
            _tournamentService.GenerateBracketAsync(tournamentId);
            return Ok();
        }
    }
}
