using Endereco.Dominio.Enderecos;
using Endereco.Dominio.ValueObjects;

namespace Endereco.Testes.Unitarios.Dominio;

public sealed class TestesModelosEndereco
{
    [Fact]
    public void Modelos_RetornamPropriedadePrincipalNoToString()
    {
        Cep cep = Cep.Criar("01001-000");
        Bairro bairro = new() { Nome = "Sé" };
        Cidade cidade = new() { Nome = "São Paulo" };
        Estado estado = new() { Nome = "São Paulo" };
        Regiao regiao = new() { Nome = "Sudeste" };
        Uf uf = new() { Sigla = "SP" };

        Assert.Equal("01001000", cep.Codigo);
        Assert.Equal("01001-000", cep.ToString());
        Assert.Equal(bairro.Nome, bairro.ToString());
        Assert.Equal(cidade.Nome, cidade.ToString());
        Assert.Equal(estado.Nome, estado.ToString());
        Assert.Equal(regiao.Nome, regiao.ToString());
        Assert.Equal(uf.Sigla, uf.ToString());
    }

    [Fact]
    public void Endereco_RetornaEnderecoFormatadoNoToString()
    {
        Endereco.Dominio.Enderecos.Endereco endereco = new()
        {
            Cep = Cep.Criar("01001-000"),
            Logradouro = "Praça da Sé",
            Bairro = new Bairro { Nome = "Sé" },
            Cidade = new Cidade { Nome = "São Paulo" },
            Uf = new Uf { Sigla = "SP" }
        };

        Assert.Equal(
            "Praça da Sé, Sé, São Paulo - SP, 01001-000",
            endereco.ToString());
    }

    [Theory]
    [InlineData("01001000", "01001000", "01001-000")]
    [InlineData("01001-000", "01001000", "01001-000")]
    public void Cep_Criar_ComValorValido_NormalizaEFormata(string valor, string codigo, string formatado)
    {
        Cep cep = Cep.Criar(valor);

        Assert.Equal(codigo, cep.Codigo);
        Assert.Equal(formatado, cep.Formatado);
    }

    [Theory]
    [InlineData("")]
    [InlineData("0100100")]
    [InlineData("010010000")]
    [InlineData("01001A00")]
    public void Cep_TentarCriar_ComValorInvalido_RetornaFalso(string valor)
    {
        bool criado = Cep.TentarCriar(valor, out Cep? cep);

        Assert.False(criado);
        Assert.Null(cep);
    }
}