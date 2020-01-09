using FlightFinder.Shared;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightFinder.Client.Test
{
    // Wraps an arbitrary gRPC client class so that tests can observe the calls and
    // supply responses. Just a quick proof-of-concept and not very tidy.
    public class TestGrpcClient<TClient> where TClient: ClientBase
    {
        readonly TestCallInvoker _testCallInvoker = new TestCallInvoker();

        public TClient Client { get; }

        public TestGrpcClient()
        {
            var ctor = typeof(TClient).GetConstructor(new[] { typeof(CallInvoker) });
            Client = (TClient)ctor.Invoke(new[] { _testCallInvoker });
        }

        public IEnumerable<ICallInfo> Calls()
            => _testCallInvoker.Calls;

        public IEnumerable<ICallInfo> Calls(string name)
            => _testCallInvoker.Calls.Where(c => c.MethodName == name);

        public IEnumerable<ICallInfo<TRequest>> Calls<TRequest>()
            => _testCallInvoker.Calls.OfType<ICallInfo<TRequest>>();

        public IEnumerable<TaskCompletionSource<TResponse>> PendingResponses<TResponse>()
        {
            return _testCallInvoker
                .Calls
                .OfType<IResponseInfo<TResponse>>()
                .Select(c => c.Response)
                .Where(c => !c.Task.IsCompleted);
        }

        public void SetResponse<TResponse>(TResponse response)
        {
            var candidates = PendingResponses<TResponse>().ToList();
            switch (candidates.Count)
            {
                case 0:
                    throw new InvalidOperationException($"There are no outstanding calls waiting for a response of type {typeof(TResponse)}.");
                case 1:
                    candidates[0].SetResult(response);
                    break;
                default:
                    throw new InvalidOperationException($"There are multiple outstanding calls waiting for a response of type {typeof(TResponse)}.");
            }
        }

        public class CallInfo<TRequest, TResponse> : ICallInfo<TRequest>, IResponseInfo<TResponse>
        {
            public Method<TRequest, TResponse> Method { get; }
            public string Host { get; }
            public CallOptions Options { get; }
            public TRequest Request { get; }
            public TaskCompletionSource<TResponse> Response { get; }

            public string MethodName => Method.Name;

            public CallInfo(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
            {
                Method = method;
                Host = host;
                Options = options;
                Request = request;
                Response = new TaskCompletionSource<TResponse>();
            }       
        }

        private interface IResponseInfo<TResponse>
        {
            TaskCompletionSource<TResponse> Response { get; }
        }

        public interface ICallInfo
        {
            string MethodName { get; }
        }

        public interface ICallInfo<TRequest> : ICallInfo
        {
            TRequest Request { get; }
        }

        class TestCallInvoker : CallInvoker
        {
            private List<ICallInfo> _calls = new List<ICallInfo>();
            public IReadOnlyCollection<ICallInfo> Calls => _calls;

            public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
                => throw new NotImplementedException();

            public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
                => throw new NotImplementedException();

            public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
                => throw new NotImplementedException();

            public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
            {
                var call = new CallInfo<TRequest, TResponse>(method, host, options, request);
                _calls.Add(call);
                return new AsyncUnaryCall<TResponse>(
                    call.Response.Task,
                    Task.FromResult(new Metadata()),
                    () => Status.DefaultSuccess,
                    () => new Metadata(),
                    () => { });
            }

            public override TResponse BlockingUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
                => throw new NotImplementedException();
        }
    }
}
