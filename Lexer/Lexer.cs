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
            return DFA(input);
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

        enum Action { Continue, Stop } //Стадии работы анализатора

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

        List<ResultToken> DFA(string input)
        {
            List<ResultToken> Tokens = new List<ResultToken>(); //список выходных токенов
            ResultToken LastToken = null; //последний сохранненый токен
            string Output = ""; //выходная строка для токена
            int CurState = 0; //текущее состояние
            int InIndex = 0; //текщий индекс входной строки
            int LastInIndex = 0;
            Action Action = Action.Continue; //текущее состояние анализатора

            while (Action != Action.Stop) //читаем входную строку пока не получим останов анализатора
            {
                //обнуляем значения при анализе нового токена 
                Output = "";
                CurState = 0;
                LastToken = null;

                while (CurState != -1) //анализируем входную строку до отсутсвия перехода
                {
                    TokenName temp = FromStateToTokenName(CurState);
                    if (temp != TokenName.Nothing)
                    {
                        LastToken = new ResultToken(temp.ToString(), Output);
                        LastInIndex = InIndex;
                    }
                    if (InIndex > input.Length-1)  //проверка на окончания входной строки
                    {
                        Action = Action.Stop;
                        break;
                    }
                    CurState = TransitionTable(CurState, input[InIndex]); //новое состояние из таблицы переходов
                    if (LastToken == null && CurState == -1) //останавливаем анализатор если попали в неизвестный символ
                        throw new LexerException(input[InIndex]);

                    if (CurState > 0) //запись символа в токен (если не пробел)
                        Output += input[InIndex];
                    InIndex++; 
                }

                InIndex = LastInIndex; //возврат входного символа к последнему успешному токену
                if (LastToken != null)
                    Tokens.Add(LastToken);              
            }
            return Tokens;
        }

        List<ResultToken> CreateList(List<ResultToken> list, ResultToken token) //функция добавляющая токен в начало списка токенов
        {
            list.Insert(0, token);
            return list;
        }

        class LexerException : Exception
        {
            public LexerException(char ch)
            {
                string ErrMes = "Ошибка: обнаружен неизвестный символ \"" + ch + "\"";
                Console.WriteLine(ErrMes);
            }
        }

    }
}
