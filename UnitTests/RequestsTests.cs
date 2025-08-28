using Entities;
using Models;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class RequestsTests
    {

        [Test]
        public void Constructor_ValidateDefault()
        {
            Request request = new Request(FloorValue.Create(3), Models.Enums.DirectionEnum.DOWN);
            Assert.That(request, Is.Not.Null);
            Assert.That(request.Floor.FloorNumber, Is.EqualTo(3));
            Assert.That(request.Direction, Is.EqualTo(Models.Enums.DirectionEnum.DOWN));
        }

        [Test]
        public void CreateRandomRequest_CreateValidRequest()
        {
            Request request = Request.CreateRandomRequest();
            Assert.That(request, Is.Not.Null);
            Assert.That(request.Floor.FloorNumber, Is.Not.GreaterThan(10));
            Assert.That(request.Floor.FloorNumber, Is.Not.LessThan(0));
        }
    }
}
