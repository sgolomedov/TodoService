using Xunit;
using System.Threading.Tasks;
using FakeItEasy;
using TodoApiDTO.Services;
using TodoApi.Database;
using TodoApi.Models;
using System;

namespace TodoApiDTO.Tests
{
    public class TodoServiceGetTodoItemTests
    {
        private readonly TodoService _sut;
        private readonly TodoItem _item = new TodoItem
        {
            Id = 1,
            IsComplete = true,
            Name = "item"
        };

        public TodoServiceGetTodoItemTests()
        {
            var repo = A.Fake<ITodoRepository>();
            A.CallTo(() => repo.FindById(A<long>._)).Returns(new ValueTask<TodoItem>((TodoItem)null));
            A.CallTo(() => repo.FindById(_item.Id)).Returns(new ValueTask<TodoItem>(_item));
            _sut = new TodoService(repo);
        }

        [Fact]
        public async Task TodoItemDoesNotExist_Throws()
        {
            // Arrange.
            long nonExistId = 666;

            // Act.
            Exception e = await Record.ExceptionAsync(() => _sut.GetTodoItem(nonExistId));

            // Assert.
            Assert.NotNull(e);
            Assert.IsType<TodoItemNotFoundException>(e);
        }

        [Fact]
        public async Task TodoItemExist_ReturnsCorrectDto()
        {
            // Act.
            TodoItemDTO dto = await _sut.GetTodoItem(_item.Id);

            // Assert.
            Assert.NotNull(dto);
            Assert.Equal(_item.Id, dto.Id);
            Assert.Equal(_item.IsComplete, dto.IsComplete);
            Assert.Equal(_item.Name, dto.Name);
        }
    }
}
