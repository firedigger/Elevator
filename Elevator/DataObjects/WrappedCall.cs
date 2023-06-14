namespace Elevator
{
    public class WrappedCall<T>
    {
        public TaskCompletionSource TaskCompletionSource { get; init; } //to private and expose task
        public T Call { get; init; }
        public WrappedCall(TaskCompletionSource taskCompletionSource, T call)
        {
            TaskCompletionSource = taskCompletionSource;
            Call = call;
        }
    }
}
