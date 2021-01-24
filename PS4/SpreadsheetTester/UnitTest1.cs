//Written by Ryan Lukas
//Date: 9/28/2018

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;

namespace SpreadsheetTester
{
    [TestClass]
    public class SpreadsheetTests
    {
        //testing evaluation
        [TestMethod]
        public void TestGetValueEmpty()
        {
            AbstractSpreadsheet x = new Spreadsheet();
            Object y = x.GetCellValue("x1");
            Object m = "";
            Assert.IsTrue(y.Equals(m));

        }

        [TestMethod]
        public void TestGetValueCallToOtherCell()
        {
            AbstractSpreadsheet x = new Spreadsheet();
            x.SetContentsOfCell("a1", "2");
            x.SetContentsOfCell("a2", "=a1");

            Object y = x.GetCellValue("a1");
            Object m = 2.0;
            Assert.IsTrue(y.Equals((double)m));

            y = x.GetCellValue("a2");
           
            Assert.IsTrue(y.Equals((double)m));
        }

        [TestMethod]
        public void TestGetValueCallAddition()
        {
            AbstractSpreadsheet x = new Spreadsheet();
            x.SetContentsOfCell("a1", "2");
            x.SetContentsOfCell("a2", "=a1+5");

            Object y = x.GetCellValue("a1");
            Object m = 2.0;
            Assert.IsTrue(y.Equals((double)m));

            y = x.GetCellValue("a2");

            m = 7.0;
            Assert.IsTrue(y.Equals((double)m));
        }



        [TestMethod]
        public void TestGetValueMultipleInputs()
        {
            AbstractSpreadsheet x = new Spreadsheet();
            x.SetContentsOfCell("a1", "2");
            x.SetContentsOfCell("a2", "=a1+5");
            x.SetContentsOfCell("a3", "=a1+a2");

            Object y = x.GetCellValue("a1");
            Object m = 2.0;
            Assert.IsTrue(y.Equals((double)m));

            y = x.GetCellValue("a2");

            m = 7.0;
            Assert.IsTrue(y.Equals((double)m));
            y = x.GetCellValue("a3");
            m = 9.0;
            Assert.IsTrue(y.Equals((double)m));
        }

        [TestMethod()]
        public void TestFormulaErrorBasic()
        {
            AbstractSpreadsheet x = new Spreadsheet();
            x.SetContentsOfCell("a1", "hi");
            x.SetContentsOfCell("a2", "=a1");
            Assert.IsInstanceOfType(x.GetCellValue("a2"), typeof(FormulaError));
        }

        [TestMethod()]
        public void TestFormulaErrorDivBy0()
        {
            AbstractSpreadsheet x = new Spreadsheet();
            x.SetContentsOfCell("a1", "0");
            x.SetContentsOfCell("a2", "=5/a1");
            Assert.IsInstanceOfType(x.GetCellValue("a2"), typeof(FormulaError));
        }

        [TestMethod()]
        public void TestFormulaErrorNotInDictonary()
        {
            AbstractSpreadsheet x = new Spreadsheet();

            x.SetContentsOfCell("a2", "=5+a1");
            Assert.IsInstanceOfType(x.GetCellValue("a2"), typeof(FormulaError));
        }

        [TestMethod]
        public void TestGetValueString()
        {
            AbstractSpreadsheet x = new Spreadsheet();
            x.SetContentsOfCell("x1", "hi");
            
            Object m = "hi";
            Assert.IsTrue(x.GetCellValue("x1").Equals(m));

        }

        [TestMethod]
        public void TestGetValueDouble()
        {
            AbstractSpreadsheet x = new Spreadsheet();
            x.SetContentsOfCell("x1", "4.67");

            Object m = 4.67;
            Assert.IsTrue(x.GetCellValue("x1").Equals(m));

        }







        //other tests
        [TestMethod]
        public void TestGetContentsEmpty()
        {
            AbstractSpreadsheet x = new Spreadsheet();
            Object y = x.GetCellContents("x1");
            Object m = "";
            Assert.IsTrue(y.Equals(m));

        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestNullGetCellContents()
        
        {
            AbstractSpreadsheet x = new Spreadsheet();
            Object y = x.GetCellContents(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellValueInvalidNameQuotes()

        {
            AbstractSpreadsheet x = new Spreadsheet();
            Object y = x.GetCellValue("");
        }



        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellValueInvalidNameNull()

        {
            AbstractSpreadsheet x = new Spreadsheet();
            Object y = x.GetCellValue(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellContentsInvalidName()

        {
            AbstractSpreadsheet x = new Spreadsheet();
            Object y = x.GetCellContents("x7+5");
        }





        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestNullInvalidName()

        {
            AbstractSpreadsheet x = new Spreadsheet();
            Object y = x.GetCellContents(null);
        }

        
        [TestMethod]
        public void TestSetContentsOfNotInDic()
        {
            AbstractSpreadsheet x = new Spreadsheet();

            x.SetContentsOfCell("A1", "=c3+4");

            object tester = x.GetCellContents("A1");

            Formula testAgainst = new Formula("c3+4");
            Assert.IsTrue(testAgainst.Equals(tester));
        }

        [TestMethod]
        public void TestSetContentsOfNotInDicThenChangeCell()
        {
            AbstractSpreadsheet x = new Spreadsheet();
            x.SetContentsOfCell("A1", "=B2+5");

            x.SetContentsOfCell("A1", "=c3+4");

            object tester = x.GetCellContents("A1");

            Formula testAgainst = new Formula("c3+4");
            Assert.IsTrue(testAgainst.Equals(tester));
        }

        [TestMethod]
        public void TestSetContentsOfFormulaWithDou()
        {
            AbstractSpreadsheet x = new Spreadsheet();


            x.SetContentsOfCell("A1", "=B2+5");
            x.SetContentsOfCell("B2", "3.0");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSetContentsOfFormulaNull()
        {
            AbstractSpreadsheet x = new Spreadsheet();

            x.SetContentsOfCell("A1", null);
        }

        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        [TestMethod]
        public void TestSetContentsOfFormulaWithDouSample()
        {
            AbstractSpreadsheet x = new Spreadsheet();
            x.SetContentsOfCell("B1", "=A1*2");
            x.SetContentsOfCell("C1", "=B1+A1");

            IEnumerator<String> tester = x.SetContentsOfCell("A1", "3").GetEnumerator();
            List<string> testAgainst = new List<string>() { "A1", "B1", "C1" };
            tester.MoveNext();
            int y = 0;
            while (tester.Current != null)
            {
                Assert.IsTrue(testAgainst.Contains(tester.Current));
                tester.MoveNext();
                y += 1;
            }

            if (y != testAgainst.Count)
            {
                throw new Exception();
            }

        }

        [TestMethod]
        public void TestGetNonEmptyCells()
        {
            AbstractSpreadsheet x = new Spreadsheet();

            x.SetContentsOfCell("B1", "=A1*2");
            x.SetContentsOfCell("C1", "=B1+A1");
            x.SetContentsOfCell("A1", "3");

            IEnumerable<string> tester = x.GetNamesOfAllNonemptyCells();
            List<string> testAgainst = new List<string>() { "A1", "B1", "C1" };
           
            int y = 0;
          foreach(string s in tester)
            {
                Assert.IsTrue(testAgainst.Contains(s));
                y++;
            }

            if (y != testAgainst.Count)
            {
                throw new Exception();
            }

        }

        [TestMethod]
        public void TestGetNonEmptyCellsEmpty()
        {
            AbstractSpreadsheet x = new Spreadsheet();
            IEnumerable<string> tester = x.GetNamesOfAllNonemptyCells();
            List<string> testAgainst = new List<string>();
            foreach (string s in tester)
            {
                testAgainst.Add(s);
            }

            Assert.IsTrue(testAgainst.Count == 0);
        }

        [TestMethod]
        public void TestSetCellOfNumber()
        {
            AbstractSpreadsheet x = new Spreadsheet();
            x.SetContentsOfCell("a1", "1");
           
            double tester = double.Parse(x.GetCellContents("a1").ToString());
            double testAgainst = 1;

            Assert.IsTrue(tester == testAgainst);

        }

        [TestMethod]
        public void TestSetCellOfNumber2()
        {
            AbstractSpreadsheet x = new Spreadsheet();
            x.SetContentsOfCell("a1", "154");
            x.SetContentsOfCell("a1", "1");

            double tester = double.Parse(x.GetCellContents("a1").ToString());
            double testAgainst = 1;

            Assert.IsTrue(tester == testAgainst);

        }


        [TestMethod]
        public void TestSetCellOfNumber3()
        {
            AbstractSpreadsheet x = new Spreadsheet();
            x.SetContentsOfCell("a1", "154");
            x.SetContentsOfCell("b1", "=a1");
            x.SetContentsOfCell("a1", "1");

            double tester = double.Parse(x.GetCellContents("a1").ToString());
            double testAgainst = 1;

            Assert.IsTrue(tester == testAgainst);

        }

        [TestMethod]
        public void TestSetCellOfNumberChangeMultipleTimes()
        {
            AbstractSpreadsheet x = new Spreadsheet();
            x.SetContentsOfCell("a1", "1");
            x.SetContentsOfCell("a1", "2");
            x.SetContentsOfCell("a1", "3");
            x.SetContentsOfCell("a1", "4");
            x.SetContentsOfCell("a1", "5");
            double tester = double.Parse(x.GetCellContents("a1").ToString());
            double testAgainst = 5;

            Assert.IsTrue(tester == testAgainst);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestNameNullNumber()
        {
            AbstractSpreadsheet x = new Spreadsheet();
            x.SetContentsOfCell(null, "1");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestNameInvalidNumber()
        {
            AbstractSpreadsheet x = new Spreadsheet();
            x.SetContentsOfCell("7#asdf", "1");
        }

        [TestMethod]
        public void TestSetCellOfText()
        {
            AbstractSpreadsheet x = new Spreadsheet();
            x.SetContentsOfCell("a1", "hi");

            string tester = x.GetCellContents("a1").ToString();
            string testAgainst = "hi";

            Assert.IsTrue(tester == testAgainst);

        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestNameNullText()
        {
            AbstractSpreadsheet x = new Spreadsheet();
            x.SetContentsOfCell(null, "a");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestNameInvalidText()
        {
            AbstractSpreadsheet x = new Spreadsheet();
            x.SetContentsOfCell("&489asdf", "a");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNameNullTextNull()
        {
            AbstractSpreadsheet x = new Spreadsheet();
            string y = null;
            x.SetContentsOfCell("a1", y);
        }

        [TestMethod]
        public void TestSetCellNumberToText()
        {
            AbstractSpreadsheet x = new Spreadsheet();
            x.SetContentsOfCell("a1", "6");
            x.SetContentsOfCell("a1", "hi");

            string tester = x.GetCellContents("a1").ToString();
            string testAgainst = "hi";

            Assert.IsTrue(tester == testAgainst);

        }



        [TestMethod]
        public void TestSetCellFormulaToText()
        {
            AbstractSpreadsheet x = new Spreadsheet();
            x.SetContentsOfCell("b1", "=a1+5");

            x.SetContentsOfCell("a1", "6");

            x.SetContentsOfCell("b1", "hi");

            string tester = x.GetCellContents("b1").ToString();
            string testAgainst = "hi";

            Assert.IsTrue(tester == testAgainst);
        }

        [TestMethod]
        public void TestSetCellDoubleMustRecalc()
        {
            AbstractSpreadsheet x = new Spreadsheet();
            x.SetContentsOfCell("b1", "=a1+5");

            x.SetContentsOfCell("a1", "6");

            



            IEnumerator<String> tester = x.SetContentsOfCell("a1", "7").GetEnumerator();
            List<string> testAgainst = new List<string>() { "a1", "b1"};
            tester.MoveNext();
            int y = 0;
            while (tester.Current != null)
            {
                Assert.IsTrue(testAgainst.Contains(tester.Current));
                tester.MoveNext();
                y += 1;
            }

            if (y != testAgainst.Count)
            {
                throw new Exception();
            }

        }

        [TestMethod]
        public void TestSetCellDoubleMustRecalcLong()
        {
            AbstractSpreadsheet x = new Spreadsheet();
            x.SetContentsOfCell("a1", "=b1");
            x.SetContentsOfCell("b1", "=c1");
            x.SetContentsOfCell("c1", "=d1");
            x.SetContentsOfCell("d1", "=e1");
            x.SetContentsOfCell("e1", "=f1");

            IEnumerator<String> tester = x.SetContentsOfCell("f1", "7").GetEnumerator();
            List<string> testAgainst = new List<string>() {"a1", "b1","c1","d1","e1","f1" };
            tester.MoveNext();
            int y = 0;
            while (tester.Current != null)
            {
                Assert.IsTrue(testAgainst.Contains(tester.Current));
                tester.MoveNext();
                y += 1;
            }

            if (y != testAgainst.Count)
            {
                throw new Exception();
            }

        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestCircleException()
        {
            AbstractSpreadsheet x = new Spreadsheet();
            x.SetContentsOfCell("a1", "=b1");
            x.SetContentsOfCell("b1", "=a1");


        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestCircleExceptionLarge()
        {
            AbstractSpreadsheet x = new Spreadsheet();

            x.SetContentsOfCell("a1", "=b1");
            x.SetContentsOfCell("b1", "=c1");
            x.SetContentsOfCell("c1", "=d1");
            x.SetContentsOfCell("d1", "=e1");
            x.SetContentsOfCell("e1", "=f1");
            x.SetContentsOfCell("f1", "=a1");
        }

        [TestMethod]
        public void TestMoreChanges()
        {
            AbstractSpreadsheet x = new Spreadsheet();
            x.SetContentsOfCell("A3", "=58/2");
            x.SetContentsOfCell("A2", "=A3*A4");
            x.SetContentsOfCell("A1", "=A2+A3");





            IEnumerator<String> tester = x.SetContentsOfCell("A4", "=2+3").GetEnumerator();
            List<string> testAgainst = new List<string>() { "A1", "A2","A4" };
            tester.MoveNext();
            int y = 0;
            while (tester.Current != null)
            {
                Assert.IsTrue(testAgainst.Contains(tester.Current));
                tester.MoveNext();
                y += 1;
            }

            if (y != testAgainst.Count)
            {
                throw new Exception();
            }
        }

        [TestMethod]
        public void TestMoreChangesNotInOrder()
        {
            AbstractSpreadsheet x = new Spreadsheet();
           
            x.SetContentsOfCell("A3", "=58/2");
            x.SetContentsOfCell("A2", "=A3*A4");
            x.SetContentsOfCell("A1", "=A2+A3");





            IEnumerator<String> tester = x.SetContentsOfCell("A4", "=2+3").GetEnumerator();
            List<string> testAgainst = new List<string>() { "A1", "A2", "A4" };
            tester.MoveNext();
            int y = 0;
            while (tester.Current != null)
            {
                Assert.IsTrue(testAgainst.Contains(tester.Current));
                tester.MoveNext();
                y += 1;
            }

            if (y != testAgainst.Count)
            {
                throw new Exception();
            }
        }

        [TestMethod]
        public void TestMoreChangesNotInOrderWithText()
        {
            AbstractSpreadsheet x = new Spreadsheet();
            x.SetContentsOfCell("A4", "=2+3");
            x.SetContentsOfCell("A3", "=A2");
            x.SetContentsOfCell("A2", "=A4+5");



            IEnumerator<String> tester = x.SetContentsOfCell("A4", "HI").GetEnumerator();
            List<string> testAgainst = new List<string>() { "A3", "A4","A2" };
            tester.MoveNext();
            int y = 0;
            while (tester.Current != null)
            {
                Assert.IsTrue(testAgainst.Contains(tester.Current));
                tester.MoveNext();
                y += 1;
            }

            if (y != testAgainst.Count)
            {
                throw new Exception();
            }
        }

        [TestMethod]
        public void TestMoreChangesNotInOrderWithNumber()
        {
            AbstractSpreadsheet x = new Spreadsheet();
            x.SetContentsOfCell("A4", "=2+3");
            x.SetContentsOfCell("A3", "=A2");
            x.SetContentsOfCell("A2", "=A4+5");



            IEnumerator<String> tester = x.SetContentsOfCell("A4", "5").GetEnumerator();
            List<string> testAgainst = new List<string>() { "A3", "A4", "A2" };
            tester.MoveNext();
            int y = 0;
            while (tester.Current != null)
            {
                Assert.IsTrue(testAgainst.Contains(tester.Current));
                tester.MoveNext();
                y += 1;
            }

            if (y != testAgainst.Count)
            {
                throw new Exception();
            }
        }


        // EMPTY SPREADSHEETS
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestEmptyGetNull()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestEmptyGetContents()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents("1AA");
        }

        [TestMethod()]
        public void TestGetEmptyContents()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual("", s.GetCellContents("A2"));
        }

        // SETTING CELL TO A DOUBLE
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetNullDouble()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "1.5");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestInvalidName()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a7a", "1.5");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestInvalidName2()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a7%", "1.5");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestInvalidName3()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a@5", "1.5");
        }





        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetInvalidNameDouble()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("1A1A", "1.5");
        }

        [TestMethod()]
        public void TestSimpleSetDouble()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "1.5");
            Assert.AreEqual(1.5, (double)s.GetCellContents("Z7"), 1e-9);
        }

        // SETTING CELL TO A STRING
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSetNullStringVal()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A8", (string)null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetNullStringName()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "hello");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetSimpleString()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("1AZ", "hello");
        }

        [TestMethod()]
        public void TestSetGetSimpleString()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "hello");
            Assert.AreEqual("hello", s.GetCellContents("Z7"));
        }

        // SETTING CELL TO A FORMULA


        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetNullFormName()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "=2");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetSimpleForm()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("1AZ", "=2");
        }

        [TestMethod()]
        public void TestSetGetForm()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "=3");
            Formula f = (Formula)s.GetCellContents("Z7");
            Assert.AreEqual(new Formula("3"), f);
            Assert.AreNotEqual(new Formula("2"), f);
        }

        // CIRCULAR FORMULA DETECTION
        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void TestSimpleCircular()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2");
            s.SetContentsOfCell("A2", "=A1");
        }

        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void TestComplexCircular()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2+A3");
            s.SetContentsOfCell("A3", "=A4+A5");
            s.SetContentsOfCell("A5", "=A6+A7");
            s.SetContentsOfCell("A7","=A1+A1");
        }

        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void TestUndoCircular()
        {
            Spreadsheet s = new Spreadsheet();
            try
            {
                s.SetContentsOfCell("A1", "=A2+A3");
                s.SetContentsOfCell("A2", "15");
                s.SetContentsOfCell("A3", "30");
                s.SetContentsOfCell("A2", "=A3*A1");
            }
            catch (CircularException e)
            {
                Assert.AreEqual(15, (double)s.GetCellContents("A2"), 1e-9);
                throw e;
            }
        }

        // NONEMPTY CELLS
        [TestMethod()]
        public void TestEmptyNames()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        [TestMethod()]
        public void TestExplicitEmptySet()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "");
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        [TestMethod()]
        public void TestExplicitEmptySetPrevious()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "4.0");
            s.SetContentsOfCell("B1", "");
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        [TestMethod()]
        public void TestSimpleNamesString()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod()]
        public void TestSimpleNamesDouble()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "52.25");
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod()]
        public void TestSimpleNamesFormula()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "=3.5");
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod()]
        public void TestMixedNames()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("C1", "hello");
            s.SetContentsOfCell("B1", "=3.5");
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "A1", "B1", "C1" }));
        }

        // RETURN VALUE OF SET CELL CONTENTS
        [TestMethod()]
        public void TestSetSingletonDouble()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            s.SetContentsOfCell("C1", "=5");
            Assert.IsTrue(s.SetContentsOfCell("A1", "17.2").SetEquals(new HashSet<string>() { "A1" }));
        }

        [TestMethod()]
        public void TestSetSingletonString()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("C1", "=5");
            Assert.IsTrue(s.SetContentsOfCell("B1", "hello").SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod()]
        public void TestSetSingletonFormula()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("B1", "hello");
            Assert.IsTrue(s.SetContentsOfCell("C1", "=5").SetEquals(new HashSet<string>() { "C1" }));
        }

        [TestMethod()]
        public void TestSetChain()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2+A3");
            s.SetContentsOfCell("A2", "6");
            s.SetContentsOfCell("A3", "=A2+A4");
            s.SetContentsOfCell("A4", "=A2+A5");
            Assert.IsTrue(s.SetContentsOfCell("A5", "82.5").SetEquals(new HashSet<string>() { "A5", "A4", "A3", "A1" }));
        }

        // CHANGING CELLS
        [TestMethod()]
        public void TestChangeFtoD()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2+A3");
            s.SetContentsOfCell("A1", "2.5");
            Assert.AreEqual(2.5, (double)s.GetCellContents("A1"), 1e-9);
        }

        [TestMethod()]
        public void TestChangeFtoS()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=A2+A3");
            s.SetContentsOfCell("A1", "Hello");
            Assert.AreEqual("Hello", (string)s.GetCellContents("A1"));
        }

        [TestMethod()]
        public void TestChangeStoF()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "Hello");
            s.SetContentsOfCell("A1", "=23");
            Assert.AreEqual(new Formula("23"), (Formula)s.GetCellContents("A1"));
            Assert.AreNotEqual(new Formula("24"), (Formula)s.GetCellContents("A1"));
        }

        // STRESS TESTS
        [TestMethod()]
        public void TestStress1()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=B1+B2");
            s.SetContentsOfCell("B1", "=C1-C2");
            s.SetContentsOfCell("B2", "=C3*C4");
            s.SetContentsOfCell("C1", "=D1*D2");
            s.SetContentsOfCell("C2", "=D3*D4");
            s.SetContentsOfCell("C3", "=D5*D6");
            s.SetContentsOfCell("C4", "=D7*D8");
            s.SetContentsOfCell("D1", "=E1");
            s.SetContentsOfCell("D2", "=E1");
            s.SetContentsOfCell("D3", "=E1");
            s.SetContentsOfCell("D4", "=E1");
            s.SetContentsOfCell("D5", "=E1");
            s.SetContentsOfCell("D6", "=E1");
            s.SetContentsOfCell("D7", "=E1");
            s.SetContentsOfCell("D8", "=E1");
            ISet<String> cells = s.SetContentsOfCell("E1", "0");
            Assert.IsTrue(new HashSet<string>() { "A1", "B1", "B2", "C1", "C2", "C3", "C4", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "E1" }.SetEquals(cells));
        }

        // Repeated for extra weight
        [TestMethod()]
        public void TestStress1a()
        {
            TestStress1();
        }
        [TestMethod()]
        public void TestStress1b()
        {
            TestStress1();
        }
        [TestMethod()]
        public void TestStress1c()
        {
            TestStress1();
        }


        //xml reader

        [TestMethod]
        public void TestxmlWritterSavedLarger()
        {
            AbstractSpreadsheet x = new Spreadsheet();
            x.SetContentsOfCell("A1", "=B1+B2");
            x.SetContentsOfCell("B1", "=C1-C2");
            x.SetContentsOfCell("B2", "=C3*C4");
            x.SetContentsOfCell("C1", "=D1*D2");
            x.SetContentsOfCell("C2", "=D3*D4");
            x.SetContentsOfCell("C3", "=D5*D6");
            x.SetContentsOfCell("C4", "=D7*D8");
            x.Save("test.xml");
            AbstractSpreadsheet y = new Spreadsheet("test.xml", s => true, s =>s, "Default");

            IEnumerable<string> xContents = x.GetNamesOfAllNonemptyCells();
            IEnumerable<string> yContents = y.GetNamesOfAllNonemptyCells();

            List<object> contentsOfX = new List<object>();
            List<object> contentsOfY = new List<object>();

            List<object> valuesOfX = new List<object>();
            List<object> valuesOfy = new List<object>();

            foreach (string s in xContents)
            {
                contentsOfX.Add(x.GetCellContents(s));
                valuesOfX.Add(x.GetCellValue(s));
            }

            foreach (string s in yContents)
            {
                contentsOfY.Add(y.GetCellContents(s));
                valuesOfy.Add(y.GetCellValue(s));
            }

            int counter = 0;
            foreach(string s in xContents)
            {
                Assert.AreEqual(contentsOfX[counter], contentsOfY[counter]);
                Assert.AreEqual(valuesOfX[counter], valuesOfy[counter]);
                counter++;
            }
            

        }


        [TestMethod]
        public void TestxmlWritterSaved()
        {
            AbstractSpreadsheet x = new Spreadsheet();
            x.SetContentsOfCell("A1", "=B1+5");
            x.SetContentsOfCell("B1", "=C1+4");
            x.SetContentsOfCell("C1", "5.0");
            x.Save("test.xml");
            AbstractSpreadsheet y = new Spreadsheet("test.xml", s => true, s => s, "Default");

            IEnumerable<string> xContents = x.GetNamesOfAllNonemptyCells();
            IEnumerable<string> yContents = y.GetNamesOfAllNonemptyCells();

            List<object> contentsOfX = new List<object>();
            List<object> contentsOfY = new List<object>();

            List<object> valuesOfX = new List<object>();
            List<object> valuesOfy = new List<object>();

            foreach (string s in xContents)
            {
                contentsOfX.Add(x.GetCellContents(s));
                valuesOfX.Add(x.GetCellValue(s));
            }

            foreach (string s in yContents)
            {
                contentsOfY.Add(y.GetCellContents(s));
                valuesOfy.Add(y.GetCellValue(s));
            }

            int counter = 0;
            foreach (string s in xContents)
            {
                Assert.AreEqual(contentsOfX[counter], contentsOfY[counter]);
                Assert.AreEqual(valuesOfX[counter], valuesOfy[counter]);
                counter++;
            }


        }

        [TestMethod]
        public void TestxmlWritterGetVersion()
        {
            AbstractSpreadsheet x = new Spreadsheet( s => true, s => s, "1.456456");
            x.SetContentsOfCell("A1", "=B1+5");
            x.SetContentsOfCell("B1", "=C1+4");
            x.SetContentsOfCell("C1", "5.0");
            x.Save("test.xml");
            AbstractSpreadsheet y = new Spreadsheet();
            Assert.AreEqual("1.456456",y.GetSavedVersion("test.xml"));

        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestxmlGetSavedVersionNotReachable()
        {
            AbstractSpreadsheet x = new Spreadsheet(s => true, s => s, "1.456456");
            x.SetContentsOfCell("A1", "=B1+5");
            x.SetContentsOfCell("B1", "=C1+4");
            x.SetContentsOfCell("C1", "5.0");
            x.Save("test.xml");
            AbstractSpreadsheet y = new Spreadsheet();
            Assert.AreEqual("1.456456", y.GetSavedVersion("incorrectFile.xml"));

        }


        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestxmlLoadFileIncorrect()
        {
            AbstractSpreadsheet x = new Spreadsheet();
            x.SetContentsOfCell("A1", "=B1+5");
            x.SetContentsOfCell("B1", "=C1+4");
            x.SetContentsOfCell("C1", "5.0");
            x.Save("test.xml");
            AbstractSpreadsheet y = new Spreadsheet("test", s => true, s => s, "Default");

        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        // Verifies cells and their values, which must alternate.
        public void VV(AbstractSpreadsheet sheet, params object[] constraints)
        {
            for (int i = 0; i < constraints.Length; i += 2)
            {
                if (constraints[i + 1] is double)
                {
                    Assert.AreEqual((double)constraints[i + 1], (double)sheet.GetCellValue((string)constraints[i]), 1e-9);
                }
                else
                {
                    Assert.AreEqual(constraints[i + 1], sheet.GetCellValue((string)constraints[i]));
                }
            }
        }


        // For setting a spreadsheet cell.
        public IEnumerable<string> Set(AbstractSpreadsheet sheet, string name, string contents)
        {
            List<string> result = new List<string>(sheet.SetContentsOfCell(name, contents));
            return result;
        }

        // Tests IsValid
        [TestMethod()]
        public void IsValidTest1()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "x");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void IsValidTest2()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => s[0] != 'A', s => s, "");
            ss.SetContentsOfCell("A1", "x");
        }

        [TestMethod()]
        public void IsValidTest3()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "= A1 + C1");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void IsValidTest4()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => s[0] != 'A', s => s, "");
            ss.SetContentsOfCell("B1", "= A1 + C1");
        }

        // Tests Normalize
        [TestMethod()]
        public void NormalizeTest1()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            Assert.AreEqual("", s.GetCellContents("b1"));
        }

        [TestMethod()]
        public void NormalizeTest2()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
            ss.SetContentsOfCell("B1", "hello");
            Assert.AreEqual("hello", ss.GetCellContents("b1"));
        }

        [TestMethod()]
        public void NormalizeTest3()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "5");
            s.SetContentsOfCell("A1", "6");
            s.SetContentsOfCell("B1", "= a1");
            Assert.AreEqual(5.0, (double)s.GetCellValue("B1"), 1e-9);
        }

        [TestMethod()]
        public void NormalizeTest4()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
            ss.SetContentsOfCell("a1", "5");
            ss.SetContentsOfCell("A1", "6");
            ss.SetContentsOfCell("B1", "= a1");
            Assert.AreEqual(6.0, (double)ss.GetCellValue("B1"), 1e-9);
        }

        // Simple tests
        [TestMethod()]
        public void EmptySheet()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            VV(ss, "A1", "");
        }


        [TestMethod()]
        public void OneString()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            OneString(ss);
        }

        public void OneString(AbstractSpreadsheet ss)
        {
            Set(ss, "B1", "hello");
            VV(ss, "B1", "hello");
        }


        [TestMethod()]
        public void OneNumber()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            OneNumber(ss);
        }

        public void OneNumber(AbstractSpreadsheet ss)
        {
            Set(ss, "C1", "17.5");
            VV(ss, "C1", 17.5);
        }


        [TestMethod()]
        public void OneFormula()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            OneFormula(ss);
        }

        public void OneFormula(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "5.2");
            Set(ss, "C1", "= A1+B1");
            VV(ss, "A1", 4.1, "B1", 5.2, "C1", 9.3);
        }


        [TestMethod()]
        public void Changed()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Assert.IsFalse(ss.Changed);
            Set(ss, "C1", "17.5");
            Assert.IsTrue(ss.Changed);
        }


        [TestMethod()]
        public void DivisionByZero1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            DivisionByZero1(ss);
        }

        public void DivisionByZero1(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "0.0");
            Set(ss, "C1", "= A1 / B1");
            Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
        }

        [TestMethod()]
        public void DivisionByZero2()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            DivisionByZero2(ss);
        }

        public void DivisionByZero2(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "5.0");
            Set(ss, "A3", "= A1 / 0.0");
            Assert.IsInstanceOfType(ss.GetCellValue("A3"), typeof(FormulaError));
        }



        [TestMethod()]
        public void EmptyArgument()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            EmptyArgument(ss);
        }

        public void EmptyArgument(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "C1", "= A1 + B1");
            Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
        }


        [TestMethod()]
        public void StringArgument()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            StringArgument(ss);
        }

        public void StringArgument(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "hello");
            Set(ss, "C1", "= A1 + B1");
            Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
        }


        [TestMethod()]
        public void ErrorArgument()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ErrorArgument(ss);
        }

        public void ErrorArgument(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "");
            Set(ss, "C1", "= A1 + B1");
            Set(ss, "D1", "= C1");
            Assert.IsInstanceOfType(ss.GetCellValue("D1"), typeof(FormulaError));
        }


        [TestMethod()]
        public void NumberFormula1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            NumberFormula1(ss);
        }

        public void NumberFormula1(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "C1", "= A1 + 4.2");
            VV(ss, "C1", 8.3);
        }


        [TestMethod()]
        public void NumberFormula2()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            NumberFormula2(ss);
        }

        public void NumberFormula2(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "= 4.6");
            VV(ss, "A1", 4.6);
        }


        // Repeats the simple tests all together
        [TestMethod()]
        public void RepeatSimpleTests()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Set(ss, "A1", "17.32");
            Set(ss, "B1", "This is a test");
            Set(ss, "C1", "= A1+B1");
            OneString(ss);
            OneNumber(ss);
            OneFormula(ss);
            DivisionByZero1(ss);
            DivisionByZero2(ss);
            StringArgument(ss);
            ErrorArgument(ss);
            NumberFormula1(ss);
            NumberFormula2(ss);
        }

        // Four kinds of formulas
        [TestMethod()]
        public void Formulas()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Formulas(ss);
        }

        public void Formulas(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.4");
            Set(ss, "B1", "2.2");
            Set(ss, "C1", "= A1 + B1");
            Set(ss, "D1", "= A1 - B1");
            Set(ss, "E1", "= A1 * B1");
            Set(ss, "F1", "= A1 / B1");
            VV(ss, "C1", 6.6, "D1", 2.2, "E1", 4.4 * 2.2, "F1", 2.0);
        }

        [TestMethod()]
        public void Formulasa()
        {
            Formulas();
        }

        [TestMethod()]
        public void Formulasb()
        {
            Formulas();
        }


        // Are multiple spreadsheets supported?
        [TestMethod()]
        public void Multiple()
        {
            AbstractSpreadsheet s1 = new Spreadsheet();
            AbstractSpreadsheet s2 = new Spreadsheet();
            Set(s1, "X1", "hello");
            Set(s2, "X1", "goodbye");
            VV(s1, "X1", "hello");
            VV(s2, "X1", "goodbye");
        }

        [TestMethod()]
        public void Multiplea()
        {
            Multiple();
        }

        [TestMethod()]
        public void Multipleb()
        {
            Multiple();
        }

        [TestMethod()]
        public void Multiplec()
        {
            Multiple();
        }

        // Reading/writing spreadsheets
        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.Save("q:\\missing\\save.txt");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest2()
        {
            AbstractSpreadsheet ss = new Spreadsheet("q:\\missing\\save.txt", s => true, s => s, "");
        }

        [TestMethod()]
        public void SaveTest3()
        {
            AbstractSpreadsheet s1 = new Spreadsheet();
            Set(s1, "A1", "hello");
            s1.Save("save1.txt");
            s1 = new Spreadsheet("save1.txt", s => true, s => s, "default");
            Assert.AreEqual("hello", s1.GetCellContents("A1"));
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest4()
        {
            using (StreamWriter writer = new StreamWriter("save2.txt"))
            {
                writer.WriteLine("This");
                writer.WriteLine("is");
                writer.WriteLine("a");
                writer.WriteLine("test!");
            }
            AbstractSpreadsheet ss = new Spreadsheet("save2.txt", s => true, s => s, "");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest5()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.Save("save3.txt");
            ss = new Spreadsheet("save3.txt", s => true, s => s, "version");
        }

        [TestMethod()]
        public void SaveTest6()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s, "hello");
            ss.Save("save4.txt");
            Assert.AreEqual("hello", new Spreadsheet().GetSavedVersion("save4.txt"));
        }

        [TestMethod()]
        public void SaveTest7()
        {
            using (XmlWriter writer = XmlWriter.Create("save5.txt"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "hello");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A2");
                writer.WriteElementString("contents", "5.0");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A3");
                writer.WriteElementString("contents", "4.0");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A4");
                writer.WriteElementString("contents", "= A2 + A3");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            AbstractSpreadsheet ss = new Spreadsheet("save5.txt", s => true, s => s, "");
            VV(ss, "A1", "hello", "A2", 5.0, "A3", 4.0, "A4", 9.0);
        }

        [TestMethod()]
        public void SaveTest8()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Set(ss, "A1", "hello");
            Set(ss, "A2", "5.0");
            Set(ss, "A3", "4.0");
            Set(ss, "A4", "= A2 + A3");
            ss.Save("save6.txt");
            using (XmlReader reader = XmlReader.Create("save6.txt"))
            {
                int spreadsheetCount = 0;
                int cellCount = 0;
                bool A1 = false;
                bool A2 = false;
                bool A3 = false;
                bool A4 = false;
                string name = null;
                string contents = null;

                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "spreadsheet":
                                Assert.AreEqual("default", reader["version"]);
                                spreadsheetCount++;
                                break;

                            case "cell":
                                cellCount++;
                                break;

                            case "name":
                                reader.Read();
                                name = reader.Value;
                                break;

                            case "contents":
                                reader.Read();
                                contents = reader.Value;
                                break;
                        }
                    }
                    else
                    {
                        switch (reader.Name)
                        {
                            case "cell":
                                if (name.Equals("A1")) { Assert.AreEqual("hello", contents); A1 = true; }
                                else if (name.Equals("A2")) { Assert.AreEqual(5.0, Double.Parse(contents), 1e-9); A2 = true; }
                                else if (name.Equals("A3")) { Assert.AreEqual(4.0, Double.Parse(contents), 1e-9); A3 = true; }
                                else if (name.Equals("A4")) { contents = contents.Replace(" ", ""); Assert.AreEqual("=A2+A3", contents); A4 = true; }
                                else Assert.Fail();
                                break;
                        }
                    }
                }
                Assert.AreEqual(1, spreadsheetCount);
                Assert.AreEqual(4, cellCount);
                Assert.IsTrue(A1);
                Assert.IsTrue(A2);
                Assert.IsTrue(A3);
                Assert.IsTrue(A4);
            }
        }


        // Fun with formulas
        [TestMethod()]
        public void Formula1()
        {
            Formula1(new Spreadsheet());
        }
        public void Formula1(AbstractSpreadsheet ss)
        {
            Set(ss, "a1", "= a2 + a3");
            Set(ss, "a2", "= b1 + b2");
            Assert.IsInstanceOfType(ss.GetCellValue("a1"), typeof(FormulaError));
            Assert.IsInstanceOfType(ss.GetCellValue("a2"), typeof(FormulaError));
            Set(ss, "a3", "5.0");
            Set(ss, "b1", "2.0");
            Set(ss, "b2", "3.0");
            VV(ss, "a1", 10.0, "a2", 5.0);
            Set(ss, "b2", "4.0");
            VV(ss, "a1", 11.0, "a2", 6.0);
        }

        [TestMethod()]
        public void Formula2()
        {
            Formula2(new Spreadsheet());
        }
        public void Formula2(AbstractSpreadsheet ss)
        {
            Set(ss, "a1", "= a2 + a3");
            Set(ss, "a2", "= a3");
            Set(ss, "a3", "6.0");
            VV(ss, "a1", 12.0, "a2", 6.0, "a3", 6.0);
            Set(ss, "a3", "5.0");
            VV(ss, "a1", 10.0, "a2", 5.0, "a3", 5.0);
        }

        [TestMethod()]
        public void Formula3()
        {
            Formula3(new Spreadsheet());
        }
        public void Formula3(AbstractSpreadsheet ss)
        {
            Set(ss, "a1", "= a3 + a5");
            Set(ss, "a2", "= a5 + a4");
            Set(ss, "a3", "= a5");
            Set(ss, "a4", "= a5");
            Set(ss, "a5", "9.0");
            VV(ss, "a1", 18.0);
            VV(ss, "a2", 18.0);
            Set(ss, "a5", "8.0");
            VV(ss, "a1", 16.0);
            VV(ss, "a2", 16.0);
        }

        [TestMethod()]
        public void Formula4()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Formula1(ss);
            Formula2(ss);
            Formula3(ss);
        }

        [TestMethod()]
        public void Formula4a()
        {
            Formula4();
        }


        [TestMethod()]
        public void MediumSheet()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            MediumSheet(ss);
        }

        public void MediumSheet(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "1.0");
            Set(ss, "A2", "2.0");
            Set(ss, "A3", "3.0");
            Set(ss, "A4", "4.0");
            Set(ss, "B1", "= A1 + A2");
            Set(ss, "B2", "= A3 * A4");
            Set(ss, "C1", "= B1 + B2");
            VV(ss, "A1", 1.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 3.0, "B2", 12.0, "C1", 15.0);
            Set(ss, "A1", "2.0");
            VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 4.0, "B2", 12.0, "C1", 16.0);
            Set(ss, "B1", "= A1 / A2");
            VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0);
        }

        [TestMethod()]
        public void MediumSheeta()
        {
            MediumSheet();
        }


        [TestMethod()]
        public void MediumSave()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            MediumSheet(ss);
            ss.Save("save7.txt");
            ss = new Spreadsheet("save7.txt", s => true, s => s, "default");
            VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0);
        }

        [TestMethod()]
        public void MediumSavea()
        {
            MediumSave();
        }


        // A long chained formula.  If this doesn't finish within 60 seconds, it fails.
        [TestMethod()]
        public void LongFormulaTest()
        {
            object result = "";
            Thread t = new Thread(() => LongFormulaHelper(out result));
            t.Start();
            t.Join(60 * 1000);
            if (t.IsAlive)
            {
                t.Abort();
                Assert.Fail("Computation took longer than 60 seconds");
            }
            Assert.AreEqual("ok", result);
        }

        public void LongFormulaHelper(out object result)
        {
            try
            {
                AbstractSpreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("sum1", "= a1 + a2");
                int i;
                int depth = 100;
                for (i = 1; i <= depth * 2; i += 2)
                {
                    s.SetContentsOfCell("a" + i, "= a" + (i + 2) + " + a" + (i + 3));
                    s.SetContentsOfCell("a" + (i + 1), "= a" + (i + 2) + "+ a" + (i + 3));
                }
                s.SetContentsOfCell("a" + i, "1");
                s.SetContentsOfCell("a" + (i + 1), "1");
                Assert.AreEqual(Math.Pow(2, depth + 1), (double)s.GetCellValue("sum1"), 1.0);
                s.SetContentsOfCell("a" + i, "0");
                Assert.AreEqual(Math.Pow(2, depth), (double)s.GetCellValue("sum1"), 1.0);
                s.SetContentsOfCell("a" + (i + 1), "0");
                Assert.AreEqual(0.0, (double)s.GetCellValue("sum1"), 0.1);
                result = "ok";
            }
            catch (Exception e)
            {
                result = e;
            }
        }




    }
}
