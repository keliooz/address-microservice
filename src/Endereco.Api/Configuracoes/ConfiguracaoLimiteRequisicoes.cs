using System.ComponentModel.DataAnnotations;

namespace Endereco.Api.Configuracoes;

public sealed class ConfiguracaoLimiteRequisicoes
{
    public const string Secao = "LimiteRequisicoes";

    [Range(1, 10_000)]
    public int LimitePorMinuto { get; set; } = 120;

    [Range(0, 10_000)]
    public int TamanhoFila { get; set; }
}