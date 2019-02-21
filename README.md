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
  

Pro všechna zadání v jednom běhu vypočítá:
* Průměrný počet použití nelogického hádání
* Maximální počet použití nelogického hádání
* Celková maximální hloubka vnoření

## Algoritmus
### Logika
Program se snaží co nejvíce využívat pravidel hry. Vždy než začne program hádat, jsou vyčerpané všechny základní logické kroky.
Používá systém nápověd, kdy o každém políčku víme všechna čísla, která jsou do něj možná zapsat. Poté používá tyto 4 algoritmy
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
  

Pokud našel chybu, automaticky vrací kroky.

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

Následující 4 globální proměnné obsahují vše potřebné pro řešení. Většinou se jedná o několika rozměrná pole, pro organizované uložení všech dat o mřížce.

* Zadání sudoku, po dobu řešení se nemění, je důležité pro výpis. `task[x,y]` je číslo na souřadnicích `x`, `y`. Pokud je pole prázdné `task[x,y] = 0`. 

`static int[,] task = new int[9, 9];`

* V  proměnné`pole` je uložen stav v průběhu řešení. Má stejnou strukturu jako `int[,] task`. Tato proměnná je využívaná všemi funkcemi na řešení.

`static int[,] pole = new int[9, 9];`

* V tomto trojrozměrném poli jsou uloženy možnosti pro každé políčko. Má klasickou strukturu jako dvě předchozí proměnné, ale navíc má třetí rozměr, ve kterém je pro každé číslo uložen `bool`. Ten říká, jestli číslo `n` může být na souřadnicích `x`, `y`.  Pokud tam číslo může být, tak `hinty[x, y, n] = true`. A jestli ne, tak `hinty[x, y, n] = false`.

`static bool[,,] hinty = new bool[9, 9, 10];`

* V tomto dynamickém poli jsou uloženy všechny minulé kroky. Díky metodě řešení, kterou jsem si zvolil, musí být postup od zadání k aktuálnímu stavu lineární, takže vždy půjde postup zaznamenat do lineárního pole.

`static List<Move> pastMoves = new List<Move>();`

##### Statistika

Pro statistické účely jsem zavedl několik proměnných počítajících kroky řešení a hlavně údaje o průběhu rekurze.

### Hlavní funkce

#### Uživatelské rozhraní

Program musí zajistit určitou úroveň uživatelského rozhraní. Výstup je konzolový. Využívá možností obarvování a formátování textu v C#.

* WelcomeNote() - Návod a přivítání.
* Info() - Informace o vyřešeném sudoku.
  * Info(true) - Kompaktní zobrazení na jeden řádek.
  * Info(flase) - Estetické zobrazení na více řádků.
* EndInfo() - Statistické údaje po vyřešení všech zadání.
* PrintSudoku() - Vypíše `pole` na výstup.
  * PrintSudoku(true) - V tabulce, zvýrazní zadání.
  * PrintSudoku(false) - 9x9 čísel

#### Hinty

Mezi funkce upravující proměnnou `hinty` patří:

* ResetHints() - Na každém políčku je možné každé číslo.
* UpdateHints() - Spouští následující funkce:
  * SetColumnHints() - Využije pravidlo o opakování ve sloupci.
  * SetRowHints() - Využije pravidlo o opakování v řádku.
  * SetSectorHints() - Využije pravidlo o opakování v sektoru.
* DeleteHintsOnTile() - Pokud je políčko vyplněné, nemůže na něm být žádné jiné číslo.

#### Logické

* SetTile() - Zapíše číslo na políčko a zaznamená krok do `pastMoves`.
* MakeImplications() - Spouští na `pole` následující funkce:
  * RowImplications() - V řadě najde, jestli nemůže něco doplnit.
  * ColumnImplications() - Ve sloupci najde, jestli nemůže něco doplnit.
  * SectorImplications() - V sektoru najde, jestli nemůže něco doplnit.
  * TileImplications() - Na políčku hledá, jestli nemůže něco doplnit.
  * Pokud jedna z funkcí něco změní, spouští se MakeImplications() znova.

#### Rekurzivní

* TakeGuess() - Rekurzivně hádá čísla na nedoplněných políčkách. 
  - RowImplications() - V řadě najde, jestli nemůže něco doplnit. Více viz. algoritmus.

#### Testovací

* IsSudokuValid() - Testuje, zda v sudoku není chyba. Používá následující funkce:
  * IsRowValid() - V řádku se nic neopakuje.
  * IsColumnValid() - Ve slouci se nic neopakuje.
  * IsSectorValid() - V sektoru se nic neopakuje.
  * IsTileValid() - Testuje, zda je na políčku číslo, nebo alespoň jedno možné číslo.
* IsSudokuComplete() - Testuje, zda jsou všechna políčka vyplněná.

#### Experimentální

Nepřidávají funkčnost algoritmu, pouze na testování a pro budoucí funkce.

* LoadFromFile() - Načítá zadání ze souboru.
* SudokuToTask() - Po načtení ze souboru vloží jedno zadání do proměnné `task`.
* RotateTask90Deg() - Otočí zadání o 90 stupňů.
* TrySameSudokuRotated() - Pokud je sudoku moc složité na výpočet pomocé rekurze, otočí ho a zkusí to znovu.

#### Ostatní

* InitializeSudoku() - Přepíše zadání a obnoví hodnoty proměnných na původní hodnotu.

## Alternativní řešení

### Logická vylepšení

Existuje množství logických úvah, které obyčejný člověk při řešení sudoku používá, ale v programu nejsou implementovány. Hodně z nich je popsáno na [této stránce](https://www.kristanix.com/sudokuepic/sudoku-solving-techniques.php).

### Praktická vylepšení

Při návrhu tohoto rekurzivního řešení probíhá hádání čísel z levého horního rohu (souřadnice 0,0). Poté se postupuje po řádcích a to může mít za efekt jednu nepříjemnost. Jelikož je tento algoritmus předvídatelný, můžeme navrhnout zadání, které bude trvat na vyřešení velmi dlouho. Proto je dobré se dívat na zadání ze 4 různých směrů. Když proměnná `numberOfGuesses` určující "computational complexity", neboli složitost výpočtu přesáhne určitou hranici, můžeme řešení pozastavit, otočit zadání o 90° a zkusit řešit toto. Ukázalo se, že v náhodně generovaných zadáních se objevují tyto složitě řešitelná zadání. A proto je tento krok užitečný i v běžné praxi, nejen když necháváme program vyřešit zadání, navržená kompletně proti jeho schopnostem.

Existuje i spousta dalších míst v kódu, kdy předpokládáme určitý postup, který při návrhu nepříjemného zadání můžeme použít. Například to, že hádá od nejmenších k největším. Příkladem nepříjemného sudoku pro tento předpoklad je řešení, kde první řádek obsahuje 987654321, ale v zadání je prázdný.

Takto můžeme udělat variace několika předpokladů a přepínat mezi nimi, v moment, kdy dojde k přesáhnutí určitého času řešení. Nejpokročilejším protivníkem nepříjemných zadání bude varianta, kde hádáme na náhodných políčkách náhodná čísla. Tato varianta je ale velice náročná na sledování postupu řešení, tedy na délu kódu.

## Komunikace s programem

### Vstupní data

Pro běh programu je nutná tato souborová struktura:

* [Hlavní složka]
  * složka "zadani"
    * textový soubor "jednoduche.txt"
    * nebo textový soubor "extremni.txt"
  * složka s programem

Jméno souboru je defaultně "jednoduche.txt". Lze upravit v kódu.

Soubor se zadáním může obsahovat neomezeně zadání. Je nutné speciální formátování.

* Jedno zadání je na jednom řádku.
  * Mřížku 9x9 rozložíme na jednotlivé řádky a ty poskládáme za sebe.
  * Prázdná políčka reprezentujeme znakem `.` a čísla reprezentujeme čísly `1`-`9`.
  * Po zadání vždy následuje prázdný řádek.

Pokud takovýto soubor obsahující zadání existuje, můžeme spustit program.

### Výstupní data

Výstup probíhá přímo do konzole. Uživatel je prováděn samotným programem.

Pro správné zobrazení nápovědy doporučujeme šířku konzole 100 znaků.

Při výstupu záleží na nastavení funkcí.

Defaultně je výstupem:

1. Vyřešená mřížka se zvýrazněným zadáním.
2. Informace, zda je sudoku validní.
3. Informace, zda je sudoku kompletní.
4. Výpočetní složitost.
5. Maximální hloubka rekurze.

Po dopočítání všech zadání program vypíše statistiky:

1. Průměrnou výpočetní složitost.
2. Maximální výpočetní složitost.
3. Maximální hloubku rekurze.

## Osobní zhodnocení

### Průběh práce

Práce na programu probíhala ve dvou vlnách.

První z nich byla hned ze začátku, kdy jsem naprogramoval veškeré logické kroky. Už od začátku jsem se snažil o strukturu programu, která mi v budoucnu nebude škodit. Poté jsem bez okomentování od programu na dva měsíce odešel.

Druhá vlna byla těsně před termínem. Začal jsem porozumněním kódu, což bylo jednak ulehčeno strukturou funkcí, ale zkomplikováno neokomentováním. V rámci porozumnění jsem vše podrobně okomentoval. Snažil jsem se doprogramovat rekurzi efektivní cestou, kdy není nutné kopírovat mřížky mezi jednotlivmi zanořeními. Tohoto rozhodnutí nelituji i když jsem nad tím strávil mnohem více času.

### Co nebylo doděláno

Ke konci práce jsem dostal několik dobrých nápadů na optimalizaci.

* Urychlení řešení složitých zadání otočením o 90°.
* Optimalizace systému nápověd, kdy každé políčko má i množství zbývajících čísel, ne jen pole, ze kterého to musím pokaždé vyčíst.
* Rozšíření na generování zadání.
* Určení, zda je moje řešení jediné. Pravidla sice nepožadují, aby hráč našel všechna řešení, ale s počítačem je to jednoduše možné.


### Co jsem se naučil
Tento projekt mě naučil vhodně popisovat části programu pomocí markup jazyka dostupného ve Visual Studiu. Také jsem k programu napsal dokumentaci.
Vyzkoušel jsem si algoritmus užívající komplexní rekurzi.
Rozmyslel jsem si několik nápadů na zlepšení, které nejsou nutné, ale zapsal si je pro budoucí vylepšení.
A v neposlední řadě jsem dokázal triviální řešení s velkou paměťovou složitostí optimalizovat.

​	