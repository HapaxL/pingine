using System.Collections.Generic;
using OpenTK.Input;

namespace pingine.Game.Handlers
{
    /* OpenTK's KeyboardState does not work (keyboard always disconnected).
     * I use this handler to circumvent this issue and have my own KeyboardState functionality */
    public class KeyboardHandler
    {
        /* represents the state of a key during an update cycle */
        public class KeyState
        {
            /* key is being held */
            public bool IsHeld { get; set; }
            /* key has been triggered somewhere during this update cycle */
            public bool IsTriggered { get; set; }
            /* key has been released somewhere during this update cycle */
            public bool IsReleased { get; set; }
            /* key was triggered at some point but hasn't been released yet */
            public bool WasTriggeredButNotReleased { get; set; }

            public KeyState()
            {
                IsHeld = false;
                IsTriggered = false;
                IsReleased = false;
                WasTriggeredButNotReleased = false;
            }
        }

        /* enables repeating key triggers after they have been pressed for a while (like when writing) */
        bool RepeatEnabled;
        /* all key states */
        IDictionary<Key, KeyState> KeyStates;

        public KeyboardHandler(bool EnableRepeat)
        {
            RepeatEnabled = EnableRepeat;
            KeyStates = new Dictionary<Key, KeyState>();
        }
        
        public void AddTriggerEvent(Key k)
        {
            var ks = GetKeyState(k);

            /* we don't want the key to be triggered again if repeat is disabled */
            if (RepeatEnabled || !ks.WasTriggeredButNotReleased)
            {
                ks.IsTriggered = true;
                ks.WasTriggeredButNotReleased = true;
                ks.IsHeld = true;
            }
        }

        public void AddReleaseEvent(Key k)
        {
            var ks = GetKeyState(k);
            ks.IsReleased = true;
            ks.WasTriggeredButNotReleased = false;
            ks.IsHeld = false;
        }

        /* reset all key trigger/release events */
        public void ResetKeys()
        {
            foreach (KeyState ks in KeyStates.Values)
            {
                ks.IsTriggered = false;
                ks.IsReleased = false;
            }
        }

        public bool IsHeld(Key k)
        {
            var ks = GetKeyState(k);
            return ks.IsHeld;
        }

        public bool IsTriggered(Key k)
        {
            var ks = GetKeyState(k);
            return ks.IsTriggered;
        }

        public bool IsReleased(Key k)
        {
            var ks = GetKeyState(k);
            return ks.IsReleased;
        }

        public bool RepeatIsEnabled()
        {
            return RepeatEnabled;
        }

        public void EnableRepeat(bool EnableRepeat)
        {
            RepeatEnabled = EnableRepeat;
        }

        /* get a key's state from the dictionary, or adds it if there wasn't one */
        private KeyState GetKeyState(Key k)
        {
            if (!KeyStates.ContainsKey(k))
            {
                KeyStates.Add(k, new KeyState());
            }

            return KeyStates[k];
        }
    }
}