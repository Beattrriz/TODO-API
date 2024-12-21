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
   ```
     Authorization: Bearer {seu_token}
   ```
  
   
