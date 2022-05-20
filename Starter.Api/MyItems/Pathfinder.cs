using Starter.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pathfinder
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
            open.Add(start);

            while (open.Count > 0)
            {
                Point current = BestPoint(open);
                if (current == target)
                {
                    return GetPath(current);
                }

                open.Remove(current);

                List<Point> directions = new List<Point> { new Point(-1, -1), new Point(-1, 1), new Point(1, -1), new Point(1, 1) };
                foreach (Point dir in directions)
                {
                    Point neighbor = current + dir;
                    int tgscore = GetGScore(current) + current.DistanceTo(neighbor);
                    if (tgscore > GetGScore(neighbor))
                    {
                        m_parents.Add(neighbor, current);
                        m_gscores.Add(neighbor, tgscore);
                        m_fscores.Add(neighbor, tgscore + neighbor.DistanceTo(target));
                        if (!open.Contains(neighbor))
                        {
                            open.Add(neighbor);
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
                path.Prepend(current);
            }

            return path;
        }

        private Point BestPoint(List<Point> possibles)
        {
            Point minPoint = null;
            int minScores = int.MaxValue;

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
                int d = int.MaxValue;
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
                int d = int.MaxValue;
                m_gscores.Add(p, d);
                return d;
            }
        }
    }
}