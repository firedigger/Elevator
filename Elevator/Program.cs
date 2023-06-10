using Elevator;

Console.WriteLine("Hello, Elevator!");

var elevator = new SimpleElevator();

elevator.Call(new ElevatorCall { SourceFloor = 5, DestinationFloor = 10 });
elevator.Call(new ElevatorCall { SourceFloor = 5, DestinationFloor = 1 });

await foreach (var floor in elevator.FloorsEnumerator(true))
{
    Console.WriteLine($"I am at floor {floor}");
}