namespace Elevator.DataObjects
{
    public class AsynchonorousElevatorResponse
    {
        private readonly TaskCompletionSource<uint> _queuedFloor;
        private readonly ICollection<WrappedCall<uint>> _floorQueue;
        private readonly Task _elevatorArrivedForPickup;

        public AsynchonorousElevatorResponse(ICollection<WrappedCall<uint>> floorQueue, Task elevatorArrivedForPickup, TaskCompletionSource<uint> queuedFloor)
        {
            _floorQueue = floorQueue;
            _elevatorArrivedForPickup = elevatorArrivedForPickup;
            _queuedFloor = queuedFloor;
        }

        public async Task SelectFloor(uint floor)
        {
            await _elevatorArrivedForPickup;
            var tsk = new TaskCompletionSource();
            lock (_floorQueue)
            {
                _floorQueue.Add(new WrappedCall<uint>(tsk, floor));
            }
            _queuedFloor.SetResult(floor); //TODO: what to do about calling elevator in the opposite direction? write a test for it, and figure it out then
            await tsk.Task;
        }

        public Task ElevatorArrivedForPickup => _elevatorArrivedForPickup;
    }
}