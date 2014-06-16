using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Interactivity;
using System.Windows;
using System.Windows.Input;

namespace Dziennik.CommandUtils
{
    public class EventToCommand : TriggerAction<DependencyObject>
    {
        public enum HowHandleRouted
        {
            /// <summary>
            /// Don't handle event by setting e.Handled to true
            /// </summary>
            DontHandle,
            /// <summary>
            /// Handle event before executing command by setting e.Handled to true
            /// </summary>
            HandleBefore,
            /// <summary>
            /// Handle event after executing command by setting e.Handled to true
            /// </summary>
            HandleAfter,
        }

        #region CommandProperty
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(EventToCommand), new PropertyMetadata(null, (s, e) =>
                                                                    {
                                                                        EventToCommand sender = s as EventToCommand;
                                                                        if (sender == null) return;

                                                                        if (e.OldValue != null)
                                                                        {
                                                                            ICommand oldCommand = e.OldValue as ICommand;
                                                                            oldCommand.CanExecuteChanged -= sender.newCommand_CanExecuteChanged;
                                                                        }
                                                                        if (e.NewValue != null)
                                                                        {
                                                                            ICommand newCommand = e.NewValue as ICommand;
                                                                            newCommand.CanExecuteChanged += sender.newCommand_CanExecuteChanged;
                                                                        }

                                                                        sender.CheckCanExecute();
                                                                    }));
        #endregion
        /// <summary>
        /// Default null
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        #region CommandParametrProperty
        public static readonly DependencyProperty CommandParametrProperty = DependencyProperty.Register("CommandParametr", typeof(object), typeof(EventToCommand), new PropertyMetadata(null, (s, e) =>
                                                                            {
                                                                                EventToCommand sender = s as EventToCommand;
                                                                                if (sender == null) return;
                                                                                sender.CheckCanExecute();
                                                                            }));
        #endregion
        /// <summary>
        /// Default null
        /// </summary>
        public object CommandParametr
        {
            get { return (object)GetValue(CommandParametrProperty); }
            set { SetValue(CommandParametrProperty, value); }
        }

        #region ToggleIsEnabledProperty
        public static readonly DependencyProperty ToggleIsEnabledProperty = DependencyProperty.Register("ToggleIsEnabled", typeof(bool), typeof(EventToCommand), new PropertyMetadata(false, (s, e) =>
                                                                            {
                                                                                EventToCommand sender = s as EventToCommand;
                                                                                if (sender == null) return;
                                                                                sender.CheckCanExecute();
                                                                            }));
        #endregion
        /// <summary>
        /// Default false
        /// </summary>
        public bool ToggleIsEnabled
        {
            get { return (bool)GetValue(ToggleIsEnabledProperty); }
            set { SetValue(ToggleIsEnabledProperty, value); }
        }

        public static DependencyProperty HandleRoutedProperty = DependencyProperty.Register("HandleRouted", typeof(HowHandleRouted), typeof(EventToCommand), new PropertyMetadata(HowHandleRouted.HandleAfter));
        /// <summary>
        /// Default HowHandleRouted.HandleAfter
        /// </summary>
        public HowHandleRouted HandleRouted
        {
            get { return (HowHandleRouted)GetValue(HandleRoutedProperty); }
            set { SetValue(HandleRoutedProperty, value); }
        }

        public static DependencyProperty PassEventArgsProperty = DependencyProperty.Register("PassEventArgs", typeof(bool), typeof(EventToCommand), new PropertyMetadata(false));
        /// <summary>
        /// Default false
        /// </summary>
        public bool PassEventArgs
        {
            get { return (bool)GetValue(PassEventArgsProperty); }
            set { SetValue(PassEventArgsProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            CheckCanExecute();
        }
        protected override void Invoke(object parametr)
        {
            CheckCanExecute();
            FrameworkElement element = GetAssociatedObjectAsFrameworkElement();
            if (element == null || !element.IsEnabled/* || Command == null*/) return;

            object passParam = CommandParametr;
            if (passParam == null && PassEventArgs) passParam = parametr;

            if (HandleRouted == HowHandleRouted.HandleBefore) TryHandleRoutedEvent(parametr);

            if (Command != null) Command.Execute(passParam);

            if (HandleRouted == HowHandleRouted.HandleAfter) TryHandleRoutedEvent(parametr);
        }

        private void TryHandleRoutedEvent(object parametr)
        {
            RoutedEventArgs routedEa = parametr as RoutedEventArgs;
            if (routedEa != null) routedEa.Handled = true;
        }
        private void CheckCanExecute()
        {
            FrameworkElement element = GetAssociatedObjectAsFrameworkElement();
            if (ToggleIsEnabled && element != null && Command != null)
            {
                element.IsEnabled = Command.CanExecute(CommandParametr);
            }
        }
        private FrameworkElement GetAssociatedObjectAsFrameworkElement()
        {
            return AssociatedObject as FrameworkElement;
        }

        private void newCommand_CanExecuteChanged(object sender, EventArgs e)
        {
            CheckCanExecute();
        }
    }
}
