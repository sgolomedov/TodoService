using Xunit;
using System.Threading.Tasks;
using FakeItEasy;
using TodoApiDTO.Services;
using TodoApi.Database;
using TodoApi.Models;

namespace TodoApiDTO.Tests
{
    public class TodoServiceCreateTodoItemTests
    {
        [Fact]
        public async Task CreatesAndStoresTodoItemInRepository()
        {
            // Arrange.
            var repo = A.Fake<ITodoRepository>();
            var item = new TodoItemDTO
            {
                Name = "item",
                IsComplete = true
            };
            var sut = new TodoService(repo);

            // Act.
            var createdItemDto = await sut.CreateTodoItem(item);

            // Assert.
            A.CallTo(() => repo.Add(A<TodoItem>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => repo.Save()).MustHaveHappenedOnceExactly();

            Assert.Equal(item.Name, createdItemDto.Name);
            Assert.Equal(item.IsComplete, createdItemDto.IsComplete);
        }
    }
}
