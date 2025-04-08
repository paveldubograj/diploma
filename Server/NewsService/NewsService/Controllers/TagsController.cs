using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsService.BusinessLogic.Models.Tag;
using NewsService.BusinessLogic.Services.Interfaces;
using NewsService.Shared.Constants;

namespace NewsService.API.Controllers
{
    [Route("api/tags")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ITagsService _tagsService;
        public TagsController(ITagsService tagsService){
            _tagsService = tagsService;
        }
        [HttpGet]
        [Route("list/")]
        public async Task<IActionResult> GetTagsAsync()
        {
            var list = await _tagsService.GetAllAsync();

            return Ok(list);
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = RoleName.Admin)]
        public async Task<IActionResult> UpdateTagAsync([FromRoute] string id, [FromBody] TagDto dto)
        {
            var newsDto = await _tagsService.UpdateAsync(id, dto);

            return Ok(newsDto);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = RoleName.Admin)]
        public async Task<IActionResult> DeleteTagAsync(string Id)
        {
            var newsDto = await _tagsService.DeleteAsync(Id);

            return Ok(newsDto);
        }


        [HttpPost]
        [Route("")]
        [Authorize(Roles = "admin, newsTeller")]
        public async Task<IActionResult> AddTagAsync([FromBody] TagDto dto)
        {
            var newsDto = await _tagsService.AddAsync(dto);
            
            return Ok(newsDto);
        }
    }
}
