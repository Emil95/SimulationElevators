namespace Models.Interfaces
{
    public interface ISimulation
    {
        public Task RunSimulationAsync(CancellationToken cancellationToken);
        public void Initialize();

    }
}
