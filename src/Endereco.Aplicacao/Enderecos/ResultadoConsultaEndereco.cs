using EnderecoDominio = Endereco.Dominio.Enderecos.Endereco;

namespace Endereco.Aplicacao.Enderecos;

public enum StatusConsultaEndereco
{
    Encontrado,
    CepInvalido,
    NaoEncontrado,
    ProvedorIndisponivel
}

public sealed class ResultadoConsultaEndereco
{
    private ResultadoConsultaEndereco(StatusConsultaEndereco status, EnderecoDominio? endereco = null)
    {
        Status = status;
        Endereco = endereco;
    }

    public StatusConsultaEndereco Status { get; }

    public EnderecoDominio? Endereco { get; }

    public static ResultadoConsultaEndereco Encontrado(EnderecoDominio endereco) =>
        new(StatusConsultaEndereco.Encontrado, endereco);

    public static ResultadoConsultaEndereco CepInvalido() =>
        new(StatusConsultaEndereco.CepInvalido);

    public static ResultadoConsultaEndereco NaoEncontrado() =>
        new(StatusConsultaEndereco.NaoEncontrado);

    public static ResultadoConsultaEndereco ProvedorIndisponivel() =>
        new(StatusConsultaEndereco.ProvedorIndisponivel);
}