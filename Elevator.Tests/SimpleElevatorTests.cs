namespace Elevator.Tests
{
    public class SimpleElevatorTests
    {
        private readonly SimpleElevator elevator = new();

        private async Task AssertFloors(params uint[] expectedFloors)
        {
            Assert.Equal(expectedFloors, await elevator.FloorsEnumerator().Take(expectedFloors.Length).ToListAsync());
        }

        [Fact]
        public async Task SingleCall()
        {
            elevator.Call(new ElevatorCall
            {
                SourceFloor = 1,
                DestinationFloor = 10
            });
            await AssertFloors(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
        }

        [Fact]
        public async Task TwoCalls()
        {
            elevator.Call(new ElevatorCall
            {
                SourceFloor = 1,
                DestinationFloor = 10
            });
            elevator.Call(new ElevatorCall
            {
                SourceFloor = 9,
                DestinationFloor = 2
            });
            await AssertFloors(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 9, 8, 7, 6, 5, 4, 3, 2);
        }

        [Fact]
        public async Task InterruptCall()
        {
            elevator.Call(new ElevatorCall
            {
                SourceFloor = 1,
                DestinationFloor = 10
            });
            await AssertFloors(1, 2, 3, 4);
            elevator.Call(new ElevatorCall
            {
                SourceFloor = 6,
                DestinationFloor = 3
            });
            await AssertFloors(4, 5, 6, 7, 8, 9, 10, 9, 8, 7, 6, 5, 4, 3);
        }
    }
}