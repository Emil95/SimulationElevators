using Models;
using Models.Enums;

namespace Entities
{
    public class Request
    {
        public FloorValue Floor { get; }

        public DirectionEnum Direction { get; }

        public Request(FloorValue number, DirectionEnum direction)
        {
            Floor = number;
            Direction = direction;
        }
        public static Request CreateRandomRequest()
        {
            var rand = new Random();
            int nextFloor =  rand.Next(0, 11);
            int direction = rand.Next(0, 2);

            // floor 10 and UP => change direction to DOWN
            if (nextFloor == 10 && direction == 0)
            {
                direction = 1;
            }

            // floor 0 and DOWN => change direction to UP
            if (nextFloor == 0 && direction == 1)
            {
                direction = 0;
            }

            return new Request(FloorValue.Create(nextFloor), (DirectionEnum) direction);
        }
    }
}
