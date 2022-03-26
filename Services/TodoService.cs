using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApi.Database;
using TodoApi.Models;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TodoApiDTO.Services
{
    public interface ITodoService
    {
        Task<IEnumerable<TodoItemDTO>> GetTodoItems();
        Task<TodoItemDTO> GetTodoItem(long id);
        Task UpdateTodoItem(long id, TodoItemDTO dto);
        Task<TodoItemDTO> CreateTodoItem(TodoItemDTO dto);
        Task DeleteTodoItem(long id);
    }

    public class TodoService : ITodoService
    {

        private readonly ITodoRepository _repository;

        public TodoService(ITodoRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TodoItemDTO>> GetTodoItems()
        {
            var items = await _repository.GetAll();
            return items.Select(ItemToDto);
        }

        public async Task<TodoItemDTO> GetTodoItem(long id)
        {
            var todoItem = await _repository.FindById(id);

            if (todoItem == null)
            {
                throw new TodoItemNotFoundException(id);
            }

            return ItemToDto(todoItem);
        }

        public async Task UpdateTodoItem(long id, TodoItemDTO dto)
        {
            if (id != dto.Id)
            {
                throw new ValidationException($"Wrong dto id parameter '{dto.Id}', should be '{id}'.");
            }

            var todoItem = await _repository.FindById(id);
            if (todoItem == null)
            {
                throw new TodoItemNotFoundException(id);
            }

            todoItem.Name = dto.Name;
            todoItem.IsComplete = dto.IsComplete;

            try
            {
                await _repository.Save();
            }
            catch (DbUpdateConcurrencyException) when (!_repository.IsExist(id))
            {
                throw new TodoItemNotFoundException(id);
            }
        }

        public async Task<TodoItemDTO> CreateTodoItem(TodoItemDTO dto)
        {
            var todoItem = new TodoItem
            {
                IsComplete = dto.IsComplete,
                Name = dto.Name
            };

            _repository.Add(todoItem);
            await _repository.Save();

            return ItemToDto(todoItem);
        }

        public async Task DeleteTodoItem(long id)
        {
            var todoItem = await _repository.FindById(id);

            if (todoItem == null)
            {
                throw new TodoItemNotFoundException(id);
            }

            _repository.Delete(todoItem);
            await _repository.Save();
        }

        private static TodoItemDTO ItemToDto(TodoItem todoItem) =>
            new TodoItemDTO
            {
                Id = todoItem.Id,
                Name = todoItem.Name,
                IsComplete = todoItem.IsComplete
            };
    }
}
