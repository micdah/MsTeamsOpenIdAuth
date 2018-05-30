using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using MsTeamsBot.State;

namespace MsTeamsBot.Flows
{
    public class LoginFlow : IFlow
    {
        public const string LoginPrompt = "LoginFlow_LoginPrompt";
        public const string LoginConfirmation = "LoginFlow_LoginConfirmation";
        
        private static readonly string LoginStartUrlKey = "LoginStartUrl";
        private readonly string _loginStartUrl;
        
        public LoginFlow(IConfiguration configuration)
        {
            _loginStartUrl = configuration[LoginStartUrlKey];
            if (string.IsNullOrEmpty(_loginStartUrl))
                throw new ArgumentException($"Missing setting '{LoginStartUrlKey}'");
        }
        
        public void AddDialogs(DialogSet dialogs)
        {
            dialogs.Add(LoginPrompt, new WaterfallStep[] {RedirectToLoginStep, ShowLoginConfirmation});
            dialogs.Add(LoginConfirmation, new WaterfallStep[] {ShowLoginConfirmation});
        }

        public async Task<bool> TryConsumeAsync(DialogContext dialogContext, string utterance)
        {
            if (utterance.Contains("login"))
            {
                await dialogContext.Begin(LoginPrompt);
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// Check if user is not already authenticated (and if so, also check that the access token is still
        /// valid), and if not prompt the user to sign in. 
        /// </summary>
        private async Task RedirectToLoginStep(DialogContext dialogContext, object args, SkipStepFunction next)
        {
            if (!string.IsNullOrEmpty(dialogContext.Context.Activity.ChannelId))
            {
                // TODO Show warning that we are not in a private conversation and end flow
            }
            
            var userState = dialogContext.Context.GetUserState<UserState>();

            if (!string.IsNullOrEmpty(userState.AccessToken))
            {
                // TODO Verify access token still works, and don't skip if access token is bad
                var stillValid = true;
                if (stillValid)
                {
                    await next();
                    return;
                }
            }
            
            userState.AccessToken = null;
            
            await dialogContext.Context.SendActivity(new Activity
            {
                Type = ActivityTypes.Message,
                Attachments = new List<Attachment>
                {
                    new HeroCard
                    {
                        Title = "Please log in to MsTeamsBot",
                        Text = "Click below to log in to MyAwesomeProduct(tm), to start using all the great features",
                        Buttons = new List<CardAction>
                        {
                            new CardAction
                            {
                                Type = "signin",
                                Title = "Log-in",
                                Text = "Log-in",
                                Value = _loginStartUrl                                
                            }
                        }
                    }.ToAttachment()
                }
            });

            // Don't continue, AuthenticationMiddleware will trigger confirmation when completed
            await dialogContext.End();
        }

        /// <summary>
        /// Show login information
        /// </summary>
        private async Task ShowLoginConfirmation(DialogContext dialogContext, object args, SkipStepFunction next)
        {
            var userState = dialogContext.Context.GetUserState<UserState>();

            if (!string.IsNullOrEmpty(userState.AccessToken))
            {
                await dialogContext.Context.SendActivity(
                    $"Hello {userState.FullName}, you are signed in as {userState.Email}\n" +
                    $"Type '`logout`' if you want to log out again");
            }
            else
            {
                await dialogContext.Context.SendActivity("Something went wrong, please try again");
            }
            
            await dialogContext.End();
        }
    }
}