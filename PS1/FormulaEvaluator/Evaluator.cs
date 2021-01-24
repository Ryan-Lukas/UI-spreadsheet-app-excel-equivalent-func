using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FormulaEvaluator
{
    public class Evaluator
    {
        //creating new stacks for variables and operators
        private Stack<int> var = new Stack<int>();
        private Stack<String> op = new Stack<String>();
        private String previous = null;
        private bool prevAlgebraVar = false;

        public delegate int Lookup(String v);


        public static int Evaluate(String exp, Lookup variableEvaluator)
        {

            Evaluator evaluator = new Evaluator();
            //creates brokenup string and puts into array
            exp = Regex.Replace(exp, @"\s", "");
            string[] token = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");


            //goes to takeOutEmptyTokens to take out all "" placed in array by regex.split
            token = takeOutEmptyTokens(token);

            checkPrenth(token);
            int position = 0;


            //possibly change to foreach loop
            while (token.Length != position)
            {

                //puts it into the stack for what type (var,op etc) in
                evaluator.checkToken(token[position], variableEvaluator);
                if (position == 0 || position == token.Length - 1)
                {
                    evaluator.checkFirstAndLast(position, token.Length - 1, token[position]);
                }

                position++;
            }

            if (position == 0)
            {
                throw new ArgumentException("Incorrect Input");
            }
            evaluator.operatorCheck();
            evaluator.previous = null;
            int final = evaluator.var.Pop();

            return final;
        }

        /// <summary>
        /// checks to see if there is the correct amount of prenthesis 
        /// </summary>
        /// <param name="token"></param>
        private static void checkPrenth(string[] token)
        {
            bool prenth = false;
            int rightPrenth = 0;
            int leftPrenth = 0;
            int operators = 0;

            foreach (string input in token)
            {
                if (input == "(")
                {
                    leftPrenth++;
                    prenth = true;
                }
                else if (input == ")")
                {
                    rightPrenth++;
                    prenth = false;

                }
                else if (prenth && input == "+" || input == "-" || input == "*" || input == "/")
                {
                    operators++;
                }
            }

            if( rightPrenth != leftPrenth)
            {
                throw new ArgumentException("Incorrect Input");
            }
            if (rightPrenth > 0 && operators == 0)
            {
                throw new ArgumentException("Incorrect Input");
            }


        }



        /// <summary>
        /// Checks token if it is a string, int, lookup, or an operation, do something if it is one of those
        /// </summary>
        /// <param name="given"></param>
        /// <param name="variableEvaluator"></param>
        private void checkToken(String given, Lookup variableEvaluator)
        {
            if (Regex.IsMatch(given, @"^\d+$"))//checking to see if it is an integer
            {
                if (prevAlgebra())
                {
                    operatorCheckWithGivenValue(int.Parse(given));
                }
                else
                {
                    var.Push(int.Parse(given));
                    checkPrevious(given);
                    previous = given;
                }

            }
            else if (Regex.IsMatch(given, "[a-zA-Z]"))//checking if the string has a-z or A-Z
            {
                //see if given is a valid variable and if you need to do previous operation
                isValidLookUp(given);
                isValidAddOperation();

                //checks if * or / is in stack, if so do operations
                if (prevAlgebra())
                {
                    operatorCheckWithGivenValue(variableEvaluator(given));
                }
                else
                {
                    var.Push(variableEvaluator(given));
                    checkPrevious(given);
                    previous = variableEvaluator(given).ToString();
                }
            }
            else if (given == "+" || given == "-") // checks if given is + or -
            {

                operatorCheck();
                op.Push(given);
                checkPrevious(given);
                previous = given;
            }
            else if (given == "*" || given == "/") // checks if given is / or *
            {
                isValidAddOperation();
                op.Push(given);
                checkPrevious(given);
                previous = given;
            }
            else if (given == "(")
            {
                op.Push(given);
                previous = given;
                

            }
            else if (given == ")")
            {

                operatorCheck();
                if (op.Count == 0) { }
                else if (op.Count > 0 && op.Peek() != "(")
                {
                    throw new Exception("Incorrect Input");
                }
                else
                {
                    op.Pop();
                    previous = given;
                }
                
                operatorCheckMultiplication();

            }

        }


        /// <summary>
        /// Checks the previous input to make sure you arent adding two operations in a row (excluding "(" and ")"
        /// also checking to make sure there aren't integers in a row exp. 4 5 or A4 A6 or + +
        /// </summary>
        /// <param name="input"></param>
        private void checkPrevious(String input)
        {
            if (previous == null)
            {
                return;
            }
            else if (Regex.IsMatch(input, @"^\d+$"))//checks for integers
            {
                if (Regex.IsMatch(previous, @"^\d+$"))
                {
                    throw new ArgumentException("Previous element was an operation");
                }
            }
            else if (Regex.IsMatch(input, "[a-zA-Z]"))// checks for letters
            {
                if (Regex.IsMatch(previous, "[a-zA-Z]"))
                {
                    throw new ArgumentException("Previous element was an operation");
                }
            }
            else if (input == "+" || input == "-" || input == "*" || input == "/")// checks for previous operators
            {
                if (!prevAlgebraVar && var.Count < 1)
                {
                    throw new ArgumentException("Previous element was an operation");
                }
                prevAlgebraVar = false;
            }

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
        /// Cant have the first input to be a operator nor the last position too
        /// </summary>
        private void checkFirstAndLast(int position, int lengthOfArray, string input)
        {
            if (position == 0)
            {
                if (input == "+" || input == "-" || input == "*" || input == "/" || input == ")")
                {
                    throw new ArgumentException("Can not have first input be an operator");
                }
            }
            else if (position == lengthOfArray)
            {
                if (input == "+" || input == "-" || input == "*" || input == "/" || input == "(")
                {
                    throw new ArgumentException("Can not have last input be an operator");
                }
            }
        }



        /// <summary>
        /// If an integer or variable is added and there is a * or / in the stack, take that variable given and use algebra to get the 
        /// answer for that given and what is on top of the stack
        /// </summary>
        /// <returns></returns>
        private bool prevAlgebra()
        {
            if (op.Count >= 1 && var.Count >= 1)
            {
                if (op.Peek() == "*" || op.Peek() == "/")
                {
                    prevAlgebraVar = true;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///  checks if the lookup value is valid, must have character(s) then followed by an integer(s)
        /// </summary>
        /// <param name="given"></param>
        private void isValidLookUp(string given)
        {
            char[] token = given.ToCharArray();
            int counter = 0;
            bool hasInt = false;
            bool hasVar = false;
            while (counter != token.Length)
            {
                if (Regex.IsMatch(token[counter].ToString(), "[a-zA-Z]"))
                {
                    hasVar = true;
                    if (counter == token.Length - 1 || Regex.IsMatch(token[counter + 1].ToString(), "[a-zA-Z]") || Regex.IsMatch(token[counter + 1].ToString(), @"^\d+$")) { }// checks if next is a letter or number 
                    else
                    {
                        throw new ArgumentException("Incorrect input");
                    }
                }
                else if (Regex.IsMatch(token[counter].ToString(), @"^\d+$")) //checks if number then it cant be a letter, has to be number
                {

                    hasInt = true;
                    if (counter == token.Length - 1 || Regex.IsMatch(token[counter + 1].ToString(), @"^\d+$")) { }
                    else
                    {
                        throw new ArgumentException("Incorrect input");
                    }
                }
                counter++;
            }

            if (!(hasInt && hasVar))
            {
                throw new ArgumentException("Not valid input");

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
                    int num2 = var.Pop();
                    int num1 = var.Pop();
                    var.Push(operatorAlgebra(op.Peek(), num1, num2));
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
                    int num2 = var.Pop();
                    int num1 = var.Pop();
                    var.Push(operatorAlgebra(op.Peek(), num1, num2));
                }
            }

        }

        /// <summary>
        /// checks if the same operator is at the top of the stack, if so, pull previous numbers and 
        /// do the previous operator on it, pops both operands and operator and puts value back into var stack 
        /// </summary>
        /// <param name="operation"></param>
        private void operatorCheckWithGivenValue(int given)
        {

            int num2 = given;
            int num1 = var.Pop();
            var.Push(operatorAlgebra(op.Peek(), num1, num2));
        }


        /// <summary>
        /// Removes all empty spaces that hold nothing in an array such as ""
        /// was getting extra "" infront of non letter elements with regex.split
        /// </summary>
        /// <param name="token"></param>
        private static string[] takeOutEmptyTokens(string[] token)
        {

            int counter = 0, minipulations = 0;
            ArrayList newToken = new ArrayList();

            //putting all var into new arraylist without "" 
            while (counter != token.Length)
            {
                if (token[counter] == "")
                {
                    counter++;
                    minipulations++;
                }
                else
                {
                    newToken.Add(token[counter]);
                    counter++;
                }
            }

            //create new array and put array list elements into array
            counter = 0;
            newToken.TrimToSize();
            string[] createNewToken = new string[newToken.Count];

            if (minipulations != 0)
            {
                while (counter != newToken.Count)
                {
                    createNewToken[counter] = (string)newToken[counter];
                    counter++;
                }
            }
            else
            {
                return token;
            }
            return createNewToken;
        }



        /// <summary>
        /// String of an operator does algebra on the numbers given, returns value after algebra
        /// </summary>
        /// <param name="given"></param>
        private int operatorAlgebra(string operation, int num1, int num2)
        {
            int sum = 0;

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
                if (num2 == 0)
                {
                    throw new ArgumentException("divisable by zero error");
                }
                sum = num1 / num2;
                op.Pop();
            }
            return sum;
        }

    }
}
