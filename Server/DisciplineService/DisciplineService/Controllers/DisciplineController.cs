using DisciplineService.BusinessLogic.Models;
using DisciplineService.BusinessLogic.Services.Interfaces;
using DisciplineService.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DisciplineService.API.Controllers
{
    [Route("api/disciplines")]
    [ApiController]
    public class DisciplineController : ControllerBase
    {
        private readonly IDisciplineService _disciplineService;
        public DisciplineController(IDisciplineService disciplineService){
            _disciplineService = disciplineService;
        }
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetDisciplinesAsync()
        {
            var list = await _disciplineService.GetAllAsync();

            return Ok(list);
        }

        [HttpGet]
        [Route("admin")]
        [Authorize(Roles = RoleName.Admin)]
        public async Task<IActionResult> GetDisciplinesAdminAsync()
        {
            var list = await _disciplineService.GetAllAdminAsync();

            return Ok(list);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetDisciplineByIdAsync([FromRoute] string id)
        {
            var list = await _disciplineService.GetByIdAsync(id);

            return Ok(list);
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = RoleName.Admin)]
        public async Task<IActionResult> UpdateDisciplineAsync([FromRoute] string id, [FromBody] DisciplineDto dto)
        {
            var newsDto = await _disciplineService.UpdateAsync(id, dto);

            return Ok(newsDto);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = RoleName.Admin)]
        public async Task<IActionResult> DeleteDisciplineAsync(string Id)
        {
            var newsDto = await _disciplineService.DeleteAsync(Id);

            return Ok(newsDto);
        }


        [HttpPost]
        [Route("")]
        [Authorize(Roles = RoleName.Admin)]
        public async Task<IActionResult> AddDisciplineAsync([FromBody] DisciplineCreateDto dto)
        {
            var newsDto = await _disciplineService.AddAsync(dto);
            
            return Ok(newsDto);
        }
    }
}
