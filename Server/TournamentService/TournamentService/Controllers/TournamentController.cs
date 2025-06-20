using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TournamentService.BusinessLogic.Models.Filters;
using TournamentService.BusinessLogic.Models.Request;
using TournamentService.BusinessLogic.Models.Tournament;
using TournamentService.BusinessLogic.Services.Interfaces;
using TournamentService.Shared.Constants;
using TournamentService.Shared.Enums;

namespace TournamentService.API.Controllers
{
    [Route("api/tournaments/")]
    [ApiController]
    public class TournamentController : ControllerBase
    {
        private readonly ITournamentService _tournamentService;
        private readonly IParticipantService _participantService;
        public TournamentController(ITournamentService tournamentService, IParticipantService participantService)
        {
            _tournamentService = tournamentService;
            _participantService = participantService;
        }
        [HttpGet]
        [Route("list/")]
        public async Task<IActionResult> GetTournamentsAsync(int page, int pageSize, TournamentSortOptions? options, [FromQuery] TournamentFilter filter)
        {
            TournamentPagedResponse response = new TournamentPagedResponse()
            {
                Tournaments = await _tournamentService.GetByFilterAsync(filter, options, page, pageSize),
                Total = await _tournamentService.GetTotalAsync()
            };

            return Ok(response);
        }

        [HttpGet]
        [Route("{userId}/list/")]
        public async Task<IActionResult> GetTournamentsByOwnerAsync(int page, int pageSize, [FromRoute] string userId)
        {
            TournamentPagedResponse response = new TournamentPagedResponse()
            {
                Tournaments = await _tournamentService.GetByOwnerAsync(userId, page, pageSize),
                Total = await _tournamentService.GetTotalAsync()
            };

            return Ok(response);
        }

        [HttpPut]
        [Route("{tournamentId}")]
        [Authorize(Roles = RoleName.Organizer)]
        public async Task<IActionResult> UpdateTournamentAsync([FromRoute] string tournamentId, [FromBody] TournamentDto dto)
        {
            TournamentDto newsDto = await _tournamentService.UpdateAsync(tournamentId, dto, User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value);

            return Ok(newsDto);
        }

        [HttpDelete]
        [Route("{tournamentId}")]
        [Authorize(Roles = RoleName.Organizer)]
        public async Task<IActionResult> DeleteTournamentAsync(string tournamentId)
        {
            TournamentDto newsDto = await _tournamentService.DeleteAsync(tournamentId, User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value);

            return Ok(newsDto);
        }


        [HttpPost]
        [Route("")]
        [Authorize(Roles = RoleName.Organizer)]
        public async Task<IActionResult> AddTournamentAsync([FromBody] TournamentCreateDto dto)
        {
            TournamentDto newsDto = await _tournamentService.AddAsync(dto, User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value);

            return Ok(newsDto);
        }

        [HttpGet]
        [Route("{tournamentId}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] string tournamentId)
        {
            TournamentDto newsDto = await _tournamentService.GetByIdAsync(tournamentId);
            newsDto.Participants = await _participantService.GetAllByTournamentAsync(tournamentId);

            return Ok(newsDto);
        }

        [HttpPut]
        [Route("{tournamentId}/round")]
        [Authorize(Roles = RoleName.Organizer)]
        public async Task<IActionResult> SetNextRound([FromRoute] string tournamentId)
        {
            _tournamentService.SetNextRound(tournamentId, User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value);
            return Ok();
        }
        [HttpPut]
        [Route("{tournamentId}/{matchId}/result")]
        [Authorize(Roles = RoleName.Organizer)]
        public async Task<IActionResult> SetWinnerForMatchAsync([FromRoute] string tournamentId, [FromRoute] string matchId, [FromBody] WinRequest request)
        {
            _tournamentService.SetWinnerForMatchAsync(tournamentId, matchId, request.WinnerId, request.LooserId, request.WinPoints, request.LoosePoints, User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value);
            return Ok();
        }
        [HttpPut]
        [Route("{tournamentId}/start")]
        [Authorize(Roles = RoleName.Organizer)]
        public async Task<IActionResult> StartTournamentAsync([FromRoute] string tournamentId)
        {
            var res = await _tournamentService.StartTournamentAsync(tournamentId, User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value);
            return Ok(res);
        }
        [HttpPut]
        [Route("{tournamentId}/end")]
        [Authorize(Roles = RoleName.Organizer)]
        public async Task<IActionResult> EndTournamentAsync([FromRoute] string tournamentId)
        {
            var res = await _tournamentService.EndTournamentAsync(tournamentId, User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value);
            return Ok(res);
        }
        [HttpPut]
        [Route("{tournamentId}/bracket")]
        [Authorize(Roles = RoleName.Organizer)]
        public async Task<IActionResult> GenerateBracketAsync([FromRoute] string tournamentId)
        {
            _tournamentService.GenerateBracketAsync(tournamentId, User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value);
            return Ok();
        }
        [HttpPost]
        [Route("{tournamentId}/image")]
        [Authorize(Roles = RoleName.Organizer)]
        public async Task<IActionResult> AddTournamentImageAsync([FromRoute] string tournamentId, IFormFile image)
        {
            var newsDto = await _tournamentService.AddImageAsync(tournamentId, image, User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value);

            return Ok(newsDto);
        }

        [HttpDelete]
        [Route("{tournamentId}/image")]
        [Authorize(Roles = RoleName.Organizer)]
        public async Task<IActionResult> DeleteTournamentImageAsync([FromRoute] string tournamentId)
        {
            var newsDto = await _tournamentService.RemoveImageAsync(tournamentId, User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value);

            return Ok(newsDto);
        }
        
        [HttpPut]
        [Route("{tournamentId}/image")]
        [Authorize(Roles = RoleName.Organizer)]
        public async Task<IActionResult> UpdateTournamentImageAsync([FromRoute] string tournamentId, IFormFile image)
        {
            var newsDto = await _tournamentService.RemoveImageAsync(tournamentId, User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value);
            var res = await _tournamentService.AddImageAsync(tournamentId, image, User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value);

            return Ok(newsDto);
        }
    }
}
