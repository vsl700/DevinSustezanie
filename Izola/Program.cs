using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Izola
{
    class Program
    {
        static int size;
        static Cell[,] cellsGrid;
        static int currentSelX, currentSelY; //Selection coordinates (sel = selection)
        static int selX, selY;

        static bool player1sTurn; //true - player1, false - player2

        static bool exit;//Whether to exit the game (when the user requests to go back to the game setup menu)


        static void Main(string[] args)
        {
            while (true)
            {
                GameSetup();

                GamePlay();
                Console.Clear();
            }
        }

        static void GamePlay()
        {
            bool tempTurn = player1sTurn;
            while (true)
            {
                Render();

                if (tempTurn != player1sTurn && IsGameOver())
                {
                    Console.SetCursorPosition(0, GetHelpTextY() + 7);
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine("Game Over!");
                    if (player1sTurn)
                        Console.WriteLine("Player 2 Wins!");
                    else
                        Console.WriteLine("Player 1 Wins!");
                    Console.WriteLine("Want To Play Again? (press Y or N key on the keyboard)");

                    Console.ResetColor();

                    if (!AcceptOrDecline())
                        break;
                    else
                        LoadGame();

                    continue;
                }

                tempTurn = player1sTurn;

                ProcessInput();
                
                if (exit)
                    break;
            }
        }

        static bool AcceptOrDecline()
        {
            ConsoleKey key;
            do
            {
                key = Console.ReadKey(true).Key;
            } while (key != ConsoleKey.Y && key != ConsoleKey.N);

            return key == ConsoleKey.Y;
        }

        static bool IsGameOver()
        {
            for (int y = selY - 1; y <= selY + 1; y++)
            {
                if (y < 0)
                    continue;

                if (y >= size)
                    break;

                for (int x = selX - 1; x <= selX + 1; x++)
                {
                    if (x < 0 || x == selX && y == selY)
                        continue;

                    if (x >= size)
                        break;

                    if (cellsGrid[y, x].IsEmpty())
                        return false;
                }
            }

            return true;
        }

        static void AllocateSelectedCell()
        {
            if (player1sTurn)
                cellsGrid[currentSelY, currentSelX].Allocation = Cell.AllocationType.PLAYER1_TERITORY;
            else
                cellsGrid[currentSelY, currentSelX].Allocation = Cell.AllocationType.PLAYER2_TERITORY;

            ChangeTurn();
        }

        //Changes the player1sTurn bool to the opposite and gets the coordinates of the following player's cell
        static void ChangeTurn()
        {
            player1sTurn = !player1sTurn;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (player1sTurn && cellsGrid[y, x].Selection == Cell.SelectionType.PLAYER1_POSITION || !player1sTurn && cellsGrid[y, x].Selection == Cell.SelectionType.PLAYER2_POSITION)
                    {
                        currentSelX = selX = x;
                        currentSelY = selY = y;
                        return;
                    }
                }
            }
        }

        static void SelectCell()
        {
            if (player1sTurn)
                cellsGrid[currentSelY, currentSelX].Selection = Cell.SelectionType.PLAYER1_POSITION;
            else
                cellsGrid[currentSelY, currentSelX].Selection = Cell.SelectionType.PLAYER2_POSITION;
        }

        static readonly string invalidInputText = "Please enter valid coordinates!";
        static readonly string invalidInputClearText = new string(' ', invalidInputText.Length);
        static void ProcessInput()
        {
            string text = Console.ReadLine().ToLower();
            if(text == "exit")
            {
                exit = true;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(invalidInputClearText);
            Console.SetCursorPosition(0, Console.CursorTop);
            if (text.Length == 0)
            {
                if(cellsGrid[currentSelY, currentSelX].Allocation != Cell.AllocationType.EMPTY)
                {
                    Console.WriteLine(invalidInputText);
                    Console.ResetColor();
                    return;
                }

                AllocateSelectedCell();
                Console.ResetColor();
                return;
            }

            int tempX = currentSelX;
            int tempY = currentSelY;

            try
            {
                int[] tempArr = text.Split(';').Select(x => int.Parse(x)).ToArray();

                if (tempArr.Length != 2 || 
                    tempArr[0] < 0 || tempArr[1] < 0 ||
                    tempArr[0] >= size || tempArr[1] >= size
                    || Math.Abs(tempArr[0] - selX) > 1 || Math.Abs(tempArr[1] - selY) > 1
                    || !cellsGrid[tempArr[1], tempArr[0]].IsEmpty())
                    throw new Exception();

                currentSelX = tempArr[0];
                currentSelY = tempArr[1];

                
            }
            catch (Exception e)
            {
                Console.WriteLine(invalidInputText);
            }

            cellsGrid[tempY, tempX].Selection = Cell.SelectionType.EMPTY;
            SelectCell();
            
            Console.ResetColor();
        }

        static readonly string coordsEmptyStr = new string(' ', 10);
        static readonly string exitHint = "To go back to game setup menu, please type 'exit' and press Enter!";
        static readonly string typeHint = "To move around the board, please type the coordinates you want to move on (like 'x;y') and press Enter,";
        static readonly string enterHint = "or press Enter without typing anything to allocate cell: ";
        static void Render()
        {
            Console.SetCursorPosition(0, 0);

            ConsoleColor diffColor = ConsoleColor.Yellow;
            for (int y = 0; y < size + 1; y++)
            {
                if (y < size)
                {
                    for (int x = 0; x < size; x++) //Draws the cells on the row
                    {
                        cellsGrid[y, x].Render();

                        /*if(x < size - 1)
                            */
                        Console.Write("|");
                        if (x == size - 1)
                        {
                            if (y % 2 == 0)
                                Console.ForegroundColor = diffColor;

                            Console.Write(y);

                            if (y == size - 1)
                                Console.Write(" - y coordinates");

                            Console.WriteLine();

                            Console.ResetColor();
                        }
                    }
                }

                for (int x = 0; x < size * 2; x++) //Draws the grid (beauty) row
                {
                    if (y < size)
                        Console.Write("-");
                    else
                    {
                        if (x / 2 % 2 != 0)
                            Console.ForegroundColor = diffColor;

                        if (x % 2 == 0)
                            Console.Write(x / 2);
                        else if (x / 2 < 10)
                            Console.Write(" ");

                        if (x == size * 2 - 1)
                        {
                            Console.ForegroundColor = diffColor;
                            Console.WriteLine("\n- x coordinates");
                        }
                    }

                    Console.ResetColor();
                }

                Console.WriteLine();
            }



            Console.SetCursorPosition(0, GetHelpTextY());
            if (player1sTurn)
            {
                Console.ForegroundColor = Cell.PLAYER1_COLOR;
                Console.WriteLine("Player 1's Turn!");
            }
            else
            {
                Console.ForegroundColor = Cell.PLAYER2_COLOR;
                Console.WriteLine("Player 2's Turn!");
            }

            Console.ResetColor();

            Console.WriteLine(exitHint);
            Console.WriteLine(typeHint);
            Console.Write(enterHint + coordsEmptyStr);
            Console.SetCursorPosition(enterHint.Length, Console.CursorTop);
            /*Console.WriteLine("Esc - go back to game setup menu");*/
        }

        static void GameSetup()
        {
            exit = false;
            size = 0;
            

            Console.WriteLine("Welcome To IZOLA!");

            do
            {
                Console.WriteLine("Please Enter The Size Of The Gameboard (between 3 and 50 inclusive, the number should be odd (3, 5, 7...)): ");
                int.TryParse(Console.ReadLine(), out size);
            } while (size < 3 || size > 50 || size % 2 == 0);
            Console.Clear();

            LoadGame();

        }

        static void LoadGame()
        {
            player1sTurn = false; //This will cause the change turn method to make player 1 the first player
            Console.Clear();

            cellsGrid = new Cell[size, size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    cellsGrid[y, x] = new Cell();
                }
            }

            Random r = new Random();
            SpawnPlayer(r, Cell.SelectionType.PLAYER1_POSITION, Cell.AllocationType.PLAYER1_TERITORY);
            SpawnPlayer(r, Cell.SelectionType.PLAYER2_POSITION, Cell.AllocationType.PLAYER2_TERITORY);

            ChangeTurn(); //Makes player1sTurn bool to true and selects Player1's cell
        }

        //I just made this in a method so that I change the things faster when I need to change something, as I repeat the same thing twice
        static void SpawnPlayer(Random r, Cell.SelectionType selectionType, Cell.AllocationType allocationType)
        {
            int x, y;
            do
            {
                x = r.Next(size);
                y = r.Next(size);
            } while (cellsGrid[y, x].Selection == selectionType);

            cellsGrid[y, x].Selection = selectionType;
            cellsGrid[y, x].Allocation = allocationType;
        }

        //Get the input instructions text Y coords
        static int GetHelpTextY()
        {
            return size * 2 + 3;
        }
    }
}
