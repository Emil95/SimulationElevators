using Controll;
using NUnit.Framework;


namespace UnitTests
{
    [TestFixture]
    public class SimulationTests
    {
        [Test]
        public void Initialize_ShouldCreateElevatorsAndDispatcher()
        {
            // Arrange
            var simulation = new Simulation();

            // Act
            simulation.Initialize();

            // Assert
            Assert.That(simulation.Elevators, Is.Not.Null);
            Assert.That(simulation.Elevators.Count, Is.EqualTo(simulation.ELEVATOR_NUMBER));
            Assert.That(simulation.Elevators.First().Id, Is.EqualTo(1));
            Assert.That(simulation.Dispatcher, Is.Not.Null);
            Assert.That(simulation.Random, Is.Not.Null);
        }

        [Test]
        public async Task RunSimulationAsync_ShouldRunAndCancelProperly()
        {
            // Arrange
            var simulation = new Simulation();
            simulation.Initialize();
            var cts = new CancellationTokenSource();
            // stop after 500ms
            cts.CancelAfter(500);

            // Act & Assert
            Assert.DoesNotThrowAsync(async () =>
            {
                await simulation.RunSimulationAsync(cts.Token);
            });
        }

        [Test]
        public async Task RanomizeRequestsAsync_ShouldCallGenerateRequest()
        {
            // Arrange
            var simulation = new Simulation();
            simulation.Initialize();
            var cts = new CancellationTokenSource();
            // stop after 500ms
            cts.CancelAfter(500); 

            // Act & Assert
            Assert.DoesNotThrowAsync(async () =>
            {
                await simulation.RanomizeRequestsAsync(cts.Token);
            });
        }
    }
}
