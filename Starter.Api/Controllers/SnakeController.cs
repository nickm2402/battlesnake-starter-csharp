using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Starter.Api.Requests;
using Starter.Api.Responses;
using Starter.Core;
using BoardInformation;
using System.Linq;
using Pathfinding;

namespace Starter.Api.Controllers
{
    [ApiController]
    public class SnakeController : ControllerBase
    {
        /// <summary>
        /// This request will be made periodically to retrieve information about your Battlesnake,
        /// including its display options, author, etc.
        /// </summary>
        [HttpGet("")]
        public IActionResult Index()
        {
            Console.WriteLine("Pong!");

            var response = new InitResponse
            {
                ApiVersion = "1",
                Author = "",
                Color = "#0B8C00",
                Head = "caffeine",
                Tail = "bolt"
            };

            return Ok(response);
        }


        /// <summary>
        /// Your Battlesnake will receive this request when it has been entered into a new game.
        /// Every game has a unique ID that can be used to allocate resources or data you may need.
        /// Your response to this request will be ignored.
        /// </summary>
        [HttpPost("start")]
        public IActionResult Start(GameStatusRequest gameStatusRequest)
        {
            Console.WriteLine("Setting up!");

            return Ok();
        }


        /// <summary>
        /// This request will be sent for every turn of the game.
        /// Use the information provided to determine how your
        /// Battlesnake will move on that turn, either up, down, left, or right.
        /// </summary>
        [HttpPost("move")]
        public IActionResult Move(GameStatusRequest gameStatusRequest)
        {
            Snake me = gameStatusRequest.You;

            Board gameBoard = gameStatusRequest.Board;

            string direction = FindNextDirection(me, gameBoard);

            Console.WriteLine(gameStatusRequest.Turn);

            var response = new MoveResponse
            {
                Move = direction,
                Shout = "I am moving!"
            };
            return Ok(response);
        }


        private string FindNextDirection(Snake me, Board gameBoard)
        {
            Dictionary<Point, string> possibleMovements = new Dictionary<Point, string>();

            possibleMovements.Add(new Point(0, 1), "up");
            possibleMovements.Add(new Point(0, -1), "down");
            possibleMovements.Add(new Point(1, 0), "right");
            possibleMovements.Add(new Point(-1, 0), "left");

            BoardRepresentation boardInfo = new BoardRepresentation(gameBoard);

            Console.WriteLine(boardInfo.ToString());

            Point currentDirection;
            Point possibleMovePoint;

            var rng = new Random();
            var direction = new List<string> { "down", "left", "right", "up" };

            List<Point> dangerZones = generateDangerZones(gameBoard, me);

            string ret = direction[rng.Next(direction.Count)];

            Console.WriteLine("Moving!");

            foreach (KeyValuePair<Point, string> kvp in possibleMovements)
            {
                currentDirection = kvp.Key;
                possibleMovePoint = me.Head + currentDirection;

                if (dangerZones.Contains(possibleMovePoint))
                {
                    direction.Remove(possibleMovements[currentDirection]);
                    continue;
                }

                EFieldInformation fieldInformation = boardInfo.GetFielInformationForPoint(possibleMovePoint);

                switch (fieldInformation)
                {
                    case EFieldInformation.EMPTY:
                        break;

                    case EFieldInformation.FOOD:
                        Console.WriteLine("Food!");
                        return possibleMovements[currentDirection];

                    case EFieldInformation.SNAKE:
                    case EFieldInformation.HAZARDS:
                    case EFieldInformation.WALL:
                    default:
                        direction.Remove(possibleMovements[currentDirection]);
                        break;
                }
            }

            if (gameBoard.Food.Count() > 0)
            {
                List<Point> path = GetPathToClosestFood(gameBoard.Food, me.Head, gameBoard);

                if (path != null)
                {
                    Point dir = path.First() - me.Head;
                    string dirStr = possibleMovements.GetValueOrDefault(dir);
                    if (direction.Contains(dirStr))
                    {
                        return dirStr;
                    }
                }
            }

            if (direction.Count > 0)
                ret = direction[rng.Next(direction.Count)];

            return ret;
        }

        private List<Point> generateDangerZones(Board gameBoard, Snake me)
        {
            List<Point> ret = new List<Point>();
            List<Point> heads = new List<Point>();

            //Save all head points except for your own
            foreach (Snake s in gameBoard.Snakes)
            {
                if (s.Id == me.Id || s.Body.Count() < me.Body.Count())
                {
                    continue;
                }
                heads.Add(s.Body.First());
            }

            //Create danger zone around heads (including the head itself.. might be important if you want to play aggressive later)
            foreach (Point h in heads)
            {
                ret.Add(h + new Point(1, 0));
                ret.Add(h - new Point(1, 0));
                ret.Add(h + new Point(0, 1));
                ret.Add(h - new Point(0, 1));
            }

            return ret;
        }

        static private List<Point> GetPathToClosestFood(IEnumerable<Point> food, Point snakeHead, Board gameBoard)
        {
            List<Point> pathToClosestFood = null;
            int minDist = int.MaxValue;

            foreach (Point target in food)
            {
                Pathfinder pf = new Pathfinder();
                List<Point> path = pf.FindPath(snakeHead, target, gameBoard);
                if (path != null && path.Count < minDist)
                {
                    pathToClosestFood = path;
                    minDist = path.Count;
                }
            }

            return pathToClosestFood;
        }

        /// <summary>
        /// Your Battlesnake will receive this request whenever a game it was playing has ended.
        /// Use it to learn how your Battlesnake won or lost and deallocated any server-side resources.
        /// Your response to this request will be ignored.
        /// </summary>
        [HttpPost("end")]
        public IActionResult End(GameStatusRequest gameStatusRequest)
        {
            return Ok();
        }
    }
}