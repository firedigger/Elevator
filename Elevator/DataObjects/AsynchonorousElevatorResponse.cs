namespace Elevator.DataObjects
{
    public class AsynchonorousElevatorResponse
    {
        private readonly TaskCompletionSource<uint> _queuedFloor;
        private readonly ICollection<WrappedCall<uint>> _floorQueue;
        private readonly object _lockObject;
        private readonly Task _elevatorArrivedForPickup;

        public AsynchonorousElevatorResponse(ICollection<WrappedCall<uint>> floorQueue, object lockObject, Task elevatorArrivedForPickup, TaskCompletionSource<uint> queuedFloor)
        {
            _floorQueue = floorQueue;
            _lockObject = lockObject;
            _elevatorArrivedForPickup = elevatorArrivedForPickup;
            _queuedFloor = queuedFloor;
        }

        public Task SelectFloor(uint floor)
        {
            //queue after the elevator has arrived for pickup or throw exception if it hasn't?
            var tsk = new TaskCompletionSource();
            lock (_lockObject)
            {
                _floorQueue.Add(new WrappedCall<uint>(tsk, floor));
            }
            _queuedFloor.SetResult(floor);
            return tsk.Task;
        }

        public Task ElevatorArrivedForPickup => _elevatorArrivedForPickup;
    }
}