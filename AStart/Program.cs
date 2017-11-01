using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AStart
{
    class Program
    {
        static void Main(string[] args)
        {
            int arraySize;
            while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
            {
                Console.WriteLine("Please put the size of path:");
                var sizeStr = Console.ReadLine();
                if (!int.TryParse(sizeStr, out arraySize))
                    arraySize = 16;
                int[,] path = new int[arraySize, arraySize];
                var rand = new Random();
                for (int i = 0; i < arraySize; i++)
                    for (int j = 0; j < arraySize; j++)
                    {
                        if (rand.Next(10) <= 2)
                            path[i, j] = 1;
                        else
                            path[i, j] = 0;
                    }
                string[] s;
                int x1 = 0, y1 = 0, x2 = arraySize - 1, y2 = arraySize - 1;
                PrintPath(arraySize, path);
                Console.WriteLine("Please press the position of start(split by ','):");
                s = Console.ReadLine().Split(',');
                if (s.Length != 2)
                {
                    Console.WriteLine("press wrong, the position set (0,0)");
                }
                else
                {
                    int.TryParse(s[0], out x1);
                    int.TryParse(s[1], out y1);
                }
                path[x1, y1] = 100;
                Console.WriteLine("Please press the position of start(split by ','):");
                s = Console.ReadLine().Split(',');
                if (s.Length != 2)
                {
                    Console.WriteLine("press wrong, the position set ({0},{0})", arraySize - 1);
                }
                else
                {
                    int.TryParse(s[0], out x2);
                    int.TryParse(s[1], out y2);
                }
                path[x2, y2] = -100;
                Console.WriteLine("the start and end is:");
                PrintPath(arraySize, path);
                /******************利用A星算法计算可行路径************************/
                List<Node> openList = new List<Node>();
                List<Node> closeList = new List<Node>();
                int[] offsetX = new int[] { 1, 1, 0, -1, -1, -1, 0, 1 };
                int[] offsetY = new int[] { 0, 1, 1, 1, 0, -1, -1, -1 };
                Node startNode = new Node() { X = x1, Y = y1, G = 0, H = HDistance(x1, y1, x2, y2), Parent = null };
                openList.Add(startNode);
                while (true)
                {
                    if (openList == null || openList.Count <= 0)
                    {
                        Console.WriteLine("can't find the way");
                        break;
                    }
                    Node endNode = closeList.FirstOrDefault(n => n.X == x2 && n.Y == y2);
                    //如果closeList中含有终点则路径规划完毕
                    if (endNode != null)
                    {
                        Node preNode = endNode;
                        while (true)
                        {
                            if (preNode.Parent == null)
                            {
                                path[preNode.X, preNode.Y] = 100;
                                break;
                            }
                            preNode = preNode.Parent;
                            //50表明为规划路径
                            path[preNode.X, preNode.Y] = 50;
                        }
                        Console.WriteLine("find the way!");
                        PrintPath(arraySize, path);
                        break;
                    }
                    var minF = openList.Min(n => n.F);
                    var minNode = openList.Where(n => n.F == minF).Last();
                    openList.Remove(minNode);
                    closeList.Add(minNode);
                    for (int i = 0; i < 8; i++)
                    {
                        int nearX = minNode.X + offsetX[i];
                        int nearY = minNode.Y + offsetY[i];
                        //相邻点越界
                        if (nearX < 0 || nearX >= arraySize || nearY < 0 || nearY >= arraySize)
                            continue;
                        //如果为墙则略过
                        if (path[nearX, nearY] == 1)
                            continue;
                        //如果为墙角也略过
                        if (Math.Abs(offsetX[i]) + Math.Abs(offsetY[i]) == 2 && (path[minNode.X, nearY] == 1 || path[nearX, minNode.Y] == 1))
                            continue;
                        //如果在关闭列表中略过
                        if (closeList.Find(n => n.X == nearX && n.Y == nearY) != null)
                            continue;
                        Node node = openList.FirstOrDefault(n => n.X == nearX && n.Y == nearY);
                        if (node == null)
                        {
                            //垂直走和沿对角走所费距离是不一样的（对角是垂直的1.4倍）
                            int g = Math.Abs(offsetX[i]) + Math.Abs(offsetY[i]) == 2 ? 14 : 10;
                            int h = HDistance(nearX, nearY, x2, y2);
                            Node newNode = new Node() { X = nearX, Y = nearY, G = g, H = h, Parent = minNode };
                            openList.Add(newNode);
                        }
                        else
                        {
                            int g = Math.Abs(offsetX[i]) + Math.Abs(offsetY[i]) == 2 ? minNode.G + 14 : minNode.G + 10;
                            if (g < node.G)
                            {
                                node.G = g;
                                node.Parent = minNode;
                            }
                        }
                    }
                }
                Console.ReadLine();
            }
        }
        //打印路径
        static void PrintPath(int arraySize, int[,] path)
        {
            for (int i = 0; i < arraySize; i++)
            {
                for (int j = 0; j < arraySize; j++)
                {
                    Console.Write(' ');
                    if (path[i, j] == 1)
                        Console.Write('#');
                    else if (path[i, j] == 100)
                        Console.Write('S');
                    else if (path[i, j] == -100)
                        Console.Write('E');
                    else if (path[i, j] == 50)
                        Console.Write('$');
                    else
                        Console.Write('*');
                }
                Console.Write('\n');
            }
        }
        //计算曼哈顿距离
        static int HDistance(int x1, int y1, int x2, int y2)
        {
            return Math.Abs(x2 - x1) + Math.Abs(y2 - y1);
        }

    }

    class Node
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int G { get; set; }
        public int H { get; set; }
        public int F { get { return G + H; } }
        public Node Parent { get; set; }
    }

}
