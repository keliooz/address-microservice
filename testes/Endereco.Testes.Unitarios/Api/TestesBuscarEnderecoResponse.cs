using Endereco.Api.Contratos.Responses;
using Endereco.Dominio.Enderecos;
using Endereco.Dominio.ValueObjects;

namespace Endereco.Testes.Unitarios.Api;

public sealed class TestesBuscarEnderecoResponse
{
    [Fact]
    public void Criar_MapeiaDominioParaContratoDaApi()
    {
        Endereco.Dominio.Enderecos.Endereco endereco = new()
        {
            Cep = Cep.Criar("01001-000"),
            Logradouro = "Praça da Sé",
            Complemento = "lado ímpar",
            Bairro = new Bairro { Nome = "Sé" },
            Cidade = new Cidade { Nome = "São Paulo", CodigoIbge = "3550308" },
            Uf = new Uf { Sigla = "SP" },
            Estado = new Estado { Nome = "São Paulo" },
            Regiao = new Regiao { Nome = "Sudeste" },
            Ddd = "11"
        };

        BuscarEnderecoResponse response = BuscarEnderecoResponse.Criar(endereco);

        Assert.Equal("01001-000", response.Cep);
        Assert.Equal("Praça da Sé", response.Logradouro);
        Assert.Equal("lado ímpar", response.Complemento);
        Assert.Equal("Sé", response.Bairro);
        Assert.Equal("São Paulo", response.Cidade);
        Assert.Equal("SP", response.Uf);
        Assert.Equal("São Paulo", response.Estado);
        Assert.Equal("Sudeste", response.Regiao);
        Assert.Equal("11", response.Ddd);
        Assert.Equal("3550308", response.CodigoIbge);
    }
}