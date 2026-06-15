using Endereco.Api.Configuracoes;
using Endereco.Api.Rotas;
using Endereco.Aplicacao;
using Endereco.Infraestrutura;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddOptions<ConfiguracaoLimiteRequisicoes>()
    .Bind(builder.Configuration.GetSection(ConfiguracaoLimiteRequisicoes.Secao))
    .ValidateDataAnnotations()
    .ValidateOnStart();

var limiteRequisicoes = builder.Configuration
    .GetSection(ConfiguracaoLimiteRequisicoes.Secao)
    .Get<ConfiguracaoLimiteRequisicoes>() ?? new ConfiguracaoLimiteRequisicoes();

builder.Services
    .AdicionarAplicacao()
    .AdicionarInfraestrutura(builder.Configuration);
builder.Services.AddRateLimiter(opcoes =>
{
    opcoes.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    opcoes.AddPolicy("consulta-cep", contexto =>
        RateLimitPartition.GetFixedWindowLimiter(
            contexto.Connection.RemoteIpAddress?.ToString() ?? "desconhecido",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = limiteRequisicoes.LimitePorMinuto,
                QueueLimit = limiteRequisicoes.TamanhoFila,
                Window = TimeSpan.FromMinutes(1)
            }));
});

var app = builder.Build();

app.UseExceptionHandler();
app.UseRateLimiter();
app.MapearRotasEndereco();

app.Run();