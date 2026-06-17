using Endereco.Dominio.ValueObjects;

namespace Endereco.Aplicacao.Enderecos;

/// <summary>
/// Porta de integração com serviços externos capazes de resolver um endereço por CEP.
/// </summary>
public interface IProvedorEnderecoExterno
{
    Task<ResultadoProvedorEndereco> BuscarAsync(Cep cep, CancellationToken cancellationToken = default);
}