using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsService.BusinessLogic.Models.Filter;
using NewsService.BusinessLogic.Models.News;
using NewsService.BusinessLogic.Services.Interfaces;
using NewsService.Shared.Constants;

namespace NewsService.API.Controllers
{
    [Route("api/news")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;
        public NewsController(INewsService newsService){
            _newsService = newsService;
        }

        [HttpGet]
        [Route("list/")]
        public async Task<IActionResult> GetNewsAsync(int page, int pageSize)
        {
            var list = await _newsService.GetAllByPageAsync(page, pageSize);

            return Ok(list);
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = RoleName.NewsTeller)]
        public async Task<IActionResult> UpdateNewsAsync([FromRoute] string id, [FromBody] NewsDto dto)
        {
            var newsDto = await _newsService.UpdateAsync(id, dto, User.Claims.First(x => x.Type.Equals(JwtRegisteredClaimNames.Jti)).Value);

            return Ok(newsDto);
        }

        [HttpDelete]
        [Route("")]
        [Authorize(Roles = RoleName.Admin)]
        public async Task<IActionResult> DeleteNewsAsync(string Id)
        {
            var newsDto = await _newsService.DeleteAsync(Id);

            return Ok(newsDto);
        }


        [HttpPost]
        [Route("")]
        [Authorize(Roles = RoleName.NewsTeller)]
        public async Task<IActionResult> AddNewsAsync([FromBody] NewsDto dto)
        {
            var newsDto = await _newsService.AddAsync(dto);
            
            return Ok(newsDto);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            var newsDto = await _newsService.GetByIdAsync(id);
            
            return Ok(newsDto);
        }

        [HttpGet]
        [Route("filter/")]
        public async Task<IActionResult> GetByFilterAsync(int page, int pageSize, [FromBody] NewsFilter filter)
        {
            var newsDto = await _newsService.GetByFilterAsync(filter, page, pageSize);
            
            return Ok(newsDto);
        }
    }
}
