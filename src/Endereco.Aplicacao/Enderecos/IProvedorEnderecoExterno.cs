using Endereco.Dominio.ValueObjects;

namespace Endereco.Aplicacao.Enderecos;

public interface IProvedorEnderecoExterno
{
    Task<ResultadoProvedorEndereco> BuscarAsync(Cep cep, CancellationToken cancellationToken = default);
}