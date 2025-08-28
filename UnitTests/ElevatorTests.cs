using Entities;
using Models;
using Models.Enums;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class ElevatorTests
    {

        private readonly int _actionTime = 1;

        [Test]
        public void Constructor_DefaultSetupOfElevator()
        {
            // Arrange and Act
            Elevator elevator = new Elevator(1, _actionTime);

       
            // Assert
            Assert.That(elevator.Status, Is.EqualTo(StatusEnum.IDLE));
            Assert.That(elevator.Direction, Is.EqualTo(DirectionEnum.IDLE));
            Assert.That(elevator.CurrentFloor.FloorNumber, Is.EqualTo(0));
            Assert.That(elevator.Id, Is.EqualTo(1));
            Assert.That(elevator.AssignedRequests, Is.Not.Null);
            Assert.That(elevator.AssignedRequests, Is.Empty);
        }


        [Test]
        public void AssignRequest_AddValidRequest()
        {
            // Arrange
            Elevator elevator = new Elevator(1, _actionTime);

            // Act
            elevator.AssignRequest(new Request(FloorValue.Create(5), DirectionEnum.UP));
            elevator.AssignRequest(new Request(FloorValue.Create(3), DirectionEnum.UP));

            // Assert
            Assert.That(elevator.AssignedRequests, Is.Not.Null);
            Assert.That(elevator.AssignedRequests, Is.Not.Empty);
            Assert.That(elevator.AssignedRequests.Count, Is.EqualTo(2));
        }

        #region  StopAsync
        [Test]
        public async Task StopAsync_ShouldChangeStatusToStopped_WhenMoving()
        {
            // Arrange
            Elevator elevator = new Elevator(1, _actionTime);
            elevator.Status = StatusEnum.MOVING;
            elevator.Direction = DirectionEnum.UP;
            elevator.CurrentFloor = FloorValue.Create(3);

            // Act
            await elevator.StopAsync();

            // Assert
            Assert.That(elevator.Status, Is.EqualTo(StatusEnum.STOPPED));
            Assert.That(elevator.Direction, Is.EqualTo(DirectionEnum.IDLE));
        }

        [Test]
        public async Task StopAsync_ShouldNotChangeStatus_WhenNotMoving()
        {
            // Arrange
            Elevator elevator = new Elevator(1, _actionTime);
            elevator.Status = StatusEnum.IDLE; // already idle
            elevator.Direction = DirectionEnum.IDLE;

            // Act
            await elevator.StopAsync();

            // Assert
            Assert.That(elevator.Status, Is.EqualTo(StatusEnum.IDLE));
            Assert.That(elevator.Direction, Is.EqualTo(DirectionEnum.IDLE));
        }

        [Test]
        public async Task StopAsync_ShouldKeepSameFloor_WhenStopped()
        {
            // Arrange
            Elevator elevator = new Elevator(1, _actionTime);
            elevator.CurrentFloor = FloorValue.Create(5);
            elevator.Status = StatusEnum.MOVING;

            // Act
            await elevator.StopAsync();

            // Assert
            Assert.That(elevator.CurrentFloor.FloorNumber, Is.EqualTo(5));
        }
        #endregion

        #region MoveAsync
        [Test]
        public async Task MoveAsync_ShouldBeIdle_WhenNoRequests()
        {
            // Arrange
            Elevator elevator = new Elevator(1, _actionTime);

            // Act
            await elevator.MoveAsync();

            // Assert
            Assert.That(elevator.Status, Is.EqualTo(StatusEnum.IDLE));
            Assert.That(elevator.Direction, Is.EqualTo(DirectionEnum.IDLE));
        }

        [Test]
        public async Task MoveAsync_ShouldMoveUp_WhenRequestAbove()
        {
            // Arrange
            Elevator elevator = new Elevator(1, _actionTime);
            elevator.AssignedRequests.Add(new Request(FloorValue.Create(5), DirectionEnum.UP));

            // Act
            await elevator.MoveAsync();

            // Assert
            Assert.That(elevator.Status, Is.EqualTo(StatusEnum.MOVING));
            Assert.That(elevator.Direction, Is.EqualTo(DirectionEnum.UP));
            Assert.That(elevator.CurrentFloor.FloorNumber, Is.EqualTo(1)); // moved up from 0 -> 1
        }

        [Test]
        public async Task MoveAsync_ShouldMoveDown_WhenRequestBelow()
        {
            // Arrange
            Elevator elevator = new Elevator(1, _actionTime);
            elevator.CurrentFloor = FloorValue.Create(5);
            elevator.AssignedRequests.Add(new Request(FloorValue.Create(2), DirectionEnum.DOWN));

            // Act
            await elevator.MoveAsync();

            // Assert
            Assert.That(elevator.Status, Is.EqualTo(StatusEnum.MOVING));
            Assert.That(elevator.Direction, Is.EqualTo(DirectionEnum.DOWN));
            Assert.That(elevator.CurrentFloor.FloorNumber, Is.EqualTo(4)); // moved down from 5
        }

        [Test]
        public async Task MoveAsync_ShouldStop_WhenReachesRequestFloor()
        {
            // Arrange
            Elevator elevator = new Elevator(1, _actionTime);
            elevator.CurrentFloor = FloorValue.Create(2);
            elevator.AssignedRequests.Add(new Request(FloorValue.Create(2), DirectionEnum.UP));
            elevator.Status = StatusEnum.MOVING;

            // Act
            await elevator.MoveAsync();

            // Assert
            Assert.That(elevator.Status, Is.EqualTo(StatusEnum.STOPPED));
            Assert.That(elevator.Direction, Is.EqualTo(DirectionEnum.IDLE));
            Assert.That(elevator.AssignedRequests.Count, Is.EqualTo(0)); // request served
        }

        [Test]
        public async Task MoveAsync_ShouldContinueUpUntilNoMoreRequestsAbove()
        {
            // Arrange
            Elevator elevator = new Elevator(1, _actionTime);
            elevator.CurrentFloor = FloorValue.Create(3);
            elevator.AssignedRequests.Add(new Request(FloorValue.Create(5), DirectionEnum.UP));
            elevator.AssignedRequests.Add(new Request(FloorValue.Create(7), DirectionEnum.UP));
            elevator.AssignedRequests.Add(new Request(FloorValue.Create(5), DirectionEnum.UP));

            // Act
            await elevator.MoveAsync(); // move 4
            await elevator.MoveAsync(); // move 5
            await elevator.MoveAsync(); // stop at 5

            // Assert
            Assert.That(elevator.CurrentFloor.FloorNumber, Is.EqualTo(5));
            Assert.That(elevator.Status, Is.EqualTo(StatusEnum.STOPPED));
            Assert.That(elevator.AssignedRequests.Exists(r => r.Floor == 5), Is.False);
        }

        [Test]
        public async Task MoveAsync_ShouldSwitchDirection_WhenNoMoreRequestsUp()
        {
            // Arrange
            Elevator elevator = new Elevator(1, _actionTime);
            elevator.CurrentFloor = FloorValue.Create(3);
            elevator.AssignedRequests.Add(new Request(FloorValue.Create(5), DirectionEnum.UP));
            elevator.AssignedRequests.Add(new Request(FloorValue.Create(1), DirectionEnum.DOWN));

            // Act
            await elevator.MoveAsync(); // move 4
            await elevator.MoveAsync(); // move 5
            await elevator.MoveAsync(); // stop at 5 
            await elevator.MoveAsync(); // should switch to DOWN

            // Assert
            Assert.That(elevator.Direction, Is.EqualTo(DirectionEnum.DOWN));
        }

        [Test]
        public async Task MoveAsync_ValidateMovement()
        {
            // Arrange
            Elevator elevator = new Elevator(1, _actionTime);
            elevator.CurrentFloor = FloorValue.Create(3);
            elevator.Direction = DirectionEnum.UP;
            elevator.Status = StatusEnum.MOVING;

            elevator.AssignedRequests.Add(new Request(FloorValue.Create(5), DirectionEnum.UP));
            elevator.AssignedRequests.Add(new Request(FloorValue.Create(1), DirectionEnum.DOWN));
            elevator.AssignedRequests.Add(new Request(FloorValue.Create(3), DirectionEnum.UP));
            elevator.AssignedRequests.Add(new Request(FloorValue.Create(8), DirectionEnum.DOWN));

            // Act
            await elevator.MoveAsync(); // stop at 3
            Assert.That(elevator.Direction, Is.EqualTo(DirectionEnum.UP));
            Assert.That(elevator.Status, Is.EqualTo(StatusEnum.STOPPED));
            Assert.That(elevator.CurrentFloor.FloorNumber, Is.EqualTo(3));
            Assert.That(elevator.AssignedRequests.Count, Is.EqualTo(3));

            await elevator.MoveAsync(); // move 4
            Assert.That(elevator.Direction, Is.EqualTo(DirectionEnum.UP));
            Assert.That(elevator.Status, Is.EqualTo(StatusEnum.MOVING));
            Assert.That(elevator.CurrentFloor.FloorNumber, Is.EqualTo(4));
            Assert.That(elevator.AssignedRequests.Count, Is.EqualTo(3));

            await elevator.MoveAsync(); // move 5
            Assert.That(elevator.Direction, Is.EqualTo(DirectionEnum.UP));
            Assert.That(elevator.Status, Is.EqualTo(StatusEnum.MOVING));
            Assert.That(elevator.CurrentFloor.FloorNumber, Is.EqualTo(5));
            Assert.That(elevator.AssignedRequests.Count, Is.EqualTo(3));
            await elevator.MoveAsync(); // stop at 5 

            Assert.That(elevator.Direction, Is.EqualTo(DirectionEnum.UP));
            Assert.That(elevator.Status, Is.EqualTo(StatusEnum.STOPPED));
            Assert.That(elevator.CurrentFloor.FloorNumber, Is.EqualTo(5));
            Assert.That(elevator.AssignedRequests.Count, Is.EqualTo(2));
            
            await elevator.MoveAsync(); // switch to DOWN move 6
            Assert.That(elevator.Direction, Is.EqualTo(DirectionEnum.UP));
            Assert.That(elevator.Status, Is.EqualTo(StatusEnum.MOVING));
            Assert.That(elevator.CurrentFloor.FloorNumber, Is.EqualTo(6));
            Assert.That(elevator.AssignedRequests.Count, Is.EqualTo(2));

            await elevator.MoveAsync(); // move 7
            Assert.That(elevator.Direction, Is.EqualTo(DirectionEnum.UP));
            Assert.That(elevator.Status, Is.EqualTo(StatusEnum.MOVING));
            Assert.That(elevator.CurrentFloor.FloorNumber, Is.EqualTo(7));
            Assert.That(elevator.AssignedRequests.Count, Is.EqualTo(2));

            await elevator.MoveAsync(); // move 8
            Assert.That(elevator.Direction, Is.EqualTo(DirectionEnum.UP));
            Assert.That(elevator.Status, Is.EqualTo(StatusEnum.MOVING));
            Assert.That(elevator.CurrentFloor.FloorNumber, Is.EqualTo(8));
            Assert.That(elevator.AssignedRequests.Count, Is.EqualTo(2));

            await elevator.MoveAsync(); // stop 8
            Assert.That(elevator.Direction, Is.EqualTo(DirectionEnum.DOWN));
            Assert.That(elevator.Status, Is.EqualTo(StatusEnum.STOPPED));
            Assert.That(elevator.CurrentFloor.FloorNumber, Is.EqualTo(8));
            Assert.That(elevator.AssignedRequests.Count, Is.EqualTo(1));

            await elevator.MoveAsync(); // stop 7
            Assert.That(elevator.Direction, Is.EqualTo(DirectionEnum.DOWN));
            Assert.That(elevator.Status, Is.EqualTo(StatusEnum.MOVING));
            Assert.That(elevator.CurrentFloor.FloorNumber, Is.EqualTo(7));
            Assert.That(elevator.AssignedRequests.Count, Is.EqualTo(1));
        }

        #endregion

    }
}
