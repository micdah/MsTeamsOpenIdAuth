using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MsTeamsBot.State
{
    public abstract class BaseState : Dictionary<string, object>
    {
        protected BaseState() : this(null)
        {
        }

        protected BaseState(IDictionary<string, object> source)
        {
            source?.ToList().ForEach(x => Add(x.Key, x.Value));
        }

        protected T GetProperty<T>([CallerMemberName]string propName = null)
        {
            if (this.TryGetValue(propName, out object value))
            {
                return (T)value;
            }
            return default(T);
        }

        protected void SetProperty(object value, [CallerMemberName]string propName = null)
        {
            this[propName] = value;
        }
    }
}