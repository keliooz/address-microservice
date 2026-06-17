using Endereco.Aplicacao.Enderecos;
using Endereco.Dominio.Enderecos;
using Endereco.Dominio.ValueObjects;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Endereco.Testes.Integracao.Infraestrutura;

public sealed class ApiEnderecoFactory(ResultadoProvedorEndereco resultado) : WebApplicationFactory<Program>
{
    public ProvedorEnderecoSimulado Provedor { get; } = new(resultado);

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(servicos =>
        {
            servicos.RemoveAll<IProvedorEndereco>();
            servicos.AddSingleton<IProvedorEndereco>(Provedor);
        });
    }
}

public sealed class ProvedorEnderecoSimulado(ResultadoProvedorEndereco resultado) : IProvedorEndereco
{
    public Cep? UltimoCepConsultado { get; private set; }

    public int QuantidadeChamadas { get; private set; }

    public Task<ResultadoProvedorEndereco> BuscarAsync(Cep cep, CancellationToken cancellationToken = default)
    {
        UltimoCepConsultado = cep;
        QuantidadeChamadas++;

        return Task.FromResult(resultado);
    }
}