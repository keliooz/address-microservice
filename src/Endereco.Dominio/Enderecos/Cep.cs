namespace Endereco.Dominio.Enderecos;

public sealed class Cep
{
    public string Codigo { get; set; } = string.Empty;

    public override string ToString() => Codigo;
}