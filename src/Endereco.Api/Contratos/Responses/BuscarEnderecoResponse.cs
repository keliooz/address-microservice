using EnderecoDominio = Endereco.Dominio.Enderecos.Endereco;

namespace Endereco.Api.Contratos.Responses;

public sealed class BuscarEnderecoResponse
{
    public string Cep { get; set; } = string.Empty;

    public string? Logradouro { get; set; }

    public string? Complemento { get; set; }

    public string? Bairro { get; set; }

    public string Cidade { get; set; } = string.Empty;

    public string Uf { get; set; } = string.Empty;

    public string Estado { get; set; } = string.Empty;

    public string Regiao { get; set; } = string.Empty;

    public string? Ddd { get; set; }

    public string? CodigoIbge { get; set; }

    public static BuscarEnderecoResponse Criar(EnderecoDominio endereco) =>
        new()
        {
            Cep = endereco.Cep.Codigo,
            Logradouro = endereco.Logradouro,
            Complemento = endereco.Complemento,
            Bairro = endereco.Bairro?.Nome,
            Cidade = endereco.Cidade.Nome,
            Uf = endereco.Uf.Sigla,
            Estado = endereco.Estado.Nome,
            Regiao = endereco.Regiao.Nome,
            Ddd = endereco.Ddd,
            CodigoIbge = endereco.Cidade.CodigoIbge
        };
}