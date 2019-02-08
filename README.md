# SudokuSolver

### Autor
Lukáš Caha
### Pro předmět
NPRG030 - Programování I
## Anotace
Tento program je navržen na řešení hádanek sudoku. K řešení se dostane kombinací logiky a hrubé síly.
Měl by dokázat vyřešit, každé řešitelné zadání. Snaží se ale uplatňovat více logiky, než hrubé síly.

Řeší zadání v lineární paměti v souvislosti s rekurzí. Rekurze nepotřebuje duplikovat tabulku, tvoří pouze pár pomocných proměnných. 

### Funkce a limitace
Na vstupu je libovolný počet zadání mřížek 9x9. 
Program řeší pouze variantu sudoku 9x9, která je popsaná [zde](https://cs.wikipedia.org/wiki/Sudoku).

Výstupem pro každé zadání můžou být následující věci:
* Vyplněná mřížka
* Statistické údaje o zadání
  * Počet použítí nelogického hádání
  * Maximální hloubka vnoření
  
Pro všechny zadání v jednom běhu vypočítá:
* Průměrný počet použítí nelogického hádání
* Maximální počet použítí nelogického hádání
* Maximální celková hloubka vnoření

## Algoritmus
### Logika
Program se snaží co nejvíce využívat pravidel hry. Vždy než začne program hádat, jsou vyčerpané všechny základní logické kroky.
Používá systém nápověd, kdy o každém políčku víme všechny čísla, která jsou do něj možná zapsat. Poté používá tyto 4 algoritmy
na logické odvození co nejvíce čísel:
* Řádková logika
  * V řádku spočítá počet možných pozic pro číslo N. Pokud najde právě jednu možnou pozici, umístí tam N.
* Sloupcová logika
  * Ve slouci spočítá počet možných pozic pro číslo N. Pokud najde právě jednu možnou pozici, umístí tam N.
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
Pak není nutné kopírovat mřížky v jednotlivých krocích. A jelikož je maximálně 81 nedoplňených políček, tak 
je maximálně 81 záznamů o krocích. Což dělá paměťovou složitost lineární s počtem vnoření rekurze.

Proto bylo nutné k rekurzivnímu prohledávání nutné doprogramovat určité množství logiky, která určitě sníží výpočetní čas.
Nemáme však jisté bez hluboké analýzy, zda vyřeší každé zadání. 
Se zmíněnými čtyřmi logickými úvahami je možné v průměru snižit hloubku vnoření do jednotek a počet vnoření pod stovku.

Je však možné vymyslet více logiky, které sníží výpočetní čas ještě více. A může některé zadání učinit řešitelné bez rekurze. 
Avšak dle mého názoru není dobré věnovat podstatně více času složitější konstrukci, která urychlí výpočet jen o jednotky (2x až 9x). 
Existuje totiž mnohem jednodušší řešení, které zadání vždy vyřeší (díky rekurzi) a je zároveň urychlené o řády (jednoduchou logikou).








## Co jsem se naučil
Tento projekt mě naučil vhodně popisovat části programu pomocí markup jazyka dostupného ve Visual Studiu. 
Vyzkoušel jsem si algoritmus užívající komplexní rekurzi.
Rozmyslel jsem si několik nápadů na zlepšení, které nejsou nutné, ale zapsal si je pro budoucí vylepšení.
A v neposlední řadě jsem dokázal triviální řešení s velkou paměťovou složitostí optimalizovat.










