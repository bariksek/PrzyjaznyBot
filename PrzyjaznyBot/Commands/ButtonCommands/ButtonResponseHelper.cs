using DSharpPlus.EventArgs;
using PrzyjaznyBot.Common;
using PrzyjaznyBot.DAL;
using System.Threading.Tasks;

namespace PrzyjaznyBot.Commands.ButtonCommands
{
    public class ButtonResponseHelper : IButtonResponseHelper
    {
        BetModule CreateModule { get; set; }
        public IBetRepository BetRepository { get; }

        public ButtonResponseHelper(IBetRepository betRepository)
        {
            BetRepository = betRepository;
            CreateModule = new BetModule(BetRepository);
        }

        public async Task Resolve(ComponentInteractionCreateEventArgs e)
        {
            switch (e.Id)
            {
                case ButtonCustomId.CreateYes:
                    break;
                case ButtonCustomId.CreateNo:
                    break;
                case ButtonCustomId.CreateInfo:
                    break;
                case ButtonCustomId.CreateShowAllBets:
                    await CreateModule.ShowAllBets(e);
                    break;
            }
        }


    }
}
