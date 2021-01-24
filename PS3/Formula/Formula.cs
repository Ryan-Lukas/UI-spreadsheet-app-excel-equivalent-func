// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

//Made by Ryan Lukas
namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        private Stack<object> var;
        private Stack<String> op;
        private IEnumerable<string> tokens;
        private List<string> normalizedVar;
        private bool notValidMultCheck;
        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        { }



        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            var = new Stack<object>();
            op = new Stack<String>();
            normalizedVar = new List<string>();
            notValidMultCheck = false;
            tokens = GetTokens(formula);
            tokens = ToNormalize(tokens, normalize, isValid);
            IsValidInput(tokens);
        }

        /// <summary>
        /// Normalizes all tokens inside the given string
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="normalize"></param>
        /// <returns></returns>
        private IEnumerable<string> ToNormalize(IEnumerable<string> tokens, Func<string, string> normalize, Func<string, bool> isValid)
        {
            List<string> newTokens = new List<string>();
            int counter = 0;
            foreach (string input in tokens)
            {
                //checking for variables, if so go into while loop to make sure there are no unvalid chars
                char underScore = '_';
                if (Regex.IsMatch(input, "[a-zA-Z]") || input == "_")
                {
                        char[] token = input.ToCharArray();
                        string newInput = normalize(input);
                        normalizedVar.Add(newInput);

                        if (!isValid(newInput))
                        {
                            throw new FormulaFormatException("IncorrectInput");
                        }

                        while (counter != token.Length - 1)
                        {
                            if(counter == 0 && Regex.IsMatch(token[0].ToString(), @"^\d+$") || token[0] == '.'){
                                Double newDouble = Double.Parse(input, System.Globalization.NumberStyles.Float);
                                newInput = newDouble.ToString();
                                break;
                            }
                            if (Regex.IsMatch(token[counter].ToString(), "[a-zA-Z]") || Regex.IsMatch(token[counter].ToString(), @"^\d+$") || token[counter].Equals(underScore)) { }// checks if next is a letter or number 
                            else
                            {
                                throw new FormulaFormatException("Incorrect input");
                            }

                            counter++;
                        }



                        newTokens.Add(newInput);
                }
                else //if op or double
                {
                    if (Regex.IsMatch(input, @"^\d+$"))
                    {
                        String parsedInput = Double.Parse(input).ToString();
                        newTokens.Add(parsedInput);
                    }
                    else
                    {
                        newTokens.Add(input);
                    }

                }
                counter = 0;
            }

            if (newTokens.Count == 0) { throw new FormulaFormatException("Incorrect input"); }

            return newTokens;
        }


        /// <summary>
        /// Checks if the formula is a valid input, if not it will throw an error
        /// </summary>
        /// <param name="tokens"></param>
        private void IsValidInput(IEnumerable<string> tokens)
        {

            bool prenth = false;
            bool firstToken = true;
            bool IsOpenParenthOrOp = false;
            bool isOtherFollowingRule = false;
            int rightPrenth = 0;
            int leftPrenth = 0;
            int operators = 0;



            foreach (string input in tokens)
            {
                //if firest token, must be number, variable, or open parenth
                if (firstToken)
                {
                    if (!(Regex.IsMatch(input, @"-?\d+(?:\.\d+)?") || Regex.IsMatch(input, "[a-zA-Z]") || input == "("))
                    {
                        throw new FormulaFormatException("Incorrect Input");
                    }
                    firstToken = false;
                }


                //must be number, variable, op, or parenth
                if (!(Regex.IsMatch(input, @"-?\d+(?:\.\d+)?") || Regex.IsMatch(input, "[a-zA-Z]") || input == "+" || input == "-" || input == "*" || input == "/" || input == "(" || input == ")"))
                {
                    throw new FormulaFormatException("Incorrect Input");
                }

                //balancing of parenth
                if (input == "(")
                {
                    leftPrenth++;
                    prenth = true;
                }
                else if (input == ")")
                {
                    rightPrenth++;
                    if (rightPrenth > leftPrenth)
                    {
                        throw new FormulaFormatException("Incorrect Input");
                    }
                    prenth = false;

                }
                else if (prenth && input == "+" || input == "-" || input == "*" || input == "/")
                {
                    operators++;
                }

                //parenth following rule
                if (IsOpenParenthOrOp)
                {
                    if (input == "+" || input == "-" || input == "*" || input == "/" || input == ")")
                    {
                        throw new FormulaFormatException("Incorrect Input");
                    }
                }
                IsOpenParenthOrOp = false;

                //extra following rule
                if (isOtherFollowingRule)
                {
                    if (!(input == "+" || input == "-" || input == "*" || input == "/" || input == ")"))
                    {
                        throw new FormulaFormatException("Incorrect Input");
                    }
                }
                isOtherFollowingRule = false;



                if (Regex.IsMatch(input, @"-?\d+(?:\.\d+)?") || Regex.IsMatch(input, "[a-zA-Z]") || input == ")")
                {
                    isOtherFollowingRule = true;
                }

                //checks to see if it is another open parenth, if so isOpenParenthOrOp is true
                if (input == "(" || input == "+" || input == "-" || input == "*" || input == "/")
                {
                    IsOpenParenthOrOp = true;
                }

            }


            //last checks before it is a valid formula
            if (tokens.Last() == "+" || tokens.Last() == "-" || tokens.Last() == "*" || tokens.Last() == "/" || tokens.Last() == "(")
            {
                throw new FormulaFormatException("Incorrect Input");
            }
            if (rightPrenth != leftPrenth)
            {
                throw new FormulaFormatException("Incorrect Input");
            }
            if (rightPrenth > 0 && operators == 0)
            {
                throw new FormulaFormatException("Incorrect Input");
            }
            if (tokens.Count() < 1)
            {
                throw new FormulaFormatException("Incorrect Input");
            }
        }




        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            return Evaluation(lookup);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="func"></param>
        /// <param name="lookup"></param>
        /// <returns></returns>
        private object Evaluation(Func<string, double> lookup)
        {
            double sum = 0.0;
            foreach (string input in tokens)
            {
                if (var.Count() != 0 && var.Peek() is FormulaError)
                {
                    return var.Pop();
                }
                if (Regex.IsMatch(input, @"-?\d+(?:\.\d+)?") && !Regex.IsMatch(input, @"[a-zA-Z_](?: [a-zA-Z_]|\d)*"))//checking to see if it is an integer
                {
                    if (PrevAlgebra())
                    {
                        operatorCheckWithGivenValue(double.Parse(input));
                    }
                    else
                    {
                        var.Push(double.Parse(input));
                    }

                }
                else if (Regex.IsMatch(input, "[a-zA-Z]"))//checking if the string has a-z or A-Z
                {
                    //see if given is a valid variable and if you need to do previous operation
                    isValidAddOperation();

                    //checks if * or / is in stack, if so do operations
                    if (PrevAlgebra())
                    {
                        try
                        {
                            object lookupInput = lookup(input);
                        }
                        catch (Exception e)
                        {
                            return new FormulaError(e.GetBaseException().ToString());
                        }
                        operatorCheckWithGivenValue(lookup(input));
                    }
                    else
                    {
                        try
                        {
                            object lookupInput = lookup(input);
                        }
                        catch (Exception e)
                        {
                            return new FormulaError(e.GetBaseException().ToString());
                        }
                        var.Push(lookup(input));
                    }
                }
                else if (input == "+" || input == "-") // checks if given is + or -
                {

                    operatorCheck();
                    op.Push(input);
                }
                else if (input == "*" || input == "/") // checks if given is / or *
                {
                    isValidAddOperation();
                    op.Push(input);
                }
                else if (input == "(")
                {
                    op.Push(input);
                }
                else if (input == ")")
                {

                    operatorCheck();
                    op.Pop();
                    operatorCheckMultiplication();
                    if(notValidMultCheck == true)
                    {
                        return new FormulaError("Divisable by 0 error");
                    }

                }
            }
            operatorCheck();
            if (var.Peek() is FormulaError)
            {
                return var.Pop();
            }
            sum = Convert.ToDouble(var.Pop());
            return sum;
        }

        /// <summary>
        /// If an integer or variable is added and there is a * or / in the stack, take that variable given and use algebra to get the 
        /// answer for that given and what is on top of the stack
        /// </summary>
        /// <returns></returns>
        private bool PrevAlgebra()
        {
            if (op.Count >= 1 && var.Count >= 1)
            {
                if (op.Peek() == "*" || op.Peek() == "/") { return true; }
            }
            return false;
        }

        /// <summary>
        /// checks if the same operator is at the top of the stack, if so, pull previous numbers and 
        /// do the previous operator on it, pops both operands and operator and puts value back into var stack 
        /// </summary>
        /// <param name="operation"></param>
        private void operatorCheckWithGivenValue(double given)
        {

            double num2 = given;
            double num1 = Convert.ToDouble(var.Pop());
            var.Push(OperatorAlgebra(op.Peek(), num1, num2));
        }


        /// <summary>
        /// checks to see if you can add an * or / into the stack 
        /// must have more than 1 input in var stack
        /// </summary>
        private void isValidAddOperation()
        {
            if (op.Count >= 1)
            {
                if (var.Count < 1 && (op.Peek() == "*" || op.Peek() == "/"))
                {
                    throw new ArgumentException("Not enough variables in stack, incorrect input");
                }
            }

        }

        /// <summary>
        /// checks if there is an operator in side stack and two numbers inside stack. If so, it will use algebra calling operatorAlgebra
        /// to output the sum and put it into stack
        /// 
        /// with + and -
        /// </summary>
        /// <param name="operation"></param>
        private void operatorCheck()
        {
            if (op.Count >= 1 && var.Count >= 2)
            {
                if (op.Peek() == "+" || op.Peek() == "-")
                {
                    double num2 = Convert.ToDouble(var.Pop());
                    double num1 = Convert.ToDouble(var.Pop());
                    var.Push(OperatorAlgebra(op.Peek(), num1, num2));
                }

            }
        }

        /// <summary>
        /// checks if there is an operator in side stack and two numbers inside stack. If so, it will use algebra calling operatorAlgebra
        /// to output the sum and put it into stack 
        /// 
        /// with * and /
        /// </summary>
        /// <param name="operation"></param>
        private void operatorCheckMultiplication()
        {
            if (op.Count >= 1 && var.Count >= 2)
            {
                if (op.Peek() == "*" || op.Peek() == "/")
                {
                    double num2 = Convert.ToDouble(var.Pop());
                    if(num2 == 0 && op.Peek() == "/")
                    {
                        notValidMultCheck = true;
                        return;
                    }
                    double num1 = Convert.ToDouble(var.Pop());
                    var.Push((double)OperatorAlgebra(op.Peek(), num1, num2));
                }
            }

        }

        /// <summary>
        /// String of an operator does algebra on the numbers given, returns value after algebra
        /// </summary>
        /// <param name="given"></param>
        private object OperatorAlgebra(string operation, double num1, double num2)
        {
            double sum = 0;

            if (operation == "+")
            {
                sum = num1 + num2;
                op.Pop();
            }
            else if (operation == "-")
            {
                sum = num1 - num2;
                op.Pop();
            }
            else if (operation == "*")
            {
                sum = num1 * num2;
                op.Pop();
            }
            else if (operation == "/")
            {
                if (num2 == 0.0)
                {
                    FormulaError formulaError = new FormulaError("Divisable by zero error");
                    return formulaError;
                }
                sum = num1 / num2;
                op.Pop();
            }
            return sum;
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            List<string> variableList = new List<string>();
            foreach (string variable in normalizedVar)
            {
                if (variableList.Contains(variable)) { }
                else
                {
                    variableList.Add(variable);
                }
            }


            return variableList;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            String toString = "";
            foreach (string s in tokens)
            {
                toString = toString + s;
            }

            return toString;
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            if(obj is null)
            {
                return false;
            }
            if (obj.ToString() == "")

            {
                return false;
            }
            List<string> f1Tokens = new List<string>();
            List<string> f2Tokens = new List<string>();
            Formula f2 = (Formula)obj;

            //putting tokens inside a list 
            foreach (string s in tokens)
            {
                f1Tokens.Add(s);
            }
            foreach (string s in f2.tokens)
            {
                f2Tokens.Add(s);
            }

            //makes sure the count is the same
            if (f1Tokens.Count() != f2Tokens.Count()) { return false; }

            //if null formulas return true
            if (f1Tokens.Count() == 0 && f2Tokens.Count() == 0) { return true; }

            int position = 0;


            //checks each position to have same variables in each position
            while (position < f1Tokens.Count())
            {

                if (Regex.IsMatch(f1Tokens[position], @"-?\d+(?:\.\d+)?") && !Regex.IsMatch(f1Tokens[position], @"[a-zA-Z_](?: [a-zA-Z_]|\d)*"))
                {
                    if (Regex.IsMatch(f2Tokens[position], @"-?\d+(?:\.\d+)?") && !Regex.IsMatch(f1Tokens[position], @"[a-zA-Z_](?: [a-zA-Z_]|\d)*"))
                    {
                        if (Double.Parse(f1Tokens[position]).Equals(Double.Parse(f2Tokens[position]))) { }
                        else { return false; }
                    }
                    else { return false; }
                }
                else
                {
                    if (f1Tokens[position].Equals(f2Tokens[position])) { }
                    else { return false; }
                }
                position++;

            }

            return true;

        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            if (f1 is null && f2 is null)
            {
                return true;
            }
            else if (f1 is Formula && f2 is null)
            {
                return false;
            }
            else if (f1 is null && f2 is Formula)
            {
                return false;
            }
            return f1.Equals(f2);
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return !f1.Equals(f2);
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            int sum = 0;
            foreach (string s in tokens)
            {
                if (Regex.IsMatch(s, "[A-Z]"))
                {
                    sum = sum + s.GetHashCode() * 2 % 4;
                }
                else
                {
                    if (Regex.IsMatch(s.ToString(), @"-?\d+(?:\.\d+)?"))
                    {
                        double check = Double.Parse(s.ToString());
                        if (check % 1 == 0)
                        {
                            sum = sum + (int)Math.Round(check).GetHashCode();
                        }
                        else
                        {
                            sum = sum + check.GetHashCode();
                        }

                    }
                    else
                    {
                        sum = sum + s.GetHashCode();
                    }
                }
            }

            return sum;
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}
