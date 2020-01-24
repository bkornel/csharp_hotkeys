using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Face
{
    using MyAction = Action<object, RoutedEventArgs>;

    public enum HotkeyGroup
    {
        General,
        CameraView,
    }

    public enum HotkeyHandler
    {
        PreviewKeyDown,
        KeyDown
    }

    public class Hotkey
    {
        public static readonly RoutedEvent HotKeyEvent = EventManager.RegisterRoutedEvent("HotKeyEvent", RoutingStrategy.Direct, typeof(MyAction), typeof(Hotkey));
        public static Dictionary<int, Hotkey> HotkeyMap = new Dictionary<int, Hotkey>();

        public int Id { get; private set; }
        public string Description { get; private set; }
        public KeyGesture KeyGesture { get; private set; }
        public MyAction Action { get; private set; }
        public HotkeyGroup Group { get; private set; }
        public HotkeyHandler Handler { get; private set; }

        public Hotkey(KeyGesture keyGesture, string description, MyAction action, HotkeyGroup group, HotkeyHandler handler)
        {
            KeyGesture = keyGesture;
            Description = description;
            Action = action;
            Group = group;
            Handler = handler;

            int virtualKeyCode = KeyInterop.VirtualKeyFromKey(KeyGesture.Key);
            Id = virtualKeyCode + ((int)KeyGesture.Modifiers * 0x10000);
        }

        public Hotkey(Key key, string description, MyAction action, HotkeyGroup group, HotkeyHandler handler) :
           this(new KeyGesture(key), description, action, group, handler)
        {
        }

        public Hotkey(Key key, ModifierKeys modifierKeys, string description, MyAction action, HotkeyGroup group, HotkeyHandler handler) :
            this(new KeyGesture(key, modifierKeys), description, action, group, handler)
        {
        }

        public static Hotkey Register(Hotkey hotkey)
        {
            HotkeyMap.Add(hotkey.Id, hotkey);
            return hotkey;
        }

        public static Hotkey Register(KeyGesture keyGesture, string description, MyAction action, HotkeyGroup group, HotkeyHandler handler)
        {
            return Register(new Hotkey(keyGesture, description, action, group, handler));
        }

        public static Hotkey Register(Key key, string description, MyAction action, HotkeyGroup group, HotkeyHandler handler)
        {
            return Register(new Hotkey(key, description, action, group, handler));
        }

        public static Hotkey Register(Key key, ModifierKeys modifierKeys, string description, MyAction action, HotkeyGroup group, HotkeyHandler handler)
        {
            return Register(new Hotkey(key, modifierKeys, description, action, group, handler));
        }

        public static bool Invoke(KeyGesture keyGesture, HotkeyHandler handler)
        {
            foreach (var hk in HotkeyMap)
            {
                var kg = hk.Value.KeyGesture;
                if (kg.Key == keyGesture.Key && kg.Modifiers == keyGesture.Modifiers && handler == hk.Value.Handler)
                    return hk.Value.Invoke();
            }

            return false;
        }

        public bool Invoke()
        {
            RoutedEventArgs e = new RoutedEventArgs(HotKeyEvent);
            Action?.Invoke(this, e);
            return e.Handled;
        }

        public string KeyToString()
        {
            return KeyGesture.Key.ToString();
        }

        public string ModifiersToString()
        {
            string modifiersStr = string.Empty;
            if (KeyGesture.Modifiers == ModifierKeys.None) return modifiersStr;

            int value = (int)KeyGesture.Modifiers;
            foreach (ModifierKeys mk in Enum.GetValues(typeof(ModifierKeys)))
            {
                if (mk == ModifierKeys.None) continue;

                int checkbit = (int)mk;
                if ((value & checkbit) == checkbit)
                {
                    if (!string.IsNullOrWhiteSpace(modifiersStr))
                        modifiersStr += " + ";

                    modifiersStr += mk.ToString();
                }
            }

            return modifiersStr;
        }

        public override string ToString()
        {
            string ms = ModifiersToString();
            string ks = KeyToString();

            return string.IsNullOrWhiteSpace(ms) ? KeyToString() : ms + " + " + ks;
        }
    }
}
