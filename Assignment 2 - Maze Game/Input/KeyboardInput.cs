using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Assignment_2___Maze_Game.Input
{
    public class KeyboardInput:IInputDevice
    {
        public void registerCommand(Keys key, bool keyPressOnly, IInputDevice.CommandDelegate callback)
        {
            if (m_commandEntries.ContainsKey(key))
            {
                m_commandEntries.Remove(key);
            }
            m_commandEntries.Add(key, new CommandEntry(key, keyPressOnly, callback));
        }

        public void registerGameCommand(Keys key, bool keyPressOnly, IInputDevice.GameDelegate callback, int param)
        {
            if (m_commandEntries.ContainsKey(key))
            {
                m_commandEntries.Remove(key);
            }
            m_commandEntries.Add(key, new CommandEntry(key, keyPressOnly, callback, param));
        }

        private Dictionary<Keys, CommandEntry> m_commandEntries = new Dictionary<Keys, CommandEntry>();

        private struct CommandEntry
        {
            public CommandEntry(Keys key, bool keyPressOnly, IInputDevice.CommandDelegate callback)
            {
                this.key = key;
                this.keyPressOnly = keyPressOnly;
                this.gameCallback = null;
                this.callback = callback;
                this.param = 0; // not used or needed
            }

            public CommandEntry(Keys key, bool keyPressOnly, IInputDevice.GameDelegate callback, int param)
            {
                this.key = key;
                this.keyPressOnly = keyPressOnly;
                this.callback = null;
                this.gameCallback = callback;
                this.param = param;
            }

            public Keys key;
            public bool keyPressOnly;
            public IInputDevice.CommandDelegate? callback;
            public IInputDevice.GameDelegate? gameCallback;
            public int param;
        }

        public void Update()
        {
            KeyboardState state = Keyboard.GetState();
            foreach (CommandEntry entry in this.m_commandEntries.Values)
            {
                if (entry.keyPressOnly && keyPressed(entry.key))
                {
                    if (entry.gameCallback != null)
                    {
                        entry.gameCallback(entry.param);
                    }
                    else
                    {
                        entry.callback();
                    }
                }
                else if (!entry.keyPressOnly && state.IsKeyDown(entry.key))
                {
                    if (entry.gameCallback != null)
                    {
                        entry.gameCallback(entry.param);
                    }
                    else
                    {
                        entry.callback();
                    }
                }
            }
            m_statePrevious = state;
        }

        private KeyboardState m_statePrevious;

        private bool keyPressed(Keys key)
        {
            return (Keyboard.GetState().IsKeyDown(key) && !m_statePrevious.IsKeyDown(key));
        }
    }
}
