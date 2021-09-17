using DSharpPlus.EventArgs;
using System.Threading.Tasks;

namespace PrzyjaznyBot
{
    public interface IButtonResponseHelper
    {
        Task Resolve(ComponentInteractionCreateEventArgs e);
    }
}
