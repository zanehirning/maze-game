using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment_2___Maze_Game
{
    public class Character
    {
        public int x { get; set; }
        public int y { get; set; }
        public Character(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
