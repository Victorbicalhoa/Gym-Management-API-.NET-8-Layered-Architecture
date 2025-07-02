using CentroTreinamento.Application.Interfaces;
using CentroTreinamento.Application.Services;
using CentroTreinamento.Domain.Entities;            
using CentroTreinamento.Domain.Repositories;
using CentroTreinamento.Infrastructure.Data; 
using CentroTreinamento.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer; 
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;               
using System.Text;                                 


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configuração do DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registro dos Repositórios para Injeção de Dependência
builder.Services.AddScoped<IAlunoRepository, AlunoRepository>();
builder.Services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();
builder.Services.AddScoped<IAdministradorRepository, AdministradorRepository>();
builder.Services.AddScoped<IInstrutorRepository, InstrutorRepository>();
builder.Services.AddScoped<IPagamentoRepository, PagamentoRepository>();
builder.Services.AddScoped<IPlanoDeTreinoRepository, PlanoDeTreinoRepository>();
builder.Services.AddScoped<IRecepcionistaRepository, RecepcionistaRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Registrar o AlunoRepository
builder.Services.AddScoped<CentroTreinamento.Domain.Repositories.IAlunoRepository, CentroTreinamento.Infrastructure.Repositories.AlunoRepository>();

// Registrar o PasswordHasher
builder.Services.AddScoped<CentroTreinamento.Application.Interfaces.IPasswordHasher, CentroTreinamento.Application.Services.BCryptPasswordHasher>();

// Registrar o AuthAppService
builder.Services.AddScoped<CentroTreinamento.Application.Interfaces.IAuthAppService, CentroTreinamento.Application.Services.AuthAppService>();

// Registro dos Serviços de Aplicação
builder.Services.AddScoped<IAlunoAppService, AlunoAppService>();

// Registro do serviço de Hashing de Senhas
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();

// Registro do futuro serviço de autenticação
builder.Services.AddScoped<IAuthAppService, AuthAppService>(); // Isso será implementado em breve!

// Configuração JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]!); // Usamos ! para garantir que não será nulo

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Em produção, SEMPRE TRUE (para desenvolvimento, false é ok)
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true, // Deve validar o emissor do token
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true, // Deve validar o público do token
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true, // Valida a data de expiração do token
        ClockSkew = TimeSpan.Zero // Não permite tolerância para o tempo de expiração
    };
});

builder.Services.AddAuthorization(); // Habilita o uso de atributos [Authorize] e [AllowAnonymous]

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Adiciona os middlewares de autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
