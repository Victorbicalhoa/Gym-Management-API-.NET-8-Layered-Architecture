// CentroTreinamento.Application/Interfaces/IAuthAppService.cs
using CentroTreinamento.Application.DTOs;
using CentroTreinamento.Domain.Entities; // Para retornar Aluno
using System.Threading.Tasks;

namespace CentroTreinamento.Application.Interfaces
{
    public interface IAuthAppService
    {
        Task<Aluno?> LoginAlunoAsync(LoginInputModel loginModel); // Retorna Aluno ou null
    }
}