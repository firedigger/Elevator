using System.Collections.Concurrent;

namespace Elevator
{
    public class SimpleElevator : IElevator
    {
        private uint currentFloor = 1;
        private readonly ConcurrentQueue<ElevatorCall> queuedCalls = new();
        private readonly Queue<uint> queuedFloors = new();

        public async IAsyncEnumerable<uint> FloorsEnumerator(bool breakOnCompleteCalls = false)
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

        public void Call(ElevatorCall call)
        {
            queuedCalls.Enqueue(call);
        }
    }
}
