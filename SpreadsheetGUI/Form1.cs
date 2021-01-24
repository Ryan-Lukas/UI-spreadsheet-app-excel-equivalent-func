using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using SpreadsheetUtilities;
using SS;

namespace SS
{
    public partial class Form1 : Form
    {
        private String currentCellId;
        private String filePath;
        private bool SaveAs;
        private bool Save;

        /// <summary>
        /// Constructor for the demo
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            // This an example of registering a method so that it is notified when
            // an event happens.  The SelectionChanged event is declared with a
            // delegate that specifies that all methods that register with it must
            // take a SpreadsheetPanel as its parameter and return nothing.  So we
            // register the displaySelection method below.

            // This could also be done graphically in the designer, as has been
            // demonstrated in class.
            spreadsheet = new Spreadsheet(s=>true,s => s.ToUpper(),"ps6");
            spreadsheetPanel1.SelectionChanged += displaySelection;
            spreadsheetPanel1.SetSelection(0, 0);
            CellTextBox.Text = "A1";
            currentCellId = "A1";
            filePath = "";
            SaveAs = false;
            Save = true;
        }

        // Every time the selection changes, this method is called with the
        // Spreadsheet as its parameter.  We display the current time in the cell.

        private void displaySelection(SpreadsheetPanel ss)
        {
            int row, col;
            ss.GetSelection(out col, out row);
            getSelectedCellId(col, row);
            getSelectedCellContents(currentCellId);
            getSelectedCellValue(currentCellId);
        }





        // Deals with the New menu
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Tell the application context to run the form on the same
            // thread as the other forms.
            GUIApplicationContext.getAppContext().RunForm(new Form1());
        }

        // Deals with the Close menu
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Save == false)
            {
                if (System.Windows.Forms.MessageBox.Show("Want to save your spreadsheet?", "confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    saveToolStripMenuItem_Click_1(sender, e);
                }
            }
            Close();
        }

        /// <summary>
        /// If you click the "X" button, it checks if you need to save and asks if you want to save
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Save == false)
            {
                if (System.Windows.Forms.MessageBox.Show("Want to save your spreadsheet?", "confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    saveToolStripMenuItem_Click_1(sender, e);
                }
            }
        }

        // Deals with the BorderColor menu
        private void borderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog1 = new ColorDialog();
            // Allows the user to select a custom color.
            colorDialog1.AllowFullOpen = true;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                spreadsheetPanel1.SetColor1(colorDialog1.Color);    //Calls a function to change the border color
            }
        }
        // Deals with the BackgroundColor menu
        private void backgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog1 = new ColorDialog();
            // Allows the user to select a custom color.
            colorDialog1.AllowFullOpen = true;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                spreadsheetPanel1.SetColor2(colorDialog1.Color);    //Calls a function to change the background color
            }
        }
        // Deals with the Help menu
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Click on a cell to change cells" +
                                                 "\n\nTo Enter Cell Contents: Click cell and use text box at the top of the spreadsheet to enter contents. Then click to arrow to the right of the text box or press enter." +
                                                 "\n\nTo Change Spreadsheet colors: Click the 'options' dropdown and select either 'border color' or 'background color'. Then choose your prefered color." +
                                                 "\n\nTo Save(CTRL+S): Click the file dropdown, select 'save' if you have previously saved the file, otherwise select 'save as' to save to your prefered location." +
                                                 "\n\nTo Close(CTRL+W): Click the file dropdown, select 'close'." +
                                                 "\n\nTo Open: Click the file dropdown, select 'open' and choose the location of your intended spreadsheet file." +
                                                 "\n\nTo open a NEW Spreadsheet(CTRL+T): Click the file dropdown, then choose 'New'.");
        }

        /// <summary>
        /// This edites the file if there is something inside the set cell contents box. When the arrow is clicked it will redraw cells.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ArrowButton_Click(object sender, EventArgs e)
        {
            try
            {
                IEnumerable<String> needsUpdate = spreadsheet.SetContentsOfCell(currentCellId, CellContentsBox.Text);
                drawValue(spreadsheetPanel1, needsUpdate);
                Save = false;
            }
            catch {
                System.Windows.Forms.MessageBox.Show("System.FormatException: 'Input string was not in a correct format.'");
            }

            
        }

        /// <summary>
        /// This save as a file if no file is created, this will default to .sprd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            SaveFileDialog newSave = new SaveFileDialog();
            newSave.InitialDirectory = @"C:\";

            //must be default to .sprd
            newSave.FileName = ".sprd";
            newSave.DefaultExt = "sprd";


            if (newSave.ShowDialog() == DialogResult.OK)
            {
                //saves xml file, permissions set to only read files and can open them.

                spreadsheet.Save(newSave.FileName);
                FileStream fileStream = new FileStream(newSave.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                fileStream.Close();

                FileInfo filesPathName = new FileInfo(newSave.FileName);
                filePath = filesPathName.FullName;
                SaveAs = true;
            }
        }

        /// <summary>
        /// This will save the file if the file is created, if not then it will have a saveDialog box open.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            SaveHelper(sender, e);
        }

        /// <summary>
        /// Helper function that saves the current spreadsheet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveHelper(object sender, EventArgs e)
        {
            if (filePath == "" || SaveAs == false)
            {
                saveAsToolStripMenuItem_Click_1(sender, e);
            }

            try  //This try eliminates a error that is thrown when pressing the X button on a save window
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                spreadsheet.Save(filePath);
                FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                fileStream.Close();
            }
            catch { }

            Save = true;
            
        }

        /// <summary>
        /// This button opens a openFileDialog to help select a new file to load into the spreadsheet
        ///     if something is not saved, it will warn you if it isn't
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog newOpen = new OpenFileDialog();
            newOpen.Filter = "Spreadsheet files (*.sprd)|*.sprd|All files (*.*)| *.*";

            IEnumerable<string> checkForSave = spreadsheet.GetNamesOfAllNonemptyCells();

            //checks if stuff is saved and if you want to save it or not
            if (Save == false && checkForSave.Count() != 0)
            {
                if(System.Windows.Forms.MessageBox.Show("Want to save your spreadsheet?", "confirm", MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    saveToolStripMenuItem_Click_1(sender, e);
                }              
            }

            try
            {
                if (newOpen.ShowDialog() == DialogResult.OK)
                {
                    //getting path file
                    FileInfo filesPathName = new FileInfo(newOpen.FileName);
                    filePath = filesPathName.FullName;

                    IEnumerable<string> reset = spreadsheet.GetNamesOfAllNonemptyCells();
                    foreach (string s in reset)
                    {
                        IEnumerable<String> reseting = spreadsheet.SetContentsOfCell(s, "");
                        drawValue(spreadsheetPanel1, reseting);
                    }

                    //creates new spreadsheet and draws it onto spreadsheet gui
                    spreadsheet = new Spreadsheet(newOpen.FileName, s => true, s => s.ToUpper(), "ps6");
                    IEnumerable<String> needsUpdate = spreadsheet.GetNamesOfAllNonemptyCells();
                    drawValue(spreadsheetPanel1, needsUpdate);

                    Save = true;
                    SaveAs = true;
                }
            }catch(Exception z)
            {
                System.Windows.Forms.MessageBox.Show("System.FormatException: 'Could not read file.'");
                IEnumerable<string> reset = spreadsheet.GetNamesOfAllNonemptyCells();
                foreach (string s in reset)
                {
                    IEnumerable<String> reseting = spreadsheet.SetContentsOfCell(s, "");
                    drawValue(spreadsheetPanel1, reseting);
                }

                //creates new spreadsheet and draws it onto spreadsheet gui
                spreadsheet = new Spreadsheet(s => true, s => s.ToUpper(), "ps6");

                Save = false;
                SaveAs = false;
            }
        }


        /// <summary>
        /// This sets the cell contents when you click on a new box, redrawing what is inside the box.
        /// </summary>
        /// <param name="currentCellId"></param>
        private void getSelectedCellContents(string currentCellId)
        {
            Object contents = spreadsheet.GetCellContents(currentCellId);
            if (contents is Formula)
            {
                CellContentsBox.Text = "=" + contents;
            }
            else
            {
                CellContentsBox.Text = contents.ToString();
            }
        }

        /// <summary>
        /// This sets the cell value box when you click on something new, it will redraw
        /// </summary>
        /// <param name="currentCellId"></param>
        private void getSelectedCellValue(string currentCellId)
        {
            String value = spreadsheet.GetCellValue(currentCellId).ToString();
            CellValueBox.Text = value;
        }



        /// <summary>
        /// This redraws the value in each box, it updates previous cells if needed.
        /// </summary>
        /// <param name="ss"></param>
        /// <param name="needsUpdate"></param>
        private void drawValue(SpreadsheetPanel ss, IEnumerable<string> needsUpdate)
        {
            int row, col;
            
            foreach(string s in needsUpdate)
            {
                getIndex(s,out col, out row);

                if (spreadsheet.GetCellValue(s) is FormulaError)
                {
                    System.Windows.Forms.MessageBox.Show("Not a valid formula in " + s);
                    IEnumerable<string> cleared = spreadsheet.SetContentsOfCell(s, "");
                    drawValue(spreadsheetPanel1, cleared);
                }

                ss.SetValue(col, row, spreadsheet.GetCellValue(s).ToString());
                
            }

            CellValueBox.Text = spreadsheet.GetCellValue(currentCellId).ToString();
        }

        /// <summary>
        /// This gets the index of a cell identification. for example A1 is column 0 row 0
        /// </summary>
        /// <param name="input"></param>
        /// <param name="col"></param>
        /// <param name="row"></param>
        private void getIndex(string input, out int col, out int row)
        {
            string[] alphabet = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

            col = Array.IndexOf(alphabet, input[0].ToString());
            row = int.Parse(input.Substring(1))-1;
        }

        /// <summary>
        /// This gets the index of the row and column and puts a cell identification on it, for col 0 row 0 it is A1
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        private void getSelectedCellId(int col, int row)
        {
            string[] alphabet = new string[] {"A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z" };
            string newCellName = "";
            newCellName += alphabet[col];
            newCellName += (row+1).ToString();
            CellTextBox.Text = newCellName;
            currentCellId = newCellName;
        }

        /// <summary>
        /// Tracks to see if CTRL+S, CTRL+W, or CTRL+T are pressed and executes their desired operations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void spreadsheetPanel1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)   //Saves spreadsheet
            {
                SaveHelper(sender, e);
            }
            if(e.Control && e.KeyCode == Keys.T)   //Opens a new spreadsheet
            {
                GUIApplicationContext.getAppContext().RunForm(new Form1());
            }
            if (e.Control && e.KeyCode == Keys.W) //Close the current spreadsheet
            {
                Close();
            }
            if(e.Control && e.KeyCode == Keys.H)
            {
                System.Diagnostics.Process.Start("https://www.youtube.com/watch?v=oHg5SJYRHA0");


            }
        }

        /// <summary>
        /// This enters the content into the cell if the user presses the enter key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CellContentsBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                ArrowButton_Click(sender, e);
                e.SuppressKeyPress = true;
            }
        }
    }
}
