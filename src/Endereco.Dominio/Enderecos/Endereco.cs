namespace Endereco.Dominio.Enderecos;

public sealed class Endereco
{
    public Cep Cep { get; set; } = new();

    public string? Logradouro { get; set; }

    public string? Complemento { get; set; }

    public Bairro? Bairro { get; set; }

    public Cidade Cidade { get; set; } = new();

    public Uf Uf { get; set; } = new();

    public Estado Estado { get; set; } = new();

    public Regiao Regiao { get; set; } = new();

    public string? Ddd { get; set; }

    public override string ToString() =>
        $"{Logradouro}, {Bairro}, {Cidade} - {Uf}, {Cep}";
}