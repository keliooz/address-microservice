using System.ComponentModel.DataAnnotations;

namespace Endereco.Infraestrutura.Enderecos.Cache;

public sealed class ConfiguracaoCacheEndereco
{
    public const string Secao = "CacheEndereco";

    [Range(1, 1_000_000)]
    public int LimiteEntradas { get; set; } = 100_000;

    [Range(1, 10_080)]
    public int DuracaoEncontradoMinutos { get; set; } = 1440;

    [Range(1, 1440)]
    public int DuracaoNaoEncontradoMinutos { get; set; } = 15;
}