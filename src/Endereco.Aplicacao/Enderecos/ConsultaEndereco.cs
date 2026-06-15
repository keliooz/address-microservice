using Endereco.Dominio.Enderecos;

namespace Endereco.Aplicacao.Enderecos;

public sealed class ConsultaEndereco(IProvedorEndereco provedor) : IConsultaEndereco
{
    public async Task<ResultadoConsultaEndereco> BuscarAsync(string cep, CancellationToken cancellationToken = default)
    {
        string? cepNormalizado = NormalizarCep(cep);

        if (cepNormalizado is null)
        {
            return ResultadoConsultaEndereco.CepInvalido();
        }

        ResultadoProvedorEndereco resultado = 
            await provedor.BuscarAsync(
                new Cep
                { 
                    Codigo = cepNormalizado
                }, 
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

    private static string? NormalizarCep(string cep)
    {
        Span<char> digitos = stackalloc char[8];
        int quantidade = 0;

        foreach (char caractere in cep)
        {
            if (caractere == '-')
            {
                continue;
            }

            if (caractere is not (>= '0' and <= '9') || quantidade == digitos.Length)
            {
                return null;
            }

            digitos[quantidade++] = caractere;
        }

        return quantidade == digitos.Length
            ? new string(digitos)
            : null;
    }
}