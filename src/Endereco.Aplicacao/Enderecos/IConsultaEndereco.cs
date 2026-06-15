namespace Endereco.Aplicacao.Enderecos;

public interface IConsultaEndereco
{
    Task<ResultadoConsultaEndereco> BuscarAsync(string cep, CancellationToken cancellationToken = default);
}