// CentroTreinamento.Application/Interfaces/IPasswordHasher.cs
namespace CentroTreinamento.Application.Interfaces
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
}