using AutoMapper; // Para usar JsonStringEnumConverter
using CentroTreinamento.Application.Interfaces;
using CentroTreinamento.Application.Interfaces.Services;
using CentroTreinamento.Application.Services;
using CentroTreinamento.Domain.Entities;            
using CentroTreinamento.Domain.Repositories;
using CentroTreinamento.Infrastructure.Data; 
using CentroTreinamento.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer; 
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;               
using System.Text;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configuração do AutoMapper
builder.Services.AddAutoMapper(cfg => { cfg.AddProfile<CentroTreinamento.Application.Mappers.MappingProfile>(); });



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

// Registro dos Serviços de Aplicação
builder.Services.AddScoped<IAdministradorAppService, AdministradorAppService>();
builder.Services.AddScoped<IAlunoAppService, AlunoAppService>();
builder.Services.AddScoped<IInstrutorAppService, InstrutorAppService>();
builder.Services.AddScoped<IRecepcionistaAppService, RecepcionistaAppService>();
builder.Services.AddScoped<IAgendamentoAppService, AgendamentoAppService>();
builder.Services.AddScoped<IPagamentoAppService, PagamentoAppService>();
builder.Services.AddScoped<IPlanoDeTreinoAppService, PlanoDeTreinoAppService>();


// Registro do serviço de Hashing de Senhas
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();

// Registro do futuro serviço de autenticação
builder.Services.AddScoped<IAuthAppService, AuthAppService>(); 

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

// Configuração do JSON para serialização de enums como strings
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    // Se estiver usando Newtonsoft.Json:
    // options.JsonSerializerOptions.SerializerSettings.Converters.Add(new StringEnumConverter());
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
