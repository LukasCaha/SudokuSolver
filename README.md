# SudokuSolver

### Autor
Lukáš Caha
### Pro předmět
NPRG030 - Programování I

Matematicko-fyzikální fakulta Univerzity Karlovy, Softwarové a datové inženýrství

## Anotace
Tento program je navržen na řešení hádanek sudoku. K řešení se dostane kombinací logiky a hrubé síly.
Měl by dokázat vyřešit každé řešitelné zadání. Snaží se ale uplatňovat více logiky, než hrubé síly.

Řeší zadání v lineární paměti v souvislosti s rekurzí. Rekurze nepotřebuje duplikovat tabulku, tvoří pouze několik pomocných proměnných. 

### Funkce a limitace
Na vstupu je libovolný počet zadání mřížek 9x9. 
Program řeší pouze variantu sudoku 9x9, která je popsaná [zde](https://cs.wikipedia.org/wiki/Sudoku).

Výstupem pro každé zadání můžou být následující věci:
* Vyplněná mřížka
* Statistické údaje o zadání
  * Počet použití nelogického hádání
  * Maximální hloubka vnoření
  

Pro všechny zadání v jednom běhu vypočítá:
* Průměrný počet použití nelogického hádání
* Maximální počet použití nelogického hádání
* Celková maximální hloubka vnoření

## Algoritmus
### Logika
Program se snaží co nejvíce využívat pravidel hry. Vždy než začne program hádat, jsou vyčerpané všechny základní logické kroky.
Používá systém nápověd, kdy o každém políčku víme všechny čísla, která jsou do něj možná zapsat. Poté používá tyto 4 algoritmy
na logické odvození co nejvíce čísel:

* Řádková logika
  * V řádku spočítá počet možných pozic pro číslo N. Pokud najde právě jednu možnou pozici, umístí tam N.
* Sloupcová logika
  * Ve slopuci spočítá počet možných pozic pro číslo N. Pokud najde právě jednu možnou pozici, umístí tam N.
* Sektorová logika
  * V sektoru 3x3 spočítá počet možných pozic pro číslo N. Pokud najde právě jednu možnou pozici, místí tam N.
* Políčková logika
  * Pokud je na políčku pouze jedno možné číslo, zapíše ho do políčka.

### Nelogické rekurzivní hádání
Když dojdou programu logické možnosti řešení, zkusí postupně hádat čísla.
Má v sobě implementované dvě metriky pro rozhodování o dalším postupu.

* Je v sudoku chyba
  * Hledá kolize čísel
  * Pokud na prázdném políčku není možné žádné číslo
* Je sudoku kompletní
  * Jsou všechna políčka vyplněná
  

Pokud našel chybu automaticky vrací kroky.

Pokud nenašel chybu a není u konce, pokračuje.

Pokud nenašel chybu a je u konce, vyřešil sudoku.

### Výběr algoritmu
Hrubou silou je možné vyřešit libovolné sudoku. Pak je úloha otázkou času a výkonu počítače.
Z pohledu paměťové složitosti je řešení navrhnuto velmi ekonomicky. Je použit záznamník kroků, který umožňuje vracet nevalidní kroky.
Pak není nutné kopírovat mřížky v jednotlivých krocích. A jelikož je maximálně 81 nedoplněných políček, tak je maximálně 81 záznamů o krocích. Což dělá paměťovou složitost lineární s počtem vnoření rekurze.

Kvůli časové náročnosti rekurzivního prohledávání bylo nutné doprogramovat určité množství logiky, která určitě sníží výpočetní čas.
Nemáme však jisté bez podrobné analýzy, zda tato logika vyřeší každé zadání. 
Se zmíněnými čtyřmi logickými úvahami je možné v průměru snížit hloubku vnoření na jednotky a počet vnoření na desítky.

Je však možné vymyslet více logiky, které sníží výpočetní čas ještě více. A může některé zadání učinit řešitelné bez rekurze. 
Avšak dle mého názoru není dobré věnovat podstatně více času složitější konstrukci, která urychlí výpočet jen o jednotky (2x až 9x). 
Existuje totiž mnohem jednodušší řešení, které zadání vždy vyřeší (díky rekurzi) a je zároveň urychlené o řády (jednoduchou logikou).

## Program

### Datové struktury a proměnné

#### Ukládání kroků

V průběhu řešení si program ukládá kroky, aby je mohl při rekurzi vracet. Jde o úsporu paměti, jelikož všechny stavy řešení jde vyjádřit původním zadáním + tímto záznamem kroků.

`struct Move
{
 	public int row;
	public int column;
	public int number;
	public bool isGuess;
}`

#### Globální proměnné

##### Řešení

Následující 4 globální proměnné obsahují vše potřebné pro řešení. Většinou se jedná o více rozměrné pole, pro organizované uložení všech dat o mřížce.

* Zadání sudoku, po dobu řešení se nemění, je důležité pro výpis. `task[x,y]` je číslo na souřadnicích `x`, `y`. Pokud je pole prázdné `task[x,y] = 0`. 

`static int[,] task = new int[9, 9];`

* V  proměnné`pole` je uložen stav v průběhu řešení. Má stejnou strukturu jako `int[,] task`. Tato proměnná je využívaná všemi funkcemi na řešení.

`static int[,] pole = new int[9, 9];`

* V tomto trojrozměrném poli jsou uloženy možnosti pro každé políčko. Má klasickou strukturu jako dvě předchozí proměnné, ale navíc má třetí rozměr, ve kterém je pro každé číslo uložen `bool`. Ten říká jestli číslo `n` může být na souřadnicích `x`, `y`.  Pokud tam číslo může být, tak `hinty[x, y, n] = true`. A jestli ne tak `hinty[x, y, n] = false`.

`static bool[,,] hinty = new bool[9, 9, 10];`

* V tomto dynamickém poli jsou uloženy všechny minulé kroky. Díky metodě řešení, kterou jsem si zvolil musí být postup od zadání k aktuálnímu stavu lineární, takže vždy půjde postup zaznamenat do lineárního pole.

`static List<Move> pastMoves = new List<Move>();`

##### Statistika

Pro statistické účely jsem zavedl několik proměnných počítající kroky řešení a hlavně údaje o průběhu rekurze.

### Hlavní funkce

#### Logické kroky

##### Uživatelské rozhraní

Program musí zajistit určitou úroveň uživatelského rozhraní. Výstup je konzolový. Využívá možností obarvování a formátování textu v C#.

* WelcomeNote() - Návod a přivítání.
* Info() - Informace o vyřešeném sudoku.
  * Info(true) - Kompaktní zobrazení na jeden řádek.
  * Info(flase) - Estetické zobrazení na více řádků.
* EndInfo() - Statistické údaje po vyřešení všech zadání.
* PrintSudoku() - Vypíše `pole` na výstup.
  * PrintSudoku(true) - V tabulce, zvýrazní zadání.
  * PrintSudoku(false) - 9x9 čísel

##### Hinty

Mezi funkce upravující proměnnou `hinty` patří:

* ResetHints() - Na každém políčku je možné každé číslo.
* UpdateHints() - Spouští následující funkce:
  * SetColumnHints() - Využije pravidlo o opakování ve sloupci.
  * SetRowHints() - Využije pravidlo o opakování v řádku.
  * SetSectorHints() - Využije pravidlo o opakování s sektoru.
* DeleteHintsOnTile() - Pokud je políčko vyplněné, nemůže na něm být žádné jiné číslo.

##### Logické

* SetTile() - Zapíše číslo na políčko a zaznamená krok do `pastMoves`.
* MakeImplications() - Spouští na `pole` následující funkce:
  * RowImplications() - V řadě najde, jestli nemůže něco doplnit.
  * ColumnImplications() - Ve sloupci najde, jestli nemůže něco doplnit.
  * SectorImplications() - V sektoru najde, jestli nemůže něco doplnit.
  * TileImplications() - Na políčku hledá, jestli nemůže něco doplnit.
  * Pokud jedna z funkcí něco změní, spouští se MakeImplications() znova.
* TakeGuess() - Rekurzivně hádá čísla na nedoplněných políčkách. 
  - RowImplications() - V řadě najde, jestli nemůže něco doplnit. Více viz. algoritmus.

##### Testovací

* IsSudokuValid() - Testuje zda v sudoku není chyba. Používá následující funkce:
  * IsRowValid() - V řádku se nic neopakuje.
  * IsColumnValid



## Alternativní řešení

## Komunikace s programem

### Vstupní data

### Výstupní data






## Co jsem se naučil
Tento projekt mě naučil vhodně popisovat části programu pomocí markdown jazyka dostupného ve Visual Studiu. 
Vyzkoušel jsem si algoritmus užívající komplexní rekurzi.
Rozmyslel jsem si několik nápadů na zlepšení, které nejsou nutné, ale zapsal si je pro budoucí vylepšení.
A v neposlední řadě jsem dokázal triviální řešení s velkou paměťovou složitostí optimalizovat.