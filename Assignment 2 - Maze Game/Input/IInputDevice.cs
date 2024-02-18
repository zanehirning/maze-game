using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Assignment_2___Maze_Game.Input
{
    public interface IInputDevice
    {
        //Code from lecture for input interface
        public delegate void CommandDelegate();
        public delegate void GameDelegate(int dimension);
        public delegate void CommandDelegatePosition(GameTime GameTime, int x, int y);

        void Update();
    }
}
