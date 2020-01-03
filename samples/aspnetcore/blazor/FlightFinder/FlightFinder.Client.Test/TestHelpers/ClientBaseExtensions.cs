using Grpc.Core;
using System.Threading.Tasks;

namespace FlightFinder.Client.Test
{
    public static class ClientBaseExtensions
    {
        public static AsyncUnaryCall<TResult> AsyncUnaryCall<TResult>(this ClientBase clientBase, TResult result)
            => clientBase.AsyncUnaryCall(Task.FromResult(result));

        public static AsyncUnaryCall<TResult> AsyncUnaryCall<TResult>(this ClientBase _, Task<TResult> resultTask)
        {
            return new AsyncUnaryCall<TResult>(
                resultTask,
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { });
        }
    }
}
