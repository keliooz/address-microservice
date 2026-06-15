using EnderecoDominio = Endereco.Dominio.Enderecos.Endereco;

namespace Endereco.Aplicacao.Enderecos;

public enum StatusProvedorEndereco
{
    Encontrado,
    CepInvalido,
    NaoEncontrado,
    Indisponivel
}

public sealed class ResultadoProvedorEndereco
{
    private ResultadoProvedorEndereco(StatusProvedorEndereco status, EnderecoDominio? endereco = null)
    {
        Status = status;
        Endereco = endereco;
    }

    public StatusProvedorEndereco Status { get; }

    public EnderecoDominio? Endereco { get; }

    public static ResultadoProvedorEndereco Encontrado(EnderecoDominio endereco) =>
        new(StatusProvedorEndereco.Encontrado, endereco);

    public static ResultadoProvedorEndereco CepInvalido() =>
        new(StatusProvedorEndereco.CepInvalido);

    public static ResultadoProvedorEndereco NaoEncontrado() =>
        new(StatusProvedorEndereco.NaoEncontrado);

    public static ResultadoProvedorEndereco Indisponivel() =>
        new(StatusProvedorEndereco.Indisponivel);
}