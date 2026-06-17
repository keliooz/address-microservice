using EnderecoDominio = Endereco.Dominio.Enderecos.Endereco;

namespace Endereco.Aplicacao.Enderecos;

/// <summary>
/// Estados possíveis do caso de uso, já traduzidos para a linguagem da aplicação.
/// </summary>
public enum StatusConsultaEndereco
{
    Encontrado,
    CepInvalido,
    NaoEncontrado,
    ProvedorIndisponivel
}

/// <summary>
/// Resultado explícito da consulta de endereço, evitando exception como fluxo esperado de negócio.
/// </summary>
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