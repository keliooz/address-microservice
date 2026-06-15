using Endereco.Aplicacao.Enderecos;
using Microsoft.Extensions.DependencyInjection;

namespace Endereco.Aplicacao;

public static class InjecaoDependencia
{
    public static IServiceCollection AdicionarAplicacao(this IServiceCollection servicos)
    {
        servicos.AddScoped<IConsultaEndereco, ConsultaEndereco>();

        return servicos;
    }
}