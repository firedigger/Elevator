using Elevator.DataObjects;

namespace Elevator.Elevators;
public class SmartElevator : IAsyncElevator
{
    private uint currentFloor = 1;
    private Direction currentDirection;
    private List<Task> floorQueuedTasks = new();
    private readonly object lockObject = new(); //TODO: lock all necessary places. Lock the 2 lists separately?
    private readonly List<WrappedCall<DirectedElevatorCall>> queuedCalls = new();
    private readonly List<WrappedCall<uint>> floorCalls = new();

    public AsynchonorousElevatorResponse Call(DirectedElevatorCall call)
    {
        var arrivedForPickup = new TaskCompletionSource();
        lock (lockObject)
        {
            queuedCalls.Add(new WrappedCall<DirectedElevatorCall>(arrivedForPickup, call));
        }
        var floorCall = new TaskCompletionSource<uint>();
        floorQueuedTasks.Add(floorCall.Task);
        return new AsynchonorousElevatorResponse(floorCalls, lockObject, arrivedForPickup.Task, floorCall);
    }

    public async void Start(Action<uint> observer, CancellationToken cancellationToken)
    {
        observer.Invoke(currentFloor);
        while (!cancellationToken.IsCancellationRequested)
        {
            //check any fulfilled calls
            lock (lockObject)
            {
                foreach (var call in floorCalls.Where(c => c.Call == currentFloor))
                {
                    call.TaskCompletionSource.SetResult();
                }
                floorCalls.RemoveAll(c => c.Call == currentFloor);
                foreach (var call in queuedCalls.Where(c => c.Call.SourceFloor == currentFloor && c.Call.Direction == currentDirection))
                {
                    call.TaskCompletionSource.SetResult();
                }
                queuedCalls.RemoveAll(c => c.Call.SourceFloor == currentFloor && c.Call.Direction == currentDirection);
            }

            //wait for a floor call to happen if there are none
            if (floorCalls.Count == 0 && floorQueuedTasks.Count > 0)
            {
                var completedTask = await Task.WhenAny(floorQueuedTasks); //TODO: timeout parameter , Task.Delay(1000)
                floorQueuedTasks.Remove(completedTask);
            }

            //find next floor to go to, either from floor calls or from queued calls
            uint? nextQueuedFloor = null;
            if (floorCalls.Count > 0)
            {
                nextQueuedFloor = floorCalls.Min(c => c.Call); //TODO: handle direction
                //find collinear calls to override nextQueuedFloor
                var closestCollinearCall = queuedCalls.Where(x => x.Call.SourceFloor >= currentFloor && x.Call.SourceFloor <= nextQueuedFloor && x.Call.Direction == currentDirection).OrderBy(c => c.Call.SourceFloor).FirstOrDefault(); //TODO: handle direction
                if (closestCollinearCall is not null)
                {
                    nextQueuedFloor = closestCollinearCall.Call.SourceFloor;
                }
            }
            else if (queuedCalls.Count > 0)
            {
                nextQueuedFloor = queuedCalls[0].Call.SourceFloor;
            }

            //move the elevator in nextQueuedFloor direction
            if (currentFloor > nextQueuedFloor)
            {
                --currentFloor;
                observer.Invoke(currentFloor);
            }
            if (currentFloor < nextQueuedFloor)
            {
                ++currentFloor;
                observer.Invoke(currentFloor);
            }

            await Task.Yield(); //TODO: how to handle iterations better?
        }
    }
}
