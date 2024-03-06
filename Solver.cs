using System;
using System.Collections;
using Microsoft.Win32.SafeHandles;

namespace MazeAI;

public class Solver
{
    public int Option { get; set; }
    public Maze Maze { get; set; } = null!;

    public string Algorithm
    {
        get
        {
            return (Option % 4) switch
            {
                0 => "DFS",
                1 => "BFS",
                2 => "dijkstra",
                _ => "aStar"
            };
        }
    }

    public void Solve()
    {
        var goal = Maze.Spaces.FirstOrDefault(s => s.Exit);

        if (Maze.Root is null || goal is null)
            return;

        switch (Option % 4)
        {
            case 0:
                DFS(Maze.Root, goal);
                break;
            case 1:
                BFS(Maze.Root, goal);
                break;
            case 2:
                Dijkstra(Maze.Root, goal);
                break;
            case 3:
                AStar(Maze.Root, goal);
                break;
        }
    }

    private static bool DFS(Space space, Space goal)
    {
        if (space.Visited)
            return false;

        space.Visited = true;

        if (space == goal)
        {
            space.IsSolution = true;
            return true;
        }

        if (space.Right is not null)
            if (DFS(space.Right, goal))
            {
                space.IsSolution = true;
                return true;
            }
        if (space.Top is not null)
            if (DFS(space.Top, goal))
            {
                space.IsSolution = true;
                return true;
            }
        if (space.Left is not null)
            if (DFS(space.Left, goal))
            {
                space.IsSolution = true;
                return true;
            }
        if (space.Bottom is not null)
            if (DFS(space.Bottom, goal))
            {
                space.IsSolution = true;
                return true;
            }

        return false;
    }

    private static bool BFS(Space start, Space goal)
    {
        var queue = new Queue<Space>();
        var prev = new Dictionary<Space, Space?>{ { start, null } };

        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var currSpace = queue.Dequeue();

            if (currSpace.Visited)
                continue;

            currSpace.Visited = true;

            if (currSpace == goal)
            {
                currSpace.IsSolution = true;
                break;
            }

            Space[] spaces = new Space[] { currSpace.Top, currSpace.Left, currSpace.Bottom, currSpace.Right };
            
            foreach (var space in spaces)
            {
                if (space is not null && !space.Visited)
                {
                    prev[space] = currSpace;
                    queue.Enqueue(space);
                }
            }
        }

        var attempt = goal;
        while (attempt != start)
        {
            if (!prev.ContainsKey(attempt))
                return false;

            attempt = prev[attempt];
            attempt.IsSolution = true;
        }

        return true;
    }

    private static bool Dijkstra(Space start, Space goal)
    {
        var queue = new PriorityQueue<Space, float>();
        var dist = new Dictionary<Space, float>();
        var prev = new Dictionary<Space, Space>();

        queue.Enqueue(start, 0);
        dist[start] = 0.0f;

        while (queue.Count > 0)
        {
            var currSpace = queue.Dequeue();

            if (currSpace.Visited)
                continue;

            currSpace.Visited = true;

            if (currSpace == goal)
            {
                currSpace.IsSolution = true;
                break;
            }

            Space[] spaces = new Space[] { currSpace.Top, currSpace.Left, currSpace.Bottom, currSpace.Right };

            foreach (var space in spaces)
            {
                if (space is not null)
                {
                    var newWeight = dist[currSpace] + 1;

                    if (!dist.ContainsKey(space))
                    {
                        dist[space] = float.PositiveInfinity;
                        prev[space] = null!;
                    }

                    if (newWeight < dist[space])
                    {
                        dist[space] = newWeight;
                        prev[space] = currSpace;
                        queue.Enqueue(space, newWeight);
                    }
                }
            }
        }

        var attempt = goal;
        while (attempt != start)
        {
            if (!prev.ContainsKey(attempt))
                return false;

            attempt = prev[attempt];
            attempt.IsSolution = true;
        }

        return true;
    }

    private static bool AStar(Space start, Space goal)
    {
        var queue = new PriorityQueue<Space, float>();
        var dist = new Dictionary<Space, float>();
        var prev = new Dictionary<Space, Space>();

        queue.Enqueue(start, 0);
        dist[start] = 0.0f;

        while (queue.Count > 0)
        {
            var currSpace = queue.Dequeue();

            if (currSpace.Visited)
                continue;

            currSpace.Visited = true;

            if (currSpace == goal)
            {
                currSpace.IsSolution = true;
                break;
            }

            Space[] spaces = new Space[] { currSpace.Top, currSpace.Left, currSpace.Bottom, currSpace.Right };

            foreach (var space in spaces)
            {
                if (space is not null)
                {
                    var dx = space.X - goal.X;
                    var dy = space.Y - goal.Y;
                    var penalty = dx * dx + dy * dy;

                    var newWeight = dist[currSpace] + 1 + penalty;

                    if (!dist.ContainsKey(space))
                    {
                        dist[space] = float.PositiveInfinity;
                        prev[space] = null!;
                    }

                    if (newWeight < dist[space])
                    {
                        dist[space] = newWeight;
                        prev[space] = currSpace;
                        queue.Enqueue(space, newWeight);
                    }
                }
            }
        }

        var attempt = goal;
        while (attempt != start)
        {
            if (!prev.ContainsKey(attempt))
                return false;

            attempt = prev[attempt];
            attempt.IsSolution = true;
        }

        return true;
    }
}