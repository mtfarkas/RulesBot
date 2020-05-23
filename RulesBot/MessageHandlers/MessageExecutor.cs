using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using YAUL.Data;

namespace RulesBot.MessageHandlers
{
    public class MessageExecutor
    {
        private readonly List<IMessageHandler> messageHandlers = new List<IMessageHandler>();
        
        public void RegisterHandler(IMessageHandler handler)
            => messageHandlers.Add(handler);

        public async Task<Result> Execute(SocketUserMessage message)
        {
            foreach(var handler in messageHandlers)
            {
                bool result = await handler.Execute(message);

                if (result) return Result.Ok();
            }

            return Result.Fail("No handler found for message.");
        }

        public static void RegisterAssemblyHandlers(Assembly asm, IServiceCollection services)
        {
            var eligibleTypes = asm.DefinedTypes.Where(t => !t.IsInterface && !t.IsAbstract && typeof(IMessageHandler).IsAssignableFrom(t));

            foreach(var type in eligibleTypes)
            {
                services.AddScoped(type);
            }
        }
    }
}
