using System.Collections.Generic;
using System.Text;

namespace FormalLanguages
{
    public class LexicalAnalyser

    {
        public List<Lexemee> Lexemes { get; private set; }

        public LexicalAnalyser()
        {
            Lexemes = new List<Lexemee>();
        }

        public bool Run(string text)
        {
            Lexemes = new List<Lexemee>();

            State state = State.Start;
            State prevState;

            bool isAbleToAdd;

            text += " ";

            StringBuilder lexBufNext = new();

            StringBuilder lexBufCur = new();

            int textIndex = 0;

            while (state != State.Error && state != State.Final)
            {
                prevState = state;

                isAbleToAdd = true;

                if (textIndex == text.Length && state != State.Error)
                {
                    state = State.Final;
                    break;
                }

                if (textIndex == text.Length)
                {
                    break;
                }

                char symbol = text[textIndex];

                switch (state)
                {
                    case State.Start:
                        {
                            if (char.IsWhiteSpace(symbol)) state = State.Start;

                            else if (char.IsDigit(symbol)) state = State.Constant;

                            else if (char.IsLetter(symbol)) state = State.Identifier;

                            else if (symbol == '>') state = State.Comparison;
                            else if (symbol == '<') state = State.ReverseComparison;

                            else if (symbol == '+' || symbol == '-' || symbol == '/' || symbol == '*') state = State.ArithmeticOperation;

                            else if (symbol == '=') state = State.Assignment;

                            else state = State.Error;

                            isAbleToAdd = false;

                            if (!char.IsWhiteSpace(symbol)) lexBufCur.Append(symbol);

                            break;
                        }

                    case State.Comparison:
                        {
                            if (char.IsWhiteSpace(symbol)) state = State.Start;

                            else if (symbol == '=')
                            {
                                state = State.Start;
                                lexBufCur.Append(symbol);
                            }

                            else if (char.IsLetter(symbol))
                            {
                                state = State.Identifier;
                                lexBufNext.Append(symbol);
                            }

                            else if (char.IsDigit(symbol))
                            {
                                state = State.Constant;
                                lexBufNext.Append(symbol);
                            }

                            else
                            {
                                state = State.Error;
                                isAbleToAdd = false;
                            }

                            break;
                        }

                    case State.ReverseComparison:
                        {
                            if (char.IsWhiteSpace(symbol)) state = State.Start;

                            else if (symbol == '>')
                            {
                                state = State.Start;
                                lexBufCur.Append(symbol);
                            }

                            else if (symbol == '=')
                            {
                                state = State.Start;
                                lexBufCur.Append(symbol);
                            }

                            else if (char.IsLetter(symbol))
                            {
                                state = State.Identifier;
                                lexBufNext.Append(symbol);
                            }

                            else if (char.IsDigit(symbol))
                            {
                                state = State.Constant;
                                lexBufNext.Append(symbol);
                            }

                            else
                            {
                                state = State.Error;
                                isAbleToAdd = false;
                            }

                            break;
                        }

                    case State.Assignment:
                        {
                            if (symbol == '=')
                            {
                                state = State.Comparison;
                                lexBufCur.Append(symbol);
                            }

                            else //if (char.IsWhiteSpace(symbol))
                            {
                                state = State.Identifier;
                                lexBufNext.Append(symbol);
                            }

                            //else
                            //{
                            //    state = State.Error;
                            //    isAbleToAdd = false;
                            //}

                            break;
                        }

                    case State.Constant:
                        {
                            if (char.IsWhiteSpace(symbol)) state = State.Start;

                            else if (char.IsDigit(symbol))
                            {
                                isAbleToAdd = false;
                                state = State.Constant;
                                lexBufCur.Append(symbol);
                            }

                            else if (symbol == '<')

                            {
                                state = State.ReverseComparison;
                                lexBufNext.Append(symbol);
                            }

                            else if (symbol == '>')
                            {
                                state = State.Comparison;
                                lexBufNext.Append(symbol);
                            }

                            else if (symbol == '=')
                            {
                                state = State.Assignment;
                                lexBufNext.Append(symbol);
                            }


                            else if (symbol == '+' || symbol == '-' || symbol == '/' || symbol == '*')
                            {
                                state = State.ArithmeticOperation;
                                lexBufNext.Append(symbol);
                            }

                            else
                            {
                                state = State.Error;
                                isAbleToAdd = false;
                            }

                            break;
                        }

                    case State.Identifier:
                        {
                            if (char.IsWhiteSpace(symbol)) state = State.Start;

                            else if (char.IsDigit(symbol) || char.IsLetter(symbol))
                            {
                                state = State.Identifier;
                                isAbleToAdd = false;
                                lexBufCur.Append(symbol);
                            }

                            else if (symbol == '<')
                            {
                                state = State.ReverseComparison;
                                lexBufNext.Append(symbol);
                            }

                            else if (symbol == '>')
                            {
                                state = State.Comparison;
                                lexBufNext.Append(symbol);
                            }

                            else if (symbol == '=')
                            {
                                state = State.Assignment;
                                lexBufNext.Append(symbol);
                            }

                            else if (symbol == '+' || symbol == '-' || symbol == '/' || symbol == '*')
                            {
                                state = State.ArithmeticOperation;
                                lexBufNext.Append(symbol);
                            }

                            else if (symbol == ':')
                            {
                                state = State.Assignment;
                                lexBufNext.Append(symbol);
                            }

                            else
                            {
                                state = State.Error;
                                isAbleToAdd = false;
                            }

                            break;
                        }

                    case State.ArithmeticOperation:
                        {
                            if (char.IsWhiteSpace(symbol))
                            {
                                state = State.Start;
                            }

                            else if (char.IsLetter(symbol))
                            {
                                state = State.Identifier;
                                lexBufNext.Append(symbol);
                            }

                            else if (char.IsDigit(symbol))
                            {
                                state = State.Constant;
                                lexBufNext.Append(symbol);
                            }

                            else if (symbol == '-' || symbol == '+' || symbol == '/' || symbol == '*')
                            {
                                state = State.ArithmeticOperation;
                                lexBufNext.Append(symbol);
                            }

                            else
                            {
                                state = State.Error;
                                isAbleToAdd = false;
                            }

                            break;
                        }
                }

                if (isAbleToAdd)
                {
                    AddLexeme(prevState, lexBufCur.ToString());
                    lexBufCur = new StringBuilder(lexBufNext.ToString());
                    lexBufNext.Clear();
                }

                textIndex++;
            }

            return state == State.Final;
        }

        private void AddLexeme(State prevState, string value)
        {
            LexemeTypes lexType = LexemeTypes.Undefined;

            LexemeClases lexClass = LexemeClases.Undefined;

            if (prevState == State.ArithmeticOperation)
            {
                lexType = LexemeTypes.ArithmeticOperation;
                lexClass = LexemeClases.SpecialSymbols;
            }
            else if (prevState == State.Assignment)
            {
                lexClass = LexemeClases.SpecialSymbols;
                if (value == "==")
                {
                    lexType = LexemeTypes.Relation;
                }
                else
                {
                    lexType = LexemeTypes.Assignment;
                }
            }
            else if (prevState == State.Constant)
            {
                lexType = LexemeTypes.Undefined;
                lexClass = LexemeClases.Constant;
            }
            else if (prevState == State.ReverseComparison)
            {
                lexType = LexemeTypes.Relation;
                lexClass = LexemeClases.SpecialSymbols;
            }
            else if (prevState == State.Comparison)
            {
                lexType = LexemeTypes.Relation;
                lexClass = LexemeClases.SpecialSymbols;
            }
            else if (prevState == State.Identifier)
            {

                bool isKeyword = true;

                if (value == "not") lexType = LexemeTypes.Not;
                else if (value == "and") lexType = LexemeTypes.And;
                else if (value == "or") lexType = LexemeTypes.Or;
                else if (value == "loop") lexType = LexemeTypes.Loop;
                else if (value == "output") lexType = LexemeTypes.Output;
                else if (value == "do") lexType = LexemeTypes.Do;
                else if (value == "while") lexType = LexemeTypes.While;
                else
                {
                    lexType = LexemeTypes.Undefined;
                    isKeyword = false;
                }

                if (isKeyword) lexClass = LexemeClases.Keyword;
                else lexClass = LexemeClases.Identifier;
            }

            var lexeme = new Lexemee
            {
                Class = lexClass,
                Type = lexType,
                Value = value.Trim(),
            };


            if (lexeme.Value.Length > 0)
            {
                Lexemes.Add(lexeme);
            }

        }
    }
}

