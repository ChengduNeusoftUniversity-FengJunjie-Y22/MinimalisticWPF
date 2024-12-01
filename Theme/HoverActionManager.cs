using Microsoft.Windows.Themes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Media;

namespace MinimalisticWPF
{
    internal class HoverActionManager
    {
        internal HoverActionManager() { }

        internal State ToTheme = new() { StateName = "TAM_ToTheme" };
        internal State ToHover = new() { StateName = "TAM_ToHover" };

        internal ConcurrentDictionary<object, StateMachine> HoverPool = new();

        internal void Enter(object target, Action<TransitionParams>? action = null)
        {
            if (HoverPool.TryGetValue(target, out var value))
            {
                value.ReSet();
                value.States.Add(ToHover);
                value.Transition(ToHover.StateName, action ?? TransitionParams.Hover);
            }
            else
            {
                var machine = new StateMachine(target, ToHover);
                machine.Transition(ToHover.StateName, action ?? TransitionParams.Hover);
                HoverPool.TryAdd(target, machine);
            }
        }
        internal void Leave(object target, Action<TransitionParams>? action = null)
        {
            if (HoverPool.TryGetValue(target, out var value))
            {
                value.ReSet();
                value.States.Add(ToTheme);
                value.Transition(ToTheme.StateName, action ?? TransitionParams.Hover);
            }
            else
            {
                var machine = new StateMachine(target, ToTheme);
                machine.Transition(ToTheme.StateName, action ?? TransitionParams.Hover);
                HoverPool.TryAdd(target, machine);
            }
        }
    }
}
