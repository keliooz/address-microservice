using Endereco.Aplicacao.Enderecos;
using Endereco.Dominio.Enderecos;

namespace Endereco.Testes.Unitarios.Aplicacao;

public sealed class TestesConsultaEndereco
{
    [Fact]
    public async Task BuscarAsync_DelegaCepAoProvedor()
    {
        ProvedorEnderecoSimulado provedor = new();
        ConsultaEndereco consulta = new(provedor);

        await consulta.BuscarAsync("01001-000");

        Assert.Equal(1, provedor.QuantidadeChamadas);
        Assert.Equal("01001000", provedor.CepConsultado?.Codigo);
    }

    [Fact]
    public async Task BuscarAsync_ComCepInvalido_NaoConsultaProvedor()
    {
        ProvedorEnderecoSimulado provedor = new();
        ConsultaEndereco consulta = new(provedor);

        ResultadoConsultaEndereco resultado = 
            await consulta.BuscarAsync("../01001000");

        Assert.Equal(StatusConsultaEndereco.CepInvalido, resultado.Status);
        Assert.Equal(0, provedor.QuantidadeChamadas);
    }

    [Fact]
    public async Task BuscarAsync_QuandoProvedorEncontraEndereco_RetornaEndereco()
    {
        var esperado = new Endereco.Dominio.Enderecos.Endereco
        {
            Cep = new Cep { Codigo = "01001000" },
            Logradouro = "Praça da Sé",
            Bairro = new Bairro { Nome = "Sé" },
            Cidade = new Cidade { Nome = "São Paulo", CodigoIbge = "3550308" },
            Uf = new Uf { Sigla = "SP" },
            Estado = new Estado { Nome = "São Paulo" },
            Regiao = new Regiao { Nome = "Sudeste" },
            Ddd = "11"
        };
        ResultadoProvedorEndereco resultadoProvedor = ResultadoProvedorEndereco.Encontrado(esperado);
        ConsultaEndereco consulta = new(new ProvedorEnderecoSimulado(resultadoProvedor));

        ResultadoConsultaEndereco resultado = await consulta.BuscarAsync("01001-000");

        Assert.Equal(StatusConsultaEndereco.Encontrado, resultado.Status);
        Assert.Equal(esperado, resultado.Endereco);
    }

    private sealed class ProvedorEnderecoSimulado(ResultadoProvedorEndereco? resultado = null) : IProvedorEndereco
    {
        public int QuantidadeChamadas { get; private set; }

        public Cep? CepConsultado { get; private set; }

        public Task<ResultadoProvedorEndereco> BuscarAsync(Cep cep, CancellationToken cancellationToken = default)
        {
            QuantidadeChamadas++;
            CepConsultado = cep;
            return Task.FromResult(
                resultado ?? ResultadoProvedorEndereco.NaoEncontrado());
        }
    }
}