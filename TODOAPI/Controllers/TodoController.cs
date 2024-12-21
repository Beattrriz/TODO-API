using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TODOAPI.Data;
using TODOAPI.DTOs;
using TODOAPI.Models;

namespace TODOAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TodoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TodoController(ApplicationDbContext context)
        {
            _context = context;
            
        }

        private string GetUserIdFromClaims()
        {
            return User.FindFirst("id")?.Value;
        }

        /// <summary>
        /// Retorna todas as tarefas do usuário autenticado.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Todo>>> GetTodos()
        {
            var userId = GetUserIdFromClaims();

            if (userId == null)
            {
                return Unauthorized(new { Message = "Usuário não autenticado" });
            }

            var todos =  await _context.Todos
                .Where(t => t.UserId.ToString() == userId)
                .ToListAsync();

            var result = todos.Select(todo => new
            {
                todo.Id,
                todo.Title,
                todo.Description,
                CreatedAt = todo.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                CompletedAt = todo.CompletedAt?.ToString("yyyy-MM-dd HH:mm:ss") 
            });

            return Ok(result);
        }

        /// <summary>
        /// Retorna uma tarefa específica do usuário autenticado.
        /// </summary>
        /// <param name="id">ID da tarefa.</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<Todo>> GetTodo(int id)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
            {
                return Unauthorized(new { Message = "Usuário não autenticado" });
            }

            var todo = await _context.Todos.FindAsync(id);
            if(todo == null)
            {
                return NotFound(new { Message = "Tarefa não encontrada" });
            }

            if (todo.UserId.ToString() != userId)
            {
                return Unauthorized(new { Message = "Você não tem permissão para acessar esta tarefa" });
            }

            var result = new
            {
                todo.Id,
                todo.Title,
                todo.Description,
                CreatedAt = todo.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                CompletedAt = todo.CompletedAt?.ToString("yyyy-MM-dd HH:mm:ss") 
            };

            return Ok(result);
        }

        /// <summary>
        /// Cria uma nova tarefa para o usuário autenticado.
        /// </summary>
        /// <param name="todoDto">Dados da tarefa a ser criada.</param>
        [HttpPost]
        public async Task<ActionResult<Todo>> CreateTodo([FromBody] TodoDto todoDto)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
            {
                return Unauthorized(new { Message = "Usuário não autenticado" });
            }

            var todo = new Todo
            {
                Title = todoDto.Title,
                Description = todoDto.Description,
                CreatedAt = DateTime.UtcNow, 
                UserId = int.Parse(userId)  
            };

            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodo), new { id = todo.Id }, todo);
        }

        /// <summary>
        /// Atualiza os dados de uma tarefa existente.
        /// </summary>
        /// <param name="id">ID da tarefa a ser atualizada.</param>
        /// <param name="todoDto">Dados atualizados da tarefa.</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo(int id, [FromBody] TodoUpdateDto todoUpdate)
        {
            var todo = await _context.Todos.FindAsync(id);

            if (todo == null)
            {
                return NotFound(new { Message = "Tarefa não encontrada" });
            }

            var userId = GetUserIdFromClaims();
            if (todo.UserId.ToString() != userId)
            {
                return Unauthorized(new { Message = "Usuário não autorizado" });
            }

            if (!string.IsNullOrEmpty(todoUpdate.Title))
            {
                todo.Title = todoUpdate.Title;  
            }

            if (!string.IsNullOrEmpty(todoUpdate.Description))
            {
                todo.Description = todoUpdate.Description; 
            }

            if (todoUpdate.CompletedAt.HasValue)
            {
                todo.CompletedAt = todoUpdate.CompletedAt.Value; 
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                return Conflict(new { Message = "Erro ao atualizar a tarefa." });
            }

            return NoContent();
        }

        /// <summary>
        /// Deleta uma tarefa do usuário autenticado.
        /// </summary>
        /// <param name="id">ID da tarefa a ser deletada.</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            var todo = await _context.Todos.FindAsync(id);

            if (todo == null)
            {
                return NotFound(new { Message = "Tarefa não encontrada" });
            }

            var userId = GetUserIdFromClaims();
            if (todo.UserId.ToString() != userId)
            {
                return Unauthorized(new { Message = "Usuário não autorizado" });
            }

            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
