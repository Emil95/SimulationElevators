using Entities;
using Models.Interfaces;

namespace Controll
{
    public class Simulation : ISimulation
    {
        public readonly int ELEVATOR_NUMBER = 1;

        public Dispatcher Dispatcher { get; set; }
        public List<Elevator> Elevators { get; set; }
        public Random Random { get; set; }

        public Simulation()
        {
            this.Initialize();
        }

        public void Initialize()
        {
            Console.WriteLine("Begin elevator initialization");
            Elevators = new List<Elevator>();

            for (int i = 1; i<= ELEVATOR_NUMBER; i++)
            {
                Elevator elevator = new Elevator(i);
                Elevators.Add(elevator);
                Console.WriteLine($"Elevator {i} created");
            }
            Dispatcher = new Dispatcher(Elevators);
            Random = new Random();
            Console.WriteLine("Initialization complete");
        }

        public async Task RunSimulationAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Simulation starting");

            /// Starts generation request process
            var requestsTask = Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await RanomizeRequestsAsync(cancellationToken);
                }
            },cancellationToken);


            /// Starts elevator movement process
            var elevatorTasks = Dispatcher.Elevators.Select(x => Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await x.MoveAsync();
                    await Task.Delay(100);
                }
            }, cancellationToken)).ToList();


            /// Starts update status process
            var statusTask = Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    Dispatcher.ElevatorStatusUpdate();
                    await Task.Delay(10000);
                }
            },cancellationToken);

            // Wait until cancellation
            await Task.WhenAll(elevatorTasks.Append(requestsTask).Append(statusTask));

            Console.WriteLine("Simulation ended");
        }

        public async Task RanomizeRequestsAsync(CancellationToken cancellationToken)
        {
            for (int j = 0; j < CreateRequests(); j++)
            {
                if (Random.Next(0, 2) == 1)
                {
                    Dispatcher.GenerateRequest();
                }
            }

            int randomSecondsToTryToGenerateRequest = Random.Next(1000, 9000);
            await Task.Delay(randomSecondsToTryToGenerateRequest);
        }

        private int CreateRequests()
        {
            int number = Random.Next(0, 4);

            return number;
        }
    }
}
