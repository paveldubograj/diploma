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
        [Authorize(Roles = $"{RoleName.Admin}, {RoleName.Organizer}")]
        public async Task<IActionResult> UpdateMatchAsync([FromRoute] string id, [FromBody] MatchDto dto)
        {
            MatchDto newsDto = await _matchService.UpdateAsync(id, dto, User);

            return Ok(newsDto);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = $"{RoleName.Admin}, {RoleName.Organizer}")]
        public async Task<IActionResult> DeleteMatchAsync(string Id)
        {
            MatchDto newsDto = await _matchService.DeleteAsync(Id, User);

            return Ok(newsDto);
        }


        [HttpPost]
        [Route("")]
        [Authorize(Roles = RoleName.Organizer)]
        public async Task<IActionResult> AddMatchAsync([FromBody] MatchDto dto)
        {
            dto.ownerId = User.Claims.First(x => x.Type.Equals(ClaimTypes.GivenName)).Value;
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
        public async Task<IActionResult> GetByFilterAsync([FromQuery] MatchFilter filter)
        {
            return Ok(await _matchService.GetByFilterAsync(filter));
        }
    }
}
