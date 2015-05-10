using n1ar.Service.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace n1ar.Service.Core.Evaluators {
    public class OrderOfOperationRPNEvaluator: IEvaluator<string> {
        private readonly List<RegexDescriptor> rxList;
        private Stack<char> operatorsStack;

        public OrderOfOperationRPNEvaluator() {
            var regexOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase;

            rxList = new List<RegexDescriptor>() {
                new RegexDescriptor { Type = TokenType.Number, Regex = new Regex(@"^[0-9.]{1}$", regexOptions) },
                new RegexDescriptor { Type = TokenType.Function, Regex = new Regex(@"^(f){1}$", regexOptions) },
                new RegexDescriptor { Type = TokenType.FunctionArgSeparator, Regex = new Regex(@"^[,]{1}$", regexOptions) },
                new RegexDescriptor { Type = TokenType.Operator, Regex = new Regex(@"^[-+*/]{1}$", regexOptions) },
                new RegexDescriptor { Type = TokenType.LeftParenthesis, Regex = new Regex(@"^[(]{1}$", regexOptions) },
                new RegexDescriptor { Type = TokenType.RightParenthesis, Regex = new Regex(@"^[)]{1}$", regexOptions) }
            };
            operatorsStack = new Stack<char>();
        }

        public string Evaluate(string expression) {
            char[] tokens = expression.ToCharArray();
            string output = "";
            for (int i = 0; i < tokens.Length; i++) {
                var token = tokens[i];
                switch (GetTokenType(token)) {
                    case TokenType.Invalid:
                        throw new ArgumentException("Invalid character found.");
                    case TokenType.Number:
                        output = AppendConsecutiveNumberToOutput(output, token, i > 0 ? tokens[i - 1] : (char?)null);
                        break;
                    case TokenType.Function:
                        operatorsStack.Push(token);
                        break;
                    case TokenType.FunctionArgSeparator:
                        if (operatorsStack.Peek() == '(') {
                            while (operatorsStack.Count > 0) {
                                output += operatorsStack.Pop() + " ";
                            }
                        }
                        else {
                            throw new ArgumentException("Invalid character found");
                        }
                        break;
                    case TokenType.Operator:
                        if (operatorsStack.Count > 0 && GetTokenType(operatorsStack.Peek()) == TokenType.Operator) {
                            if (StackedOperatorHasToBeOnOutput(token, operatorsStack.Peek())) {
                                output += operatorsStack.Pop() + " ";
                            }
                        }
                        operatorsStack.Push(token);
                        break;
                    case TokenType.LeftParenthesis:
                        operatorsStack.Push(token);
                        break;
                    case TokenType.RightParenthesis:
                        bool leftParenthesisPresent = false;
                        while (operatorsStack.Count > 0) {
                            if (operatorsStack.Peek() == '(') {
                                leftParenthesisPresent = true;
                                break;
                            }
                            output += operatorsStack.Pop() + " ";
                        }
                        if (!leftParenthesisPresent) {
                            throw new ArgumentException("Matching left parenthesis not found");
                        }
                        break;
                    default:
                        continue;
                }
            }
            while (operatorsStack.Count > 0) {
                if (operatorsStack.Peek() == '(') {
                    operatorsStack.Pop();
                }
                output += operatorsStack.Pop() + " ";
            }

            operatorsStack.Clear();

            return output;
        }

        private string AppendConsecutiveNumberToOutput(string output, char token, char? previousToken ) {
            Regex numberMatcher = rxList[0].Regex;
            // If there is at least a 2-char outputted value and
            // if the last output was a number
            // (thus checking the last concatenated string and discard the appended whitespace)
            // and previous checked token was a number too
            // then format the output discarding the whitespace
            if (previousToken.HasValue && numberMatcher.IsMatch(previousToken.Value.ToString()) &&
                output.Length >= 2 && numberMatcher.IsMatch(output.Substring(output.Length - 2, 1))) {
                output = output.TrimEnd(' ');
            }
            // Join the last number
            output += token + " ";
            
            return output;
        }

        private bool StackedOperatorHasToBeOnOutput(char currentToken, char tokenOnStack) {
            // Description for operators: { operator, precedence, associativity }
            string[] operatorPriority = new string[] { "^,4,r", "*,3,l", "/,3,l", "+,2,l", "-,2,l" };
            string currentOpPrority = null;
            string stackedOpPrority = null;
            bool isCurrentAssociativityLeft = false;
            int currentOpPrecedence = 0;
            int stackedOpPrecedence = 0;

            foreach (var priority in operatorPriority) {
                if (currentToken == priority[0]) {
                    currentOpPrority = priority;
                }
                if (tokenOnStack == priority[0]) {
                    stackedOpPrority = priority;
                }
            }

            isCurrentAssociativityLeft = currentOpPrority.Split(',')[2] == "l";
            currentOpPrecedence = Convert.ToInt32(currentOpPrority.Split(',')[1]);
            stackedOpPrecedence = Convert.ToInt32(stackedOpPrority.Split(',')[1]);

            if (isCurrentAssociativityLeft) {
                if (currentOpPrecedence <= stackedOpPrecedence) {
                    return true;
                }
            }
            else {
                if (currentOpPrecedence < stackedOpPrecedence) {
                    return true;
                }
            }

            return false;
        }

        private TokenType GetTokenType(char token) {
            foreach (var regex in rxList) {
                if (regex.Regex.IsMatch(token.ToString())) {
                    return regex.Type;
                }
            }
            return TokenType.Invalid;
        }

        private class RegexDescriptor {
            public TokenType Type { get; set; }
            public Regex Regex { get; set; }
        }

    }
}
