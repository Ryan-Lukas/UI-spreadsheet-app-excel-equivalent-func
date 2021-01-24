Name: Ryan Lukas
Data: 9/27/2018


# Design #

Spreadsheet constructor:	
	This project will be laid out such that spreadsheet will be the builder and holder of cell objects. This will have a map of all cell objects that have data 
implemented inside of it. This will be held in a Dictionary data structure with <name, cell object>. Cell objects will contain the name, contents, value, and 
dependents/dependees. Other constructors get file name (if you want to load a previous file), isvalid (tells if variables are valid), normalizer(changes variables to specific representations), and version (the given version of peoples api).

GetSavedVersion:
	This method returns the sepecified version of the file passed in.

Save:
	This method saves all values inside spreadsheet and puts it into an xml document.

GetCellvalue:
	This method will find the name thats passed through in the Dictionary and return the cell contents by calling GetValue of a cell object. Contents can be
either double, string, or a FormulaError. 

GetCellContents:
	This method will find the name thats passed through in the Dictionary and return the cell contents by calling GetContents of a cell object. Contents can be
either double, string, or a Formula. 

SetCellContents:
	This method will take a name and add it to a cell file if it is not in the Dictionary. If it is in the dictonary, it will replace the contents of the cell.
This will return an ISET that contains all the cells that have to be recalculated/that need to be changed.

SetContentsOfCell:
	This method will check inputs to see if they are valid. Then they will call specific SetCellContents if it is a formula, double, or text.

GetDirectDependents:
	This method will return the names dependents.

PS2 File Last Commit - 05e0203
PS3 File Last Commit - 0fe3b3a

Extra comments:
	In my ps2, I still have yet to finish and get working tests 15-17 stress tests. It's accidentally creating a copy in my dependents and still need to look into it.
		