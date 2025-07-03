// CentroTreinamento.Application/Services/AuthAppService.cs
using CentroTreinamento.Application.Interfaces; // Para IPasswordHasher
using CentroTreinamento.Domain.Repositories;
using CentroTreinamento.Domain.Entities; // Para acessar Aluno, Administrador e Instrutor
using System;
using System.Linq;
using System.Threading.Tasks;
using CentroTreinamento.Application.DTOs.Auth;
using CentroTreinamento.Application.DTOs.Aluno;
using CentroTreinamento.Application.DTOs.Administrador;
using CentroTreinamento.Application.DTOs.Instrutor; // Adicionado para InstrutorViewModel
using CentroTreinamento.Application.Interfaces.Services;
using CentroTreinamento.Domain.Enums; // Para UserRole, StatusInstrutor etc.

namespace CentroTreinamento.Application.Services
{
    public class AuthAppService : IAuthAppService
    {
        private readonly IAlunoRepository _alunoRepository;
        private readonly IAdministradorRepository _administradorRepository;
        private readonly IInstrutorRepository _instrutorRepository; // NOVO: Para buscar instrutores
        private readonly IPasswordHasher _passwordHasher;

        public AuthAppService(IAlunoRepository alunoRepository,
                              IAdministradorRepository administradorRepository,
                              IInstrutorRepository instrutorRepository, // NOVO: Injetar o repositório do instrutor
                              IPasswordHasher passwordHasher)
        {
            _alunoRepository = alunoRepository;
            _administradorRepository = administradorRepository;
            _instrutorRepository = instrutorRepository; // Atribua o repositório do instrutor
            _passwordHasher = passwordHasher;
        }

        public async Task<AlunoViewModel?> LoginAlunoAsync(LoginInputModel loginModel)
        {
            var cpfLoginLimpo = loginModel.Cpf!.Replace(".", "").Replace("-", "");

            var aluno = (await _alunoRepository.FindAsync(a => a.Cpf == cpfLoginLimpo))
                                 .FirstOrDefault();

            if (aluno == null)
            {
                return null;
            }

            if (!_passwordHasher.VerifyPassword(loginModel.Senha!, aluno.SenhaHash!))
            {
                return null;
            }

            return new AlunoViewModel
            {
                Id = aluno.Id,
                Nome = aluno.Nome,
                Cpf = aluno.Cpf,
                // Status = aluno.Status, // Verifique se Aluno tem um Status, se não, remova
                Role = aluno.Role
            };
        }

        public async Task<AdministradorViewModel?> LoginAdministradorAsync(LoginInputModel loginModel)
        {
            var cpfLoginLimpo = loginModel.Cpf!.Replace(".", "").Replace("-", "");

            var administrador = (await _administradorRepository.FindAsync(a => a.Cpf == cpfLoginLimpo))
                                       .FirstOrDefault();

            if (administrador == null)
            {
                return null;
            }

            if (!_passwordHasher.VerifyPassword(loginModel.Senha!, administrador.SenhaHash!))
            {
                return null;
            }

            return new AdministradorViewModel
            {
                Id = administrador.Id,
                Nome = administrador.Nome,
                Cpf = administrador.Cpf,
                Status = administrador.Status,
                Role = administrador.Role
            };
        }

        // NOVO: Método de login para Instrutor
        public async Task<InstrutorViewModel?> LoginInstrutorAsync(LoginInputModel loginModel)
        {
            var cpfLoginLimpo = loginModel.Cpf!.Replace(".", "").Replace("-", "");

            var instrutor = (await _instrutorRepository.FindAsync(i => i.Cpf == cpfLoginLimpo))
                                       .FirstOrDefault();

            if (instrutor == null)
            {
                return null; // Instrutor não encontrado
            }

            // A senha que o usuário digitou (loginModel.Senha) deve ser verificada contra a SenhaHash do banco
            if (!_passwordHasher.VerifyPassword(loginModel.Senha!, instrutor.SenhaHash!))
            {
                return null; // Senha incorreta
            }

            // Mapeia a entidade Instrutor para InstrutorViewModel
            return new InstrutorViewModel
            {
                Id = instrutor.Id,
                Nome = instrutor.Nome!,
                Cpf = instrutor.Cpf,
                Cref = instrutor.Cref!, // Adicione o CREF
                Status = instrutor.Status, // Adicione o Status
                Role = instrutor.Role
            };
        }
    }
}