namespace Models.Interfaces
{
    public interface IElevator
    {
        public Task MoveAsync();
        public Task StopAsync();
        public void StatusUpdate();

    }
}
