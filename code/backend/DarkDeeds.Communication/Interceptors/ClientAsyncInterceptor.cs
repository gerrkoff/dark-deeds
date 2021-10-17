using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace DarkDeeds.Communication.Interceptors
{
    public abstract class ClientAsyncInterceptor : Interceptor
    {
        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            Func<Status> getStatusFunc = () => new Status(0, string.Empty);
            Func<Metadata> getTrailersFunc = () => null;
            Action disposeAction = () => { };
            
            var continuationTask = Task.Run(async () =>
            {
                await Intercept(request, context);
                return continuation(request, context);
            }).ContinueWith(task =>
            {
                getStatusFunc = task.Result.GetStatus;
                getTrailersFunc = task.Result.GetTrailers;
                disposeAction = task.Result.Dispose;
                return task.Result;
            });

            return new AsyncUnaryCall<TResponse>(
                Task.Run(async () => await (await continuationTask).ResponseAsync),
                Task.Run(async () => await (await continuationTask).ResponseHeadersAsync),
                getStatusFunc,
                getTrailersFunc,
                disposeAction);
        }

        protected virtual Task Intercept<TRequest, TResponse>(TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context)
            where TRequest : class where TResponse : class
        {
            return Task.CompletedTask;
        }
    }
}