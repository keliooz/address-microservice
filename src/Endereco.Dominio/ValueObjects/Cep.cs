using System.Diagnostics.CodeAnalysis;

namespace Endereco.Dominio.ValueObjects;

public sealed record Cep
{
    private Cep(string codigo)
    {
        Codigo = codigo;
    }

    public string Codigo { get; }

    public string Formatado => $"{Codigo[..5]}-{Codigo[5..]}";

    public static Cep Criar(string? valor)
    {
        if (!TentarCriar(valor, out Cep? cep))
        {
            throw new ArgumentException("O CEP deve conter exatamente 8 dígitos.", nameof(valor));
        }

        return cep!;
    }

    public static bool TentarCriar(string? valor, [NotNullWhen(true)] out Cep? cep)
    {
        cep = null;

        if (string.IsNullOrWhiteSpace(valor))
        {
            return false;
        }

        Span<char> digitos = stackalloc char[8];
        int quantidade = 0;

        foreach (char caractere in valor)
        {
            if (caractere == '-')
            {
                continue;
            }

            if (caractere is not (>= '0' and <= '9') || quantidade == digitos.Length)
            {
                return false;
            }

            digitos[quantidade++] = caractere;
        }

        if (quantidade != digitos.Length)
        {
            return false;
        }

        cep = new Cep(new string(digitos));
        return true;
    }

    public override string ToString() => Formatado;
}