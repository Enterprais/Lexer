using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = Console.ReadLine();
            List<ResultToken> Tokens = DFA(new Baggage(new FullState(input, "", 0), null));
            foreach (var item in Tokens)
            {
                Console.Write("[" + item.name + ",\"" + item.output + "\"] ");
            }
        }

        enum TokenName
        {
            Number,
            Operator,
            Id,
            Lparen,
            Rparen,
            Comma,
            Nothing
        }

        enum Action { Continue, Restart, Stop }

        static TokenName FromStateToTokenName(int state)
        {
            switch (state)
            {
                case 1: return TokenName.Number; 
                case 2: return TokenName.Number; 
                case 5: return TokenName.Number; 
                case 6: return TokenName.Operator; 
                case 7: return TokenName.Id; 
                case 8: return TokenName.Lparen; 
                case 9: return TokenName.Rparen; 
                case 10: return TokenName.Comma; 
                default: return TokenName.Nothing;
            }
        }

        static int TrasitionTable(int state, char ch)
        {
            switch (state)
            {
                case 0:
                    {
                        if (Char.IsLetter(ch))      return 7;
                        if (ch == '(')              return 8;
                        if (ch == ')')              return 9;
                        if (ch == ',')              return 10;
                        if ("+*/^%-".Contains(ch))  return 6;
                        if (char.IsDigit(ch))       return 1;
                        if (ch == ' ')              return 0;
                    }; break;
                case 1:
                    {                       
                        if (Char.IsDigit(ch))       return 1;
                        if (ch == '.')              return 2;
                    }; break;
                case 2:
                    {
                        if (Char.IsDigit(ch))       return 2;
                        if ("eE".Contains(ch))      return 3;
                    }; break;
                case 3:
                    {
                        if ("+-".Contains(ch))      return 4;
                        if (char.IsDigit(ch))       return 5;
                    }; break;
                case 4:
                    {
                        if (char.IsDigit(ch))       return 5;
                    }; break;
                case 5:
                    {
                        if (char.IsDigit(ch))       return 5;
                    }; break;
                case 7:
                    {
                        if (char.IsDigit(ch) || Char.IsLetter(ch)) return 7;
                    }; break;
                default:
                    return -1;
            }
            return -1;
        }

        class FullState
        {
            public string input;
            public string output;
            public int stateNum;

            public FullState(string inp, string outp, int state)
            {
                this.input = inp;
                this.output = outp;
                this.stateNum = state;
            }
        }

        class Baggage
        {
            public FullState current;
            public FullState last;

            public Baggage(FullState cur, FullState last)
            {
                this.current = cur;
                this.last = last;
            }
        }

        class StepResult
        {
            public Action action;
            public Baggage baggage;  
            
            public StepResult(Action act, Baggage bag)
            {
                this.action = act;
                this.baggage = bag;
            }
        }

        class ResultToken
        {
            public string name;
            public string output;

            public ResultToken(string name, string outp)
            {
                this.name = name;
                this.output = outp;
            }
        }

        static FullState NewLastSt(FullState curState, FullState lastState)
        {
            TokenName token = FromStateToTokenName(curState.stateNum);
            if (token != TokenName.Nothing)
                return curState;
            else
                return lastState;
        }

        static StepResult OneStepDFA(Baggage baggage)
        {
            string CurInput = baggage.current.input;
            string CurOutput = baggage.current.output;
            int CurState = baggage.current.stateNum;

            if (CurInput == "")
                return new StepResult(Action.Stop,
                                      new Baggage(baggage.current,
                                                  NewLastSt(baggage.current, baggage.last)));
            else if (TrasitionTable(CurState, CurInput[0]) == -1)
                return new StepResult(Action.Restart, new Baggage(baggage.current, NewLastSt(baggage.current, baggage.last)));
            else
                return new StepResult(Action.Continue,
                                      new Baggage(new FullState(CurInput.Substring(1),
                                                                CurInput[0] == ' ' ? CurOutput : CurOutput + CurInput[0],
                                                                TrasitionTable(CurState, CurInput[0])),
                                                  NewLastSt(baggage.current, baggage.last)));
        }

        static List<ResultToken> DFA(Baggage baggage)
        {
            StepResult result = OneStepDFA(baggage);
            if(result.action == Action.Stop)
            {
                if (result.baggage == null)
                    return new List<ResultToken>();
                else
                {
                    List<ResultToken> resultTokens = new List<ResultToken>();
                    resultTokens.Add(new ResultToken(FromStateToTokenName(result.baggage.last.stateNum).ToString(),
                                                                          result.baggage.last.output));
                    return resultTokens;
                    
                }
            }
            else if(result.action == Action.Restart)
            {
                if (result.baggage == null)
                    return new List<ResultToken>();
                else
                {
                    return CreateList(DFA(new Baggage(new FullState(result.baggage.last.input, "", 0), null)),
                                          new ResultToken(FromStateToTokenName(result.baggage.last.stateNum).ToString(),
                                                                               result.baggage.last.output));
                }
            }
            else
            {
                return DFA(result.baggage);
            }
        }

        static List<ResultToken> CreateList(List<ResultToken> list, ResultToken token)
        {
            list.Insert(0, token);
            return list;
        }

    }
}
