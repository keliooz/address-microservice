using Endereco.Dominio.ValueObjects;

namespace Endereco.Aplicacao.Enderecos;

public interface IProvedorEndereco
{
    Task<ResultadoProvedorEndereco> BuscarAsync(Cep cep, CancellationToken cancellationToken = default);
}