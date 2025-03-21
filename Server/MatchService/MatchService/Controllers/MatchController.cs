using System.IdentityModel.Tokens.Jwt;
using MatchService.BusinessLogic.Models.Filter;
using MatchService.BusinessLogic.Models.Match;
using MatchService.BusinessLogic.Services;
using MatchService.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace MatchService.API.Controllers
{
    [Route("api/matches")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly MatchService.BusinessLogic.Services.MatchService _matchService;
        public MatchController(MatchService.BusinessLogic.Services.MatchService matchService){
            _matchService = matchService;
        }

        [HttpGet]
        [Route("list/")]
        public async Task<IActionResult> GetMatchesAsync([FromRoute]int page, [FromRoute]int pageSize)
        {
            var list = await _matchService.GetAllByPageAsync(page, pageSize);

            return Ok(list);
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = RoleName.Organizer)]
        public async Task<IActionResult> UpdateMatchAsync([FromRoute] string id, [FromBody] MatchDto dto)
        {
            var newsDto = await _matchService.UpdateAsync(id, dto, User.Claims.First(x => x.Type.Equals(JwtRegisteredClaimNames.Jti)).Value);

            return Ok(newsDto);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = RoleName.Organizer)]
        public async Task<IActionResult> DeleteMatchAsync(string Id)
        {
            var newsDto = await _matchService.DeleteAsync(Id, User.Claims.First(x => x.Type.Equals(JwtRegisteredClaimNames.Jti)).Value);

            return Ok(newsDto);
        }


        [HttpPost]
        [Route("")]
        [Authorize(Roles = RoleName.Organizer)]
        public async Task<IActionResult> AddMatchAsync([FromBody] MatchDto dto)
        {
            var newsDto = await _matchService.AddAsync(dto);
            
            return Ok(newsDto);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            var newsDto = await _matchService.GetByIdAsync(id);
            
            return Ok(newsDto);
        }

        [HttpGet]
        [Route("filter/")]
        public async Task<IActionResult> GetByFilterAsync(int page, int pageSize, [FromBody] MatchFilter filter)
        {
            var newsDto = await _matchService.GetByFilterAsync(filter, page, pageSize);
            
            return Ok(newsDto);
        }

        [HttpGet]
        [Route("tournament/")]
        public async Task<IActionResult> GetTournamentStructureAsync([FromRoute]string TournamentId)
        {
            var newsDto = await _matchService.GetTournamentStructureAsync(TournamentId);
            
            return Ok(newsDto);
        }

        [HttpPost]
        [Route("winner/")]
        [Authorize(Roles = RoleName.Organizer)]
        public async Task<IActionResult> SetMatchWinner([FromBody]string matchId, [FromBody]string winnerId, [FromBody]int winPoints, [FromBody]int loosePoints){
            var match = await _matchService.SetWinnerAsync(matchId, winnerId, winPoints, loosePoints, User.Claims.First(x => x.Type.Equals(JwtRegisteredClaimNames.Jti)).Value);

            return Ok(match);
        }
    }
}
