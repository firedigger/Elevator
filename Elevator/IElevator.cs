namespace Elevator
{
    public interface IElevator
    {
        public IAsyncEnumerable<uint> FloorsEnumerator(bool breakOnCompleteCalls);
        public void Call(ElevatorCall call);
    }
}
