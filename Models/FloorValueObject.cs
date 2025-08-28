namespace Models
{
    public class FloorValue : IComparable<FloorValue>
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

        public int CompareTo(FloorValue other)
        {
            if (other == null) return 1;
            return this.FloorNumber.CompareTo(other.FloorNumber);
        }

        public static bool operator ==(FloorValue a, FloorValue b)
        {
            return a?.FloorNumber == b?.FloorNumber;
        }

        public static bool operator !=(FloorValue a, FloorValue b)
        {
            return !(a == b);
        }

        public static bool operator > (FloorValue floor1, FloorValue floor2)
        {
            return floor1.FloorNumber > floor2.FloorNumber;
        }

        public static bool operator <(FloorValue floor1, FloorValue floor2)
        {
            return floor1.FloorNumber < floor2.FloorNumber;
        }

        public static implicit operator int(FloorValue f)
        {
            return f.FloorNumber;
        }

        public static implicit operator FloorValue(int floorNumber)
        {
            return new FloorValue(floorNumber);
        }

        public override string ToString()
        {
            return $"{FloorNumber}";
        }
    }
}

