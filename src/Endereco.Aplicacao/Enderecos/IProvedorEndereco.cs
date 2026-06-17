using Endereco.Dominio.ValueObjects;

namespace Endereco.Aplicacao.Enderecos;

/// <summary>
/// Porta usada pela aplicação para consultar endereços sem conhecer cache ou integrações externas.
/// </summary>
public interface IProvedorEndereco
{
    Task<ResultadoProvedorEndereco> BuscarAsync(Cep cep, CancellationToken cancellationToken = default);
}