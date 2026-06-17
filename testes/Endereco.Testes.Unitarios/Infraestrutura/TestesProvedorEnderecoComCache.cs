using Endereco.Aplicacao.Enderecos;
using Endereco.Dominio.Enderecos;
using Endereco.Dominio.ValueObjects;
using Endereco.Infraestrutura.Enderecos.Cache;
using Microsoft.Extensions.Options;

namespace Endereco.Testes.Unitarios.Infraestrutura;

public sealed class TestesProvedorEnderecoComCache
{
    [Theory]
    [InlineData(StatusProvedorEndereco.Encontrado)]
    [InlineData(StatusProvedorEndereco.NaoEncontrado)]
    public async Task BuscarAsync_QuandoResultadoPodeSerCacheado_ConsultaViaCepUmaVez(StatusProvedorEndereco status)
    {
        ProvedorExternoSimulado externo = new(CriarResultado(status));
        ProvedorEnderecoComCache provedor = CriarProvedor(externo);
        Cep cep = Cep.Criar("01001000");

        await provedor.BuscarAsync(cep);
        await provedor.BuscarAsync(cep);

        Assert.Equal(1, externo.QuantidadeChamadas);
    }

    [Theory]
    [InlineData(StatusProvedorEndereco.CepInvalido)]
    [InlineData(StatusProvedorEndereco.Indisponivel)]
    public async Task BuscarAsync_QuandoResultadoNaoPodeSerCacheado_ConsultaViaCepNovamente(StatusProvedorEndereco status)
    {
        ProvedorExternoSimulado externo = new(CriarResultado(status));
        ProvedorEnderecoComCache provedor = CriarProvedor(externo);
        Cep cep = Cep.Criar("01001000");

        await provedor.BuscarAsync(cep);
        await provedor.BuscarAsync(cep);

        Assert.Equal(2, externo.QuantidadeChamadas);
    }

    [Fact]
    public async Task BuscarAsync_ComConsultasConcorrentes_ConsultaProvedorExternoUmaVez()
    {
        ProvedorExternoSimulado externo = new(
            CriarResultado(StatusProvedorEndereco.Encontrado),
            TimeSpan.FromMilliseconds(50));
        ProvedorEnderecoComCache provedor = CriarProvedor(externo);
        Cep cep = Cep.Criar("01001000");

        await Task.WhenAll(Enumerable.Range(0, 20).Select(_ => provedor.BuscarAsync(cep)));

        Assert.Equal(1, externo.QuantidadeChamadas);
    }

    private static ProvedorEnderecoComCache CriarProvedor(IProvedorEnderecoExterno externo)
    {
        IOptions<ConfiguracaoCacheEndereco> configuracao = Options.Create(new ConfiguracaoCacheEndereco
        {
            LimiteEntradas = 100,
            DuracaoEncontradoMinutos = 60,
            DuracaoNaoEncontradoMinutos = 5
        });

        return new ProvedorEnderecoComCache(
            externo,
            new CacheEnderecoMemoria(configuracao),
            new ControleConcorrenciaCep(),
            configuracao);
    }

    private static ResultadoProvedorEndereco CriarResultado(StatusProvedorEndereco status) =>
        status switch
        {
            StatusProvedorEndereco.Encontrado => ResultadoProvedorEndereco.Encontrado(
                new Endereco.Dominio.Enderecos.Endereco
                {
                    Cep = Cep.Criar("01001-000")
                }),
            StatusProvedorEndereco.CepInvalido => ResultadoProvedorEndereco.CepInvalido(),
            StatusProvedorEndereco.NaoEncontrado => ResultadoProvedorEndereco.NaoEncontrado(),
            _ => ResultadoProvedorEndereco.Indisponivel()
        };

    private sealed class ProvedorExternoSimulado(ResultadoProvedorEndereco resultado, TimeSpan? atraso = null) : IProvedorEnderecoExterno
    {
        public int QuantidadeChamadas { get; private set; }

        public async Task<ResultadoProvedorEndereco> BuscarAsync(Cep cep, CancellationToken cancellationToken)
        {
            QuantidadeChamadas++;

            if (atraso is not null)
            {
                await Task.Delay(atraso.Value, cancellationToken);
            }

            return resultado;
        }
    }
}