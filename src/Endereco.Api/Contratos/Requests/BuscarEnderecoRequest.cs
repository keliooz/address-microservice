namespace Endereco.Api.Contratos.Requests;

public sealed class BuscarEnderecoRequest
{
    public string Cep { get; set; } = string.Empty;
}