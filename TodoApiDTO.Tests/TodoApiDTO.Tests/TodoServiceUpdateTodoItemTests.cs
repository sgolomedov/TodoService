using Xunit;
using System.Threading.Tasks;
using FakeItEasy;
using TodoApiDTO.Services;
using TodoApi.Database;
using TodoApi.Models;
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TodoApiDTO.Tests
{
    public class TodoServiceUpdateTodoItemTests
    {
        private readonly TodoService _sut;
        private readonly ITodoRepository _repo = A.Fake<ITodoRepository>();
        private readonly TodoItemDTO _updatedItemDto = new TodoItemDTO
        {
            Id = 1,
            IsComplete = false,
            Name = "updated"
        };
        private readonly TodoItem _item = new TodoItem
        {
            Id = 1,
            IsComplete = true,
            Name = "item"
        };

        public TodoServiceUpdateTodoItemTests()
        {
            A.CallTo(() => _repo.FindById(A<long>._)).Returns(new ValueTask<TodoItem>((TodoItem)null));
            A.CallTo(() => _repo.FindById(_item.Id)).Returns(new ValueTask<TodoItem>(_item));
            _sut = new TodoService(_repo);
        }

        [Fact]
        public async Task DtoIdAndParameterIdDoesNotMatch_Throws()
        {
            // Act.
            Exception e = await Record.ExceptionAsync(
                () => _sut.UpdateTodoItem(_updatedItemDto.Id + 1, _updatedItemDto));

            // Assert.
            Assert.NotNull(e);
            Assert.IsType<ValidationException>(e);
        }

        [Fact]
        public async Task TodoItemDoesNotExist_Throws()
        {
            // Arrange.
            long nonExistId = 666;
            var updatedDto = new TodoItemDTO
            {
                Id = nonExistId,
                IsComplete = false,
                Name = "updated"
            };

            // Act.
            Exception e = await Record.ExceptionAsync(() => _sut.UpdateTodoItem(nonExistId, updatedDto));

            // Assert.
            Assert.NotNull(e);
            Assert.IsType<TodoItemNotFoundException>(e);
        }

        [Fact]
        public async Task DbUpdateConcurrencyExceptionOnSaveItemDoesNotExists_ThrowsItemNotFound()
        {
            // Arrange.
            A.CallTo(() => _repo.Save()).Throws<DbUpdateConcurrencyException>();
            A.CallTo(() => _repo.IsExist(_updatedItemDto.Id)).Returns(false);

            // Act.
            Exception e = await Record.ExceptionAsync(
                () => _sut.UpdateTodoItem(_updatedItemDto.Id, _updatedItemDto));

            // Assert.
            Assert.NotNull(e);
            Assert.IsType<TodoItemNotFoundException>(e);
        }

        [Fact]
        public async Task DbUpdateConcurrencyExceptionOnSaveItemDoesExists_ThrowsOriginalException()
        {
            // Arrange.
            A.CallTo(() => _repo.Save()).Throws<DbUpdateConcurrencyException>();
            A.CallTo(() => _repo.IsExist(_updatedItemDto.Id)).Returns(true);

            // Act.
            Exception e = await Record.ExceptionAsync(
                () => _sut.UpdateTodoItem(_updatedItemDto.Id, _updatedItemDto));

            // Assert.
            Assert.NotNull(e);
            Assert.IsType<DbUpdateConcurrencyException>(e);
        }

        [Fact]
        public async Task HappyPath_TodoItemUpdated()
        {
            // Arrange.
            Assert.NotEqual(_item.IsComplete, _updatedItemDto.IsComplete);
            Assert.NotEqual(_item.Name, _updatedItemDto.Name);

            // Act.
            await _sut.UpdateTodoItem(_updatedItemDto.Id, _updatedItemDto);

            // Assert.
            Assert.Equal(_item.IsComplete, _updatedItemDto.IsComplete);
            Assert.Equal(_item.Name, _updatedItemDto.Name);
            A.CallTo(() => _repo.Save()).MustHaveHappenedOnceExactly();
        }
    }
}
