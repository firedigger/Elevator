using Elevator.DataObjects;

namespace Elevator
{
    public interface IAsyncElevator
    {
        public AsynchonorousElevatorResponse Call(DirectedElevatorCall call);
        public void Start(Action<uint> observer, CancellationToken cancellationToken);

    }
}
