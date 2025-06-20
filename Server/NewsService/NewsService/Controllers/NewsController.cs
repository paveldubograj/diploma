using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
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
        public NewsController(INewsService newsService)
        {
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
        public async Task<IActionResult> UpdateNewsAsync([FromRoute] string id, [FromBody] NewsUpdateDto dto)
        {
            var newsDto = await _newsService.UpdateAsync(id, dto, User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value);

            return Ok(newsDto);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = RoleName.Admin)]
        public async Task<IActionResult> DeleteNewsAsync([FromRoute] string Id)
        {
            var newsDto = await _newsService.DeleteAsync(Id);

            return Ok(newsDto);
        }


        [HttpPost]
        [Route("")]
        [Authorize(Roles = RoleName.NewsTeller)]
        public async Task<IActionResult> AddNewsAsync([FromBody] NewsUpdateDto dto)
        {
            var newsDto = await _newsService.AddAsync(dto,
                User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value,
                User.Claims.First(x => x.Type.Equals(ClaimTypes.GivenName)).Value);

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
        public async Task<ActionResult<NewsPagedResponse>> GetByFilterAsync(int page, int pageSize, [FromQuery] NewsFilter filter)
        {
            NewsPagedResponse response = new NewsPagedResponse()
            {
                News = await _newsService.GetByFilterAsync(filter, page, pageSize),
                Total = await _newsService.GetTotalAsync()
            };
            return Ok(response);
        }

        [HttpGet]
        [Route("{userId}/list")]
        public async Task<ActionResult<NewsPagedResponse>> GetByUserAsync(int page, int pageSize, [FromRoute] string userId)
        {
            NewsPagedResponse response = new NewsPagedResponse()
            {
                News = await _newsService.GetByUserAsync(userId, page, pageSize),
                Total = await _newsService.GetTotalAsync()
            };
            return Ok(response);
        }

        [HttpPut]
        [Route("{id}/{tagId}")]
        [Authorize(Roles = RoleName.NewsTeller)]
        public async Task<IActionResult> AddNewsTagsAsync([FromRoute] string id, [FromRoute] string tagId)
        {
            var newsDto = await _newsService.AddTagAsync(id, tagId, User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value);

            return Ok(newsDto);
        }

        [HttpDelete]
        [Route("{id}/{tagId}")]
        [Authorize(Roles = RoleName.NewsTeller)]
        public async Task<IActionResult> DeleteNewsTagAsync([FromRoute] string id, [FromRoute] string tagId)
        {
            var newsDto = await _newsService.RemoveTagAsync(id, tagId, User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value);

            return Ok(newsDto);
        }

        [HttpPost]
        [Route("{id}/image")]
        [Authorize(Roles = RoleName.NewsTeller)]
        public async Task<IActionResult> AddNewsImageAsync([FromRoute] string id, IFormFile image)
        {
            var newsDto = await _newsService.AddImageAsync(id, image, User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value);

            return Ok(newsDto);
        }

        [HttpDelete]
        [Route("{id}/image")]
        [Authorize(Roles = RoleName.NewsTeller)]
        public async Task<IActionResult> DeleteNewsImageAsync([FromRoute] string id)
        {
            var newsDto = await _newsService.DeleteImageAsync(id, User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value);

            return Ok(newsDto);
        }
        
        [HttpPut]
        [Route("{id}/image")]
        [Authorize(Roles = RoleName.NewsTeller)]
        public async Task<IActionResult> UpdateNewsImageAsync([FromRoute] string id, IFormFile image)
        {
            var newsDto = await _newsService.DeleteImageAsync(id, User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value);
            var res = await _newsService.AddImageAsync(id, image, User.Claims.First(x => x.Type.Equals(ClaimTypes.Name)).Value);

            return Ok(newsDto);
        }
    }
}
