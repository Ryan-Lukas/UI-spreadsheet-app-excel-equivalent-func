using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormulaEvaluator;

namespace FormulaEvaluatorTester
{
    class Program
    {
        static void Main(string[] args)
        {
            FakeSpreadsheet sheet = new FakeSpreadsheet();

            sheet.AddVariable("A1", 2);
            sheet.AddVariable("C5", 7);
            sheet.AddVariable("B89", 0);
            sheet.AddVariable("a7a", 0);
            sheet.AddVariable("a&", 0);




            try
            {
                Console.WriteLine(Evaluator.Evaluate("C5 *10", sheet.LookupVariable)); //"" = 70
                Console.WriteLine(Evaluator.Evaluate(" 5+3 * 5", sheet.LookupVariable)); // operation simple = 20
                Console.WriteLine(Evaluator.Evaluate("A1 + 5", sheet.LookupVariable)); // check for lookup working = 7 
                Console.WriteLine(Evaluator.Evaluate("(10+4)*2", sheet.LookupVariable)); // op with per = 28
                Console.WriteLine(Evaluator.Evaluate("((4+5)*2)+4*8", sheet.LookupVariable)); // op double per = 50
                Console.WriteLine(Evaluator.Evaluate("(8+10)/(5-2)", sheet.LookupVariable)); // op double per = 6.
                Console.WriteLine(Evaluator.Evaluate("(8/4)*(2*16)", sheet.LookupVariable)); // op double per = 64
                Console.WriteLine(Evaluator.Evaluate("(7+8)*(2-0)/(18/3)", sheet.LookupVariable)); // op double per = 5
                Console.WriteLine(Evaluator.Evaluate("(9+11)*(2*4)/2", sheet.LookupVariable)); // op double per = 80
                Console.WriteLine(Evaluator.Evaluate("(5-1)/(3+1)", sheet.LookupVariable)); // =1
                Console.WriteLine(Evaluator.Evaluate("(6+2)*(3*1)+(8-5)", sheet.LookupVariable)); // =27
                Console.WriteLine(Evaluator.Evaluate("16(/(1+1))", sheet.LookupVariable)); // =8


                // Console.Read();



                //Console.WriteLine(Evaluator.Evaluate("*5", sheet.LookupVariable)); // value stack is empty exception
                //Console.WriteLine(Evaluator.Evaluate("/8", sheet.LookupVariable)); // value stack is empty wiht /
                //Console.WriteLine(Evaluator.Evaluate(" 5/0", sheet.LookupVariable)); // divisable by zero exception
                //Console.WriteLine(Evaluator.Evaluate(" (6+3)/B89", sheet.LookupVariable)); // divisable by zero exception
                //Console.WriteLine(Evaluator.Evaluate("(*A1+4", sheet.LookupVariable)); // value stack is empty exception
                //Console.WriteLine(Evaluator.Evaluate("A1/0", sheet.LookupVariable)); // divisable by zero exception
                //Console.WriteLine(Evaluator.Evaluate("2+", sheet.LookupVariable)); // not enough operandees
                //Console.WriteLine(Evaluator.Evaluate("2+9/", sheet.LookupVariable)); // not enough operandees
                //Console.WriteLine(Evaluator.Evaluate("(3+5))*5", sheet.LookupVariable)); // no closing "("
                //Console.WriteLine(Evaluator.Evaluate("2+*1", sheet.LookupVariable)); // double operators
                //Console.WriteLine(Evaluator.Evaluate("a7a", sheet.LookupVariable)); // not correct lookup
                //Console.WriteLine(Evaluator.Evaluate("a&", sheet.LookupVariable)); // not correct lookup
                //Console.WriteLine(Evaluator.Evaluate("7a+8*10", sheet.LookupVariable)); // wrong lookup
                Console.WriteLine(Evaluator.Evaluate("8+8+*10", sheet.LookupVariable)); // two ops in a row








            }

            catch (ArgumentException e)
            {
                Console.WriteLine("exception caught: " + e);
            }
            Console.Read();
            // Keep the console window open.

        }

    }


    public class FakeSpreadsheet
    {

        // For variables, we want mapping between string -> int
        // e.g.
        //  "a1"   -> 50
        //  "ZZ14" -> 9
        // A Dictionary provides such a mapping.
        private Dictionary<String, int> vars;


        /// <summary>
        /// Default constructor.
        /// </summary>
        public FakeSpreadsheet()
        {
            vars = new Dictionary<string, int>();
        }


        /// <summary>
        /// Add a variable to the tracker.
        /// </summary>
        /// <param name="var">The name of the variable</param>
        /// <param name="value">The value of the variable</param>
        public void AddVariable(string var, int value)
        {
            vars.Add(var, value);
        }


        /// <summary>
        /// Lookup a variable in the tracker. Throw ArgumentException if the variable doesn't exist.
        /// </summary>
        /// <param name="v">The variable to lookup.</param>
        /// <returns>Returns the value of the variable.</returns>
        public int LookupVariable(String v)
        {
            if (vars.ContainsKey(v))
                return vars[v];
            throw new ArgumentException();
        }

    }
}
