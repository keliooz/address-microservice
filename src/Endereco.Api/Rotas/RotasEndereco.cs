using Endereco.Aplicacao.Enderecos;
using Endereco.Api.Contratos.Requests;
using Endereco.Api.Contratos.Responses;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Endereco.Api.Rotas;

public static class RotasEndereco
{
    public static IEndpointRouteBuilder MapearRotasEndereco(this IEndpointRouteBuilder rotas)
    {
        rotas.MapGet("/api/enderecos/{cep}", BuscarEnderecoAsync)
            .WithName("BuscarEnderecoPorCep")
            .RequireRateLimiting("consulta-cep");

        return rotas;
    }

    private static async Task<Results<Ok<BuscarEnderecoResponse>, ProblemHttpResult, NotFound>> BuscarEnderecoAsync(
        [AsParameters] BuscarEnderecoRequest request,
        IConsultaEndereco consultaEndereco,
        CancellationToken cancellationToken)
    {
        ResultadoConsultaEndereco resultado = 
            await consultaEndereco.BuscarAsync(request.Cep, cancellationToken);

        return resultado.Status switch
        {
            StatusConsultaEndereco.Encontrado =>
                TypedResults.Ok(BuscarEnderecoResponse.Criar(resultado.Endereco!)),
            StatusConsultaEndereco.CepInvalido =>
                TypedResults.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "CEP inválido",
                    detail: "O CEP deve conter exatamente 8 dígitos."),
            StatusConsultaEndereco.NaoEncontrado => 
                TypedResults.NotFound(),
                _ => TypedResults.Problem(
                    statusCode: StatusCodes.Status503ServiceUnavailable,
                    title: "Provedor de endereços indisponível",
                    detail: "O provedor de endereços está indisponível no momento.")
        };
    }
}