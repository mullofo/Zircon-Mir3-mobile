using Client.Envir;
using Client.Scenes.Views;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Client.Models
{
    /// <summary>
    /// ·��������
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Heap<T> where T : IHeapItem<T>
    {
        private T[] items;

        public Heap(int maxHeapSize)
        {
            items = new T[maxHeapSize];
        }

        public void Add(T item)
        {
            item.HeapIndex = Count;
            items[Count] = item;
            SortUp(item);
            Count++;
        }

        public T RemoveFirst()
        {
            T firstItem = items[0];
            Count--;

            items[0] = items[Count];
            items[0].HeapIndex = 0;

            SortDown(items[0]);

            return firstItem;
        }

        public void UpdateItem(T item)
        {
            SortUp(item);
        }

        public int Count { get; private set; }

        public bool Contains(T item)
        {
            return Equals(items[item.HeapIndex], item);
        }

        private void SortDown(T item)
        {
            while (true)
            {
                int childIndexLeft = (item.HeapIndex * 2) + 1;
                int childIndexRight = (item.HeapIndex * 2) + 2;
                int swapIndex = 0;

                if (childIndexLeft < Count)
                {
                    swapIndex = childIndexLeft;
                    if (childIndexRight < Count)
                    {
                        if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                        {
                            swapIndex = childIndexRight;
                        }
                    }

                    if (item.CompareTo(items[swapIndex]) < 0)
                    {
                        Swap(item, items[swapIndex]);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }

        //�����������Ҫ����
        private void SortUp(T item)
        {
            int parentIndex = (item.HeapIndex - 1) / 2;

            while (true)
            {
                T parentItem = items[parentIndex];
                if (item.CompareTo(parentItem) > 0)
                {
                    Swap(item, parentItem);
                }
                else
                    break;

                parentIndex = (item.HeapIndex - 1) / 2;
            }
        }

        private void Swap(T itemA, T itemB)
        {
            items[itemA.HeapIndex] = itemB;
            items[itemB.HeapIndex] = itemA;

            int itemAIndex = itemA.HeapIndex;
            itemA.HeapIndex = itemB.HeapIndex;
            itemB.HeapIndex = itemAIndex;
        }
    }

    //�Ƚ�fCost,C#�еĻ������Ͷ��ṩ��Ĭ�ϵıȽ��㷨��C#���Ե��ñȽ��㷨Ϊ�������͵������������
    //��ϣ�����Խ�����бȽϻ�������ô����ʹ��IComparable<T> ��IComparer<T>�ӿڡ�openSet��Ҫ�õ�����������
    public interface IHeapItem<T> : IComparable<T>
    {
        int HeapIndex
        {
            get; set;
        }
    }

    public class Node : IHeapItem<Node>
    {
        //�ڵ�ռ�λ��
        public Point Location;
        //��ǰ�ڵ�ĸ��ڵ�
        public Node Parent;
        //��ʼ�ڵ㵽��ǰ�ڵ�ľ����ֵ
        public int GCost;
        //��ǰ�ڵ㵽Ŀ��ڵ�ľ����ֵ
        public int HCost;

        public int FCost
        {
            get { return GCost + HCost; }
        }

        private int _heapIndex;

        public int HeapIndex
        {
            get { return _heapIndex; }
            set { _heapIndex = value; }
        }

        //�Զ���ȽϺ���
        public int CompareTo(Node nodeToCompare)
        {
            int compare = FCost.CompareTo(nodeToCompare.FCost);
            if (compare == 0)
            {
                compare = HCost.CompareTo(nodeToCompare.HCost);
            }
            return -compare;
        }

        public Node(int x, int y)
        {
            Location = new Point(x, y);
        }
    }
    /// <summary>
    /// ·��������
    /// </summary>
    public class PathFinder
    {
        private Node[,] Grid;

        public MapControl Map;

        public bool bSearching;

        public int MaxSize
        {
            get { return Map.Width * Map.Height; }
        }

        public PathFinder(MapControl map)
        {
            Map = map;
            Grid = new Node[Map.Width, Map.Height];
        }

        /// <summary>
        /// A*�㷨��Ѱ�����·��
        /// </summary>
        /// <param name="start"></param>
        /// <param name="target"></param>
        public List<Node> FindPath(Point start, Point target)
        {
            //�����ʼ�ڵ��Ŀ��ڵ���ȣ�����
            if (start == target) return null;
            if (!Map.Cells.EmptyCell(target.X, target.Y))
                target = GetNeighbours(target);

            try
            {
                Node startNode = GetNode(start.X, start.Y);
                Node targetNode = GetNode(target.X, target.Y);

                Heap<Node> openSet = new Heap<Node>(MaxSize);
                HashSet<Node> closedSet = new HashSet<Node>();

                openSet.Add(startNode);

                while (openSet.Count > 0)
                {
                    // �ѵ�ǰ�ڵ�ӿ����б����Ƴ��������뵽�ر��б���
                    Node currentNode = openSet.RemoveFirst();
                    closedSet.Add(currentNode);

                    // �����Ŀ�Ľڵ㣬����
                    if (currentNode == targetNode)
                    {
                        return RetracePath(startNode, targetNode);
                    }
                    // ������ǰ�ڵ���������ڽڵ�
                    foreach (Node neighbor in GetNeighbours(currentNode))
                    {
                        // ����ڵ㲻��ͨ���������ڹر��б��У�����
                        if (!Map.Cells.EmptyCell(neighbor.Location.X, neighbor.Location.Y) || closedSet.Contains(neighbor))
                            continue;

                        int newMovementCostToNeighbor = currentNode.GCost + GetDistance(currentNode, neighbor);
                        // �����·�������ڵ�ľ������ ���߲��ڿ����б���
                        if (newMovementCostToNeighbor < neighbor.GCost || !openSet.Contains(neighbor))
                        {
                            // �������ڵ��F��G��H
                            neighbor.GCost = newMovementCostToNeighbor;
                            neighbor.HCost = GetDistance(neighbor, targetNode);
                            // �������ڵ�ĸ��ڵ�Ϊ��ǰ�ڵ�
                            neighbor.Parent = currentNode;
                            // ������ڿ����б��У����뵽�����б���
                            if (!openSet.Contains(neighbor))
                                openSet.Add(neighbor);
                            else
                            {
                                openSet.UpdateItem(neighbor);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //GameScene.Game.ReceiveChat("�Զ�Ѱ·���������쳣".Lang(), Library.MessageType.Normal);
                CEnvir.SaveError(ex.ToString());
            }
            return null;
        }

        /// <summary>
        /// ����·��
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="endNode"></param>
        public List<Node> RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }

            path.Add(startNode);
            path.Reverse();

            return path;
        }

        /// <summary>
        /// ��������ڵ�ľ���
        /// </summary>
        /// <param name="nodeA"></param>
        /// <param name="nodeB"></param>
        /// <returns></returns>
        private int GetDistance(Node nodeA, Node nodeB)
        {
            int distX = Math.Abs(nodeA.Location.X - nodeB.Location.X);
            int distY = Math.Abs(nodeA.Location.Y - nodeB.Location.Y);

            if (distX > distY)
                return 14 * distY + (10 * (distX - distY));

            return 14 * distX + (10 * (distY - distX));
        }

        private Node GetNode(int x, int y)
        {
            if (Grid[x, y] == null) Grid[x, y] = new Node(x, y);
            return Grid[x, y];
        }

        /// <summary>
        /// ��õ�ǰ�ڵ�����ڽڵ�
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;

                    int checkX = node.Location.X + x;
                    int checkY = node.Location.Y + y;

                    if (checkX >= 0 && checkX < Grid.GetLength(0) && checkY >= 0 && checkY < Grid.GetLength(1))
                    {
                        neighbours.Add(GetNode(checkX, checkY));
                    }
                }
            }

            return neighbours;
        }

        /// <summary>
        /// ��õ�ǰ�ڵ�����ڽڵ�
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private Point GetNeighbours(Point point)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;

                    Point emp = new Point(point.X + x, point.Y + y);
                    if (Map.Cells.EmptyCell(emp.X, emp.Y))
                        return emp;
                }
            }

            return point;
        }

    }
}
