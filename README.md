# Gym Management System

Um sistema abrangente para gerenciar academias, incluindo funcionalidades para membros, treinos, pagamentos e muito mais.

---

## Visão Geral do Projeto

Este projeto consiste em uma API RESTful desenvolvida em ASP.NET Core para gerenciar as operações de um centro de treinamento. Ele segue uma **arquitetura em camadas** (Domain, Infrastructure, Application, API) para promover a separação de responsabilidades, manutenibilidade e escalabilidade.

Até o momento, o sistema possui **gerenciamento completo de usuários** para as quatro entidades principais: **Administradores, Alunos, Instrutores e Recepcionistas**, incluindo **autenticação e autorização robustas via JWT**. Além disso, o **gerenciamento de agendamentos**, **planos de treino** e **pagamentos** já estão implementados, com suas respectivas regras de negócio e cobertos por testes unitários abrangentes.

---

## Tecnologias Utilizadas

* **Linguagem:** C#
* **Framework:** .NET 8.0
* **Arquitetura:** Camadas (Domain, Infrastructure, Application, API)
* **Banco de Dados:** SQL Server
* **ORM:** Entity Framework Core 9.0.6
* **Autenticação/Autorização:** JWT (JSON Web Tokens)
* **Hashing de Senhas:** BCrypt.NET (ou similar via `IPasswordHasher`)
* **Documentação da API:** Swagger/Swashbuckle.AspNetCore 9.0.1
* **Mapeamento de Objetos:** AutoMapper
* **Testes Unitários:** XUnit, Moq
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

4.  **Configure a String de Conexão e Chave JWT:**
    * Abra o arquivo `appsettings.json` no projeto **`CentroTreinamento.Api`**.
    * Atualize a string de conexão `DefaultConnection` para apontar para o seu servidor SQL Server.
        ```json
        {
          "ConnectionStrings": {
            "DefaultConnection": "Server=SUA_INSTANCIA_SQL;Database=CentroTreinamentoDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
          },
          "JwtSettings": {
            "Secret": "SUA_CHAVE_SECRETA_MUITO_LONGA_E_SEGURA_AQUI_PELO_MENOS_32_CARACTERES", // Altere isso!
            "Issuer": "GymManagementSystem",
            "Audience": "GymClients",
            "ExpirationInMinutes": 60
          },
          // ... outras configurações
        }
        ```
        *Substitua `SUA_INSTANCIA_SQL` pelo nome do seu servidor/instância SQL Server (ex: `(localdb)\\MSSQLLocalDB` ou `localhost`).*
        *`SUA_CHAVE_SECRETA_MUITO_LONGA_E_SEGURA_AQUI_PELO_MENOS_32_CARACTERES` deve ser uma string forte e secreta.*

5.  **Aplique as Migrações do Banco de Dados:**
    * Abra o **Package Manager Console** no Visual Studio (View > Other Windows > Package Manager Console).
    * Defina o **"Default project"** para **`CentroTreinamento.Infrastructure`**.
    * Execute os comandos para criar ou atualizar o banco de dados:
        ```powershell
        Update-Database
        ```
        *Se esta for a primeira vez e você ainda não tiver migrações criadas, pode ser necessário gerar uma inicial primeiro (ex: `Add-Migration InitialCreate`):*
        ```powersershell
        Add-Migration NomeDaSuaPrimeiraMigracao
        Update-Database
        ```
        *Alternativamente, você pode usar o .NET CLI a partir da raiz da solução:*
        ```bash
        dotnet ef database update --project CentroTreinamento.Infrastructure --startup-project CentroTreinamento.Api
        ```

6.  **Abra e Execute no Visual Studio:**
    * Abra o arquivo de solução `CentroTreinamento.sln` no Visual Studio.
    * Defina o projeto **`CentroTreinamento.Api`** como o **projeto de inicialização** (clique com o botão direito no projeto > Set as Startup Project).
    * Pressione `F5` ou clique no botão `Run` para iniciar a API. O navegador deve abrir a página do Swagger UI (geralmente em `https://localhost:PORTA/swagger`).

---

## Endpoints Disponíveis

Atualmente, a API oferece gerenciamento completo (CRUD) e autenticação para as seguintes entidades:

* **Administradores:**
    * `GET /api/administradores`
    * `GET /api/administradores/{id}`
    * `POST /api/administradores`
    * `PUT /api/administradores/{id}`
    * `PATCH /api/administradores/{id}/status`
    * `DELETE /api/administradores/{id}`
    * `POST /api/Auth/login/administrador` (Autenticação)

* **Alunos:**
    * `GET /api/alunos`
    * `GET /api/alunos/{id}`
    * `POST /api/alunos`
    * `PUT /api/alunos/{id}`
    * `PATCH /api/alunos/{id}/status`
    * `DELETE /api/alunos/{id}`
    * `POST /api/Auth/login/aluno` (Autenticação)

* **Instrutores:**
    * `GET /api/instrutores`
    * `GET /api/instrutores/{id}`
    * `POST /api/instrutores`
    * `PUT /api/instrutores/{id}`
    * `PATCH /api/instrutores/{id}/status`
    * `DELETE /api/instrutores/{id}`
    * `POST /api/Auth/login/instrutor` (Autenticação)

* **Recepcionistas:**
    * `GET /api/recepcionistas`
    * `GET /api/recepcionistas/{id}`
    * `POST /api/recepcionistas`
    * `PUT /api/recepcionistas/{id}`
    * `PATCH /api/recepcionistas/{id}/status`
    * `DELETE /api/recepcionistas/{id}`
    * `POST /api/Auth/login/recepcionista` (Autenticação)

* **Agendamentos:**
    * `GET /api/agendamentos`
    * `GET /api/agendamentos/{id}`
    * `GET /api/agendamentos/aluno/{alunoId}`
    * `GET /api/agendamentos/instrutor/{instrutorId}`
    * `POST /api/agendamentos`
    * `PUT /api/agendamentos/{id}`
    * `PATCH /api/agendamentos/{id}/status`
    * `DELETE /api/agendamentos/{id}`

* **Planos de Treino:**
    * `GET /api/planosdetreino`
    * `GET /api/planosdetreino/{id}`
    * `POST /api/planosdetreino`
    * `PUT /api/planosdetreino/{id}`
    * `DELETE /api/planosdetreino/{id}`

* **Pagamentos:**
    * `GET /api/pagamentos`
    * `GET /api/pagamentos/{id}`
    * `POST /api/pagamentos`
    * `PUT /api/pagamentos/{id}`
    * `PATCH /api/pagamentos/{id}/status` (para atualizar status como pago/pendente)
    * `DELETE /api/pagamentos/{id}`

**Autenticação:** Para acessar a maioria dos endpoints de gerenciamento, é necessário obter um JWT válido através dos endpoints de login e incluí-lo no cabeçalho `Authorization` como `Bearer Token`.

---

## Próximos Passos e Funcionalidades Futuras

O projeto está em constante evolução. As próximas etapas importantes incluem:

* **Validações de Negócio Avançadas:** Continuar a incorporar validações mais complexas e regras de negócio específicas em nível de domínio, para além das validações de DTO.
* **Políticas de Autorização Detalhadas:** Refinar as políticas de autorização (`[Authorize(Roles = "...")`) em todos os endpoints para um controle de acesso granular baseado nas regras de negócio de cada papel.
* **Integração com Gateway de Pagamento:** Explorar a integração com gateways de pagamento externos para simular transações reais.
* **Funcionalidades de Relatórios:** Adicionar endpoints para geração de relatórios e análises de dados.

---

## Contribuição

Contribuições são bem-vindas! Sinta-se à vontade para abrir issues e pull requests para melhorias, novas funcionalidades ou correções de bugs.

---

## Licença

[MIT License](https://opensource.org/licenses/MIT)
