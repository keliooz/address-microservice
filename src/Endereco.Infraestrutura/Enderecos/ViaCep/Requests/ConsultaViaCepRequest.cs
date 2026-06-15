using Endereco.Dominio.Enderecos;

namespace Endereco.Infraestrutura.Enderecos.ViaCep.Requests;

public sealed class ConsultaViaCepRequest
{
    public Cep Cep { get; set; } = new();

    public override string ToString() => $"{Cep.Codigo}/json/";
}