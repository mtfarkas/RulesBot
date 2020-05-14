using Discord.WebSocket;
using System.Threading.Tasks;

namespace RulesBot.MessageHandlers
{
    public interface IMessageHandler
    {
        Task<bool> Execute(SocketUserMessage message);
    }
}
