using Elevator.DataObjects;
using Elevator.Elevators;
using System.Threading;

namespace Elevator.Tests;

public class SmartElevatorTests
{
    private readonly SmartElevator _elevator = new();
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly List<uint> _visitedFloors = new();

    public SmartElevatorTests()
    {
        _elevator.Start(_visitedFloors.Add, _cancellationTokenSource.Token);
    }

    [Fact]
    public async Task SingeCall()
    {
        var response = _elevator.Call(new DirectedElevatorCall
        {
            SourceFloor = 1,
            Direction = Direction.Up
        });
        await response.ElevatorArrivedForPickup;
        await response.SelectFloor(2);
        _cancellationTokenSource.Cancel();
        Assert.Equal(new uint[] { 1, 2 }, _visitedFloors);
    }

    [Fact]
    public async Task ShouldCombineTwoCollinearCalls()
    {
        var response1 = _elevator.Call(new DirectedElevatorCall
        {
            SourceFloor = 1,
            Direction = Direction.Up
        });
        await response1.ElevatorArrivedForPickup;
        var response2 = _elevator.Call(new DirectedElevatorCall
        {
            SourceFloor = 3,
            Direction = Direction.Up
        });
        var arrival1 = response1.SelectFloor(6);
        await response2.ElevatorArrivedForPickup;
        var arrival2 = response2.SelectFloor(5);
        await arrival2;
        await arrival1;
        _cancellationTokenSource.Cancel();
        Assert.Equal(ExpandFloorSequence(1, 6), _visitedFloors);
    }

    [Fact]
    public async Task ShouldNotCombineTwoOppositeCalls()
    {
        var response1 = _elevator.Call(new DirectedElevatorCall
        {
            SourceFloor = 1,
            Direction = Direction.Up
        });
        await response1.ElevatorArrivedForPickup;
        var response2 = _elevator.Call(new DirectedElevatorCall
        {
            SourceFloor = 3,
            Direction = Direction.Down
        });
        var arrival1 = response1.SelectFloor(6);
        await response2.ElevatorArrivedForPickup;
        Assert.True(arrival1.IsCompletedSuccessfully);
        var arrival2 = response2.SelectFloor(1);
        await arrival2;
        _cancellationTokenSource.Cancel();
        Assert.Equal(ExpandFloorSequence(1, 6, 1), _visitedFloors);
    }

    [Fact]
    public async Task OppositeCallsFromOneFloor()
    {
        var response1 = _elevator.Call(new DirectedElevatorCall
        {
            SourceFloor = 3,
            Direction = Direction.Up
        });
        var response2 = _elevator.Call(new DirectedElevatorCall
        {
            SourceFloor = 3,
            Direction = Direction.Down
        });
        await response1.ElevatorArrivedForPickup;
        var arrival1 = response1.SelectFloor(6);
        var arrival2 = response2.SelectFloor(1);
        var c = await Task.WhenAny(arrival1, response2.ElevatorArrivedForPickup);
        Assert.True(c == arrival1);
        await arrival2;
        _cancellationTokenSource.Cancel();
        Assert.Equal(ExpandFloorSequence(1, 6, 1), _visitedFloors);
    }

    private static IEnumerable<uint> ExpandFloorSequence(params uint[] floors)
    {
        if (floors.Length == 0)
        {
            yield break;
        }
        yield return floors[0];
        for (int i = 0; i < floors.Length - 1; i++)
        {
            var currentFloor = floors[i];
            var nextFloor = floors[i + 1];
            if (currentFloor < nextFloor)
            {
                for (uint j = currentFloor + 1; j <= nextFloor; j++)
                {
                    yield return j;
                }
            }
            else if (currentFloor > nextFloor)
            {
                for (uint j = currentFloor - 1; j >= nextFloor; j--)
                {
                    yield return j;
                }
            }
        }
    }
}