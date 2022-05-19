using Starter.Api.Requests;
using Starter.Core;

namespace BoardInformation
{
    public class BoardRepresentation
    {

        private EFieldInformation[,] m_boardInformation;

        private BoardRepresentation()
        {

        }

        public BoardRepresentation(Board currentBoard)
        {
            FillBoard(currentBoard);
        }

        private void FillBoard(Board currentBoard)
        {
            m_boardInformation = new EFieldInformation[currentBoard.Width, currentBoard.Height];

            foreach (Point foodPoint in currentBoard.Food)
            {
                m_boardInformation[foodPoint.X, foodPoint.Y] = EFieldInformation.FOOD;
            }

            foreach (Point hazardPoint in currentBoard.Hazards)
            {
                m_boardInformation[hazardPoint.X, hazardPoint.Y] = EFieldInformation.HAZARDS;
            }

            foreach (Snake snake in currentBoard.Snakes)
            {
                foreach (Point bodyPoint in snake.Body)
                {
                    m_boardInformation[bodyPoint.X, bodyPoint.Y] = EFieldInformation.SNAKE;
                }
            }
        }

        public EFieldInformation GetFielInformationForPoint(Point point)
        {
            if (point.X >= m_boardInformation.GetLength(0) || point.Y >= m_boardInformation.GetLength(1) || point.X < 0 || point.Y < 0)
                return EFieldInformation.WALL;

            return m_boardInformation[point.X, point.Y];
        }

        public override string ToString()
        {
            string board = "";

            for(int x = 0; x < m_boardInformation.GetLength(0); x++)
            {
                for (int y = 0; y < m_boardInformation.GetLength(1); y++)
                {
                    EFieldInformation currentField = GetFielInformationForPoint(new Point(x, y));

                    switch (currentField)
                    {
                        case EFieldInformation.EMPTY:
                            board += ".";
                            break;
                        case EFieldInformation.FOOD:
                            board += "O";
                            break;
                        case EFieldInformation.SNAKE:
                        case EFieldInformation.HAZARDS:
                            board += "X";
                            break;
                        case EFieldInformation.WALL:
                            board += "T";
                            break;
                        default:
                            break;
                    }
                }

                board += "\n";
            }

            return board;
        }
    }

    public enum EFieldInformation
    {
        EMPTY,
        FOOD,
        SNAKE,
        HAZARDS,
        WALL,
    }
}
