using EnderecoDominio = Endereco.Dominio.Enderecos.Endereco;

namespace Endereco.Aplicacao.Enderecos;

/// <summary>
/// Estados retornados por provedores de endereço antes da tradução para o caso de uso.
/// </summary>
public enum StatusProvedorEndereco
{
    Encontrado,
    CepInvalido,
    NaoEncontrado,
    Indisponivel
}

/// <summary>
/// Resultado explícito de um provedor, isolando falhas externas dos contratos HTTP da API.
/// </summary>
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