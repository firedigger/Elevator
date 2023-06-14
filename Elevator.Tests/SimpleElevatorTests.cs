using Elevator.DataObjects;
using Elevator.Elevators;

namespace Elevator.Tests
{
    public class SimpleElevatorTests
    {
        private readonly SimpleElevator elevator = new();

        private void AssertFloors(params uint[] expectedFloors)
        {
            Assert.Equal(expectedFloors, elevator.FloorsEnumerator().Take(expectedFloors.Length).ToList());
        }

        [Fact]
        public void SingleCall()
        {
            elevator.Call(new CompleteElevatorCall
            {
                SourceFloor = 1,
                DestinationFloor = 10
            });
            AssertFloors(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
        }

        [Fact]
        public void TwoCalls()
        {
            elevator.Call(new CompleteElevatorCall
            {
                SourceFloor = 1,
                DestinationFloor = 10
            });
            elevator.Call(new CompleteElevatorCall
            {
                SourceFloor = 9,
                DestinationFloor = 2
            });
            AssertFloors(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 9, 8, 7, 6, 5, 4, 3, 2);
        }

        [Fact]
        public void InterruptCall()
        {
            elevator.Call(new CompleteElevatorCall
            {
                SourceFloor = 1,
                DestinationFloor = 10
            });
            AssertFloors(1, 2, 3, 4);
            elevator.Call(new CompleteElevatorCall
            {
                SourceFloor = 6,
                DestinationFloor = 3
            });
            AssertFloors(4, 5, 6, 7, 8, 9, 10, 9, 8, 7, 6, 5, 4, 3);
        }
    }
}