using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Face
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            RegisterHotkeys();
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            HandleHotkey(e, HotkeyHandler.PreviewKeyDown);
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            HandleHotkey(e, HotkeyHandler.KeyDown);

            if (e.Key == Key.Escape)
            {
                // do something
            }
        }

        private void HandleHotkey(KeyEventArgs e, HotkeyHandler handler)
        {
            if (e.IsRepeat) return;

            try
            {
                KeyGesture gesture = new KeyGesture(e.Key, Keyboard.Modifiers);
                e.Handled = Hotkey.Invoke(gesture, handler);
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(this, ex.ToString());
            }
        }

        private void RegisterHotkeys()
        {
            Hotkey hk = Hotkey.Register(Key.F1, "Help", topMenu.HelpMenuViewHelp_Click, HotkeyGroup.General, HotkeyHandler.PreviewKeyDown);
            topMenu.helpMenuViewHelp.InputGestureText = hk.ToString();
            hk = Hotkey.Register(Key.S, ModifierKeys.Control, "Save", topMenu.FileMenuSave_Click, HotkeyGroup.General, HotkeyHandler.PreviewKeyDown);
            topMenu.fileMenuSave.InputGestureText = hk.ToString();
            hk = Hotkey.Register(Key.O, ModifierKeys.Control, "Open", topMenu.FileMenuOpen_Click, HotkeyGroup.General, HotkeyHandler.PreviewKeyDown);
            topMenu.fileMenuOpen.InputGestureText = hk.ToString();
        }
    }
}
