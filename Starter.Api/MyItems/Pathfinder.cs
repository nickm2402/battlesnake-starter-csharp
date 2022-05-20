using Starter.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pathfinding
{
    public class Pathfinder
    {
        private Dictionary<Point, int> m_fscores;
        private Dictionary<Point, int> m_gscores;

        private Dictionary<Point, Point> m_parents;

        public List<Point> FindPath(Point start, Point target, Board gameBoard)
        {
            List<Point> open = new();
            m_fscores = new();
            m_gscores = new();
            m_parents = new();

            m_fscores.Add(start, start.DistanceTo(target));
            m_gscores.Add(start, 0);
            open.Add(start);

            while (open.Count > 0)
            {
                Point current = BestPoint(open);
                if (current == target)
                {
                    List<Point> path = GetPath(current);
                    path.RemoveAt(0);
                    path.Add(target);
                    return path;
                }

                open.Remove(current);

                List<Point> directions = new List<Point> { new Point(-1, 0), new Point(1, 0), new Point(0, -1), new Point(0, 1) };
                Random rand = new Random();
                directions = directions.OrderBy(x => rand.Next()).ToList();
                foreach (Point dir in directions)
                {
                    Point neighbor = current + dir;
                    if (!gameBoard.IsWalkable(neighbor))
                    {
                        continue;
                    }
                    int tgscore = GetGScore(current) + current.DistanceTo(neighbor);
                    if (tgscore < GetGScore(neighbor))
                    {
                        m_parents[neighbor] = current;
                        m_gscores[neighbor] = tgscore;
                        m_fscores[neighbor] = tgscore + neighbor.DistanceTo(target);
                        if (!open.Contains(neighbor))
                        {
                            open.Add(neighbor);
                            Console.WriteLine("added " + neighbor.X + " " + neighbor.Y);
                        }
                    }
                }
            }

            return null;
        }

        private List<Point> GetPath(Point current)
        {
            List<Point> path = new();

            while (m_parents.ContainsKey(current))
            {
                current = m_parents.GetValueOrDefault(current);
                path = path.Prepend(current).ToList();
            }
            return path;
        }

        private Point BestPoint(List<Point> possibles)
        {
            Point minPoint = null;
            int minScores = 10000000;

            foreach (Point possibility in possibles)
            {
                int score = GetFScore(possibility);
                if (score < minScores)
                {
                    minPoint = possibility;
                    minScores = score;
                }
            }

            if (minPoint == null)
            {
                throw new System.Exception("Couldn't find best point");
            }

            return minPoint;
        }

        private int GetFScore(Point p)
        {
            if (m_fscores.ContainsKey(p))
            {
                return m_fscores.GetValueOrDefault(p);
            }
            else
            {
                int d = 10000000;
                m_fscores.Add(p, d);
                return d;
            }
        }

        private int GetGScore(Point p)
        {
            if (m_gscores.ContainsKey(p))
            {
                return m_gscores.GetValueOrDefault(p);
            }
            else
            {
                int d = 10000000;
                m_gscores.Add(p, d);
                return d;
            }
        }
    }
}