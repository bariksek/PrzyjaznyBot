using DSharpPlus.EventArgs;
using System.Threading.Tasks;

namespace PrzyjaznyBot.Commands.ButtonCommands
{
    public interface IButtonResponseHelper
    {
        Task Resolve(ComponentInteractionCreateEventArgs e);
    }
}
