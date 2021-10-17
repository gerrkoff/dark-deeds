using System;
using System.Threading.Tasks;
using DarkDeeds.Common.Validation.Exceptions;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace DarkDeeds.Communication.Interceptors
{
    public class ExceptionInterceptor : Interceptor
    {
        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                return await continuation(request, context);
            }
            catch (ModelValidationException e)
            {
                var metadata = new Metadata();
                foreach (var error in e.Errors)
                {
                    metadata.Add("error", error.ErrorMessage);
                }
                
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Argument"), metadata);
            }
            catch (Exception e)
            {
                throw new RpcException(new Status(StatusCode.Unknown, "Unknown Exception", e));
            }
        }
    }
}