using Endereco.Aplicacao.Enderecos;
using Endereco.Dominio.ValueObjects;
using Microsoft.Extensions.Options;

namespace Endereco.Infraestrutura.Enderecos.Cache;

public sealed class ProvedorEnderecoComCache(IProvedorEnderecoExterno provedorExterno, CacheEnderecoMemoria cache,
                                             ControleConcorrenciaCep controleConcorrencia, IOptions<ConfiguracaoCacheEndereco> configuracao) : IProvedorEndereco
{
    public async Task<ResultadoProvedorEndereco> BuscarAsync(Cep cep, CancellationToken cancellationToken = default)
    {
        string chave = $"endereco:{cep.Codigo}";

        if (cache.TentarObter(chave, out ResultadoProvedorEndereco? resultadoCache))
        {
            return resultadoCache!;
        }

        using IDisposable bloqueio = 
            await controleConcorrencia.EntrarAsync(cep.Codigo, cancellationToken);

        if (cache.TentarObter(chave, out resultadoCache))
        {
            return resultadoCache!;
        }

        ResultadoProvedorEndereco resultado = 
            await provedorExterno.BuscarAsync(cep, cancellationToken);

        TimeSpan? duracao = ObterDuracao(resultado.Status);

        if (duracao is not null)
        {
            cache.Armazenar(chave, resultado, duracao.Value);
        }

        return resultado;
    }

    private TimeSpan? ObterDuracao(StatusProvedorEndereco status) =>
        status switch
        {
            StatusProvedorEndereco.Encontrado =>
                TimeSpan.FromMinutes(configuracao.Value.DuracaoEncontradoMinutos),
            StatusProvedorEndereco.NaoEncontrado =>
                TimeSpan.FromMinutes(configuracao.Value.DuracaoNaoEncontradoMinutos),
            _ => null
        };
}