
using Models.Interfaces;
namespace Entities
{
    public class Dispatcher : IDispatcher
    {
        public IEnumerable<Elevator> Elevators { get; private set; }

        public Dispatcher(IEnumerable<Elevator> elevators)
        {
            if(elevators == null)
            {
                throw new ArgumentNullException();
            }

            Elevators = elevators;
        }

        public void GenerateRequest()
        {
            Request request = Request.CreateRandomRequest();
            RequestAssign(request);
        }

        public void ElevatorStatusUpdate()
        {
           foreach(Elevator elevator in Elevators)
            {
                elevator.StatusUpdate();
            }
        }
        public void RequestAssign(Request request)
        {
            var elevatorsGoingInSameDirection = Elevators.Where(x => x.Direction == request.Direction || 
                                                                     x.Direction == Models.Enums.DirectionEnum.IDLE)
                                                         .ToList();

            if (!elevatorsGoingInSameDirection.Any())
            {
                elevatorsGoingInSameDirection = Elevators.ToList();
            }
            var closestToTheFloor = elevatorsGoingInSameDirection.OrderBy(e => e.AssignedRequests.Count)
                                                                 .ThenBy(e => Math.Abs(e.CurrentFloor.FloorNumber - request.Floor.FloorNumber))
                                                                 .First();

            closestToTheFloor.AssignRequest(request);
            Console.WriteLine($"{DateTime.UtcNow.ToString("hh:mm:ss")}-> Request generated: {request.Floor.FloorNumber} floor, {request.Direction}: direction -> Elevator{closestToTheFloor.Id} ");
        }
    }
}
