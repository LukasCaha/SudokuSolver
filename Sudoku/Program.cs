using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class Program
    {
        static int[,] task = new int[9, 9];
        static int[,] pole = new int[9, 9];
        static bool[,,] hinty = new bool[9, 9, 10];

        static void Main(string[] args)
        {
            //before start
            InitializeSudoku();
            ResetHints();

            //before display
            PrintSudoku(true);

            //solving
            UpdateHints();
            MakeImplications();
            //TakeGuess(pole, hinty, out pole, out hinty);

            //after display
            PrintSudoku(true);

            //validity check
            Info();


            //end
            Console.ReadKey();
        }

        static void Info()
        {
            if (IsSudokuValid() == "správně")
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("Sudoku v sobě nemá žádné chyby.");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Sudoku v sobě má chybu.");
                Console.WriteLine("Error: " + IsSudokuValid());
                Console.ForegroundColor = ConsoleColor.White;
            }
            if (IsSudokuComplete())
            {
                Console.WriteLine("Sudoku je kompletně vyplněné.");
            }
            else
            {
                Console.WriteLine("Pro dokončení sudoku musíš ještě pokračovat.");
            }
        }

        static void InitializeSudoku()
        {
            //blank
            /*task = new int[,] {   { 0,0,0,0,0,0,0,0,0 },
                                    { 0,0,0,0,0,0,0,0,0 },
                                    { 0,0,0,0,0,0,0,0,0 },
                                    { 0,0,0,0,0,0,0,0,0 },
                                    { 0,0,0,0,0,0,0,0,0 },
                                    { 0,0,0,0,0,0,0,0,0 },
                                    { 0,0,0,0,0,0,0,0,0 },
                                    { 0,0,0,0,0,0,0,0,0 },
                                    { 0,0,0,0,0,0,0,0,0 }};*/

            //easy
            /*task = new int[,] { { 5,3,0,0,7,0,0,0,0 },
                                { 6,0,0,1,9,5,0,0,0 },
                                { 0,9,8,0,0,0,0,6,0 },
                                { 8,0,0,0,6,0,0,0,3 },
                                { 4,0,0,8,0,3,0,0,1 },
                                { 7,0,0,0,2,0,0,0,6 },
                                { 0,6,0,0,0,0,2,8,0 },
                                { 0,0,0,4,1,9,0,0,5 },
                                { 0,0,0,0,8,0,0,7,9 } };*/

            /*task = new int[,] {     { 0,3,0,2,0,0,0,0,6 },
                                    { 0,0,0,0,0,9,0,0,4 },
                                    { 7,6,0,0,0,0,0,0,0 },
                                    { 0,0,0,0,5,0,7,0,0 },
                                    { 0,0,0,0,0,1,8,6,0 },
                                    { 0,5,0,4,8,0,0,9,0 },
                                    { 8,0,0,0,0,0,0,0,0 },
                                    { 0,0,0,0,7,6,0,0,0 },
                                    { 0,7,5,0,0,8,1,0,0 }};*/

            //prej hardest
            task = new int[,] {     { 8,0,0,0,0,0,0,0,0 },
                                    { 0,0,3,6,0,0,0,0,0 },
                                    { 0,7,0,0,9,0,2,0,0 },
                                    { 0,5,0,0,0,7,0,0,0 },
                                    { 0,0,0,0,4,5,7,0,0 },
                                    { 0,0,0,1,0,0,0,3,0 },
                                    { 0,0,1,0,0,0,0,6,8 },
                                    { 0,0,8,5,0,0,0,1,0 },
                                    { 0,9,0,0,0,0,4,0,0 }};

            for (int row = 0; row < 9; row++)
            {
                for (int column = 0; column < 9; column++)
                {
                    pole[row, column] = task[row, column];
                }
            }
        }

        static void ResetHints()
        {
            for (int row = 0; row < 9; row++)
            {
                for (int column = 0; column < 9; column++)
                {
                    hinty[row, column, 0] = false;
                    for (int number = 1; number < 10; number++)
                    {
                        hinty[row, column, number] = true;
                    }
                }
            }
        }

        static void UpdateHints()
        {
            for (int row = 0; row < 9; row++)
            {
                for (int column = 0; column < 9; column++)
                {
                    int currentNumber = pole[row, column];
                    if (currentNumber == 0)
                    {
                        continue;
                    }

                    DeleteExistingNumbersHints();
                    SetColumnHints(column, currentNumber, false);
                    SetRowHints(row, currentNumber, false);
                    SetSectorHints(row, column, currentNumber, false);
                }
            }
        }

        static void DeleteExistingNumbersHints()
        {
            for (int row = 0; row < 9; row++)
            {
                for (int column = 0; column < 9; column++)
                {
                    if (pole[row, column] != 0)
                    {
                        for (int hint = 0; hint < 10; hint++)
                        {
                            hinty[row, column, hint] = false;
                        }
                    }
                }
            }
        }

        static void SetColumnHints(int columnId, int number, bool value)
        {
            for (int row = 0; row < 9; row++)
            {
                hinty[row, columnId, number] = value;
            }
        }

        static void SetRowHints(int rowId, int number, bool value)
        {
            for (int column = 0; column < 9; column++)
            {
                hinty[rowId, column, number] = value;
            }
        }

        static void SetSectorHints(int row, int column, int number, bool value)
        {
            int startRow = row - (row % 3);
            int startColumn = column - (column % 3);

            for (int rowIndex = startRow; rowIndex < startRow + 3; rowIndex++)
            {
                for (int columnIndex = startColumn; columnIndex < startColumn + 3; columnIndex++)
                {
                    hinty[rowIndex, columnIndex, number] = value;
                }
            }
        }

        static void MakeImplications()
        {
            bool somethingChanged = false;

            //row
            for (int row = 0; row < 9; row++)
            {
                for (int number = 1; number < 10; number++)
                {
                    somethingChanged = somethingChanged || RowImplications(number, row);
                }
            }
            //column
            for (int column = 0; column < 9; column++)
            {
                for (int number = 1; number < 10; number++)
                {
                    somethingChanged = somethingChanged || ColumnImplications(number, column);
                }
            }
            //sector
            for (int bigRow = 0; bigRow < 3; bigRow++)
            {
                for (int bigColumn = 0; bigColumn < 3; bigColumn++)
                {
                    for (int number = 1; number < 10; number++)
                    {
                        somethingChanged = somethingChanged || SectorImplications(number,bigRow,bigColumn);
                    }
                }
            }

            //tile
            for (int row = 0; row < 9; row++)
            {
                for (int column = 0; column < 9; column++)
                {
                    somethingChanged = somethingChanged || TileImplications(row, column);
                }
            }


            if (somethingChanged)
            {
                UpdateHints();
                MakeImplications();
            }
        }

        static bool RowImplications(int number, int rowId)
        {
            //jestli je jen na jedné pozici v řádku možné jedno číslo
            int countPossibleTiles = 0;
            int lastColumn = 0;
            for (int column = 0; column < 9; column++)
            {
                if (hinty[rowId, column, number])
                {
                    countPossibleTiles++;
                    lastColumn = column;
                }
            }
            if (countPossibleTiles == 1)
            {
                pole[rowId, lastColumn] = number;
                return true;
            }
            return false;
        }

        static bool ColumnImplications(int number, int columnId)
        {
            //jeslit je jen na jedné pozici v sloupci možné jedno číslo
            int countPossibleTiles = 0;
            int lastRow = 0;
            for (int row = 0; row < 9; row++)
            {
                if (hinty[row, columnId, number])
                {
                    countPossibleTiles++;
                    lastRow = row;
                }
            }
            if (countPossibleTiles == 1)
            {
                pole[lastRow, columnId] = number;
                return true;
            }
            return false;
        }

        static bool SectorImplications(int number, int bigRow, int bigColumn)
        {
            //jestli je jen na jedné pozici v sektoru možné jedno číslo 
            int countPossibleTiles = 0;
            int lastTileRow = 0;
            int lastTileColumn = 0;
            for (int row = bigRow*3; row < bigRow * 3+3; row++)
            {
                for (int column = bigColumn * 3; column < bigColumn*3+3; column++)
                {
                    if (hinty[row, column, number])
                    {
                        countPossibleTiles++;
                        lastTileRow = row;
                        lastTileColumn = column;
                    }
                }
            }
            if(countPossibleTiles == 1)
            {
                pole[lastTileRow,lastTileColumn] = number;
                return true;
            }
            return false;
        }

        static bool TileImplications(int row, int column)
        {
            //jestli je na políčku jen jedno možné číslo
            bool foundOne = false;
            int thatNumber = 0;
            for (int number = 0; number < 9; number++)
            {
                if (foundOne && hinty[row, column, number])
                {
                    return false;
                }
                if (!foundOne && hinty[row, column, number])
                {
                    foundOne = true;
                    thatNumber = number;
                }
            }
            if (thatNumber != 0)
            {
                pole[row, column] = thatNumber;
                return true;
            }
            return false;
        }

        //duplikuje, hodí jedno číslo dle hintů, zkontroluje a učiní akci
        static void TakeGuess(int[,] poleP, bool[,,] hintyP, out int[,] poleO, out bool[,,] hintyO)
        {
            //duplikace
            int[,] pole = new int[poleP.GetLength(0), poleP.GetLength(1)];
            for (int x = 0; x < pole.GetLength(0); x++)
            {
                for (int y = 0; y < pole.GetLength(1); y++)
                {
                    pole[x, y] = poleP[x, y];
                }
            }

            bool[,,] hinty = new bool[hintyP.GetLength(0), hintyP.GetLength(1), hintyP.GetLength(2)];
            for (int x = 0; x < hinty.GetLength(0); x++)
            {
                for (int y = 0; y < hinty.GetLength(1); y++)
                {
                    for (int z = 0; z < hinty.GetLength(1); z++)
                    {

                        hinty[x, y, z] = hintyP[x, y, z];
                    }
                }
            }

            //zkusí jedno číslo dle hintů
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    if (pole[x, y] == 0)
                    {
                        for (int number = 1; number < 10; number++)
                        {
                            if (hinty[x, y, number])
                            {
                                pole[x, y] = number;

                                //zjistím jak na tom jsme
                                bool isComplete = IsSudokuComplete(pole);
                                bool isValid = IsSudokuValid(pole);

                                //konec
                                if(isComplete && isValid)
                                {
                                    poleO = pole;
                                    hintyO = hinty;
                                    return;
                                }
                                //pokračujem
                                if (!isComplete && isValid)
                                {
                                    TakeGuess(pole,hinty,out pole, out hinty);
                                }
                                //vracíme se
                                if (!isValid)
                                {
                                    
                                }
                            }
                        }
                    }
                }
            }

            //vše vyplněné
            poleO = poleP;
            hintyO = hintyP;
            return;
        }

        static string IsSudokuValid()
        {
            for (int row = 0; row < 9; row++)
            {
                if (!IsRowValid(row))
                    return "řádky " + row;
            }

            for (int column = 0; column < 9; column++)
            {
                if (!IsColumnValid(column))
                    return "sloupce " + column;
            }

            for (int bigRow = 0; bigRow < 3; bigRow++)
            {
                for (int bigColumn = 0; bigColumn < 3; bigColumn++)
                {
                    if (!IsSectorValid(bigRow, bigColumn))
                        return "sektory " + bigRow + " " + bigColumn;
                }
            }

            return "správně";
        }

        static bool IsSudokuValid(int[,] pole)
        {
            for (int row = 0; row < 9; row++)
            {
                if (!IsRowValid(row,pole))
                    return false;
            }

            for (int column = 0; column < 9; column++)
            {
                if (!IsColumnValid(column, pole))
                    return false;
            }

            for (int bigRow = 0; bigRow < 3; bigRow++)
            {
                for (int bigColumn = 0; bigColumn < 3; bigColumn++)
                {
                    if (!IsSectorValid(bigRow, bigColumn, pole))
                        return false;
                }
            }

            return true;
        }

        static bool IsRowValid(int rowId)
        {
            bool[] oneToNine = new bool[9];
            for (int i = 0; i < 9; i++)
            {
                oneToNine[i] = false;
            }

            for (int column = 0; column < 9; column++)
            {
                int currentNumber = pole[rowId, column] - 1;
                if (currentNumber < 0)
                    continue;
                if (oneToNine[currentNumber])
                    return false;
                oneToNine[currentNumber] = true;
            }

            return true;
        }

        static bool IsColumnValid(int columnId)
        {
            bool[] oneToNine = new bool[9];
            for (int i = 0; i < 9; i++)
            {
                oneToNine[i] = false;
            }

            for (int row = 0; row < 9; row++)
            {
                int currentNumber = pole[row, columnId] - 1;
                if (currentNumber < 0)
                    continue;
                if (oneToNine[currentNumber])
                    return false;
                oneToNine[currentNumber] = true;
            }

            return true;
        }

        static bool IsSectorValid(int bigRow, int bigColumn)
        {
            bool[] oneToNine = new bool[9];
            for (int i = 0; i < 9; i++)
            {
                oneToNine[i] = false;
            }

            for (int row = 0; row < 3; row++)
            {
                for (int column = 0; column < 3; column++)
                {
                    int currentNumber = pole[bigRow * 3 + row, bigColumn * 3 + column] - 1;
                    if (currentNumber < 0)
                        continue;
                    if (oneToNine[currentNumber])
                        return false;
                    oneToNine[currentNumber] = true;
                }
            }

            return true;
        }

        static bool IsRowValid(int rowId, int[,] pole)
        {
            bool[] oneToNine = new bool[9];
            for (int i = 0; i < 9; i++)
            {
                oneToNine[i] = false;
            }

            for (int column = 0; column < 9; column++)
            {
                int currentNumber = pole[rowId, column] - 1;
                if (currentNumber < 0)
                    continue;
                if (oneToNine[currentNumber])
                    return false;
                oneToNine[currentNumber] = true;
            }

            return true;
        }

        static bool IsColumnValid(int columnId, int[,] pole)
        {
            bool[] oneToNine = new bool[9];
            for (int i = 0; i < 9; i++)
            {
                oneToNine[i] = false;
            }

            for (int row = 0; row < 9; row++)
            {
                int currentNumber = pole[row, columnId] - 1;
                if (currentNumber < 0)
                    continue;
                if (oneToNine[currentNumber])
                    return false;
                oneToNine[currentNumber] = true;
            }

            return true;
        }

        static bool IsSectorValid(int bigRow, int bigColumn, int[,] pole)
        {
            bool[] oneToNine = new bool[9];
            for (int i = 0; i < 9; i++)
            {
                oneToNine[i] = false;
            }

            for (int row = 0; row < 3; row++)
            {
                for (int column = 0; column < 3; column++)
                {
                    int currentNumber = pole[bigRow * 3 + row, bigColumn * 3 + column] - 1;
                    if (currentNumber < 0)
                        continue;
                    if (oneToNine[currentNumber])
                        return false;
                    oneToNine[currentNumber] = true;
                }
            }

            return true;
        }

        static bool IsSudokuComplete()
        {
            for (int row = 0; row < 9; row++)
            {
                for (int colum = 0; colum < 9; colum++)
                {
                    if (pole[row, colum] == 0)
                        return false;
                }
            }

            return true;
        }

        static bool IsSudokuComplete(int[,] pole)
        {
            for (int row = 0; row < 9; row++)
            {
                for (int column = 0; column < 9; column++)
                {
                    if (pole[row, column] == 0)
                        return false;
                }
            }

            return true;
        }

        static void PrintSudoku(bool pretty)
        {
            if (!pretty)
            {
                for (int row = 0; row < 9; row++)
                {
                    for (int column = 0; column < 9; column++)
                    {
                        Console.Write(pole[row, column]);
                        if (column < 8)
                            Console.Write(" ");
                    }
                    Console.WriteLine();
                }
            }
            else
            {
                for (int row = 0; row < 9; row++)
                {
                    if (row > 0)
                        Console.WriteLine("|");
                    if ((row) % 3 == 0)
                        Console.WriteLine("+-----+-----+-----+");
                    for (int column = 0; column < 9; column++)
                    {
                        if ((column) % 3 == 0)
                            Console.Write("|");
                        else
                            Console.Write(" ");
                        if (pole[row, column] == 0)
                            Console.Write(".");
                        else
                        {
                            if (task[row, column] != 0)
                            {
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            Console.Write(pole[row, column]);
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                }
                Console.Write("|\n+-----+-----+-----+");
            }
            Console.WriteLine();
        }
    }
}
