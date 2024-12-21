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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Todo>>> GetTodos()
        {
            var userId = User.FindFirst("id")?.Value;

            if(userId == null)
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

        [HttpGet("{id}")]
        public async Task<ActionResult<Todo>> GetTodo(int id)
        {
            var userId = User.FindFirst("id")?.Value;
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

        [HttpPost]
        public async Task<ActionResult<Todo>> CreateTodo([FromBody] TodoDto todoDto)
        {
            var userId = User.FindFirst("id")?.Value;
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo(int id, [FromBody] TodoUpdateDto updatedTodo)
        {
            var todo = await _context.Todos.FindAsync(id);

            if (todo == null)
            {
                return NotFound(new { Message = "Tarefa não encontrada" });
            }

            var userId = User.FindFirst("id")?.Value;
            if (todo.UserId.ToString() != userId)
            {
                return Unauthorized(new { Message = "Usuário não autorizado" });
            }

            if (!string.IsNullOrEmpty(updatedTodo.Title))
            {
                todo.Title = updatedTodo.Title;  
            }

            if (!string.IsNullOrEmpty(updatedTodo.Description))
            {
                todo.Description = updatedTodo.Description; 
            }

            if (updatedTodo.CompletedAt.HasValue)
            {
                todo.CompletedAt = updatedTodo.CompletedAt.Value; 
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            var todo = await _context.Todos.FindAsync(id);

            if (todo == null)
            {
                return NotFound(new { Message = "Tarefa não encontrada" });
            }

            var userId = User.FindFirst("id")?.Value;
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
