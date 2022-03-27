using System;

namespace TodoApiDTO.Services
{
    public class TodoItemNotFoundException : ApplicationException
    {
        public TodoItemNotFoundException(long id) : base($"Todo item not found with id='{id}'.")
        {
        }
    }
}