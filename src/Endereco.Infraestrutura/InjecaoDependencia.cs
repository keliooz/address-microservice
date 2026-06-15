using Endereco.Aplicacao.Enderecos;
using Endereco.Infraestrutura.Enderecos.ViaCep;
using Endereco.Infraestrutura.Enderecos.Cache;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Endereco.Infraestrutura;

public static class InjecaoDependencia
{
    public static IServiceCollection AdicionarInfraestrutura(this IServiceCollection servicos,
                                                             IConfiguration configuracao)
    {
        servicos.AddOptions<ConfiguracaoCacheEndereco>()
            .Bind(configuracao.GetSection(ConfiguracaoCacheEndereco.Secao))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        servicos.AddOptions<ConfiguracaoViaCep>()
            .Bind(configuracao.GetSection(ConfiguracaoViaCep.Secao))
            .ValidateDataAnnotations()
            .Validate(
                opcoes => Uri.TryCreate(opcoes.UrlBase, UriKind.Absolute, out var uri) &&
                          uri.Scheme == Uri.UriSchemeHttps,
                "ViaCep:UrlBase deve ser uma URL HTTPS absoluta.")
            .ValidateOnStart();

        servicos.AddSingleton<CacheEnderecoMemoria>();
        servicos.AddSingleton<ControleConcorrenciaCep>();

        servicos.AddHttpClient<IProvedorEnderecoExterno, ProvedorViaCep>((provedor, cliente) =>
        {
            ConfiguracaoViaCep opcoes = provedor.GetRequiredService<IOptions<ConfiguracaoViaCep>>().Value;

            cliente.BaseAddress = new Uri(opcoes.UrlBase);
            cliente.Timeout = TimeSpan.FromSeconds(opcoes.TempoLimiteSegundos);
        });
        servicos.AddScoped<IProvedorEndereco, ProvedorEnderecoComCache>();

        return servicos;
    }
}