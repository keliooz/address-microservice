namespace Endereco.Dominio.Enderecos;

public sealed class Cidade
{
    public string Nome { get; set; } = string.Empty;

    public string? CodigoIbge { get; set; }

    public override string ToString() => Nome;
}