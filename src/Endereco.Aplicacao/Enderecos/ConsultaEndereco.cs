using Endereco.Dominio.ValueObjects;

namespace Endereco.Aplicacao.Enderecos;

/// <summary>
/// Orquestra a consulta de endereço e traduz respostas do provedor para estados do caso de uso.
/// </summary>
public sealed class ConsultaEndereco(IProvedorEndereco provedor) : IConsultaEndereco
{
    public async Task<ResultadoConsultaEndereco> BuscarAsync(string cep, CancellationToken cancellationToken = default)
    {
        if (!Cep.TentarCriar(cep, out Cep? cepValido))
        {
            return ResultadoConsultaEndereco.CepInvalido();
        }

        ResultadoProvedorEndereco resultado = 
            await provedor.BuscarAsync(
                cepValido, 
                cancellationToken);

        return resultado.Status switch
        {
            StatusProvedorEndereco.Encontrado =>
                ResultadoConsultaEndereco.Encontrado(resultado.Endereco!),
            StatusProvedorEndereco.CepInvalido =>
                ResultadoConsultaEndereco.CepInvalido(),
            StatusProvedorEndereco.NaoEncontrado =>
                ResultadoConsultaEndereco.NaoEncontrado(),
            _ => ResultadoConsultaEndereco.ProvedorIndisponivel()
        };
    }
}