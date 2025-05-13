using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TournamentService.BusinessLogic.Models.ParticipantDtos;
using TournamentService.BusinessLogic.Services.Interfaces;
using TournamentService.Shared.Constants;
using TournamentService.Shared.Enums;

namespace TournamentService.API.Controllers
{
    [Route("api/tournaments/{tournamentId}/participants")]
    [ApiController]
    public class ParticipantController : ControllerBase
    {
        private readonly IParticipantService _participantService;
        public ParticipantController(IParticipantService participantService){
            _participantService = participantService;
        }
        [HttpGet]
        [Route("list/")]
        public async Task<IActionResult> GetParticipantsAsync([FromRoute]string tournamentId, ParticipantSortOptions? options, int page, int pageSize)
        {
            var list = await _participantService.GetAllByPageAsync(tournamentId, options, page, pageSize);

            return Ok(list);
        }

        [HttpGet]
        [Route("plays/")]
        public async Task<IActionResult> GetPlayingParticipantsAsync([FromRoute]string tournamentId)
        {
            var list = await _participantService.GetPlayingByTournamentAsync(tournamentId);

            return Ok(list);
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = RoleName.Organizer)]
        public async Task<IActionResult> UpdateParticipantAsync([FromRoute] string id, [FromBody] ParticipantDto dto)
        {
            ParticipantDto newsDto = await _participantService.UpdateAsync(id, dto, User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value);

            return Ok(newsDto);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = RoleName.Organizer)]
        public async Task<IActionResult> DeleteParticipantAsync(string Id)
        {
            ParticipantDto newsDto = await _participantService.DeleteAsync(Id, User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value);

            return Ok(newsDto);
        }


        [HttpPost]
        [Route("")]
        [Authorize(Roles = RoleName.Organizer)]
        public async Task<IActionResult> AddParticipantAsync([FromRoute]string tournamentId, [FromBody] ParticipantAddDto dto)
        {

            var newsDto = await _participantService.AddAsync(dto, tournamentId);
            
            return Ok(newsDto);
        }

        [HttpPost]
        [Route("register")]
        [Authorize(Roles = RoleName.Organizer)]
        public async Task<IActionResult> RegisterForTournamentAsync([FromRoute]string tournamentId, [FromBody] string userName)
        {
            RegisterForTournamentDto dto = new RegisterForTournamentDto(){UserId = User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value, Name = userName};
            var newsDto = await _participantService.RegisterAsync(dto, tournamentId);
            
            return Ok(newsDto);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            var newsDto = await _participantService.GetByIdAsync(id);
            
            return Ok(newsDto);
        }
    }
}
