using Endereco.Aplicacao.Enderecos;
using Endereco.Dominio.Enderecos;
using Endereco.Dominio.ValueObjects;
using Endereco.Infraestrutura.Enderecos.ViaCep.Requests;
using Endereco.Infraestrutura.Enderecos.ViaCep.Responses;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using EnderecoDominio = Endereco.Dominio.Enderecos.Endereco;

namespace Endereco.Infraestrutura.Enderecos.ViaCep;

/// <summary>
/// Integra com o ViaCEP e traduz respostas HTTP/JSON para resultados compreendidos pela aplicação.
/// </summary>
public sealed class ProvedorViaCep(HttpClient clienteHttp) : IProvedorEnderecoExterno
{
    public async Task<ResultadoProvedorEndereco> BuscarAsync(Cep cep, CancellationToken cancellationToken = default)
    {
        try
        {
            ConsultaViaCepRequest request = new() 
            { 
                Cep = cep
            };

            using HttpResponseMessage? resposta =
                await clienteHttp.GetAsync(request.ToString(), cancellationToken);

            if (resposta.StatusCode == HttpStatusCode.BadRequest)
            {
                return ResultadoProvedorEndereco.CepInvalido();
            }

            if (!resposta.IsSuccessStatusCode)
            {
                return Indisponivel();
            }

            ConsultaViaCepResponse? respostaViaCep = 
                await resposta.Content.ReadFromJsonAsync<ConsultaViaCepResponse>(cancellationToken);

            if (respostaViaCep is null)
            {
                return Indisponivel();
            }

            if (respostaViaCep.Erro)
            {
                return ResultadoProvedorEndereco.NaoEncontrado();
            }

            if (!Cep.TentarCriar(respostaViaCep.Cep, out Cep? cepResposta))
            {
                return Indisponivel();
            }

            return ResultadoProvedorEndereco.Encontrado(MapearEndereco(respostaViaCep, cepResposta));
        }
        catch (HttpRequestException)
        {
            return Indisponivel();
        }
        catch (JsonException)
        {
            return Indisponivel();
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return Indisponivel();
        }
    }

    private static EnderecoDominio MapearEndereco(ConsultaViaCepResponse resposta, Cep cep) =>
        new()
        {
            Cep = cep,
            Logradouro = NuloQuandoVazio(resposta.Logradouro),
            Complemento = NuloQuandoVazio(resposta.Complemento),
            Bairro = string.IsNullOrEmpty(resposta.Bairro)
                ? null
                : new Bairro { Nome = resposta.Bairro },
            Cidade = new Cidade
            {
                Nome = resposta.Localidade ?? string.Empty,
                CodigoIbge = resposta.Ibge
            },
            Uf = new Uf { Sigla = resposta.Uf ?? string.Empty },
            Estado = new Estado { Nome = resposta.Estado ?? string.Empty },
            Regiao = new Regiao { Nome = resposta.Regiao ?? string.Empty },
            Ddd = NuloQuandoVazio(resposta.Ddd)
        };

    private static string? NuloQuandoVazio(string? valor) =>
        string.IsNullOrEmpty(valor) ? null : valor;

    private static ResultadoProvedorEndereco Indisponivel() =>
        ResultadoProvedorEndereco.Indisponivel();
}
