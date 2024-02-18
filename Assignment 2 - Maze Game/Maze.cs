using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Assignment_2___Maze_Game.Maze;

namespace Assignment_2___Maze_Game
{

    public class Maze
    {
        public class Cell
        {
            public int x { get; set; }
            public int y { get; set; }
            public Cell? topNeighbor { get; set; }
            public Cell? leftNeighbor { get; set; }
            public Cell? bottomNeighbor { get; set; }
            public Cell? rightNeighbor { get; set; }
            public bool visited { get; set; }
            public Cell? prevCell { get; set; }

            public Cell(int x, int y, Cell topNeighbor, Cell leftNeighbor, Cell bottomNeighbor, Cell rightNeighbor)
            {
                this.x = x;
                this.y = y;
                this.topNeighbor = topNeighbor;
                this.leftNeighbor = leftNeighbor;
                this.bottomNeighbor = bottomNeighbor;
                this.rightNeighbor = rightNeighbor;
            }

            public bool hasConnection()
            {
                return this.topNeighbor != null || this.leftNeighbor != null || this.rightNeighbor != null || this.bottomNeighbor != null;
            }
        }

        //variables
        private List<Cell> _frontier = new List<Cell>();
        private List<List<Cell>> _cells = new List<List<Cell>>();
        public List<List<Cell>> _maze = new List<List<Cell>>();

        public Maze(int dimension)
        {
            //Initialize the original maze state
            for (int row = 0; row < dimension; row++)
            {
                _cells.Add(new List<Cell>());
                _maze.Add(new List<Cell>());
                for (int col = 0; col < dimension; col++)
                {
                    Cell cell = new Cell(row, col, null, null, null, null);
                    _cells[row].Add(cell);
                    _maze[row].Add(null);
                }
            }
            primGeneration();
        }

        private void setNeighbors(Cell currentCell, Cell frontierCell)
        {
            if (currentCell.x == frontierCell.x - 1)
            {
                currentCell.bottomNeighbor = frontierCell;
                frontierCell.topNeighbor = currentCell;
            }

            if (currentCell.x == frontierCell.x + 1)
            {
                currentCell.topNeighbor = frontierCell;
                frontierCell.bottomNeighbor = currentCell;
            }

            if (currentCell.y == frontierCell.y - 1)
            {
                currentCell.rightNeighbor = frontierCell;
                frontierCell.leftNeighbor = currentCell;
            }

            if (currentCell.y == frontierCell.y + 1)
            {
                currentCell.leftNeighbor = frontierCell;
                frontierCell.rightNeighbor = currentCell;
            }
        }

        private List<Cell> getCellsInMaze()
        {
            List<Cell> cellsInMaze = new List<Cell>();
            for (int row = 0; row < _maze.Count; row++)
            {
                for (int col = 0; col < _maze.Count; col++)
                {
                    if (_maze[row][col] != null)
                    {
                        cellsInMaze.Add(_maze[row][col]);
                    }
                }
            }
            return cellsInMaze;
        }

        #region Getting neighbors on different conditions
        private List<Cell> getNeighbors(Cell currentCell)
        {
            List<Cell> neighbors = new List<Cell>();
            if (currentCell.x != 0)
            {
                neighbors.Add(_cells[currentCell.x - 1][currentCell.y]);
            }
            if (currentCell.y != 0)
            {
                neighbors.Add(_cells[currentCell.x][currentCell.y - 1]);
            }
            if (currentCell.x != _cells.Count - 1)
            {
                neighbors.Add(_cells[currentCell.x + 1][currentCell.y]);
            }
            if (currentCell.y != _cells[0].Count - 1)
            {
                neighbors.Add(_cells[currentCell.x][currentCell.y + 1]);
            }
            return neighbors;
        }

        private List<Cell> getNeighborsInMaze(Cell currentCell)
        {
            List<Cell> neighbors = new List<Cell>();
            if (currentCell.x != 0 && _maze[currentCell.x - 1][currentCell.y] == _cells[currentCell.x - 1][currentCell.y])
            {
                neighbors.Add(_cells[currentCell.x - 1][currentCell.y]);
            } 
            if (currentCell.y != 0 && _maze[currentCell.x][currentCell.y-1] == _cells[currentCell.x][currentCell.y-1])
            {
                neighbors.Add(_cells[currentCell.x][currentCell.y - 1]);
            }
            if (currentCell.x != _cells.Count - 1 && _maze[currentCell.x + 1][currentCell.y] == _cells[currentCell.x + 1][currentCell.y]) 
            {
                neighbors.Add(_cells[currentCell.x + 1][currentCell.y]);
            }
            if (currentCell.y != _cells[0].Count - 1 && _maze[currentCell.x][currentCell.y + 1] == _cells[currentCell.x][currentCell.y + 1])
            {
                neighbors.Add(_cells[currentCell.x][currentCell.y + 1]);
            }
            return neighbors;
        }

        private List<Cell> getNeighborsWithPath(Cell cell)
        {
            List<Cell> neighbors = new List<Cell>();
            if (cell.topNeighbor != null)
            {
                neighbors.Add(_maze[cell.x - 1][cell.y]);
            }
            if (cell.leftNeighbor != null)
            {
                neighbors.Add(_maze[cell.x][cell.y - 1]);
            }
            if (cell.bottomNeighbor != null)
            {
                neighbors.Add(_maze[cell.x + 1][cell.y]);
            }
            if (cell.rightNeighbor != null)
            {
                neighbors.Add(_maze[cell.x][cell.y + 1]);
            }
            return neighbors;
        }
        #endregion

        private void primGeneration()
        {
            //Initialize frontier with the starting spot's neighbors (right and bottom)
            _frontier.Add(_cells[1][0]);
            _frontier.Add(_cells[0][1]);

            Random rnd = new Random();
            //add first to maze
            _maze[0][0] = _cells[0][0];
            while (_frontier.Count != 0)
            {
                // random from frontier, add to maze, set neighbors
                Cell randomFrontierCell = _frontier[rnd.Next(_frontier.Count)];
                _frontier.Remove(randomFrontierCell);
                _maze[randomFrontierCell.x][randomFrontierCell.y] = randomFrontierCell;

                //randomly choose frontiers neighbors that are in the maze and set the neighbors with the chosen cell
                List<Cell> neighbors = getNeighborsInMaze(randomFrontierCell);
                Cell chosenCell = neighbors[rnd.Next(neighbors.Count)];
                setNeighbors(chosenCell, randomFrontierCell);

                //update frontier
                List<Cell> cellsInMaze = getCellsInMaze();
                foreach (Cell cell in getNeighbors(randomFrontierCell))
                {
                    if (!_frontier.Contains(cell) && !cellsInMaze.Contains(cell))
                    {
                        _frontier.Add(cell);
                    }
                }
            }
        }

        public Stack<Cell> dfs()
        {
            Stack<Cell> stack = new Stack<Cell>();
            //Push start cell
            stack.Push(_maze[0][0]);
            
            while (stack.Count != 0)
            {
                Cell currentCell = stack.Pop();
                
                if (currentCell.x == _maze.Count - 1 && currentCell.y == _maze[0].Count - 1)
                {
                    return getDFSPath(currentCell);
                }

                //Mark cell as visited
                currentCell.visited = true;
                foreach (Cell neighbor in getNeighborsWithPath(currentCell))
                {
                    if (!neighbor.visited)
                    {
                        neighbor.prevCell = currentCell;
                        stack.Push(neighbor);
                    }
                }
            }
            //There exists no path. This should never occur, but it will satisfy any errors
            return null;
        }

        public Stack<Cell> getDFSPath(Cell lastCell)
        {
            Stack<Cell> stack = new Stack<Cell>();
            var currentCell = lastCell;

            while (currentCell != null)
            {
                stack.Push(currentCell);
                //reset visited
                currentCell.visited = false;
                Cell prevCell = currentCell.prevCell;
                currentCell = prevCell;
            }
            stack.Pop();
            return stack;
        }

        public List<List<Cell>> getMaze()
        {
            return _maze;
        }
    }
}
