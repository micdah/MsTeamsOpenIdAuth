using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using MsTeamsBot.Flows;

namespace MsTeamsBot.Bots
{
    public class MsTeamsBot : IBot
    {
        private readonly DialogSet _dialogSet;
        private readonly ICollection<IFlow> _flows;

        public MsTeamsBot(DialogSet dialogSet, ICollection<IFlow> flows)
        {
            _dialogSet = dialogSet;
            _flows = flows;
        }
        
        public async Task OnTurn(ITurnContext context)
        {
            switch (context.Activity.Type)
            {
                case ActivityTypes.ConversationUpdate:
                    await HandleConversationUpdateAsync(context);
                    break;
                case ActivityTypes.Message:
                    await HandleMessageAsync(context);
                    break;
            }
        }

        private async Task HandleConversationUpdateAsync(ITurnContext context)
        {
            var activity = context.Activity;
            
            // Check if new members have been added, and send them a greeting
            if (activity.MembersAdded != null && activity.MembersAdded.Any())
            {
                foreach (var member in activity.MembersAdded)
                {
                    if (member.Id == activity.Recipient.Id) continue;
                    
                    await context.SendActivity($"Greetings {member.Name}, I am the MsTeamsBot.\n" +
                                               $"Please type '`login`' to start logging in.");
                }
            }
        }

        private async Task HandleMessageAsync(ITurnContext context)
        {
            var state = context.GetConversationState<Dictionary<string,object>>();
            var dialogContext = _dialogSet.CreateContext(context, state);

            // Cancel active dialog if user writes 'cancel'
            var utterance = context.Activity.Text.ToLowerInvariant();
            if (utterance == "cancel")
            {
                if (dialogContext.ActiveDialog != null)
                {
                    await context.SendActivity("Alright, just forget it then");
                    dialogContext.EndAll();
                }
                else
                {
                    await context.SendActivity("Sorry, I cannot do that Dave");
                }
            }

            if (!context.Responded)
            {
                await dialogContext.Continue();

                if (!context.Responded)
                {
                    var consumed = false;
                    foreach (var flow in _flows)
                    {
                        if (await flow.TryConsumeAsync(dialogContext, utterance))
                        {
                            consumed = true;
                            break;
                        }
                    }
                    
                    // No flow matches
                    if (!consumed)
                        await context.SendActivity("Sorry, I didn't understand that.");
                }
            }
        }
        
        
    }
}