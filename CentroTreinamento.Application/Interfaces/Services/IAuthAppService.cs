// CentroTreinamento.Application/Interfaces/IAuthAppService.cs
using CentroTreinamento.Application.DTOs.Administrador;
using CentroTreinamento.Application.DTOs.Aluno;
using CentroTreinamento.Application.DTOs.Auth;
using CentroTreinamento.Application.DTOs.Instrutor;
using CentroTreinamento.Application.DTOs.Recepcionista;




// Remova using CentroTreinamento.Domain.Entities; se não for usado aqui para tipos de retorno
using System.Threading.Tasks;

namespace CentroTreinamento.Application.Interfaces.Services
{
    public interface IAuthAppService
    {
        //  Adicione o método para login de Aluno
        Task<AlunoViewModel?> LoginAlunoAsync(LoginInputModel loginModel);

        // Adicione o método para login de Administrador
        Task<AdministradorViewModel?> LoginAdministradorAsync(LoginInputModel loginModel);

        // Adicione o método para login de Instrutor
        Task<InstrutorViewModel?> LoginInstrutorAsync(LoginInputModel loginModel);

        // Adicione o método para login de Recepcionista
        Task<RecepcionistaViewModel?> LoginRecepcionistaAsync(LoginInputModel loginModel); // Adicionar este método
    }
}