using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace MsTeamsBot.Flows
{
    public interface IFlow
    {
        /// <summary>
        /// Add required dialogs for this flow
        /// </summary>
        /// <param name="dialogs">Collection of all dialogs</param>
        void AddDialogs(DialogSet dialogs);
        
        /// <summary>
        /// Try to consume utterance and begin dialog
        /// </summary>
        /// <param name="dialogContext">Dialog context</param>
        /// <param name="utterance">User utterance</param>
        /// <returns>True if flow could consume utterance and start a dialog, false otherwise</returns>
        Task<bool> TryConsumeAsync(DialogContext dialogContext, string utterance);
    }
}