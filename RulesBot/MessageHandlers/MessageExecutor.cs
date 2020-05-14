using Discord.WebSocket;
using System.Collections.Generic;
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
    }
}
