using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace MsTeamsBot.Flows
{
    public class HelpFlow : IFlow
    {
        public const string Help = "HelpFlow_Help";
        
        public void AddDialogs(DialogSet dialogs)
        {
            dialogs.Add(Help, new WaterfallStep[] { ShowHelpStep });
        }

        public async Task<bool> TryConsumeAsync(DialogContext dialogContext, string utterance)
        {
            if (utterance.Contains("help"))
            {
                await dialogContext.Begin(Help);
                return true;
            }

            return false;
        }

        private async Task ShowHelpStep(DialogContext dialogContext, object args, SkipStepFunction next)
        {
            await dialogContext.Context.SendActivity("These are the commands I support:\n" +
                                                     ".... To be implemented");
            await dialogContext.End();
        }
    }
}