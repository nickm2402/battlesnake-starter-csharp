using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Starter.Api.Requests;
using Starter.Api.Responses;
using Starter.Core;
using BoardInformation;
using System.Linq;

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
                Point targetApple = GetClosestFood(gameBoard.Food, me.Head);

                if (me.Head.X < targetApple.X && direction.Contains("right"))
                {
                    return "right";
                }
                else if (me.Head.X > targetApple.X && direction.Contains("left"))
                {
                    return "left";
                }
                else if (me.Head.Y > targetApple.Y && direction.Contains("down"))
                {
                    return "down";
                }
                else if (me.Head.Y < targetApple.Y && direction.Contains("up"))
                {
                    return "up";
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
                if (s.Id == me.Id)
                {
                    continue;
                }
                heads.Add(s.Body.First());
            }

            //Create danger zone around heads (including the head itself.. might be important if you want to play aggressive later)
            foreach (Point h in heads)
            {
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if (x == 0 && y == 0)
                        {
                            continue;
                        }

                        ret.Add(h + new Point(x, y));
                    }
                }
            }

            return ret;
        }

        private Point GetClosestFood(IEnumerable<Point> food, Point snakeHead)
        {
            if(food.Count() == 0)
            {
                return new Point(-1, -1);
            }

            Point headToFood;
            int shortestDistance = int.MaxValue;
            Point closestFood = food.First();

            foreach(Point point in food)
            {                
                headToFood = point - snakeHead;
                int currentDistance = Math.Abs(headToFood.X) + Math.Abs(headToFood.Y);

                if(currentDistance < shortestDistance)
                {
                    shortestDistance = currentDistance;
                    closestFood = point;
                }
            }

            return closestFood;
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