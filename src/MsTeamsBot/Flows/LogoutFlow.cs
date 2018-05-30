using System.Threading.Tasks;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Dialogs;
using MsTeamsBot.State;

namespace MsTeamsBot.Flows
{
    public class LogoutFlow : IFlow
    {
        public const string LogoutPrompt = "LogoutFlow_LogoutPrompt";
        
        public void AddDialogs(DialogSet dialogs)
        {
            dialogs.Add(LogoutPrompt, new WaterfallStep[] {LogoutPromptStep});
        }

        public async Task<bool> TryConsumeAsync(DialogContext dialogContext, string utterance)
        {
            if (utterance.Contains("logout"))
            {
                await dialogContext.Begin(LogoutPrompt);
                return true;
            }

            return false;
        }

        private async Task LogoutPromptStep(DialogContext dialogContext, object args, SkipStepFunction next)
        {
            var userState = dialogContext.Context.GetUserState<UserState>();
            userState.AccessToken = null;

            await dialogContext.Context.SendActivity("You are now signed out");
            await dialogContext.End();
        }
    }
}