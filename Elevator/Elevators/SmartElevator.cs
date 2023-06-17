using Elevator.DataObjects;

namespace Elevator.Elevators;
public class SmartElevator : IAsyncElevator
{
    private uint currentFloor = 1;
    private Direction nextDirection;
    private Direction currentDirection;
    private readonly List<Task> floorQueuedTasks = new();
    private readonly List<WrappedCall<DirectedElevatorCall>> queuedCalls = new();
    private readonly List<WrappedCall<uint>> floorCalls = new();

    public AsynchonorousElevatorResponse Call(DirectedElevatorCall call)
    {
        var arrivedForPickup = new TaskCompletionSource();
        lock (queuedCalls)
        {
            queuedCalls.Add(new WrappedCall<DirectedElevatorCall>(arrivedForPickup, call));
        }
        var floorCall = new TaskCompletionSource<uint>();
        arrivedForPickup.Task.ContinueWith(_ =>
        {
            currentDirection = call.Direction;
            lock (floorQueuedTasks)
            {
                floorQueuedTasks.Add(floorCall.Task);
            }
        });
        return new AsynchonorousElevatorResponse(floorCalls, arrivedForPickup.Task, floorCall);
    }

    public async void Start(Action<uint> observer, CancellationToken cancellationToken)
    {
        observer.Invoke(currentFloor);
        while (!cancellationToken.IsCancellationRequested)
        {
            //check any fulfilled calls
            lock (floorCalls)
            {
                bool matchFloorCalls(WrappedCall<uint> c) => c.Call == currentFloor;
                foreach (var call in floorCalls.Where(matchFloorCalls))
                {
                    call.TaskCompletionSource.SetResult();
                }
                floorCalls.RemoveAll(matchFloorCalls);
            }
            lock (queuedCalls)
            {
                bool matchQueuedCalls(WrappedCall<DirectedElevatorCall> c) => c.Call.SourceFloor == currentFloor && c.Call.Direction == nextDirection;
                foreach (var call in queuedCalls.Where(matchQueuedCalls))
                {
                    call.TaskCompletionSource.SetResult();
                }
                queuedCalls.RemoveAll(matchQueuedCalls);
            }

            //wait for a floor call to happen if there are none
            if (floorCalls.Count == 0 && floorQueuedTasks.Count > 0)
            {
                await Task.Yield();
                List<Task> floorQueuedTasksCopy;
                lock (floorQueuedTasks)
                {
                    floorQueuedTasksCopy = floorQueuedTasks.ToList();
                }
                await Task.WhenAny(floorQueuedTasksCopy);
                lock (floorQueuedTasks)
                {
                    floorQueuedTasks.RemoveAll(t => t.IsCompleted);
                }
            }

            //find next floor to go to, either from floor calls or from queued calls
            uint? nextQueuedFloor = null;
            if (floorCalls.Count > 0)
            {
                nextQueuedFloor = currentDirection == Direction.Up ? floorCalls.Min(c => c.Call) : floorCalls.Max(c => c.Call);
                //find collinear calls to override nextQueuedFloor
                var closestCollinearCall = queuedCalls.Where(x => x.Call.SourceFloor >= currentFloor && x.Call.SourceFloor <= nextQueuedFloor && x.Call.Direction == currentDirection).OrderBy(c => c.Call.SourceFloor * (currentDirection == Direction.Up ? 1 : -1)).FirstOrDefault();
                if (closestCollinearCall is not null)
                {
                    nextQueuedFloor = closestCollinearCall.Call.SourceFloor;
                }
            }
            else if (queuedCalls.Count > 0)
            {
                nextQueuedFloor = queuedCalls[0].Call.SourceFloor;
                nextDirection = queuedCalls[0].Call.Direction;
                currentDirection = currentFloor > nextQueuedFloor ? Direction.Down : Direction.Up;
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

            await Task.Delay(40, cancellationToken);
        }
    }
}
