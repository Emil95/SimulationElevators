using Models;
using Models.Enums;
using Models.Interfaces;
namespace Entities
{
    public class Elevator : IElevator
    {
        private readonly int _actionTime;

        public int Id { get; }

        public FloorValue CurrentFloor { get;  set; }

        public DirectionEnum Direction { get; set; }

        public StatusEnum Status { get; set; }

        public List<Request> AssignedRequests { get; set; }

        public Elevator(int id, int actionTime = 10_000)
        {
            AssignedRequests = new List<Request>();
            Id = id;
            CurrentFloor = FloorValue.Create(0);
            Direction = DirectionEnum.IDLE;
            Status = StatusEnum.IDLE;
            _actionTime = actionTime;
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
                var nextRequest = GetNextRequest();
                if (nextRequest == null)
                {
                    Direction = DirectionEnum.IDLE;
                    Status = StatusEnum.IDLE;
                    await Task.Delay(500);
                }
                else
                {
                    if (nextRequest.Floor == CurrentFloor)
                    {
                        await StopAsync();
                        AssignedRequests.Remove(nextRequest);
                        AssignedRequests = AssignedRequests.Where(x => !(x.Floor == nextRequest.Floor && 
                                                                         x.Direction == nextRequest.Direction)).ToList();
                        SetDirection();
                    }
                    else
                    {
                        Status = StatusEnum.MOVING;
                        if (CurrentFloor.FloorNumber < nextRequest.Floor)
                        {
                            await SetMovingUpAsync(nextRequest.Floor);
                        }
                        else
                        {
                            await SetMovingDownAsync(nextRequest.Floor);
                        }
                    }
                }
            }
        }

        public void StatusUpdate()
        {
            Console.WriteLine($"{DateTime.UtcNow.ToString("hh:mm:ss")} timestamp: Car {Id} is on {CurrentFloor.ToString()} floor, Status:{Status}, Direction:{Direction}");
        }

        public async Task StopAsync()
        {
            if (Status == StatusEnum.MOVING)
            {
                Status = StatusEnum.STOPPED;
                Direction = DirectionEnum.IDLE;
                await Task.Delay(_actionTime);
                Console.WriteLine($"Car {Id} is on {CurrentFloor.ToString()} floor has stopped for {DateTime.UtcNow.ToString("hh:mm:ss")} timestamp");
            }
        }

        public void AssignRequest(Request request)
        {
            AssignedRequests.Add(request);
            if (Direction == DirectionEnum.IDLE)
            {
                SetDirection();
            }
        }

        private Request? GetNextRequest()
        {
            if (!AssignedRequests.Any())
            {
                return null;
            }

            if (Direction == DirectionEnum.UP)
            {
                return AssignedRequests.Where(r => r.Floor >= CurrentFloor).OrderBy(r => r.Floor).FirstOrDefault()
                   ?? AssignedRequests.OrderByDescending(r => r.Floor).FirstOrDefault();
            }

            else if (Direction == DirectionEnum.DOWN)
            {
                return AssignedRequests.Where(r => r.Floor.FloorNumber <= CurrentFloor).OrderByDescending(r => r.Floor).FirstOrDefault()
                    ?? AssignedRequests.OrderBy(r => r.Floor).FirstOrDefault();
            }
                
            return AssignedRequests.OrderBy(r => Math.Abs(r.Floor - CurrentFloor)).FirstOrDefault();
        }

        private void SetDirection()
        {
            if (!AssignedRequests.Any())
            {
                Direction = DirectionEnum.IDLE;
            }

            if (AssignedRequests.Any(r => r.Floor > CurrentFloor))
            {
                Direction = DirectionEnum.UP;
            }
            else if (AssignedRequests.Any(r => r.Floor < CurrentFloor))
            {
                Direction = DirectionEnum.DOWN;
            }
            else
            {
                Direction = DirectionEnum.IDLE;
            }
        }

        private async Task SetMovingDownAsync(int moveToFlorNumber)
        {
            Direction = DirectionEnum.DOWN;
            Status = StatusEnum.MOVING;
            CurrentFloor.FloorNumber--;
            await Task.Delay(_actionTime);
            Console.WriteLine($"Car {Id} is on {CurrentFloor.ToString()} floor is moving DOWN to {moveToFlorNumber} floor, {DateTime.UtcNow.ToString("hh:mm:ss")} timestamp");
        }

        private async Task SetMovingUpAsync(int moveToFlorNumber)
        {
            Direction = DirectionEnum.UP;
            Status = StatusEnum.MOVING;
            CurrentFloor.FloorNumber++;
            await Task.Delay(_actionTime);
            Console.WriteLine($"Car {Id} is on {CurrentFloor.ToString()} floor is moving UP to {moveToFlorNumber} floor, {DateTime.UtcNow.ToString("hh:mm:ss")} timestamp");
        }

    }
}
