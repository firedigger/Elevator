using System.Collections.Concurrent;
using Elevator.DataObjects;

namespace Elevator.Elevators;
public class SimpleElevator
{
    private uint currentFloor = 1;
    private readonly ConcurrentQueue<CompleteElevatorCall> queuedCalls = new();
    private readonly Queue<uint> queuedFloors = new();

    public IEnumerable<uint> FloorsEnumerator(bool breakOnCompleteCalls = false) //TODO: make sync Enumerable
    {
        yield return currentFloor;
        while (!breakOnCompleteCalls || !queuedCalls.IsEmpty || queuedFloors.Count > 0)
        {
            if (queuedFloors.Count > 0 && currentFloor == queuedFloors.Peek())
            {
                queuedFloors.Dequeue();
            }
            if (queuedFloors.Count == 0 && !queuedCalls.IsEmpty)
            {
                if (queuedCalls.TryDequeue(out var call))
                {
                    queuedFloors.Enqueue(call.SourceFloor);
                    queuedFloors.Enqueue(call.DestinationFloor);
                }
            }
            while (queuedFloors.Count > 0 && currentFloor == queuedFloors.Peek())
                queuedFloors.TryDequeue(out var _);
            if (queuedFloors.Count > 0)
            {
                var destinationFloor = queuedFloors.Peek();
                if (currentFloor > destinationFloor)
                {
                    --currentFloor;
                }
                if (currentFloor < destinationFloor)
                {
                    ++currentFloor;
                }
            }
            yield return currentFloor;
        }
    }

    public void Call(CompleteElevatorCall call)
    {
        queuedCalls.Enqueue(call);
    }
}
