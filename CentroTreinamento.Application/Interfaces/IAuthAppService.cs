// CentroTreinamento.Application/Interfaces/IAuthAppService.cs
using CentroTreinamento.Application.DTOs;
// Remova using CentroTreinamento.Domain.Entities; se não for usado aqui para tipos de retorno
using System.Threading.Tasks;

namespace CentroTreinamento.Application.Interfaces
{
    public interface IAuthAppService
    {
        // Altere para retornar AlunoViewModel?
        Task<AlunoViewModel?> LoginAlunoAsync(LoginInputModel loginModel);

        // Adicione o método para login de Administrador
        Task<AdministradorViewModel?> LoginAdministradorAsync(LoginInputModel loginModel);
    }
}