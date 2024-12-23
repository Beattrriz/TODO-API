using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Linq.Expressions;
using System.Security.Claims;
using TODOAPI.Controllers;
using TODOAPI.Data;
using TODOAPI.DTOs;
using TODOAPI.Models;
using TODOAPI.Services;
using TODOAPI.Tests.Helper;

namespace TODOAPI.Tests.Controllers
{
    public class TodoControllerTests
    {
        private readonly TodoController _todoController;
        private readonly ApplicationDbContext _context;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;

        public TodoControllerTests()
        {
            _context = InMemoryDbContextHelper.CreateDbContext();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            _todoController = new TodoController(_context);
        }

        [Fact]
        public async Task GetTodos_DeveRetornarTodosAsTarefasDoUsuarioAutenticado()
        {
            // Arrange
            var userId = 1;
            TodoControllerTestsMock.SetupHttpContext(_todoController, userId);

            TodoControllerTestsMock.CreateTodoForUser(_context, userId);

            // Act
            var result = await _todoController.GetTodos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var todos = Assert.IsAssignableFrom<IEnumerable<dynamic>>(okResult.Value);
            Assert.NotEmpty(todos);
        }

        [Fact]
        public async Task GetTodos_DeveRetornarUnauthorized_SeUsuarioNaoEstiverAutenticado()
        {
            // Arrange
            TodoControllerTestsMock.SetupUnauthenticatedHttpContext(_todoController);

            // Act
            var result = await _todoController.GetTodos();

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
            Assert.Contains("Usuário não autenticado", unauthorizedResult.Value.ToString());
        }

        [Fact]
        public async Task GetTodo_DeveRetornarTarefa_SeUsuarioForProprietario()
        {
            // Arrange
            var userId = 1;
            TodoControllerTestsMock.SetupHttpContext(_todoController, userId);

            var todo = new Todo { UserId = userId, Title = "Test Task", Description = "Test Description", CreatedAt = DateTime.UtcNow };
            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();

            // Act
            var result = await _todoController.GetTodo(todo.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);

            var resultValue = okResult.Value;
            Assert.NotNull(resultValue);

            var idProperty = resultValue.GetType().GetProperty("Id");
            var titleProperty = resultValue.GetType().GetProperty("Title");
            var descriptionProperty = resultValue.GetType().GetProperty("Description");
            var createdAtProperty = resultValue.GetType().GetProperty("CreatedAt");
            var completedAtProperty = resultValue.GetType().GetProperty("CompletedAt");

            Assert.Equal(todo.Id, (int)idProperty.GetValue(resultValue));
            Assert.Equal(todo.Title, titleProperty.GetValue(resultValue).ToString());
            Assert.Equal(todo.Description, descriptionProperty.GetValue(resultValue).ToString());
            Assert.Equal(todo.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"), createdAtProperty.GetValue(resultValue).ToString());
            Assert.Null(completedAtProperty.GetValue(resultValue));
        }

        [Fact]
        public async Task GetTodo_DeveRetornarUnauthorized_SeUsuarioNaoEstiverAutenticado()
        {
            // Arrange
            TodoControllerTestsMock.SetupUnauthenticatedHttpContext(_todoController);

            // Act
            var result = await _todoController.GetTodo(1);

            // Assert
            var unauthorizedResult = result.Result as UnauthorizedObjectResult;
            Assert.NotNull(unauthorizedResult);
            Assert.Equal(401, unauthorizedResult.StatusCode);
            Assert.Contains("Usuário não autenticado", unauthorizedResult.Value.ToString());
        }

        [Fact]
        public async Task CreateTodo_DeveCriarTarefa_SeUsuarioForAutenticado()
        {
            // Arrange
            var userId = 1;
            TodoControllerTestsMock.SetupHttpContext(_todoController, userId);

            var todoDto = TodoControllerTestsMock.CreateTodoDto();

            // Act
            var result = await _todoController.CreateTodo(todoDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdTodo = Assert.IsType<Todo>(createdResult.Value);
            Assert.Equal(todoDto.Title, createdTodo.Title);
            Assert.Equal(todoDto.Description, createdTodo.Description);
            Assert.Equal(userId, createdTodo.UserId);
            Assert.NotNull(createdTodo.CreatedAt);

            var todoInDb = await _context.Todos.FindAsync(createdTodo.Id);
            Assert.NotNull(todoInDb);
            Assert.Equal(todoDto.Title, todoInDb.Title);
            Assert.Equal(todoDto.Description, todoInDb.Description);
            Assert.Equal(userId, todoInDb.UserId);
        }

        [Fact]
        public async Task CreateTodo_DeveRetornarUnauthorized_SeUsuarioNaoEstiverAutenticado()
        {
            // Arrange
            TodoControllerTestsMock.SetupUnauthenticatedHttpContext(_todoController);

            var todoDto = TodoControllerTestsMock.CreateTodoDto();

            // Act
            var result = await _todoController.CreateTodo(todoDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
            Assert.Contains("Usuário não autenticado", unauthorizedResult.Value.ToString());
        }

        [Fact]
        public async Task UpdateTodo_DeveAtualizarTarefa_SeUsuarioForProprietario()
        {
            // Arrange
            var userId = 1;
            TodoControllerTestsMock.SetupHttpContext(_todoController, userId);

            var todo = TodoControllerTestsMock.CreateTodoForUser(_context, userId);
            var todoUpdate = TodoControllerTestsMock.CreateTodoUpdateDto();

            // Act
            var result = await _todoController.UpdateTodo(todo.Id, todoUpdate);

            // Assert
            Assert.IsType<NoContentResult>(result);

            var updatedTodo = await _context.Todos.FindAsync(todo.Id);
            Assert.Equal(todoUpdate.Title, updatedTodo.Title);
            Assert.Equal(todoUpdate.Description, updatedTodo.Description);
        }

        [Fact]
        public async Task UpdateTodo_DeveRetornarUnauthorized_SeUsuarioNaoForProprietario()
        {
            // Arrange
            var userId = 1;
            var anotherUserId = 2;
            TodoControllerTestsMock.SetupHttpContext(_todoController, userId);

            var todo = new Todo
            {
                Title = "Tarefa de Outro Usuário",
                Description = "Descrição",
                UserId = anotherUserId,
                CreatedAt = DateTime.UtcNow
            };
            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();

            var todoUpdate = TodoControllerTestsMock.CreateTodoUpdateDto();

            // Act
            var result = await _todoController.UpdateTodo(todo.Id, todoUpdate);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
            Assert.Contains("Usuário não autorizado", unauthorizedResult.Value.ToString());
        }

        [Fact]
        public async Task DeleteTodo_DeveExcluirTarefa_SeUsuarioForProprietario()
        {
            // Arrange
            var userId = 1;
            TodoControllerTestsMock.SetupHttpContext(_todoController, userId);

            var todo = TodoControllerTestsMock.CreateTodoForUser(_context, userId);

            // Act
            var result = await _todoController.DeleteTodo(todo.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);

            var deletedTodo = await _context.Todos.FindAsync(todo.Id);
            Assert.Null(deletedTodo);
        }

        [Fact]
        public async Task DeleteTodo_DeveRetornarUnauthorized_SeUsuarioNaoForProprietario()
        {
            // Arrange
            var userId = 1;
            var anotherUserId = 2;
            TodoControllerTestsMock.SetupHttpContext(_todoController, userId);

            var todo = new Todo
            {
                Title = "Tarefa de Outro Usuário",
                Description = "Descrição da Tarefa",
                UserId = anotherUserId,
                CreatedAt = DateTime.UtcNow
            };
            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();

            // Act
            var result = await _todoController.DeleteTodo(todo.Id);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
            Assert.Contains("Usuário não autorizado", unauthorizedResult.Value.ToString());
        }

        [Fact]
        public async Task DeleteTodo_DeveRetornarNotFound_SeTarefaNaoExistir()
        {
            // Arrange
            var userId = 1;
            TodoControllerTestsMock.SetupHttpContext(_todoController, userId);

            var nonExistentTodoId = 999;

            // Act
            var result = await _todoController.DeleteTodo(nonExistentTodoId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Contains("Tarefa não encontrada", notFoundResult.Value.ToString());
        }
    }
}