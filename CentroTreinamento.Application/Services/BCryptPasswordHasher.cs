using BCrypt.Net;
using CentroTreinamento.Application.Interfaces;

namespace CentroTreinamento.Application.Services
{
    public class BCryptPasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            // BCrypt.Net.BCrypt.HashPassword é o método para gerar o hash
            // O segundo parâmetro é o "cost factor" ou "work factor", que determina a complexidade do hash.
            // 10 é um bom valor padrão. Valores mais altos são mais seguros, mas mais lentos.
            return BCrypt.Net.BCrypt.HashPassword(password, 10);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            // BCrypt.Net.BCrypt.Verify é o método para verificar se uma senha corresponde a um hash
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}