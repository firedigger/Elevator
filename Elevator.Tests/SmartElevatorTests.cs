using Elevator.DataObjects;
using Elevator.Elevators;

namespace Elevator.Tests;

public class SmartElevatorTests
{
    [Fact]
    public async Task SingeCall()
    {
        var elevator = new SmartElevator();
        var cancellationTokenSource = new CancellationTokenSource();
        var visitedFloors = new List<uint>();
        elevator.Start(visitedFloors.Add, cancellationTokenSource.Token);
        var response = elevator.Call(new DirectedElevatorCall
        {
            SourceFloor = 1,
            Direction = Direction.Up
        });
        await response.ElevatorArrivedForPickup;
        await response.SelectFloor(2);
        cancellationTokenSource.Cancel();
        Assert.Equal(new uint[] { 1, 2 }, visitedFloors);
    }

    [Fact]
    public async Task ShouldCombineTwoCollinearCalls() //TODO: check why this test hangs if ran all tests, xunit parallel testing?
    {
        var elevator = new SmartElevator();
        var cancellationTokenSource = new CancellationTokenSource();
        var visitedFloors = new List<uint>();
        elevator.Start(visitedFloors.Add, cancellationTokenSource.Token);
        var response1 = elevator.Call(new DirectedElevatorCall
        {
            SourceFloor = 1,
            Direction = Direction.Up
        });
        await response1.ElevatorArrivedForPickup;
        var response2 = elevator.Call(new DirectedElevatorCall
        {
            SourceFloor = 3,
            Direction = Direction.Up
        });
        var arrival1 = response1.SelectFloor(6);
        await response2.ElevatorArrivedForPickup;
        var arrival2 = response2.SelectFloor(5);
        await arrival2;
        await arrival1;
        cancellationTokenSource.Cancel();
        Assert.Equal(new uint[] { 1, 2, 3, 4, 5, 6 }, visitedFloors);
    }

    //[Fact]
    //public async Task AsyncCall()
    //{
    //    var elevator = new SmartElevator();
    //    var response = elevator.Call2(new DirectedElevatorCall
    //    {
    //        SourceFloor = 1,
    //        Direction = Direction.Up
    //    });
    //    elevator.Start();
    //    await response.ElevatorArrivedForPickup.ContinueWith(_ => response._queuedFloor.SetResult(2));
    //}
}