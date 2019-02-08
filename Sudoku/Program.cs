using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class Program
    {
        #region variables & structures
        public struct Move
        {
            public int row;
            public int column;
            public int number;
            public bool isGuess;
            public Move(int r, int c, int n, bool g)
            {
                row = r;
                column = c;
                number = n;
                isGuess = g;
            }
        }

        static int[,] task = new int[9, 9];
        static int[,] pole = new int[9, 9];
        static bool[,,] hinty = new bool[9, 9, 10];
        static List<Move> pastMoves = new List<Move>();

        //benchmarks
        static int numberOfGuesses = 0;
        static List<string> sudokus = new List<string>();
        #endregion

        static void Main(string[] args)
        {
            //Načte všechna zadání ze souboru
            LoadFromFile("zadani.txt");

            foreach (string sudoku in sudokus)
            {
                SudokuToTask(sudoku);
                InitializeSudoku();
                //PrintSudoku(true);
                UpdateHints();
                MakeImplications();
                TakeGuess(0, 0, 1);
                //PrintSudoku(true);
                //Info();
                if (numberOfGuesses > 60000)
                {
                    PrintSudoku(true);
                    Info();
                }
            }
            /*
            //před startem
            InitializeSudoku();
            ResetHints();

            //zobrazení
            PrintSudoku(true);

            //řešení
            UpdateHints();
            MakeImplications();
            TakeGuess(0, 0, 1);

            //zobrazení výsledku
            PrintSudoku(true);

            //kontrola
            Info();*/

            //konec
            Console.ReadKey();
        }

        /// <summary>Info je funkce vypysující shrnující informace o aktuálním stavu sudoku.
        /// <para>Nejprve zjistí pomocí funkce <see cref="IsSudokuValid()"/> jestli nějaké dvě čísla v mřížce kolidují.</para>
        /// <para>Poté zkontroluje jestli je sudoku plně vyplněné funkcí <see cref="IsSudokuComplete()"/>.</para>
        /// <para>Tato funkce je čistě informativní a nemá vliv na chod programu.</para>
        /// </summary>
        static void Info()
        {
            if (IsSudokuValid() == "správně")
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("Sudoku v sobě nemá žádné kolize.");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Sudoku v sobě má kolizi.");
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
            Console.WriteLine("Computational complexity:" + numberOfGuesses);
        }
        /// <summary>Obsahuje zadání sudoku.
        /// <para>Přepíše zadání do řešící mřížky a zachovává původní zadání pro pozdější využití.</para>
        /// </summary>
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
            //anti-my program?
            /*task = new int[,] {     { 0,0,0,0,0,0,0,0,0 },
                                    { 0,0,0,0,0,3,0,8,5 },
                                    { 0,0,1,0,2,0,0,0,0 },
                                    { 0,0,0,5,0,7,0,0,0 },
                                    { 0,0,4,0,0,0,1,0,0 },
                                    { 0,9,0,0,0,0,0,0,0 },
                                    { 5,0,0,0,0,0,0,7,3 },
                                    { 0,0,2,0,1,0,0,0,0 },
                                    { 0,0,0,0,4,0,0,0,9 }};*/
            //prej hardest
            /*task = new int[,] {     { 8,0,0,0,0,0,0,0,0 },
                                    { 0,0,3,6,0,0,0,0,0 },
                                    { 0,7,0,0,9,0,2,0,0 },
                                    { 0,5,0,0,0,7,0,0,0 },
                                    { 0,0,0,0,4,5,7,0,0 },
                                    { 0,0,0,1,0,0,0,3,0 },
                                    { 0,0,1,0,0,0,0,6,8 },
                                    { 0,0,8,5,0,0,0,1,0 },
                                    { 0,9,0,0,0,0,4,0,0 }};*/

            for (int row = 0; row < 9; row++)
            {
                for (int column = 0; column < 9; column++)
                {
                    pole[row, column] = task[row, column];
                }
            }

            ResetHints();

            pastMoves.Clear();

            numberOfGuesses = 0;
        }
        /// <summary>Vyprázdní tabulku nápověd.
        /// <para>Tabulka nápověd říká, zda na políčku (row,column) může být číslo (number)</para>     
        /// <para><code>hinty[row, column, number] = true;</code> ... může být</para>
        /// <para><code>hinty[row, column, number] = false;</code> ... nemůže být</para>
        /// <para>Ze začátku resetuje tahle funkce všechny hodnoty na true.</para>  
        /// </summary>
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
        /// <summary>Pozmění tabulku nápověd podle vyplnění tabulky.
        /// <para>Pro každé políčko na kterém je číslo spustí tato funkce tyto funkce:</para>     
        /// <list type="bullet">
        /// <item>
        /// <para><see cref="DeleteHintsOnTile(int row, int column)"/></para>
        /// </item>
        /// <item>
        /// <para><see cref="SetColumnHints(int columnId, int number, bool value)"/></para>
        /// </item>
        /// <item>
        /// <para><see cref="SetRowHints(int rowId, int number, bool value)"/></para>
        /// </item>
        /// <item>
        /// <para><see cref="SetSectorHints(int row, int column, int number, bool value)"/></para>
        /// </item>
        /// </list>
        /// </summary>
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

                    DeleteHintsOnTile(row, column);
                    SetColumnHints(column, currentNumber, false);
                    SetRowHints(row, currentNumber, false);
                    SetSectorHints(row, column, currentNumber, false);
                }
            }
        }
        /// <summary>
        /// Zapíše do políčka (row, column) číslo (number). A uloží tento zápis.
        /// <para>Pokud je tento zápis nejistý (isGuess)=true.</para>
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="number"></param>
        /// <param name="isGuess"></param>
        static void SetTile(int row, int column, int number, bool isGuess)
        {
            //set tile
            pole[row, column] = number;
            //save this move
            //if its guess mark this move as guess
            pastMoves.Add(new Move(row, column, number, isGuess));
            if (isGuess)
            {
                numberOfGuesses++;
                //Console.WriteLine(i + " " + pastMoves.Count());
                //Console.ReadKey();
            }
        }
        /// <summary>
        /// <para>Smaže všechny nápovědy na políčku (row,column).</para>
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        static void DeleteHintsOnTile(int row, int column)
        {
            for (int number = 0; number < 10; number++)
            {
                hinty[row, column, number] = false;
            }
        }
        /// <summary>
        /// <para>Smaže všechny nápovědy pro číslo (number) ve sloupečku (column).</para>
        /// </summary>
        /// <param name="columnId"></param>
        /// <param name="number"></param>
        /// <param name="value"></param>
        static void SetColumnHints(int columnId, int number, bool value)
        {
            for (int row = 0; row < 9; row++)
            {
                hinty[row, columnId, number] = value;
            }
        }
        /// <summary>
        /// <para>Smaže všechny nápovědy pro číslo (number) v řádku (row).</para>
        /// </summary>
        /// <param name="rowId"></param>
        /// <param name="number"></param>
        /// <param name="value"></param>
        static void SetRowHints(int rowId, int number, bool value)
        {
            for (int column = 0; column < 9; column++)
            {
                hinty[rowId, column, number] = value;
            }
        }
        /// <summary>
        /// <para>Smaže všechny nápovědy pro číslo (number) v sektoru (row - (row % 3), column - (column % 3)).</para>
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="number"></param>
        /// <param name="value"></param>
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
        /// <summary>
        /// Rekurzivní funkce, vyplňující tabulku dle pravidel sudoku.
        /// <para>Sleduje změny po každém zavolání následujících funkcí. Pokud nastane změna (je vyplněné políčko) zavolá funkce <see cref="UpdateHints()"/> a sama sebe.</para>
        /// <para>Na každém řádku spustí <see cref="RowImplications(int number, int rowId)"/>.</para>
        /// <para>Na každém sloupci spustí <see cref="ColumnImplications(int number, int columnId)"/>.</para>
        /// <para>V každém sektoru spustí <see cref="SectorImplications(int number, int bigRow, int bigColumn)"/>.</para>
        /// <para>Na každém políčku spustí <see cref="TileImplications(int row, int column)"/>.</para>
        /// </summary>
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
                        somethingChanged = somethingChanged || SectorImplications(number, bigRow, bigColumn);
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
        /// <summary>
        /// <para>Zjistí na kolika pozicích v řádku (rowId) může být číslo (number). Pokud najde pouze jednu možnou pozici. Vloží tam číslo (number).</para>
        /// <para>Pokud funkce vložila číslo vrací true, jinak false.</para>
        /// </summary>
        /// <param name="number"></param>
        /// <param name="rowId"></param>
        /// <returns></returns>
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
                SetTile(rowId, lastColumn, number, false);
                return true;
            }
            return false;
        }
        /// <summary>
        /// <para>Zjistí na kolika pozicích ve sloupci (columnId) může být číslo (number). Pokud najde pouze jednu možnou pozici. Vloží tam číslo (number).</para>
        /// <para>Pokud funkce vložila číslo vrací true, jinak false.</para>
        /// </summary>
        /// <param name="number"></param>
        /// <param name="columnId"></param>
        /// <returns></returns>
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
                SetTile(lastRow, columnId, number, false);
                return true;
            }
            return false;
        }
        /// <summary>
        /// <para>Zjistí na kolika pozicích v sektoru (bigRow, bigColumn) může být číslo (number). Pokud najde pouze jednu možnou pozici. Vloží tam číslo (number).</para>
        /// <para>Pokud funkce vložila číslo vrací true, jinak false.</para>
        /// </summary>
        /// <param name="number"></param>
        /// <param name="bigRow"></param>
        /// <param name="bigColumn"></param>
        /// <returns></returns>
        static bool SectorImplications(int number, int bigRow, int bigColumn)
        {
            //jestli je jen na jedné pozici v sektoru možné jedno číslo 
            int countPossibleTiles = 0;
            int lastTileRow = 0;
            int lastTileColumn = 0;
            for (int row = bigRow * 3; row < bigRow * 3 + 3; row++)
            {
                for (int column = bigColumn * 3; column < bigColumn * 3 + 3; column++)
                {
                    if (hinty[row, column, number])
                    {
                        countPossibleTiles++;
                        lastTileRow = row;
                        lastTileColumn = column;
                    }
                }
            }
            if (countPossibleTiles == 1)
            {
                SetTile(lastTileRow, lastTileColumn, number, false);
                return true;
            }
            return false;
        }
        /// <summary>
        /// <para>Zjistí kolik čísel může být v políčku (row, column). Pokud políčku vyhovuje pouze jedno číslo vloží ho tam.</para>
        /// <para>Pokud funkce vložila číslo vrací true, jinak false.</para>
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        static bool TileImplications(int row, int column)
        {
            //jestli je na políčku jen jedno možné číslo
            bool foundOne = false;
            int thatNumber = 0;
            for (int number = 1; number < 10; number++)
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
            //nemůže z hintů vyčíst 0, ale může to zůstat inicializované na nulu
            if (thatNumber != 0)
            {
                SetTile(row, column, thatNumber, false);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="number"></param>
        static void TakeGuess(int x, int y, int number)
        {

            if (pole[x, y] == 0)
            {
                //pro každé číslo
                //zapni rekurzi
                for (number = 1; number < 10; number++)
                {
                    if (hinty[x, y, number])
                    {
                        start:
                        SetTile(x, y, number, true);
                        MakeImplications();
                        //zjistím jak na tom jsme
                        bool isComplete = IsSudokuComplete();
                        bool isValid = IsSudokuValid(true);
                        
                        //konec
                        if (isComplete && isValid)
                        {
                            return;
                        }
                        //pokračujem
                        if (!isComplete && isValid)
                        {
                            TakeGuess(x, y, 1);
                            return;
                        }
                        //vracíme se
                        if (!isValid)
                        {
                            delete:
                            Move lastMove = pastMoves[pastMoves.Count() - 1];
                            while (!lastMove.isGuess)
                            {
                                //vrátí krok
                                pole[lastMove.row, lastMove.column] = 0;

                                pastMoves.RemoveAt(pastMoves.Count() - 1);
                                lastMove = pastMoves[pastMoves.Count() - 1];
                            }
                            //poslední krok je typnutý
                            number = lastMove.number+1;
                            pole[lastMove.row, lastMove.column] = 0;
                            x = lastMove.row;
                            y = lastMove.column;
                            pastMoves.RemoveAt(pastMoves.Count() - 1);
                            if (number > 9)
                            {
                                goto delete;
                            }

                            //vypočítat znovu hinty
                            ResetHints();
                            UpdateHints();
                            goto start;
                        }
                    }
                }
            }
            y++;
            if (y > 8)
                x++;
            y %= 9;
            if (y > 8 || x > 8)
                return;
            TakeGuess(x, y, 1);
        }
        /// <summary>
        /// Vrací string určující správnost sudoku. Spouští na každý řádek, sloupec a sektor následující funkce:
        /// <para>Na každém řádku spustí <see cref="IsRowValid(int rowId)"/>. Pokud najde chybu vrací "řádek " + (číslo řádku).</para>
        /// <para>Na každém sloupci spustí <see cref="IsColumnValid(int columnId)"/>. Pokud najde chybu vrací "sloupec " + (číslo sloupce).</para>
        /// <para>V každém sektoru spustí <see cref="IsSectorValid(int bigRow, int bigColumn)"/>. Pokud najde chybu vrací "sektor " + (číslo sektoru).</para>
        /// <para>Pokud nenajde chybu vrací "správně"</para>
        /// </summary>
        /// <returns></returns>
        static string IsSudokuValid()
        {
            for (int row = 0; row < 9; row++)
            {
                if (!IsRowValid(row))
                    return "řádek " + row;
            }

            for (int column = 0; column < 9; column++)
            {
                if (!IsColumnValid(column))
                    return "sloupec " + column;
            }

            for (int bigRow = 0; bigRow < 3; bigRow++)
            {
                for (int bigColumn = 0; bigColumn < 3; bigColumn++)
                {
                    if (!IsSectorValid(bigRow, bigColumn))
                        return "sektor " + bigRow + " " + bigColumn;
                }
            }

            for (int row = 0; row < 9; row++)
            {
                for (int column = 0; column < 9; column++)
                {
                    if (!IsTileValid(row, column))
                        return "políčko " + row + " " + column;
                }
            }

            return "správně";
        }
        /// <summary>
        /// Vrací (true) pro "správně" a (false) pro zbytek.
        /// <see cref="IsSudokuValid()"/>
        /// </summary>
        /// <param name="returnBool"></param>
        /// <returns></returns>
        static bool IsSudokuValid(bool returnBool)
        {
            for (int row = 0; row < 9; row++)
            {
                if (!IsRowValid(row))
                    return false;
            }

            for (int column = 0; column < 9; column++)
            {
                if (!IsColumnValid(column))
                    return false;
            }

            for (int bigRow = 0; bigRow < 3; bigRow++)
            {
                for (int bigColumn = 0; bigColumn < 3; bigColumn++)
                {
                    if (!IsSectorValid(bigRow, bigColumn))
                        return false;
                }
            }

            for (int row = 0; row < 9; row++)
            {
                for (int column = 0; column < 9; column++)
                {
                    if (!IsTileValid(row, column))
                        return false;
                }
            }

            return true;
        }
        /// <summary>
        /// Spočítá v řádku (rowId) počet všech čísel. Pokud najde jedno číslo 2x, vrací (false), jinak vrací (true).
        /// </summary>
        /// <param name="rowId"></param>
        /// <returns></returns>
        static bool IsRowValid(int rowId)
        {
            bool[] oneToNine = new bool[9];
            for (int i = 0; i < 9; i++)
            {
                oneToNine[i] = false;
            }

            for (int column = 0; column < 9; column++)
            {
                //převde údaj z tabulky na id
                int currentNumber = pole[rowId, column] - 1;
                //nepočítá nulu
                if (currentNumber < 0)
                    continue;
                if (oneToNine[currentNumber])
                    return false;
                oneToNine[currentNumber] = true;
            }

            return true;
        }
        /// <summary>
        /// Spočítá ve sloupci (columnId) počet všech čísel. Pokud najde jedno číslo 2x, vrací (false), jinak vrací (true).
        /// </summary>
        /// <param name="columnId"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Spočítá v sektoru (bigRow, bigColumn) počet všech čísel. Pokud najde jedno číslo 2x, vrací (false), jinak vrací (true).
        /// </summary>
        /// <param name="bigRow"></param>
        /// <param name="bigColumn"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Zjistí jestli na políčku (row, column) může být nějaké číslo.
        /// <para>Jestli na políčku může být 0 čísel --> (false)</para>
        /// <para>Jinak --> (true)</para>
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        static bool IsTileValid(int row, int column)
        {
            if (pole[row, column] != 0)
                return true;
            int possibleNumbers = 0;
            for (int hint = 1; hint < 10; hint++)
            {
                if (hinty[row, column, hint])
                {
                    possibleNumbers++;
                }
            }
            if (possibleNumbers == 0)
                return false;
            else
                return true;
        }
        /// <summary>
        /// Pokud najde prázdné políčko, není sudoku vyplněné a vrací (false), jinak vrátí (true).
        /// </summary>
        /// <returns></returns>
        static bool IsSudokuComplete()
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
        /// <summary>
        /// Vytiskne sudoku na výstup.
        /// <para>pretty = false --> 9x9 čísel</para>
        /// <para>pretty = true --> Tabulka s oddělynými sektory. Barevně odlišené zadání od vyplněných políček.</para>
        /// </summary>
        /// <param name="pretty"></param>
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
                Console.ForegroundColor = ConsoleColor.White;
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
                        {
                            ConsoleColor before = Console.ForegroundColor;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.Write(".");
                            Console.ForegroundColor = before;
                        }
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

        #region experimental
        static void LoadFromFile(string fileName)
        {
            string[] lines = System.IO.File.ReadAllLines(fileName);
            for (int line = 0; line < lines.Length; line+=2)
            {
                sudokus.Add(lines[line]);
            }
        }
        static void SudokuToTask(string sudoku)
        {
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    if (sudoku[x * 9 + y] == '.')
                        task[x, y] = 0;
                    else
                        task[x, y] = (int)sudoku[x * 9 + y]-48;
                }
            }
        }
        #endregion
    }
}