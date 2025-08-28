using Controll;
public class Program
{
    static async Task Main(string[] args)
    {
        Simulation simulation = new Simulation();
        CancellationTokenSource cancellationToken = new CancellationTokenSource();
        cancellationToken.CancelAfter(TimeSpan.FromMinutes(5));
        await simulation.RunSimulationAsync(cancellationToken.Token);
    }
}

