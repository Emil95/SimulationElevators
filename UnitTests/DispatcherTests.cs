namespace UnitTests
{
    using Entities;
    using Models;
    using Models.Enums;
    using NUnit.Framework;

    [TestFixture]
    public class DispatcherTests
    {
        [Test]
        public void Constructor_NullElevators_ShouldThrowException()
        {
            // Arrange
            // Assert
            Assert.Throws<ArgumentNullException>(() => new Dispatcher(null));
        }

        [Test]
        public void Constructor_ListElevators_ValidObject()
        {
            // Arrange
            var elevatorList = CreateElavatorsList(4);
            Dispatcher dispatcher = new Dispatcher(elevatorList);

            // Assert
            Assert.That(dispatcher, Is.Not.Null);
            Assert.That(dispatcher.Elevators, Is.Not.Null);
            Assert.That(dispatcher.Elevators.Count(), Is.EqualTo(elevatorList.Count));
        }

        [Test]
        public void RequestAssign_ShouldAssignToIdleElevator()
        {
            // Arrange
            var elevatorList = CreateElavatorsList(4);
            Dispatcher dispatcher = new Dispatcher(elevatorList);
            var request = new Request(FloorValue.Create(5), DirectionEnum.UP);

            // Act
            dispatcher.RequestAssign(request);

            // Assert
            Assert.That(dispatcher.Elevators.Any(e => e.AssignedRequests.Contains(request)), Is.True);
        }

        [Test]
        public void RequestAssign_ShouldPreferElevatorWithFewestRequests()
        {
            // Arrange
            var elevatorList = CreateElavatorsList(4);
            Dispatcher dispatcher = new Dispatcher(elevatorList);
            var request1 = new Request(FloorValue.Create(4), DirectionEnum.UP);
            var request2 = new Request(FloorValue.Create(6), DirectionEnum.UP);

            // Elevator 1 already has a request
            elevatorList[0].AssignRequest(request1);

            // Act
            dispatcher.RequestAssign(request2);

            // Assert -> request2 should go to elevator 2 or 3 (not elevator 1)
            Assert.That(elevatorList[0].AssignedRequests.Contains(request2), Is.False);
        }

        [Test]
        public async Task ElevatorStatusUpdate_ShouldUpdateAllElevators()
        {
            // Arrange
            var elevatorList = CreateElavatorsList(4);
            Dispatcher dispatcher = new Dispatcher(elevatorList);
            foreach (var elevator in elevatorList)
            {
                elevator.AssignRequest(new Request(FloorValue.Create(2), DirectionEnum.UP));
                await elevator.MoveAsync();
            }

            // Act
            dispatcher.ElevatorStatusUpdate();

            // Assert
            Assert.That(elevatorList.All(e => e.Status != StatusEnum.IDLE), Is.True);
        }

        [Test]
        public void RequestAssign_ShouldFallback_WhenNoElevatorInSameDirection()
        {
            // Arrange
            // Put all elevators in DOWN direction
            var elevatorList = CreateElavatorsList(4);
            Dispatcher dispatcher = new Dispatcher(elevatorList);
            foreach (var elevator in elevatorList)
            {
                elevator.Direction = DirectionEnum.DOWN;
            }

            var request = new Request(FloorValue.Create(8), DirectionEnum.UP);

            // Act
            dispatcher.RequestAssign(request);

            // Assert
            Assert.That(elevatorList.Any(e => e.AssignedRequests.Contains(request)), Is.True);
        }

        private List<Elevator> CreateElavatorsList(int numberOfElevators)
        {
            var list = new List<Elevator>();
            for(int i = 0; i<numberOfElevators; i++)
            {
                list.Add(new Elevator(i));
            }
            return list;
        }
    }
}
