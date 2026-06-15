namespace Endereco.Dominio.Enderecos;

public sealed class Bairro
{
    public string Nome { get; set; } = string.Empty;

    public override string ToString() => Nome;
}