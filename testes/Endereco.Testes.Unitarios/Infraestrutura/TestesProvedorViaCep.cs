using System.Net;
using System.Text;
using Endereco.Aplicacao.Enderecos;
using Endereco.Dominio.Enderecos;
using Endereco.Dominio.ValueObjects;
using Endereco.Infraestrutura.Enderecos.ViaCep;

namespace Endereco.Testes.Unitarios.Infraestrutura;

public sealed class TestesProvedorViaCep
{
    [Fact]
    public async Task BuscarAsync_QuandoCepExiste_RetornaEndereco()
    {
        const string json = """
            {
              "cep": "01001-000",
              "logradouro": "Praça da Sé",
              "complemento": "lado ímpar",
              "bairro": "Sé",
              "localidade": "São Paulo",
              "uf": "SP",
              "estado": "São Paulo",
              "regiao": "Sudeste",
              "ibge": "3550308",
              "ddd": "11"
            }
            """;
        ProvedorViaCepTestavel provedor = CriarProvedor(HttpStatusCode.OK, json);

        ResultadoProvedorEndereco resultado = await provedor.BuscarAsync(Cep.Criar("01001000"));

        Assert.Equal(StatusProvedorEndereco.Encontrado, resultado.Status);
        Assert.Equal("01001000", resultado.Endereco!.Cep.Codigo);
        Assert.Equal("01001-000", resultado.Endereco.Cep.Formatado);
        Assert.Equal("Praça da Sé", resultado.Endereco.Logradouro);
        Assert.Equal("Sé", resultado.Endereco.Bairro!.Nome);
        Assert.Equal("São Paulo", resultado.Endereco.Cidade.Nome);
        Assert.Equal("3550308", resultado.Endereco.Cidade.CodigoIbge);
        Assert.Equal("SP", resultado.Endereco.Uf.Sigla);
        Assert.Equal("São Paulo", resultado.Endereco.Estado.Nome);
        Assert.Equal("Sudeste", resultado.Endereco.Regiao.Nome);
        Assert.Equal("11", resultado.Endereco.Ddd);
        Assert.Equal(
            "https://viacep.com.br/ws/01001000/json/",
            provedor.UltimaUrlConsultada);
    }

    [Fact]
    public async Task BuscarAsync_QuandoCepNaoExiste_RetornaNaoEncontrado()
    {
        ProvedorViaCepTestavel provedor = CriarProvedor(HttpStatusCode.OK, """{"erro": true}""");

        ResultadoProvedorEndereco resultado =
            await provedor.BuscarAsync(Cep.Criar("00000000"));

        Assert.Equal(StatusProvedorEndereco.NaoEncontrado, resultado.Status);
        Assert.Null(resultado.Endereco);
    }

    [Fact]
    public async Task BuscarAsync_QuandoViaCepFalha_RetornaIndisponivel()
    {
        ProvedorViaCepTestavel provedor = CriarProvedor(HttpStatusCode.InternalServerError, string.Empty);

        ResultadoProvedorEndereco resultado = 
            await provedor.BuscarAsync(Cep.Criar("01001000"));

        Assert.Equal(StatusProvedorEndereco.Indisponivel, resultado.Status);
    }

    [Fact]
    public async Task BuscarAsync_QuandoViaCepRetornaBadRequest_RetornaCepInvalido()
    {
        ProvedorViaCepTestavel provedor = CriarProvedor(HttpStatusCode.BadRequest, string.Empty);

        ResultadoProvedorEndereco resultado = 
            await provedor.BuscarAsync(Cep.Criar("01001000"));

        Assert.Equal(StatusProvedorEndereco.CepInvalido, resultado.Status);
    }

    [Fact]
    public async Task BuscarAsync_QuandoCamposEstaoVazios_MapeiaComoNulo()
    {
        const string json = """
            {
              "cep": "01001-000",
              "logradouro": "",
              "complemento": "",
              "bairro": "",
              "localidade": "São Paulo",
              "uf": "SP",
              "estado": "São Paulo",
              "regiao": "Sudeste",
              "ibge": "3550308",
              "ddd": ""
            }
            """;
        ProvedorViaCepTestavel provedor = CriarProvedor(HttpStatusCode.OK, json);

        ResultadoProvedorEndereco resultado = await provedor.BuscarAsync(Cep.Criar("01001000"));

        Assert.Null(resultado.Endereco!.Logradouro);
        Assert.Null(resultado.Endereco.Complemento);
        Assert.Null(resultado.Endereco.Bairro);
        Assert.Null(resultado.Endereco.Ddd);
    }

    private static ProvedorViaCepTestavel CriarProvedor(HttpStatusCode status, string conteudo)
    {
        ManipuladorHttpSimulado manipulador = new(status, conteudo);
        HttpClient cliente = new(manipulador)
        {
            BaseAddress = new Uri("https://viacep.com.br/ws/")
        };

        return new ProvedorViaCepTestavel(new ProvedorViaCep(cliente), manipulador);
    }

    private sealed class ProvedorViaCepTestavel(ProvedorViaCep provedor, ManipuladorHttpSimulado manipulador)
    {
        public string? UltimaUrlConsultada => manipulador.UltimaUrlConsultada;

        public Task<ResultadoProvedorEndereco> BuscarAsync(Cep cep) =>
            provedor.BuscarAsync(cep);
    }

    private sealed class ManipuladorHttpSimulado(HttpStatusCode status, string conteudo) : HttpMessageHandler
    {
        public string? UltimaUrlConsultada { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            UltimaUrlConsultada = request.RequestUri?.ToString();

            HttpResponseMessage resposta = new(status)
            {
                Content = new StringContent(conteudo, Encoding.UTF8, "application/json"),
                RequestMessage = request
            };

            return Task.FromResult(resposta);
        }
    }
}