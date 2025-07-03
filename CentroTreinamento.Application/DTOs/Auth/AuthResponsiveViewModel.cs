namespace CentroTreinamento.Application.DTOs.Auth
{
    public class AuthResponseViewModel
    {
        public string AccessToken { get; set; } = string.Empty;
        public string TokenType { get; set; } = "Bearer";
        public int ExpiresIn { get; set; } // Tempo de expiração em segundos
        public string RefreshToken { get; set; } = string.Empty; // Opcional, para futuros Refresh Tokens
        public string Cpf { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}