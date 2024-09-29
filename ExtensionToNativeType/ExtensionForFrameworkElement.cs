using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MinimalisticWPF
{
    public static class ExtensionForFrameworkElement
    {
        /// <summary>
        /// MVVM模式下,在控件初始化时加载状态机条件切换功能块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="viewModel">控件DataContext</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static StateMachine StateMachineLoading<T>(this FrameworkElement source, T viewModel) where T : class
        {
            var vectorInterface = viewModel as IConditionalTransition<T> ?? throw new ArgumentException($"The [ {nameof(T)} ] Is Not A [ {nameof(IConditionalTransition<T>)} ]");

            var StateFields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(x => x.FieldType == typeof(State)).ToArray();
            var StateProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.PropertyType == typeof(State)).ToArray();
            var StateVectorField = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(x => x.PropertyType == typeof(StateVector<T>));

            var FieldStates = StateFields.Select(x => (State?)x.GetValue(viewModel)).ToArray();
            var PropertyStates = StateProperties.Select(x => (State?)x.GetValue(viewModel)).ToArray();
            var States = PropertyStates.Concat(FieldStates);
            var StateVector = (StateVector<T>?)StateVectorField?.GetValue(viewModel);

            var machine = StateMachine.Create(viewModel);
            vectorInterface.StateMachine = machine;
            if (States != null)
            {
                foreach (var state in States)
                {
                    if (state != null)
                    {
                        vectorInterface.StateMachine.States.Add(state);
                    }
                }
            }
            if (StateVector != null)
            {
                vectorInterface.StateVector = StateVector;
            }

            return machine;
        }

        /// <summary>
        /// 加载指向父级容器尺寸的过渡动画
        /// </summary>
        public static T TransitionToParentSize<T>(this T element, double rate, Action<TransitionParams> set) where T : FrameworkElement
        {
            var Parent = element.Parent as FrameworkElement;
            if (Parent == null) return element;
            var newWidth = Parent.Width * rate;
            var newHeight = Parent.Height * rate;
            var machine = element.FindStateMachine();
            if (machine == null) return element;
            var board = Transition.CreateBoardFromType<FrameworkElement>()
                .SetProperty(x => x.Width, newWidth)
                .SetProperty(x => x.Height, newHeight)
                .SetParams(set);
            element.BeginTransition(board);
            return element;
        }

        /// <summary>
        /// 加载指向父级容器高度的过渡动画
        /// </summary>
        public static T TransitionToParentHeight<T>(this T element, double rate, Action<TransitionParams> set) where T : FrameworkElement
        {
            var Parent = element.Parent as FrameworkElement;
            if (Parent == null) return element;
            var newHeight = Parent.Height * rate;
            var machine = element.FindStateMachine();
            if (machine == null) return element;
            var board = Transition.CreateBoardFromType<FrameworkElement>()
                .SetProperty(x => x.Height, newHeight)
                .SetParams(set);
            element.BeginTransition(board);
            return element;
        }

        /// <summary>
        /// 加载指向父级容器宽度的过渡动画
        /// </summary>
        public static T TransitionToParentWidth<T>(this T element, double rate, Action<TransitionParams> set) where T : FrameworkElement
        {
            var Parent = element.Parent as FrameworkElement;
            if (Parent == null) return element;
            var newWidth = Parent.Width * rate;
            var machine = element.FindStateMachine();
            if (machine == null) return element;
            var board = Transition.CreateBoardFromType<FrameworkElement>()
                .SetProperty(x => x.Width, newWidth)
                .SetParams(set);
            element.BeginTransition(board);
            return element;
        }
    }
}
