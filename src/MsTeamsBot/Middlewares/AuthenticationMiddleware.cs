using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using MsTeamsBot.Bots;
using MsTeamsBot.Flows;
using MsTeamsBot.State;

namespace MsTeamsBot.Middlewares
{
    public class AuthenticationMiddleware : IMiddleware
    {
        private readonly DialogSet _dialogSet;
        private static readonly string ActivityStateSigninVerifystate = "signin/verifyState";

        public AuthenticationMiddleware(DialogSet dialogSet)
        {
            _dialogSet = dialogSet;
        }

        public async Task OnTurn(ITurnContext context, MiddlewareSet.NextDelegate next)
        {
            var wasConsumed = false;
            var activity = context.Activity;

            if (activity != null && activity.Type == ActivityTypes.Invoke)
            {
                wasConsumed = await TryHandleInvoke(context, activity);
            }

            if (!wasConsumed)
            {
                await next().ConfigureAwait(false);
            }
        }

        private async Task<bool> TryHandleInvoke(ITurnContext context, Activity activity)
        {
            if (activity.Name != ActivityStateSigninVerifystate) return false;
            if (activity.Value == null) return false;

            dynamic payload = activity.Value;

            if (!TryGetValue(payload, new Func<dynamic, string>(p => (string) p.state.accessToken), out string accessToken))
                return false;

            var userState = context.GetUserState<UserState>();
            
            // TODO Use accessToken to fetch profile information from backend system

            userState.Email = "foo@mail.com";
            userState.FullName = "Mr Foo";
            userState.AccessToken = accessToken;
            
            // Show confirmation (check success/failure)
            var state = context.GetConversationState<Dictionary<string, object>>();
            var dialogContext = _dialogSet.CreateContext(context, state);
            await dialogContext.Begin(LoginFlow.LoginConfirmation);
            
            // Send invoke response
            await context.SendActivity(new Activity
            {
                Type = "invokeResponse",
                Value = new InvokeResponse()
            });
            
            return true;
        }

        private static bool TryGetValue<T>(dynamic dyn, Func<dynamic, T> selector, out T value)
        {
            try
            {
                value = selector(dyn);
                return true;
            }
            catch
            {
                value = default(T);
                return false;
            }
        }
    }
}