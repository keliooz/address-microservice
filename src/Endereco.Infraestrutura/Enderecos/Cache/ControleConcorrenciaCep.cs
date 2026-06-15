namespace Endereco.Infraestrutura.Enderecos.Cache;

public sealed class ControleConcorrenciaCep
{
    private readonly Dictionary<string, BloqueioCep> bloqueios = [];
    private readonly Lock sincronizacao = new();

    public async ValueTask<IDisposable> EntrarAsync(string cep, CancellationToken cancellationToken)
    {
        BloqueioCep bloqueio;

        lock (sincronizacao)
        {
            if (!bloqueios.TryGetValue(cep, out bloqueio!))
            {
                bloqueio = new BloqueioCep();
                bloqueios.Add(cep, bloqueio);
            }

            bloqueio.QuantidadeUsuarios++;
        }

        try
        {
            await bloqueio.Semaforo.WaitAsync(cancellationToken);
            return new Saida(this, cep, bloqueio);
        }
        catch
        {
            RemoverUsuario(cep, bloqueio);
            throw;
        }
    }

    private void Sair(string cep, BloqueioCep bloqueio)
    {
        bloqueio.Semaforo.Release();
        RemoverUsuario(cep, bloqueio);
    }

    private void RemoverUsuario(string cep, BloqueioCep bloqueio)
    {
        lock (sincronizacao)
        {
            bloqueio.QuantidadeUsuarios--;

            if (bloqueio.QuantidadeUsuarios == 0)
            {
                bloqueios.Remove(cep);
                bloqueio.Semaforo.Dispose();
            }
        }
    }

    private sealed class BloqueioCep
    {
        public SemaphoreSlim Semaforo { get; } = new(1, 1);

        public int QuantidadeUsuarios { get; set; }
    }

    private sealed class Saida(ControleConcorrenciaCep controle, string cep, BloqueioCep bloqueio) : IDisposable
    {
        private bool liberado;

        public void Dispose()
        {
            if (liberado)
            {
                return;
            }

            liberado = true;
            controle.Sair(cep, bloqueio);
        }
    }
}
