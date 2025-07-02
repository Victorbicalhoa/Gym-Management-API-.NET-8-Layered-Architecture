# Gym Management System

Um sistema abrangente para gerenciar academias, incluindo funcionalidades para membros, treinos, pagamentos e muito mais.

---

## Visão Geral do Projeto

Este projeto consiste em uma API RESTful desenvolvida em ASP.NET Core para gerenciar as operações de um centro de treinamento. Ele segue uma **arquitetura em camadas** (Domain, Infrastructure, Application, API) para promover a separação de responsabilidades, manutenibilidade e escalabilidade.

---

## Tecnologias Utilizadas

* **Linguagem:** C#
* **Framework:** .NET 8.0
* **Arquitetura:** Camadas (Domain, Infrastructure, Application, API)
* **Banco de Dados:** SQL Server
* **ORM:** Entity Framework Core 9.0.6
* **Documentação da API:** Swagger/Swashbuckle.AspNetCore 9.0.1
* **Controle de Versão:** Git / GitHub

---

## Status do Build

[![.NET CI/CD](https://github.com/Victorbicalhoa/GymManagementSystem/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Victorbicalhoa/GymManagementSystem/actions/workflows/dotnet.yml)

---

## Como Rodar o Projeto Localmente

### Pré-requisitos

* [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
* [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (ou SQL Server Express / LocalDB)
* [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) (recomendado) ou Visual Studio Code com extensões C#
* Git

### Passos

1.  **Clone o repositório:**
    ```bash
    git clone [https://github.com/Victorbicalhoa/GymManagementSystem.git](https://github.com/Victorbicalhoa/GymManagementSystem.git)
    cd GymManagementSystem
    ```

2.  **Restaure as dependências:**
    ```bash
    dotnet restore
    ```

3.  **Compile o projeto:**
    ```bash
    dotnet build
    ```

4.  **Configure a String de Conexão:**
    * Abra o arquivo `appsettings.json` no projeto **`CentroTreinamento.Api`**.
    * Atualize a string de conexão `DefaultConnection` para apontar para o seu servidor SQL Server.
        ```json
        {
          "ConnectionStrings": {
            "DefaultConnection": "Server=SUA_INSTANCIA_SQL;Database=CentroTreinamentoDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
          },
          // ... outras configurações
        }
        ```
        *Substitua `SUA_INSTANCIA_SQL` pelo nome do seu servidor/instância SQL Server (ex: `(localdb)\\MSSQLLocalDB` ou `localhost`).*

5.  **Aplique as Migrações do Banco de Dados:**
    * Abra o **Package Manager Console** no Visual Studio (View > Other Windows > Package Manager Console).
    * Defina o **"Default project"** para **`CentroTreinamento.Infrastructure`**.
    * Execute os comandos para criar ou atualizar o banco de dados:
        ```powershell
        Update-Database
        ```
        *Se esta for a primeira vez e você ainda não tiver migrações criadas, pode ser necessário gerar uma inicial primeiro:*
        ```powersershell
        Add-Migration InitialCreate
        Update-Database
        ```

6.  **Abra e Execute no Visual Studio:**
    * Abra o arquivo de solução `CentroTreinamento.sln` no Visual Studio.
    * Defina o projeto **`CentroTreinamento.Api`** como o **projeto de inicialização** (clique com o botão direito no projeto > Set as Startup Project).
    * Pressione `F5` ou clique no botão `Run` para iniciar a API. O navegador deve abrir a página do Swagger UI (geralmente em `https://localhost:PORTA/swagger`).

---

## Endpoints Disponíveis (Alunos)

Atualmente, os seguintes endpoints para a entidade `Aluno` estão implementados:

* `GET /api/alunos`: Retorna uma lista de todos os alunos cadastrados.
* `GET /api/alunos/{id}`: Retorna um aluno específico pelo seu ID (GUID).
* `POST /api/alunos`: Cria um novo aluno com base nos dados fornecidos.
* `PUT /api/alunos/{id}`: Atualiza os dados de um aluno existente.
* `PATCH /api/alunos/{id}/status`: Atualiza apenas o status (ex: Ativo, Inativo) de um aluno.
* `DELETE /api/alunos/{id}`: Exclui um aluno do sistema.

---

## Próximos Passos e Funcionalidades Futuras

O projeto está em constante evolução. As próximas etapas importantes incluem:

* **Implementar Autenticação e Autorização (JWT):** Desenvolver um sistema de segurança robusto para proteger os endpoints da API.
* **Desenvolvimento de Outras Entidades:** Implementar funcionalidades CRUD completas para Instrutores, Agendamentos, Planos de Treino, Pagamentos e Administradores, seguindo o mesmo padrão arquitetural.
* **Testes Unitários:** Adicionar testes unitários para as camadas `Domain` e `Application` a fim de garantir a qualidade e a confiabilidade do código.
* **Validações de Negócio Avançadas:** Incorporar validações mais complexas e regras de negócio específicas em nível de domínio.

---

## Contribuição

Contribuições são bem-vindas! Sinta-se à vontade para abrir issues e pull requests para melhorias, novas funcionalidades ou correções de bugs.

---

## Licença

[MIT License](https://opensource.org/licenses/MIT)
