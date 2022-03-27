using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApiDTO.Services;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly ITodoService _service;

        public TodoItemsController(ITodoService service)
        {
            _service = service;
        }

        [HttpGet]
        public Task<IEnumerable<TodoItemDTO>> GetTodoItems()
        {
            return _service.GetTodoItems();
        }

        [HttpGet("{id}")]
        public Task<TodoItemDTO> GetTodoItem(long id)
        {
            return _service.GetTodoItem(id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoItem(long id, TodoItemDTO todoItemDTO)
        {
            await _service.UpdateTodoItem(id, todoItemDTO);
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<TodoItemDTO>> CreateTodoItem(TodoItemDTO todoItemDTO)
        {
            var newItemDto = await _service.CreateTodoItem(todoItemDTO);
            return CreatedAtAction(nameof(GetTodoItem), new { id = newItemDto.Id }, newItemDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            await _service.DeleteTodoItem(id);
            return NoContent();
        }
    }
}
