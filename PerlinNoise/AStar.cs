using System;
using System.Collections.Generic;
using System.Linq;

namespace PerlinNoise
{
	public class AStar
	{
		public static Tuple<List<Node>, int> findPath(int[,] map, int x, int y, int endX, int endY)
		{
			List<Node> returnList = new List<Node>();
			List<Node> openList = new List<Node>();
			List<Node> closedList = new List<Node>();

			int numTiles = map.GetLength(0) * map.GetLength(1);
			int cost = 0;
			bool found = false;
			Node start = new Node(x, y, map[y, x], 0);
			start.calculateScores(endX, endY);
			Node current = null;

			// start by adding the original position to the open list
			openList.Add(start);

			while (openList.Count > 0)
			{
				// get the square with the lowest F score
				var lowest = openList.Min(l => l.getF());
				current = openList.First(l => l.getF() == lowest);

				// add the current square to the closed list
				closedList.Add(current);

				// show current square on the map
				Console.SetCursorPosition(current.getX(), current.getY());
				Console.Write('.');
				Console.SetCursorPosition(current.getX(), current.getY());
				System.Threading.Thread.Sleep(50);

				// remove it from the open list
				openList.Remove(current);

				// if we added the destination to the closed list, we've found a path
				if (closedList.FirstOrDefault(l => l.getX() == endX && l.getY() == endY) != null)
				{
					cost = current.getG();
					found = true;
					break;
				}

				List<Node> neighbors = getNeighbors(map, current);
				foreach (Node neighbor in neighbors)
				{
					neighbor.calculateScores(endX, endY);
				}

				foreach (Node neighbor in neighbors)
				{
					// if this adjacent square is already in the closed list, ignore it
					if (closedList.FirstOrDefault(l => l.getX() == neighbor.getX()
												  && l.getY() == neighbor.getY()) != null)
						continue;

					// if it's not in the open list...
					if (openList.FirstOrDefault(l => l.getX() == neighbor.getX()
												&& l.getY() == neighbor.getY()) == null)
					{
						// compute its score, set the parent
						neighbor.setParent(current);

						// and add it to the open list
						openList.Insert(0, neighbor);
					}
					else
					{
						// test if using the current G score makes the adjacent square's F score
						// lower, if yes update the parent because it means it's a better path
						current.calculateScores(endX, endY);
						if (current.getG() + neighbor.getH() < neighbor.getF())
						{
							neighbor.setParent(current);
						}
					}
				}
			}

			// assume path was found; let's show it
			while (current != null)
			{
				if (found == true)
				{
					Console.SetCursorPosition(current.getX(), current.getY());
					Console.Write('_');
					Console.SetCursorPosition(current.getX(), current.getY());
					returnList.Add(current);
					current = current.getParent();
					System.Threading.Thread.Sleep(50);
				}
				else
				{
					Console.Write("Not Found");
					return null;
				}
			}

			// end

			return Tuple.Create(returnList, cost);
		}

		public static List<Node> getNeighbors(int[,] map, Node node)
		{
			List<Node> neighbors = new List<Node>();

			if (isValidPoint(map, node.getX() - 1, node.getY()))
			{
				neighbors.Add(new Node(node.getX() - 1, node.getY(), map[node.getY(), node.getX() - 1], node.getG()));
			}

			if (isValidPoint(map, node.getX() + 1, node.getY()))
			{
				neighbors.Add(new Node(node.getX() + 1, node.getY(), map[node.getY(), node.getX() + 1], node.getG()));
			}

			if (isValidPoint(map, node.getX(), node.getY() - 1))
			{
				neighbors.Add(new Node(node.getX(), node.getY() - 1, map[node.getY() - 1, node.getX()], node.getG()));
			}

			if (isValidPoint(map, node.getX(), node.getY() + 1))
			{
				neighbors.Add(new Node(node.getX(), node.getY() + 1, map[node.getY() + 1, node.getX()], node.getG()));
			}

			return neighbors;
		}

		public static bool isValidPoint(int[,] map, int x, int y)
		{
			return !(x < 0 || x >= map.GetLength(0) || y < 0 || y >= map.GetLength(1)) && (map[y, x] != 0);
		}
	}

	public class Node
	{
		private int x = -1;
		private int y = -1;
		private int difficulty = -1;
		private int g = -1;
		private int h = -1;
		private int f = -1;
		private Node parent = null;

		public Node(int x, int y, int difficulty, int g)
		{
			this.x = x;
			this.y = y;
			this.difficulty = difficulty;
			this.g = g + difficulty;
		}

		private void calculateF()
		{
			f = g + h;
		}

		public void calculateScores(int targetX, int targetY)
		{
			calculateH(targetX, targetY);
			calculateF();
		}

		private void calculateH(int targetX, int targetY)
		{
			h = Math.Abs(targetX - x) + Math.Abs(targetY - y);
		}

		// Getters and Setters

		public int getX()
		{
			return x;
		}

		public int getY()
		{
			return y;
		}

		public int getDifficulty()
		{
			return difficulty;
		}

		public int getG()
		{
			return g;
		}

		public int getH()
		{
			return h;
		}

		public int getF()
		{
			return f;
		}

		public Node getParent()
		{
			return parent;
		}

		public void setParent(Node parent)
		{
			this.parent = parent;
		}
	}
}