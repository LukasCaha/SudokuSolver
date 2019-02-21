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
        /// <summary>
        /// <para>Struktura reprezentující jeden "krok" ve vyplňování sudoku. 
        /// Jelikož můžu napsat číslo pouze na prázdnou pozici, stačí nám zaznamenat pozici (row, column) a číslo (number), které jsme na pozici zapsali.
        /// Proměnná (isGuess) je true u kroků, které nejsou podloženy logikou vycházející z pravidel.</para>
        /// 
        /// <para>Struktura obsahuje konstruktor, který přebírá všechny hodnoty proměnných a předává je stuktuře.</para>
        /// </summary>
        public struct Move
        {
            public int row;
            public int column;
            public int number;
            public bool isGuess;
            /// <summary>
            /// Konstruktor struktury <see cref="Move"/>
            /// </summary>
            /// <param name="r">Řádek</param>
            /// <param name="c">Sloupec</param>
            /// <param name="n">Číslo</param>
            /// <param name="g">Je tenhle krok logicky podložený --> (false) jinak --> (true)</param>
            public Move(int r, int c, int n, bool g)
            {
                row = r;
                column = c;
                number = n;
                isGuess = g;
            }
        }

        /// <summary>
        /// Dvojrozměrné pole ve kterém je uložené zadání.
        /// <para>Později se hodí mít zadání uložené, abychom rozpoznali, které čísla můžeme nebo nemůžeme měnit.</para>
        /// <para>Na každém políčku můžou být čísla (1-9) --> hodnoty vyplnění, nebo (0) --> políčko je prázdné.</para>
        /// </summary>
        static int[,] task = new int[9, 9];
        /// <summary>
        /// Tato proměnná má v sobě uloženou aktuální pozici mřížky na sudoku.
        /// <para>Na každém políčku můžou být čísla (1-9) --> hodnoty vyplnění, nebo (0) --> políčko je prázdné.</para>
        /// </summary>
        static int[,] pole = new int[9, 9];
        /// <summary>
        /// V tomto trojrozměrním poli je uložené, jestli na souřadnicích (x,y) může být číslo (n) --> <code>hinty[x, y, n]</code>
        /// <para><code>hinty[x, y, n]</code> (true) --> může být, (false) --> nemůže být</para>
        /// </summary>
        static bool[,,] hinty = new bool[9, 9, 10];
        /// <summary>
        /// List všech kroků, které vedli ze zadání do aktuální podoby, používané na vracení tahů.
        /// <para><code>pastMoves.RemoveAt(pastMoves.Count() - 1)</code> vrátí poslední krok.</para>
        /// <para><code>pastMoves.Add(<see cref="Move"/>)</code> vrátí poslední krok.</para>
        /// </summary>
        static List<Move> pastMoves = new List<Move>();

        //benchmarks nepřispívá k řešení
        static int numberOfGuesses = 0;
        static int depth = 0;
        static int maxDepth = 0;
        static int maxOverallDepth = 0;
        static int averageComplexity = 0;
        static int maximumComplexity=0;
        static int counter=0;
        static List<string> sudokus = new List<string>();
        #endregion

        static void Main(string[] args)
        {
            //Uvítání uživatele
            WelcomeNote();
            //Načte všechna zadání ze souboru
            LoadFromFile("../zadani/jednoduche.txt");
            //LoadFromFile("../zadani/extremni.txt");
            
            foreach (string sudoku in sudokus)
            {
                //přenese sudoku do (task) pole
                SudokuToTask(sudoku);
                //připraví a resetuje vše pro řešení nového sudoku
                InitializeSudoku();
                //vytiskne zadání
                //PrintSudoku(true);
                //obnoví všechny nápovědy
                UpdateHints();
                //zkusí řešit sudoku logicky
                MakeImplications();
                //začne střídavě rekurzivně hádat a logicky řešit sudoku
                TakeGuess(0, 0);
                //vytiskne řešení
                PrintSudoku(true);
                //vytiskne údaje o řešení sudoku
                Info(false);
            }

            //konec
            EndInfo();
            Console.ReadKey();
        }
        /// <summary>
        /// Uvítací text. Obsahuje základní informace o funkčnosti programu.
        /// </summary>
        static void WelcomeNote()
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.WriteLine("Vítej v programu na řešení sudoku.");
            Console.WriteLine("\tAutorem je Lukáš Caha z MFF UK, toto je zápočtový program pro Programování I");
            Console.WriteLine("\nCo program umí?");
            Console.WriteLine("\t1. Řeší sudoku z libovolného zadání");
            Console.WriteLine("\t2. Umí načítat zadání ze souboru");
            Console.WriteLine("\t3. Řešení u většiny zadání vypočítá v řádu milisekund");
            Console.WriteLine("\t4. Vypisuje statistiky jednotilvých zadání a také souhrné statitstiky všech zadání");
            Console.WriteLine("\nPro nápovědu zmáčkni tlačítko H");
            Console.WriteLine("Pro pokračování zmáčkni libovolné tlačítko");
            ConsoleKey key = Console.ReadKey().Key;
            if (key == ConsoleKey.H)
            {
                Console.Clear();
                Console.WriteLine("+-------------------------------+---------------------------------------+-----------------------+");
                Console.WriteLine("| Název\t\t\t\t| Vysvětlení \t\t\t\t| Možné hodnoty \t|");
                Console.WriteLine("+-------------------------------+---------------------------------------+-----------------------+");
                Console.WriteLine("| Computational complexity \t| počet nelogických hádání čísel, \t| (0-infinity)\t\t|");
                Console.WriteLine("|\t\t\t\t| neobjektivní hodnocení složitosti \t|\t\t\t|");
                Console.WriteLine("|\t\t\t\t| zadání \t\t\t\t|\t\t\t|");
                Console.WriteLine("+-------------------------------+---------------------------------------+-----------------------+");
                Console.WriteLine("| Maximal depth \t\t| maximální hloubka vnoření rekurze, \t| (0-infinity)\t\t|");
                Console.WriteLine("|\t\t\t\t| neobjektivní hodnocení složitosti \t|\t\t\t|");
                Console.WriteLine("|\t\t\t\t| zadání \t\t\t\t|\t\t\t|");
                Console.WriteLine("+-------------------------------+---------------------------------------+-----------------------+");
                Console.WriteLine("Pro pokračování zmáčkni libovolné tlačítko");
                key = Console.ReadKey().Key;
            }
        }

        /// <summary>Info je funkce vypysující shrnující informace o aktuálním stavu sudoku.
        /// <para>Nejprve zjistí pomocí funkce <see cref="IsSudokuValid()"/> jestli nějaké dvě čísla v mřížce kolidují.</para>
        /// <para>Poté zkontroluje jestli je sudoku plně vyplněné funkcí <see cref="IsSudokuComplete()"/>.</para>
        /// <para>Tato funkce je čistě informativní a nemá vliv na chod programu.</para>
        /// </summary>
        static void Info(bool compact)
        {
            if (compact)
            {
                if (IsSudokuValid(true) && IsSudokuComplete())
                {
                    maximumComplexity = Math.Max(maximumComplexity, numberOfGuesses);
                    averageComplexity = (averageComplexity * counter + numberOfGuesses) / ++counter;
                    Console.WriteLine("complex:" + numberOfGuesses + " \taverage:" + averageComplexity + " \tmax complex:" + maximumComplexity + " \tmax depth:" + maxDepth);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("Error: " + IsSudokuValid() + " | " + IsSudokuComplete());
                    Console.ReadKey();
                }
            }
            else
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
                maximumComplexity = Math.Max(maximumComplexity, numberOfGuesses);
                averageComplexity = (averageComplexity * counter + numberOfGuesses) / ++counter;
                Console.WriteLine("Computational complexity:\t" + numberOfGuesses);
                Console.WriteLine("Maximal depth:\t\t\t" + maxDepth);
            }
        }
        /// <summary>
        /// Finální statistické údaje o všech řešených sudoku.
        /// </summary>
        static void EndInfo()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\nFinal statistics");
            Console.WriteLine("Average computational complexity:\t" + averageComplexity);
            Console.WriteLine("Maximal computational complexity:\t" + maximumComplexity);
            Console.WriteLine("Maximal overall depth:\t\t\t" + maxOverallDepth);
        }
        /// <summary>Připraví program na řešení nového sudoku
        /// <para>Přepíše zadání do řešící mřížky a zachovává původní zadání pro pozdější využití.</para>
        /// <para>Resetuje všechny ostatní proměnné.</para>
        /// </summary>
        static void InitializeSudoku()
        {
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
            depth = 0;
            maxDepth = 0;
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
        /// <param name="row">Řádek do kterého vkládáme.</param>
        /// <param name="column">Sloupeček do kterého vkládáme.</param>
        /// <param name="number">Číslo které vkládáme.</param>
        /// <param name="isGuess">Jestli je tento krok logicky podložený --> (false) nebo je pouhým typem --> (true).</param>
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
                depth++;
                maxDepth = Math.Max(depth,maxDepth);
                maxOverallDepth = Math.Max(maxDepth, maxOverallDepth);
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
        /// Funkce rekurzivně hádá čísla a pokud je uhodne špatně, vrací se zpět pomocí proměnné <see cref="pastMoves;"/>.
        /// </summary>
        /// <param name="x">Řádek aktuálního políčka</param>
        /// <param name="y">Sloupec aktuálního políčka</param>
        static void TakeGuess(int x, int y)
        {
            //pokud je políčko prázdné
            if (pole[x, y] == 0)
            {
                //for + podmínka projdou postupně všechny možné možnosti pro políčko (x,y)
                for (int number = 1; number < 10; number++)
                {
                    if (hinty[x, y, number])
                    {
                        //tento cyklus opouštím pouze přes returny v podmínkách
                        while (true)
                        {
                            //toto číslo pouze hádám
                            SetTile(x, y, number, true);
                            MakeImplications();
                            //zjištění stavu tabulky správnost a kompletnost
                            bool isComplete = IsSudokuComplete();
                            bool isValid = IsSudokuValid(true);

                            //sudoku vyřešené
                            if (isComplete && isValid)
                            {
                                //konec
                                return;
                            }
                            //nevyřešené ale správně
                            if (!isComplete && isValid)
                            {
                                //hádá další čísla
                                TakeGuess(x, y);
                                return;
                            }
                            //vrátím se zpět
                            if (!isValid)
                            {
                                do
                                {
                                    //vrátím všechny logické kroky
                                    Move lastMove = pastMoves[pastMoves.Count() - 1];
                                    while (!lastMove.isGuess)
                                    {
                                        //vrátí krok
                                        pole[lastMove.row, lastMove.column] = 0;

                                        pastMoves.RemoveAt(pastMoves.Count() - 1);
                                        lastMove = pastMoves[pastMoves.Count() - 1];
                                    }
                                    //a pak ještě ten který jsem hádal
                                    pole[lastMove.row, lastMove.column] = 0;
                                    x = lastMove.row;
                                    y = lastMove.column;
                                    number = lastMove.number + 1;
                                    pastMoves.RemoveAt(pastMoves.Count() - 1);
                                    depth--;
                                } while (number > 9);
                                // ^--- pokud se vrátím na políčko, kde jsem už vyčerpal všechny možnosti, je také špatně uhodnuté, tedy se musím vrátit ještě jednou

                                //vypočítat znovu nápovědy
                                ResetHints();
                                UpdateHints();
                            }
                        }
                    }
                }
            }
            //pokud dojdu sem, je jasné, že na políčku (x,y) je číslo => posunu se o políčko
            y++;
            if (y > 8)
                x++;
            y %= 9;
            if (y > 8 || x > 8)
                return;
            //hádám znovu
            TakeGuess(x, y);
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
        static void RotateTask90Deg()
        {
            int[,] copy = new int[9, 9];
            for (int row = 0; row < 9; row++)
            {
                for (int column = 0; column < 9; column++)
                {
                    copy[row, column] = task[row, column];
                }
            }
            for (int row = 0; row < 9; row++)
            {
                for (int column = 0; column < 9; column++)
                {
                    task[row, column] = copy[8-column, row];
                }
            }
        }
        static void TrySameSudokuRotated(int iteration)
        {
            RotateTask90Deg();
            InitializeSudoku();
            UpdateHints();
            MakeImplications();
            TakeGuess(0, 0);
        }

        #endregion
    }
}