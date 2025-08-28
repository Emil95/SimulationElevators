using Controll;
public class Program
{
    static async Task Main(string[] args)
    {
        Simulation simulation = new Simulation();
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(TimeSpan.FromMinutes(5));
        await simulation.RunSimulationAsync(cancellationTokenSource.Token);
    }
}

