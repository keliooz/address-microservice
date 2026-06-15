namespace Endereco.Dominio.Enderecos;

public sealed class Regiao
{
    public string Nome { get; set; } = string.Empty;

    public override string ToString() => Nome;
}