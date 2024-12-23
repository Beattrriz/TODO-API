using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TODOAPI.Controllers;
using TODOAPI.Data;
using TODOAPI.DTOs;
using TODOAPI.Models;

namespace TODOAPI.Tests.Controllers
{
    public class TodoControllerTestsMock
    {
        public static void SetupHttpContext(TodoController controller, int userId)
        {
            var claims = new List<Claim> { new Claim("id", userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "mock");
            var principal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = principal };

            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        public static void SetupUnauthenticatedHttpContext(TodoController controller)
        {
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        }

        public static Todo CreateTodoForUser(ApplicationDbContext context, int userId)
        {
            var todo = new Todo
            {
                Title = "Tarefa Original",
                Description = "Descrição Original",
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            context.Todos.Add(todo);
            context.SaveChangesAsync().Wait();  
            return todo;
        }

        public static TodoUpdateDto CreateTodoUpdateDto()
        {
            return new TodoUpdateDto
            {
                Title = "Tarefa Atualizada",
                Description = "Descrição Atualizada"
            };
        }

        public static TodoDto CreateTodoDto()
        {
            return new TodoDto
            {
                Title = "Nova Tarefa",
                Description = "Descrição da tarefa"
            };
        }
    }
}

