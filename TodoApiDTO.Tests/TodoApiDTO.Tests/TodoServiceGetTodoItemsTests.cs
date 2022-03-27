using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using TodoApiDTO.Services;
using TodoApi.Database;
using TodoApi.Models;

namespace TodoApiDTO.Tests
{
    public class TodoServiceGetTodoItemsTests
    {
        [Fact]
        public async Task HappyPath_ReturnAllTodoItemsAsDTOs()
        {
            // Arrange.
            TodoItem[] todoItems = {
                new TodoItem
                {
                    Id = 1,
                    IsComplete = false,
                    Name = "first"
                },
                new TodoItem
                {
                    Id = 2,
                    IsComplete = false,
                    Name = "second"
                }
            };
            var repo = A.Fake<ITodoRepository>();
            A.CallTo(() => repo.GetAll()).Returns(Task.FromResult<IEnumerable<TodoItem>>(todoItems));
            var sut = new TodoService(repo);

            // Act.
            IEnumerable<TodoItemDTO> result = await sut.GetTodoItems();

            // Assert.
            var listResult = result.ToList();
            Assert.Equal(todoItems.Length, listResult.Count);

            Assert.Equal(todoItems[0].Id, listResult[0].Id);
            Assert.Equal(todoItems[0].IsComplete, listResult[0].IsComplete);
            Assert.Equal(todoItems[0].Name, listResult[0].Name);

            Assert.Equal(todoItems[1].Id, listResult[1].Id);
            Assert.Equal(todoItems[1].IsComplete, listResult[1].IsComplete);
            Assert.Equal(todoItems[1].Name, listResult[1].Name);
        }
    }
}
