namespace Models
{
    public class FloorValue
    {
        private static int MAX_FLOOR = 10;
        private static int MIN_FLOOR = 0;
        public int FloorNumber { get; set; }

        public FloorValue()
        {
            FloorNumber = 0;
        }

        FloorValue(int floorNum)
        {
            if(MIN_FLOOR > floorNum)
            {
                Console.WriteLine($"Exception:Invalid floor number provided, minimum floor is {MIN_FLOOR}");
                throw new ArgumentOutOfRangeException();
            }

            if (MAX_FLOOR < floorNum)
            {
                Console.WriteLine($"Exception:Invalid floor number provided, maximum floor is {MAX_FLOOR}");
                throw new ArgumentOutOfRangeException();
            }

            FloorNumber = floorNum;
        }

        public static FloorValue Create(int floorNum)
        {
            return new FloorValue(floorNum);
        }

        public static bool operator > (FloorValue floor1, FloorValue floor2)
        {
            return floor1.FloorNumber > floor2.FloorNumber;
        }

        public static bool operator <(FloorValue floor1, FloorValue floor2)
        {
            return floor1.FloorNumber < floor2.FloorNumber;
        }
    }
}

