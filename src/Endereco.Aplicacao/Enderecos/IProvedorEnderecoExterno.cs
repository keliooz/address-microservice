using Endereco.Dominio.Enderecos;

namespace Endereco.Aplicacao.Enderecos;

public interface IProvedorEnderecoExterno
{
    Task<ResultadoProvedorEndereco> BuscarAsync(Cep cep, CancellationToken cancellationToken = default);
}