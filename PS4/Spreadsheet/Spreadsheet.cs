//Written by Ryan Lukas
//Date: 9/28/2018
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using SpreadsheetUtilities;
using SS;


namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {

        //create dictionary to hold name then hold cell object, cell object should be a new class that holds contents, values, dependencys, etc
        private Dictionary<String, Cell> Cells;
        private DependencyGraph Dependency;
        private bool hasChanged;



        /// <summary>
        /// Creates default Constructor
        /// </summary>
        public Spreadsheet() : base(s => true, s => s, "default")
        {
            Dependency = new DependencyGraph();
            Cells = new Dictionary<string, Cell>();
            hasChanged = false;
        }

        /// <summary>
        /// Creates Constructor with personal normalizer,isvalid, and version
        /// </summary>
        /// <param name="isValid"></param>
        /// <param name="normalize"></param>
        /// <param name="version"></param>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            Dependency = new DependencyGraph();
            Cells = new Dictionary<string, Cell>();
            hasChanged = false;
        }

        /// <summary>
        /// Creates Constructor with personal normalizer,isvalid, version, and pathoffile
        /// </summary>
        /// <param name="pathOfFile"></param>
        /// <param name="isValid"></param>
        /// <param name="normalize"></param>
        /// <param name="version"></param>
        public Spreadsheet(String pathOfFile, Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            Dependency = new DependencyGraph();
            Cells = new Dictionary<string, Cell>();
            loadFile(pathOfFile);
        }

        /// <summary>
        /// This creates and loads a previous file that was given into spreadsheets
        /// This will put cells and contents into the dictionaries
        /// </summary>
        /// <param name="pathOfFile"></param>
        private void loadFile(string pathOfFile)
        {

            string cellName = "";
            string cellContents = "";
            try
            {
                using (XmlReader reader = XmlReader.Create(pathOfFile))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "cell":
                                    break;


                                case "name":
                                    reader.Read();
                                    cellName = reader.Value.ToString();
                                    reader.Read(); // </name>
                                    break;

                                case "contents":
                                    reader.Read();
                                    cellContents = reader.Value.ToString();
                                    reader.Read(); // </contents>

                                    this.SetContentsOfCell(cellName, cellContents);
                                    break;

                                case "spreadsheet":
                                    String VersionCheck = reader.GetAttribute("version");
                                    if (VersionCheck != Version)
                                    {
                                        if (VersionCheck.ToLower() == Version.ToLower()) { }
                                        else { throw new SpreadsheetReadWriteException("Not correct version"); }

                                    }
                                    Version = VersionCheck;
   
                                    break;


                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw new SpreadsheetReadWriteException("Can't get saved version");
            }

            hasChanged = false;
        }

        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved                  
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed { get => hasChanged; protected set => this.Changed = hasChanged; }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        public override object GetCellContents(string name)
        {
            object returnContent = "";

            name = validName(name);

            if (Cells.ContainsKey(name))
            {
                returnContent = Cells[name].Contents;
            }

            return returnContent;
        }

        // ADDED FOR PS5
        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            if (name is null)
            {
                throw new InvalidNameException();
            }
            name = validName(name);


            object returnContent = "";

            if (Cells.ContainsKey(name))
            {
                returnContent = Cells[name].Value;
            }


            return returnContent;

        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            //creating coppy of dictionary so user can change actual dictionary
            List<string> returnList = new List<string>();

            foreach (KeyValuePair<String, Cell> s in Cells)
            {
                //adds every key in the dictionary to returnList
                returnList.Add(s.Key);
            }
            return returnList;

        }

        // ADDED FOR PS5
        /// <summary>
        /// Returns the version information of the spreadsheet saved in the named file.
        /// If there are any problems opening, reading, or closing the file, the method
        /// should throw a SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override string GetSavedVersion(string filename)
        {
            string fileVersion = "";
            bool foundVersion = false;
            try
            {
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    while (reader.Read())
                    {

                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "spreadsheet":
                                    fileVersion = reader.GetAttribute("version");
                                    foundVersion = true;
                                    break;


                            }
                        }
                        if (foundVersion)
                        {
                            reader.Close();
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw new SpreadsheetReadWriteException("Can't get saved version");
            }

            return fileVersion;
        }

        // ADDED FOR PS5
        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>
        /// cell name goes here
        /// </name>
        /// <contents>
        /// cell contents goes here
        /// </contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.ToString() should be written as the contents.  
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override void Save(string filename)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("  ");





            try
            {
                using (XmlWriter writter = XmlWriter.Create(filename, settings))
                {
                    writter.WriteStartDocument();
                    writter.WriteStartElement("spreadsheet");// start spreadsheets
                    writter.WriteAttributeString("version", Version); // added version

                    foreach (var cell in Cells)
                    {
                        writter.WriteStartElement("cell"); // start cell
                        writter.WriteStartElement("name"); // start name
                        writter.WriteString(cell.Key);
                        writter.WriteEndElement(); // end name
                        writter.WriteStartElement("contents");
                        if (cell.Value.Contents is Formula)
                        {
                            writter.WriteString("=" + cell.Value.Contents.ToString());
                        }
                        else
                        {
                            writter.WriteString(cell.Value.Contents.ToString());
                        }
                        writter.WriteEndElement(); // end contents
                        writter.WriteEndElement(); // end cell
                    }
                    writter.WriteEndElement(); // end spreadsheet
                    writter.WriteEndDocument();
                }
            }
            catch (Exception)
            {
                throw new SpreadsheetReadWriteException("Can't get saved version");
            }




        }

        // ADDED FOR PS5
        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        /// 
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor.  There are then three possibilities:
        /// 
        ///   (1) If the remainder of content cannot be parsed into a Formula, a 
        ///       SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown.
        ///       
        ///   (3) Otherwise, the contents of the named cell becomes f.
        /// 
        /// Otherwise, the contents of the named cell becomes content.
        /// 
        /// If an exception is not thrown, the method returns a set consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            if (content is null)
            {
                throw new ArgumentNullException();
            }
            name = validName(name);

            hasChanged = true;

            if (content == "")
            {
                HashSet<string> returnSet = new HashSet<string>();
                if (Cells.ContainsKey(name))
                {
                    //change contents
                    Cells[name].Contents = content;


                    HashSet<string> updates = new HashSet<string>();
                    //getting needed updated cells
                    IEnumerable<string> needsUpdate = GetCellsToRecalculate(name);

                    foreach (string s in needsUpdate)
                    {
                        returnSet.Add(s);
                    }
                    Cells.Remove(name);

                    return returnSet;
                }
                else
                {
                    returnSet.Add(name);
                    return returnSet;
                }
            }



            char equals = '=';

            //checking if formula, double, or string
            if (content[0].Equals(equals))
            {
                Formula inputForFormula = new Formula(content.Substring(1), Normalize, IsValid);
                return SetCellContents(name, inputForFormula);
            }
            else if (Regex.IsMatch(content, @"-?\d+(?:\.\d+)?"))
            {
                return SetCellContents(name, double.Parse(content));
            }
            else
            {
                return SetCellContents(name, content);
            }


        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, double number)
        {

            HashSet<string> returnSet = new HashSet<string>();


            if (Cells.ContainsKey(name))
            {
                //change contents
                Cells[name].Contents = number;

                if (Dependency.HasDependents(name))
                {
                    IEnumerable<string> t = Dependency.GetDependees(name);
                    HashSet<string> remove = new HashSet<string>();
                    foreach (string s in t)
                    {
                        remove.Add(s);
                    }

                    foreach (string s in remove)
                    {
                        Dependency.RemoveDependency(name, s);

                    }
                }

                HashSet<string> updates = new HashSet<string>();
                //getting needed updated cells
                IEnumerable<string> needsUpdate = GetCellsToRecalculate(name);

                foreach (string s in needsUpdate)
                {
                    returnSet.Add(s);
                }

            }
            else
            {
                //create new cell
                Cell newCell = new Cell(number);
                Cells.Add(name, newCell);


                HashSet<string> updates = new HashSet<string>();
                //get updating cells
                IEnumerable<string> needsUpdate = GetCellsToRecalculate(name);

                foreach (string s in needsUpdate)
                {
                    returnSet.Add(s);
                }

            }
            Cells[name].Value = number;

            returnSet.Add(name);

            foreach (string s in returnSet)
            {
                if (Cells[s].Contents is Formula || Cells[s].Value is FormulaError)
                {
                    Formula formula = new Formula(Cells[s].Contents.ToString());
                    Cells[s].Value = formula.Evaluate(Evaluation);
                }
            }



            return returnSet;
        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, string text)
        {

            HashSet<string> returnSet = new HashSet<string>();




            if (Cells.ContainsKey(name))
            {
                //change contents
                Cells[name].Contents = text;


                HashSet<string> updates = new HashSet<string>();
                //getting needed updated cells
                IEnumerable<string> needsUpdate = GetCellsToRecalculate(name);

                foreach (string s in needsUpdate)
                {
                    returnSet.Add(s);
                }

            }
            else
            {
                //create new cell
                Cell newCell = new Cell(text);
                Cells.Add(name, newCell);


                HashSet<string> updates = new HashSet<string>();
                //get updating cells
                IEnumerable<string> needsUpdate = GetCellsToRecalculate(name);

                foreach (string s in needsUpdate)
                {
                    returnSet.Add(s);
                }

            }

            Cells[name].Value = text;

            returnSet.Add(name);
            foreach (string s in returnSet)
            {

                if (Cells[s].Contents is Formula || Cells[s].Value is FormulaError)
                {
                    Formula formula = new Formula(Cells[s].Contents.ToString());
                    Cells[s].Value = formula.Evaluate(Evaluation);
                }

            }



            return returnSet;
        }

        /// <summary>
        /// If the formula parameter is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException.  (No change is made to the spreadsheet.)
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, Formula formula)
        {

            HashSet<string> returnSet = new HashSet<string>();

            returnSet.Add(name);
            //add name to iset

            //if in dictionary
            if (Cells.ContainsKey(name))
            {
                //replace dependees and dependents of previous variables

                //create name into ienumerable to be able to replace
                List<string> previousName = new List<string>();
                previousName.Add(name);

                //get new variables
                Formula f = formula;
                IEnumerable<string> newVariables = formula.GetVariables();

                //replacing dependents of name
                Dependency.ReplaceDependees(name, newVariables);

                //get cells to recalculate of new variables inside name, add those to return iset
                IEnumerable<string> needsUpdate = GetCellsToRecalculate(name);


                foreach (string s in needsUpdate)
                {
                    returnSet.Add(s);
                }

                //set new contents of cell
                Cells[name].Contents = formula;
            }
            else
            {
                //get updating cells
                Cell newCell = new Cell(formula);
                Cells.Add(name, newCell);

                Formula f = formula;
                IEnumerable<string> newVariables = formula.GetVariables();

                foreach (string s in newVariables)
                {
                    Dependency.AddDependency(s, name);

                }

                HashSet<string> updates = new HashSet<string>();
                //get cells to recalculate of new variables inside name, add those to return iset
                IEnumerable<string> needsUpdate = GetCellsToRecalculate(name);

                foreach (string s in needsUpdate)
                {
                    returnSet.Add(s);
                }



            }

            //Cells[name].Value = formula.Evaluate(Evaluation);
            //return the iset

            foreach (string s in returnSet)
            {

                if (Cells[s].Contents is Formula || Cells[s].Value is FormulaError)
                {
                    Formula newFormula = new Formula(Cells[s].Contents.ToString());
                    Cells[s].Value = newFormula.Evaluate(Evaluation);
                }

            }



            return returnSet;


        }

        /// <summary>
        /// Pulls previous variable value if there is one
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private double Evaluation(string name)
        {
            if (!Cells.ContainsKey(name))
            {
                throw new ArgumentException();
            }
            if (Cells[name].Value is string)
            {
                throw new ArgumentException();
            }

            return (double)Cells[name].Value;
        }



        /// <summary>
        /// If name is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
        /// 
        /// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            name = validName(name);

            List<string> returnList = new List<string>();

            //use getDependents and add name to it too

            if (Cells.ContainsKey(name))
            {
                foreach (String s in Dependency.GetDependents(name))
                {
                    returnList.Add(s);
                }
            }


            return returnList;
        }

        /// <summary>
        /// Checks if the names that are passed in are valid names for cells
        /// 
        /// Throws invalid Name Exception if the name is null or invalid
        /// </summary>
        /// <param name="name"></param>
        private String validName(string name)
        {

            if (name == null)
            {
                throw new InvalidNameException();
            }

            //normalizing
            name = Normalize(name);

            //catching if it is an invalid name 
            char[] token = name.ToCharArray();
            List<char> visited = new List<char>();
            int start = 0;
            int counter = 0;
            if (token.Length <= 1)
            {
                throw new InvalidNameException();
            }

            foreach (char s in token)
            {
                if (counter == start)
                {
                    if (!Regex.IsMatch(s.ToString(), "[a-zA-Z]")) //check if a-z or A-Z
                    {
                        throw new InvalidNameException();
                    }
                    visited.Add(s);
                    counter++;
                }
                else if (counter == token.Length - 1)
                {
                    if (!Regex.IsMatch(s.ToString(), @"^\d+$")) //Check if integer
                    {
                        throw new InvalidNameException();
                    }
                    visited.Add(s);
                    counter++;
                }
                else if (Regex.IsMatch(visited.Last().ToString(), "[a-zA-Z]")) //Check if a-z or A-Z
                {
                    if (!Regex.IsMatch(s.ToString(), "[a-zA-Z]") && !Regex.IsMatch(s.ToString(), @"^\d+$")) //Check if not alphnumeric
                    {
                        throw new InvalidNameException();
                    }
                    visited.Add(s);
                    counter++;
                }
                else if (Regex.IsMatch(visited.Last().ToString(), @"^\d+$")) // check if previous is number, if it is, must be number
                {
                    if (!Regex.IsMatch(s.ToString(), @"^\d+$"))
                    {
                        throw new InvalidNameException();
                    }
                    visited.Add(s);
                    counter++;
                }
            }



            try
            {
                Formula f = new Formula(name, Normalize, IsValid); // check isValid
            }
            catch (Exception)
            {
                throw new InvalidNameException();
            }

            return name;
        }



        /// <summary>
        /// This class creates a cell object that holds contents and value of that cell
        /// Constructor takes in contents
        /// </summary>
        private class Cell
        {

            private object contents;
            private object value;
            public Cell(object contents)
            {

                this.contents = contents;

            }


            public object Contents { get => contents; set => contents = value; }
            public object Value { get => value; set => this.value = value; }
        }


    }
}
