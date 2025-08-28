using Models;
using Models.Enums;
using Models.Interfaces;
namespace Entities
{
    public class Elevator : IElevator
    {
        /// <summary>
        /// 10 second action timer
        /// </summary>
        private static int ACTION_TIME = 10_000;

        public int Id { get; }

        public FloorValue CurrentFloor { get;  set; }

        public DirectionEnum Direction { get; set; }

        public StatusEnum Status { get; set; }

        public List<Request> AssignedRequests { get; set; }

        public Elevator(int id)
        {
            AssignedRequests = new List<Request>();
            Id = id;
            CurrentFloor = FloorValue.Create(0);
            Direction = DirectionEnum.IDLE;
            Status = StatusEnum.IDLE;
        }

        public async Task MoveAsync()
        {
             // No request stay idle check again in 0,5 secconds
             if (!AssignedRequests.Any())
             {
                 Status = StatusEnum.IDLE;
                 Direction = DirectionEnum.IDLE;
                 await Task.Delay(500);
             }
             else
             {   
                 if(AssignedRequests.Any(x=> x.Floor.FloorNumber == CurrentFloor.FloorNumber))
                 {
                    DirectionEnum directionBeforeStop = Direction;
                     await StopAsync();
                     SetNextRequest(directionBeforeStop);
                 }
                 else
                 {   if(CurrentFloor < AssignedRequests.First().Floor)
                     {
                         await SetMovingUpAsync();
                     }
                     else
                     {
                         await SetMovingDownAsync();
                     }
                 }
             }
        }

        public void StatusUpdate()
        {
            Console.WriteLine($"{DateTime.UtcNow.ToString("hh:mm:ss")} timestamp: Car {Id} is on {CurrentFloor.FloorNumber} floor, Status:{Status}, Direction:{Direction}");
        }

        public async Task StopAsync()
        {
            if (Status == StatusEnum.MOVING)
            {
                Status = StatusEnum.STOPPED;
                Direction = DirectionEnum.IDLE;
                await Task.Delay(ACTION_TIME);
                Console.WriteLine($"Car {Id} is on {CurrentFloor.FloorNumber} floor has stoped for {DateTime.UtcNow.ToString("hh:mm:ss")} timestamp");
            }
        }

        public void AssignRequest(Request request)
        {
            AssignedRequests.Add(request);
        }

        private async Task SetMovingDownAsync()
        {
            Direction = DirectionEnum.DOWN;
            Status = StatusEnum.MOVING;
            CurrentFloor.FloorNumber--;
            await Task.Delay(ACTION_TIME);
            Console.WriteLine($"Car {Id} is on {CurrentFloor.FloorNumber} floor is moving DOWN to {AssignedRequests.First().Floor.FloorNumber} floor, {DateTime.UtcNow.ToString("hh:mm:ss")} timestamp");
        }

        private async Task SetMovingUpAsync()
        {
            Direction = DirectionEnum.UP;
            Status = StatusEnum.MOVING;
            CurrentFloor.FloorNumber++;
            await Task.Delay(ACTION_TIME);
            Console.WriteLine($"Car {Id} is on {CurrentFloor.FloorNumber} floor is moving UP to {AssignedRequests.First().Floor.FloorNumber} floor, {DateTime.UtcNow.ToString("hh:mm:ss")} timestamp");
        }

        private void SetNextRequest(DirectionEnum directionEnum)
        {
            AssignedRequests = AssignedRequests.Where(x => x.Floor.FloorNumber != CurrentFloor.FloorNumber && 
                                                           x.Direction != directionEnum)
                                               .DistinctBy(x=> x.Floor.FloorNumber)
                                               .ToList();

            AssignedRequests = AssignedRequests.OrderBy(x => x.Direction == Direction)
                                               .ThenBy(x => Math.Abs(CurrentFloor.FloorNumber - x.Floor.FloorNumber))
                                               .ToList();
        }
    }
}
