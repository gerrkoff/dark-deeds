using Grpc.Core;
using Microsoft.Net.Http.Headers;

namespace DarkDeeds.Communication
{
    public static class GrpcExtensions
    {
        public static void AddAuthorization(this Metadata headers, string token)
        {
            headers.Add(HeaderNames.Authorization, $"Bearer {token}");
        }
    }
}