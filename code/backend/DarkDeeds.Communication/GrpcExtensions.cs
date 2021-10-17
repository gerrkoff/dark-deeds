using System.Linq;
using Grpc.Core;
using Microsoft.Net.Http.Headers;

namespace DarkDeeds.Communication
{
    public static class GrpcExtensions
    {
        public static void AddAuthorizationIfEmpty(this Metadata headers, string token)
        {
            if (headers.All(x => x.Key != HeaderNames.Authorization))
                headers.Add(HeaderNames.Authorization, $"Bearer {token}");
        }
    }
}