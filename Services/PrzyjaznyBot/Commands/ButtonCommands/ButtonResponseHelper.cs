using DSharpPlus.EventArgs;
using PrzyjaznyBot.Common;

namespace PrzyjaznyBot.Commands.ButtonCommands
{
    public class ButtonResponseHelper : IButtonResponseHelper
    {
        BetModule BetModule { get; set; }
        private readonly BetService.BetService.BetServiceClient _betServiceClient;
        private readonly UserService.UserService.UserServiceClient _userServiceClient;

        public ButtonResponseHelper(BetService.BetService.BetServiceClient betServiceClient,
            UserService.UserService.UserServiceClient userServiceClient)
        {
            _betServiceClient = betServiceClient;
            _userServiceClient = userServiceClient;
            BetModule = new BetModule(_betServiceClient, _userServiceClient);
        }

        public async Task Resolve(ComponentInteractionCreateEventArgs e)
        {
            var id = e.Id.Split('+')[0];
            var betId = e.Id.Split('+')[1];

            switch (id)
            {
                case ButtonCustomId.CreateYes:
                    await BetModule.BetCommand(e, betId, BetService.Condition.Yes);
                    break;
                case ButtonCustomId.CreateNo:
                    await BetModule.BetCommand(e, betId, BetService.Condition.No);
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
