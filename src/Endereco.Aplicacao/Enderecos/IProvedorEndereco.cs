using Endereco.Dominio.Enderecos;

namespace Endereco.Aplicacao.Enderecos;

public interface IProvedorEndereco
{
    Task<ResultadoProvedorEndereco> BuscarAsync(Cep cep, CancellationToken cancellationToken = default);
}