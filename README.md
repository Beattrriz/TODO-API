# TODO API

---

## Visão Geral

A Todo API é uma aplicação RESTful desenvolvida em C#/.NET para gerenciamento de tarefas (TODOs). Cada usuário autenticado pode criar, ler, atualizar e excluir tarefas, garantindo que somente o criador de uma tarefa possa visualizá-la ou editá-la. A API é protegida por autenticação JWT e implementa boas práticas de desenvolvimento.

## Requisitos para Instalação

Antes de iniciar o projeto, certifique-se de que você possui os seguintes requisitos instalados:

- **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)**  
- **[PostgreSQL](https://www.postgresql.org/download/)**  
- **[Visual Studio 2022](https://visualstudio.microsoft.com/)** ou outro editor de sua preferência  
- **[Git](https://git-scm.com/)**  
- **[Postman](https://www.postman.com/)** (opcional, para testar os endpoints)

---

## **Configuração do Ambiente**

1. Clone este repositório:  
   ```bash
   git clone https://github.com/Beattrriz/TODO-API.git
   cd TODO-API
   ```
2. Restaure as dependências do projeto:
     ```bash
        dotnet restore
      ```
3. Configure o banco de dados PostgreSQL:
   - Crie um banco de dados com o nome TodoApiDbou outro de sua escolha.
   - Configurar um ConnectionStringno arquivo appsettings.json:
     ```
     "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Database=TodoApiDb;Username=seu_usuario;Password=sua_senha"
     }
4. Aplique as migrações para criar as tabelas no banco:
   ```bash
   dotnet ef database update
   ```
5. Execute o projeto:
    ```bash
   dotnet run
   ```
6. Acesse o Swagger para documentação e testes:
   - URL padrão:https://localhost:5001/swagger
---
  
## Autenticação JWT

Para usar endpoints protegidos, você precisará de um token JWT. Siga os passos abaixo:

1. Registre um usuário sem endpoint de autenticação:
    ```bash
   POST /api/Auth/register
      {
        "email": "usuario@example.com",
        "password": "sua_senha"
      }
   ```
2. Faça login para obter o token JWT:
   ```bash
   POST /api/Auth/login
      {
        "email": "usuario@example.com",
        "password": "sua_senha"
      }
   ```
   - A resposta incluirá o token no campo **accessToken**.
3. Insira o token no cabeçalho **Authorization** dos endpoints protegidos:
   - No cabeçalho da página do Swagger contém um botão (Authorize) em verde. Ao clica-lo você deverá inserir no campo Value o comando a seguir e clicar em Authorize para poder utilizar os endpoints protegidos.
   ```
     Bearer {seu_token}
   ```
---
## Endpoints Disponíveis

### TODO

1. Crie uma nova tarefa
   ```bash
      POST /api/todos
         {
           "title": "Nova Tarefa",
           "description": "Descrição da tarefa"
         }
   ```
2. Listar todas as tarefas do usuário autenticado
   ```bash
     GET /api/todos
   ```
3. Obter uma tarefa específica
   ```bash
     GET /api/todos/{id}
   ```
4. Atualizar uma tarefa existente
   Caso o usuário não queira atualizar todos os campos basta deixar o campo que não se deseja atualizar como uma string vazia (" ") ou null.
    ```bash
     PUT /api/todos/{id}
      {
         "title": "Título atualizado",
         "description": "Descrição atualizada",
         "completedAt": "2024-12-21 15:00:00"
      }
   ```
5. Excluir uma tarefa
   ```bash
     DELETE /api/todos/{id}
   ```

## Pontos Importantes
- **Autenticação**: Ceritfique-se de se autenticar para conseguir utilizar os endpoints das tarefas.
- **Validação de campos**: Certifique-se de enviar todos os campos obrigatórios nos formatos corretos.
- **Acesso restrito**: Apenas o usuário que criou uma tarefa pode visualizá-la, atualizá-la ou excluí-la.
- **Conclusão de tarefas**: O campo completedAt é gerado automaticamente.


  
   
