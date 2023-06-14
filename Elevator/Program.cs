using Elevator.DataObjects;
using Elevator.Elevators;

Console.WriteLine("Hello, Elevator!");

var elevator = new SimpleElevator();

elevator.Call(new CompleteElevatorCall { SourceFloor = 5, DestinationFloor = 10 });
elevator.Call(new CompleteElevatorCall { SourceFloor = 5, DestinationFloor = 1 });

foreach (var floor in elevator.FloorsEnumerator(true))
{
    Console.WriteLine($"I am at floor {floor}");
}