# Gym Management System

Um sistema abrangente para gerenciar academias, incluindo funcionalidades para membros, treinos, pagamentos e muito mais.

## Visão Geral do Projeto

Este projeto está sendo desenvolvido utilizando as seguintes tecnologias e conceitos:

* **Linguagem:** C#
* **Framework:** .NET 8
* **Arquitetura:** Domain-Driven Design (DDD)
* **Camadas:** Domain, Application, Infrastructure, Presentation (a serem desenvolvidas)
* **Banco de Dados:** (A definir, ex: SQL Server, PostgreSQL)

## Status do Build

[![.NET CI/CD](https://github.com/Victorbicalhoa/GymManagementSystem/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Victorbicalhoa/GymManagementSystem/actions/workflows/dotnet.yml)

## Como Rodar o Projeto Localmente

### Pré-requisitos

* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) (ou outro IDE compatível com .NET)
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

4.  **Execute os testes (se houver):**
    ```bash
    dotnet test
    ```

5.  **Abra no Visual Studio:**
    Abra o arquivo `CentroTreinamento.sln` no Visual Studio.

## Contribuição

Contribuições são bem-vindas! Sinta-se à vontade para abrir issues e pull requests.

## Licença

[MIT License](https://opensource.org/licenses/MIT) (ou a licença que você preferir)
