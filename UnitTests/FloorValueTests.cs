using NUnit.Framework;
using Models;

namespace UnitTests
{
    [TestFixture]
    public class FloorValueTests
    {
        [Test]
        public void Create_NotFloorNumber_Negative_ArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => FloorValue.Create(-1));
        }

        [Test]
        public void Create_NotFloorNumber_BigerThenMax_ArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => FloorValue.Create(11));
        }

        [Test]
        public void Create_ValidFloorNumber_MAX_ShouldSucceed()
        {
            FloorValue floorValue = FloorValue.Create(10);
            Assert.That(floorValue, Is.Not.Null);
            Assert.That(floorValue.FloorNumber, Is.EqualTo(10));
        }

        [Test]
        public void Create_ValidFloorNumber_MIN_ShouldSucceed()
        {
            FloorValue floorValue = FloorValue.Create(0);
            Assert.That(floorValue, Is.Not.Null);
            Assert.That(floorValue.FloorNumber, Is.EqualTo(0));
        }

        [Test]
        public void Create_ValidFloorNumber_ShouldSucceed()
        {
            FloorValue floorValue = FloorValue.Create(5);
            Assert.That(floorValue, Is.Not.Null);
            Assert.That(floorValue.FloorNumber, Is.EqualTo(5));
        }

        [Test]
        public void OperatorGreaterThan_ShouldReturnTrue_WhenLeftIsHigher()
        {
            var f1 = FloorValue.Create(7);
            var f2 = FloorValue.Create(3);

            Assert.That(f1 > f2, Is.True);
        }

        [Test]
        public void OperatorLessThan_ShouldReturnTrue_WhenLeftIsLower()
        {
            var f1 = FloorValue.Create(2);
            var f2 = FloorValue.Create(8);

            Assert.That(f1 < f2, Is.True);
        }

        [Test]
        public void OperatorGreaterThan_ShouldReturnTrue_WhenEqual()
        {
            var f1 = FloorValue.Create(7);
            var f2 = FloorValue.Create(7);

            Assert.That(f1 == f2, Is.True);
        }

        [Test]
        public void OperatorLessThan_ShouldReturnTrue_WhenNotEqual()
        {
            var f1 = FloorValue.Create(2);
            var f2 = FloorValue.Create(8);

            Assert.That(f1 != f2, Is.True);
        }
    }
}
