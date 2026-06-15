using System.Net;
using System.Net.Http.Json;
using Endereco.Api.Contratos.Responses;
using Endereco.Aplicacao.Enderecos;
using Endereco.Dominio.Enderecos;
using Endereco.Testes.Integracao.Infraestrutura;
using Microsoft.AspNetCore.Mvc;

namespace Endereco.Testes.Integracao.Api;

public sealed class TestesConsultaEnderecoEndpoint
{
    [Fact]
    public async Task Get_ComCepEncontrado_Retorna200EContratoEsperado()
    {
        await using ApiEnderecoFactory factory = new(
            ResultadoProvedorEndereco.Encontrado(CriarEndereco()));
        using HttpClient cliente = factory.CreateClient();

        using HttpResponseMessage resposta = 
            await cliente.GetAsync("/api/enderecos/01001-000");
        BuscarEnderecoResponse? conteudo = 
            await resposta.Content.ReadFromJsonAsync<BuscarEnderecoResponse>();

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        Assert.NotNull(conteudo);
        Assert.Equal("01001-000", conteudo.Cep);
        Assert.Equal("Praça da Sé", conteudo.Logradouro);
        Assert.Equal("São Paulo", conteudo.Cidade);
        Assert.Equal("SP", conteudo.Uf);
        Assert.Equal("Sudeste", conteudo.Regiao);
        Assert.Equal("01001000", factory.Provedor.UltimoCepConsultado?.Codigo);
    }

    [Fact]
    public async Task Get_ComCepInvalido_Retorna400SemConsultarProvedor()
    {
        await using ApiEnderecoFactory factory = new(
            ResultadoProvedorEndereco.NaoEncontrado());
        using HttpClient cliente = factory.CreateClient();

        using HttpResponseMessage resposta =
            await cliente.GetAsync("/api/enderecos/invalido");
        ProblemDetails? problema = 
            await resposta.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        Assert.Equal("CEP inválido", problema?.Title);
        Assert.Equal(0, factory.Provedor.QuantidadeChamadas);
    }

    [Fact]
    public async Task Get_ComCepNaoEncontrado_Retorna404()
    {
        await using ApiEnderecoFactory factory = new(
            ResultadoProvedorEndereco.NaoEncontrado());
        using HttpClient cliente = factory.CreateClient();

        using HttpResponseMessage resposta = 
            await cliente.GetAsync("/api/enderecos/00000000");

        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
    }

    [Fact]
    public async Task Get_ComProvedorIndisponivel_Retorna503SemDetalhesInternos()
    {
        await using ApiEnderecoFactory factory = new(
            ResultadoProvedorEndereco.Indisponivel());
        using HttpClient cliente = factory.CreateClient();

        using HttpResponseMessage resposta = 
            await cliente.GetAsync("/api/enderecos/01001000");
        ProblemDetails? problema =
            await resposta.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.ServiceUnavailable, resposta.StatusCode);
        Assert.Equal("Provedor de endereços indisponível", problema?.Title);
        Assert.False(problema?.Extensions.ContainsKey("exception"));
    }

    [Fact]
    public async Task Get_AcimaDoLimitePorIp_Retorna429()
    {
        await using ApiEnderecoFactory factory = new(
            ResultadoProvedorEndereco.NaoEncontrado());
        using HttpClient cliente = factory.CreateClient();

        HttpResponseMessage[] respostasPermitidas = await Task.WhenAll(
            Enumerable.Range(0, 120)
                      .Select(_ => cliente.GetAsync("/api/enderecos/01001000")));
        using HttpResponseMessage respostaExcedente = 
            await cliente.GetAsync("/api/enderecos/01001001");

        try
        {
            Assert.All(
                respostasPermitidas,
                resposta => Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode));
            Assert.Equal(HttpStatusCode.TooManyRequests, respostaExcedente.StatusCode);
        }
        finally
        {
            foreach (HttpResponseMessage resposta in respostasPermitidas)
            {
                resposta.Dispose();
            }
        }
    }

    private static Dominio.Enderecos.Endereco CriarEndereco() =>
        new()
        {
            Cep = new Cep { Codigo = "01001-000" },
            Logradouro = "Praça da Sé",
            Bairro = new Bairro { Nome = "Sé" },
            Cidade = new Cidade { Nome = "São Paulo", CodigoIbge = "3550308" },
            Uf = new Uf { Sigla = "SP" },
            Estado = new Estado { Nome = "São Paulo" },
            Regiao = new Regiao { Nome = "Sudeste" },
            Ddd = "11"
        };
}