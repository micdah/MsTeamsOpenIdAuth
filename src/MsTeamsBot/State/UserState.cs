using System.Collections.Generic;

namespace MsTeamsBot.State
{
    public class UserState : BaseState
    {
        public UserState()
        {
            Email = null;
        }

        public UserState(IDictionary<string, object> source) : base(source)
        {
            Email = null;
        }
        
        public string Email
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        public string AccessToken
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        public string FullName
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }
    }
}