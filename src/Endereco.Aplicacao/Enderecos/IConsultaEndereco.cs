namespace Endereco.Aplicacao.Enderecos;

/// <summary>
/// Executa o caso de uso de consulta de endereço, validando a entrada antes de acessar provedores.
/// </summary>
public interface IConsultaEndereco
{
    Task<ResultadoConsultaEndereco> BuscarAsync(string cep, CancellationToken cancellationToken = default);
}