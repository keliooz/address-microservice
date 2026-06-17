using Endereco.Dominio.ValueObjects;

namespace Endereco.Infraestrutura.Enderecos.ViaCep.Requests;

public sealed class ConsultaViaCepRequest
{
    public required Cep Cep { get; set; }

    public override string ToString() => $"{Cep.Codigo}/json/";
}