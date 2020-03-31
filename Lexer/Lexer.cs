using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer
{
    public class Lexer
    {

        public List<ResultToken> GetTokens(string input) // функции получения списка токенов из входной строки
        {
            return DFA(new Baggage(new FullState(input, "", 0), null));
        }

        enum TokenName //Имена токенов 
        {
            Number,
            Operator,
            Id,
            Lparen,
            Rparen,
            Comma,
            Nothing
        }

        enum Action { Continue, Restart, Stop } //Стадии работы анализатора

        TokenName FromStateToTokenName(int state) //функция конвертирующая состояние в соответсвующий токен
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

        int TransitionTable(int state, char ch) //таблица переходов для состояний
        {
            switch (state)
            {
                case 0:
                    {
                        if (Char.IsLetter(ch)) return 7;
                        else if (ch == '(') return 8;
                        else if (ch == ')') return 9;
                        else if (ch == ',') return 10;
                        else if ("+*/^-".Contains(ch)) return 6;
                        else if (char.IsDigit(ch)) return 1;
                        else if (ch == ' ') return 0;
                    }; break;
                case 1:
                    {
                        if (Char.IsDigit(ch)) return 1;
                        else if (ch == '.') return 2;
                    }; break;
                case 2:
                    {
                        if (Char.IsDigit(ch)) return 2;
                        else if ("eE".Contains(ch)) return 3;
                    }; break;
                case 3:
                    {
                        if ("+-".Contains(ch)) return 4;
                        else if (char.IsDigit(ch)) return 5;
                    }; break;
                case 4:
                    {
                        if (char.IsDigit(ch)) return 5;
                    }; break;
                case 5:
                    {
                        if (char.IsDigit(ch)) return 5;
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

        class FullState //класс описания состояния анализатора на определенном шаге 
        {
            public string input; //входная строка
            public string output; // то что накопилось в выходной строке
            public int stateNum; //номер состояния

            public FullState(string inp, string outp, int state)
            {
                this.input = inp;
                this.output = outp;
                this.stateNum = state;
            }
        }

        class Baggage //класс состояния анализатора с сохранением последнего состояния токена
        {
            public FullState current; //текущее состояние
            public FullState last; //последнее состояние токена

            public Baggage(FullState cur, FullState last)
            {
                this.current = cur;
                this.last = last;
            }
        }

        class StepResult //результат вычисления шага анализатора
        {
            public Action action; //дальнейшая стадия
            public Baggage baggage; //состояние анализатора

            public StepResult(Action act, Baggage bag)
            {
                this.action = act;
                this.baggage = bag;
            }
        }

        public class ResultToken //итоговый токен
        {
            public string name; //имя токена
            public string output; //значение токена

            public ResultToken(string name, string outp)
            {
                this.name = name;
                this.output = outp;
            }
        }

        FullState NewLastSt(FullState curState, FullState lastState) //вычисление нового сохраненного состояния токена
        {
            TokenName token = FromStateToTokenName(curState.stateNum);
            if (token != TokenName.Nothing) //если на данном шаге есть токен, то сохранить состояние
                return curState;
            else
                return lastState;
        }
         
        StepResult OneStepDFA(Baggage baggage) //вычисление одного шага анализатора
        {
            string CurInput = baggage.current.input;
            string CurOutput = baggage.current.output;
            int CurState = baggage.current.stateNum;

            if (CurInput == "") //если входная строка пуста, то останавливаем анализатор и возвращаем текущее и последнее сохр. состояние
                return new StepResult(Action.Stop,
                                      new Baggage(baggage.current,
                                                  NewLastSt(baggage.current, baggage.last)));
            else if (TransitionTable(CurState, CurInput[0]) == -1) //если нет дальнейшего перехода, то откатываем результат на предыдущий сохраненный
                return new StepResult(Action.Restart, new Baggage(baggage.current, NewLastSt(baggage.current, baggage.last)));
            else //иначе продолжаем работу
                return new StepResult(Action.Continue,
                                      new Baggage(new FullState(CurInput.Substring(1),                                      //передаем дальше хвост вх. строки
                                                                CurInput[0] == ' ' ? CurOutput : CurOutput + CurInput[0],   //записываем в выходную голову строки
                                                                TransitionTable(CurState, CurInput[0])),                    //и получаем следущее состояние 
                                                  NewLastSt(baggage.current, baggage.last)));
        }

        List<ResultToken> DFA(Baggage baggage)
        {
            StepResult result = OneStepDFA(baggage); //вычисление шага анализатора
            if (result.action == Action.Stop) //остановка анализатора
            {
                if (result.baggage.last == null) //если нет последнего сохраненного состояния (не нашли токен)
                    return new List<ResultToken>(); //то вернем пустой список токенов
                else //иначе добавим в список токенов новое значение из последнего состояния
                {
                    List<ResultToken> resultTokens = new List<ResultToken>();
                    resultTokens.Add(new ResultToken(FromStateToTokenName(result.baggage.last.stateNum).ToString(),
                                                                          result.baggage.last.output));
                    return resultTokens;

                }
            }
            else if (result.action == Action.Restart) //если перезапустили анализатор
            {
                if (result.baggage.last == null) //если нет последнего сохраненного состояния (не нашли токен)
                    return new List<ResultToken>(); //то вернем пустой список токенов
                else //иначе записываем токен в начало списка токенов и перезапускаем анлизатор с 0 состояния
                {
                    return CreateList(DFA(new Baggage(new FullState(result.baggage.last.input, "", 0), null)),
                                          new ResultToken(FromStateToTokenName(result.baggage.last.stateNum).ToString(),
                                                                               result.baggage.last.output));
                }
            }
            else //если продолжение работы, то просто передаем состояние анализатора на дальнейший шаг
            {
                return DFA(result.baggage);
            }
        }

        List<ResultToken> CreateList(List<ResultToken> list, ResultToken token) //функция добавляющая токен в начало списка токенов
        {
            list.Insert(0, token);
            return list;
        }
    }
}
