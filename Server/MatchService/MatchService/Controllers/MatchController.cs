using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MatchService.BusinessLogic.Models.Filter;
using MatchService.BusinessLogic.Models.Match;
using MatchService.BusinessLogic.Services;
using MatchService.BusinessLogic.Services.Interfaces;
using MatchService.Shared.Constants;
using MatchService.Shared.Enums;
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
        private readonly IMatchService _matchService;
        public MatchController(IMatchService matchService){
            _matchService = matchService;
        }

        [HttpGet]
        [Route("list/")]
        public async Task<IActionResult> GetMatchesAsync(int page, int pageSize)
        {
            List<MatchListDto> list = await _matchService.GetAllByPageAsync(page, pageSize);

            return Ok(list);
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = RoleName.Organizer)]
        public async Task<IActionResult> UpdateMatchAsync([FromRoute] string id, [FromBody] MatchDto dto)
        {
            MatchDto newsDto = await _matchService.UpdateAsync(id, dto, User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value);

            return Ok(newsDto);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = RoleName.Organizer)]
        public async Task<IActionResult> DeleteMatchAsync(string Id)
        {
            MatchDto newsDto = await _matchService.DeleteAsync(Id, User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value);

            return Ok(newsDto);
        }


        [HttpPost]
        [Route("")]
        [Authorize(Roles = RoleName.Organizer)]
        public async Task<IActionResult> AddMatchAsync([FromBody] MatchDto dto)
        {
            MatchDto newsDto = await _matchService.AddAsync(dto);
            
            return Ok(newsDto);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            MatchDto newsDto = await _matchService.GetByIdAsync(id);
            
            return Ok(newsDto);
        }

        [HttpGet]
        [Route("filter/")]
        public async Task<IActionResult> GetByFilterAsync(int page, int pageSize, SortOptions? options, [FromQuery] MatchFilter filter)
        {
            MatchPagedResponse response = new MatchPagedResponse(){
                Matches = await _matchService.GetByFilterAsync(filter, options, page, pageSize),
                Total = await _matchService.GetTotalAsync()
            };
            
            return Ok(response);
        }

        // [HttpGet]
        // [Route("tournament/{TournamentId}")]
        // public async Task<IActionResult> GetByTournamentAsync([FromRoute]string TournamentId)
        // {
        //     var newsDto = await _matchService.GetTournamentStructureAsync(TournamentId);
            
        //     return Ok(newsDto);
        // }

        // [HttpPost]
        // [Route("winner/")]
        // [Authorize(Roles = RoleName.Organizer)]
        // public async Task<IActionResult> SetMatchWinner([FromBody]string matchId, [FromBody]string winnerId, [FromBody]int winPoints, [FromBody]int loosePoints){
        //     var match = await _matchService.SetWinnerAsync(matchId, winnerId, winPoints, loosePoints, User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value);

        //     return Ok(match);
        // }

        // [HttpGet]
        // [Route("byRound")]
        // public async Task<IActionResult> GetByRoundName(string tournamentId, string round){
        //     var match = await _matchService.GetByRoundAsync(tournamentId, round);
        //     return Ok(match);
        // }

        // [HttpPost]
        // [Route("list/add")]
        // public async Task<IActionResult> AddMatchesAsync([FromBody] List<MatchDto> matches){
        //     _matchService.AddMatchesAsync(matches);
        //     return Ok();
        // }
    }
}
