using Xunit;
using System.Threading.Tasks;
using FakeItEasy;
using TodoApiDTO.Services;
using TodoApi.Database;
using TodoApi.Models;
using System;

namespace TodoApiDTO.Tests
{
    public class TodoServiceDeleteTodoItemTests
    {
        private readonly ITodoRepository _repo = A.Fake<ITodoRepository>();
        private readonly TodoService _sut;
        private readonly TodoItem _item = new TodoItem
        {
            Id = 1,
            IsComplete = true,
            Name = "item"
        };

        public TodoServiceDeleteTodoItemTests()
        {
            A.CallTo(() => _repo.FindById(A<long>._)).Returns(new ValueTask<TodoItem>((TodoItem)null));
            A.CallTo(() => _repo.FindById(_item.Id)).Returns(new ValueTask<TodoItem>(_item));
            _sut = new TodoService(_repo);
        }

        [Fact]
        public async Task TodoItemDoesNotExist_Throws()
        {
            // Arrange.
            long nonExistId = 666;

            // Act.
            Exception e = await Record.ExceptionAsync(() => _sut.DeleteTodoItem(nonExistId));

            // Assert.
            Assert.NotNull(e);
            Assert.IsType<TodoItemNotFoundException>(e);
        }

        [Fact]
        public async Task TodoItemExist_DeletesFromRepository()
        {
            // Act.
            await _sut.DeleteTodoItem(_item.Id);

            // Assert.
            A.CallTo(() => _repo.Delete(_item)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _repo.Save()).MustHaveHappenedOnceExactly();
        }
    }
}
