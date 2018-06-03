using System.Windows;
using System.Windows.Input;
using Microsoft.VisualStudio.Shell;

namespace SeeSharper.Options
{
    public partial class OptionsControl
    {
        public OptionsControl()
        {
            InitializeComponent();
            DimPatterns.AddHandler(UIElementDialogPage.DialogKeyPendingEvent, new RoutedEventHandler(OnDialogKeyPendingEvent));
        }

        private static void OnDialogKeyPendingEvent(object sender, RoutedEventArgs e)
        {
            if (e is DialogKeyEventArgs a && (a.Key == Key.Enter || a.Key == Key.Return))
            {
                a.Handled = true;
            }
        }
    }
}
