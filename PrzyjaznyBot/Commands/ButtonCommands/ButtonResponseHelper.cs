using DSharpPlus.EventArgs;
using PrzyjaznyBot.Common;
using PrzyjaznyBot.DAL;
using System.Threading.Tasks;

namespace PrzyjaznyBot.Commands.ButtonCommands
{
    public class ButtonResponseHelper : IButtonResponseHelper
    {
        BetModule BetModule { get; set; }
        public IBetRepository BetRepository { get; }
        public IUserRepository UserRepository { get; }

        public ButtonResponseHelper(IBetRepository betRepository, IUserRepository userRepository)
        {
            BetRepository = betRepository;
            UserRepository = userRepository;
            BetModule = new BetModule(BetRepository, UserRepository);
        }

        public async Task Resolve(ComponentInteractionCreateEventArgs e)
        {
            var id = e.Id.Split('+')[0];
            var betId = e.Id.Split('+')[1];

            switch (id)
            {
                case ButtonCustomId.CreateYes:
                    await BetModule.BetCommand(e, betId, Condition.Yes);
                    break;
                case ButtonCustomId.CreateNo:
                    await BetModule.BetCommand(e, betId, Condition.No);
                    break;
                case ButtonCustomId.CreateInfo:
                    await BetModule.BetInfoCommand(e, betId);
                    break;
                case ButtonCustomId.CreateShowAllBets:
                    await BetModule.ShowAllBetsCommand(e);
                    break;
            }
        }


    }
}
