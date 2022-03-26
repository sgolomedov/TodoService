using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Database
{
    public interface ITodoRepository
    {
        Task<IEnumerable<TodoItem>> GetAll();
        ValueTask<TodoItem> FindById(long id);
        void Add(TodoItem item);
        void Delete(TodoItem item);
        Task Save();
        bool IsExist(long id);
    }

    public class TodoRepository : ITodoRepository
    {
        private readonly TodoContext _context;

        public TodoRepository(TodoContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TodoItem>> GetAll()
        {
            return await _context.TodoItems.ToArrayAsync();
        }

        public ValueTask<TodoItem> FindById(long id)
        {
            return _context.FindAsync<TodoItem>(id);
        }

        public void Add(TodoItem item)
        {
            _context.TodoItems.Add(item);
        }

        public void Delete(TodoItem item)
        {
            _context.TodoItems.Remove(item);
        }

        public Task Save()
        {
            return _context.SaveChangesAsync();
        }

        public bool IsExist(long id)
        {
            return _context.TodoItems.Any(i => i.Id == id);
        }
    }
}
