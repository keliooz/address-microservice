using System.ComponentModel.DataAnnotations;

namespace Endereco.Infraestrutura.Enderecos.ViaCep;

public sealed class ConfiguracaoViaCep
{
    public const string Secao = "ViaCep";

    [Required]
    public string UrlBase { get; set; } = "https://viacep.com.br/ws/";

    [Range(1, 30)]
    public int TempoLimiteSegundos { get; set; } = 5;
}