using System.Text.Json.Serialization;

namespace Endereco.Infraestrutura.Enderecos.ViaCep.Responses;

public sealed class ConsultaViaCepResponse
{
    public string? Cep { get; set; }

    public string? Logradouro { get; set; }

    public string? Complemento { get; set; }

    public string? Bairro { get; set; }

    public string? Localidade { get; set; }

    public string? Uf { get; set; }

    public string? Estado { get; set; }

    public string? Regiao { get; set; }

    public string? Ibge { get; set; }

    public string? Ddd { get; set; }

    [JsonPropertyName("erro")]
    public bool Erro { get; set; }
}