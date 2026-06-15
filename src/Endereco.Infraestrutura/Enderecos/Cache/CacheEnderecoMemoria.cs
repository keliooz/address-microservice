using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Endereco.Aplicacao.Enderecos;

namespace Endereco.Infraestrutura.Enderecos.Cache;

public sealed class CacheEnderecoMemoria : IDisposable
{
    private readonly MemoryCache cache;

    public CacheEnderecoMemoria(IOptions<ConfiguracaoCacheEndereco> configuracao)
    {
        cache = new MemoryCache(new MemoryCacheOptions
        {
            SizeLimit = configuracao.Value.LimiteEntradas
        });
    }

    public bool TentarObter(string chave, out ResultadoProvedorEndereco? resultado) =>
        cache.TryGetValue(chave, out resultado);

    public void Armazenar(string chave, ResultadoProvedorEndereco resultado, TimeSpan duracao)
    {
        MemoryCacheEntryOptions opcoes = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(duracao)
            .SetSize(1);

        cache.Set(chave, resultado, opcoes);
    }

    public void Dispose() => cache.Dispose();
}