namespace Endereco.Dominio.Enderecos;

public sealed class Uf
{
    public string Sigla { get; set; } = string.Empty;

    public override string ToString() => Sigla;
}