namespace Trace.GraphQL.Subscriptions
{
    using HotChocolate.AspNetCore;
    using HotChocolate.AspNetCore.Subscriptions;
    using HotChocolate.AspNetCore.Subscriptions.Protocols;
    using System.Text.Json;

    public sealed class JwtWebSocketAuthInterceptor : DefaultSocketSessionInterceptor
    {
        public override async ValueTask<ConnectionStatus> OnConnectAsync(
            ISocketSession session,
            IOperationMessagePayload payload,
            CancellationToken cancellationToken)
        {
            Console.WriteLine("🧩 [JwtWebSocketAuthInterceptor] OnConnectAsync triggered");

            try
            {
                // ✅ Deserialize into a dictionary (works in HotChocolate 14+)
                var payloadDict = payload.As<Dictionary<string, object>>();

                if (payloadDict == null || payloadDict.Count == 0)
                {
                    Console.WriteLine("⚠️ No connectionParams received or payload empty.");
                }
                else
                {
                    var json = JsonSerializer.Serialize(payloadDict);
                    Console.WriteLine($"🔍 Payload JSON: {json}");

                    if (payloadDict.TryGetValue("Authorization", out var auth))
                    {
                        Console.WriteLine($"🔑 Authorization: {auth}");
                    }
                    else
                    {
                        Console.WriteLine("⚠️ No Authorization field found in payload.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Interceptor error: {ex.Message}");
            }

            return await base.OnConnectAsync(session, payload, cancellationToken);
        }
    }
}
