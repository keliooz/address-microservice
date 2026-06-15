# Microsserviço de endereços

API de consulta de endereços por CEP para consumo por projetos pessoais.

## Estrutura

- `Endereco.Api`: API mínima e composição da aplicação.
- `Endereco.Aplicacao`: casos de uso e contratos.
- `Endereco.Dominio`: modelos e regras de domínio.
- `Endereco.Infraestrutura`: integrações e implementações externas.
- `Endereco.Testes.Unitarios`: testes unitários.
- `Endereco.Testes.Integracao`: testes de integração.

As classes de domínio permanecem `sealed`. Contratos recebidos e retornados por
APIs são mantidos separadamente em pastas `Requests` e `Responses`.

## Endpoint

```http
GET /api/enderecos/{cep}
```

## Provedor de consulta

A consulta de endereços utiliza o ViaCEP através de um `HttpClient` configurado.
O contrato público inclui CEP, logradouro, complemento, bairro, cidade, UF,
estado, região, DDD e código IBGE.

```json
"ViaCep": {
  "UrlBase": "https://viacep.com.br/ws/",
  "TempoLimiteSegundos": 5
}
```

## Cache em memória

Consultas encontradas são armazenadas por 24 horas e CEPs não encontrados por
15 minutos. CEPs inválidos e falhas do provedor não são armazenados.

```json
"CacheEndereco": {
  "LimiteEntradas": 100000,
  "DuracaoEncontradoMinutos": 1440,
  "DuracaoNaoEncontradoMinutos": 15
}
```

O cache possui limite dedicado de entradas e controla consultas concorrentes
para o mesmo CEP, evitando múltiplas chamadas simultâneas ao provedor.

## Segurança

- CEPs são normalizados e rejeitados antes de acessar cache ou rede.
- O endpoint possui limitação de requisições por endereço IP.
- O provedor externo aceita somente URL base HTTPS.
- Configurações inválidas impedem a inicialização da aplicação.
- Exceções inesperadas são retornadas como problemas HTTP sem detalhes internos.

```json
"LimiteRequisicoes": {
  "LimitePorMinuto": 120,
  "TamanhoFila": 0
}
```

Em ambientes com proxy reverso, os proxies confiáveis devem ser configurados
explicitamente antes de utilizar cabeçalhos encaminhados para identificar o IP
real do cliente.

## Testes de integração

Os testes hospedam a API em memória e validam respostas `200`, `400`, `404`,
`503` e `429`, incluindo binding da rota, normalização do CEP, serialização do
contrato público, composição da aplicação e limitação de requisições.
