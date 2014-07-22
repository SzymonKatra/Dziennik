using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Dziennik
{
    public enum SelectionParserError
    {
        None,
        IncorrectFormat,
        UnexpectedCharacters,
    }

    public static class SelectionParser
    {
        public static string Create(List<int> selected)
        {
            string result = string.Empty;

            int startRange = -1;
            int endRange = -1;

            foreach (int sel in selected)
            {
                if (endRange < 0)
                {
                    startRange = endRange = sel;
                }
                else
                {
                    if (sel == endRange + 1)
                    {
                        endRange++;
                    }
                    else
                    {
                        if (startRange != endRange)
                        {
                            result += startRange.ToString();
                            result += '-';
                            result += endRange.ToString();
                            result += "; ";
                        }
                        else
                        {
                            result += endRange.ToString();
                            result += "; ";
                        }

                        startRange = endRange = sel;
                    }
                }
            }

            if (endRange >= 0)
            {
                if (startRange != endRange)
                {
                    result += startRange.ToString();
                    result += '-';
                    result += endRange.ToString();
                    result += "; ";
                }
                else
                {
                    result += endRange.ToString();
                    result += "; ";
                }
            }

            return result;
        }

        public static List<int> Parse(string input)
        {
            SelectionParserError outputError; ;
            return Parse(input, out outputError);
        }
        public static List<int> Parse(string input, out string error)
        {
            SelectionParserError outputError;
            List<int> result = Parse(input, out outputError);

            switch(outputError)
            {
                case SelectionParserError.IncorrectFormat: error = "Nieprawidłowy format. Zakresy oddziel jednym myślnikiem(-)"; break;
                case SelectionParserError.UnexpectedCharacters: error = "Niedozwolone znaki"; break;
                case SelectionParserError.None:
                default:
                    error = string.Empty;
                    break;
            }

            return result;
        }
        public static List<int> Parse(string input, out SelectionParserError error)
        {
            List<int> result = new List<int>();
            error = SelectionParserError.None;
            if (string.IsNullOrWhiteSpace(input)) return result;

            try
            {
                string toParse = input.Replace(" ", "");
                while (toParse[toParse.Length - 1] == ',' || toParse[toParse.Length - 1] == ';') toParse = toParse.Remove(toParse.Length - 1);
                if (string.IsNullOrWhiteSpace(toParse)) return result;

                string[] tokens = input.Split(',', ';');

                foreach (string item in tokens)
                {
                    if (string.IsNullOrWhiteSpace(item)) continue;
                    int valResult;
                    if (int.TryParse(item, out valResult))
                    {
                        result.Add(valResult);
                        continue;
                    }

                    string[] rangeStr = item.Split('-');
                    if (rangeStr.Length != 2)
                    {
                        error = SelectionParserError.IncorrectFormat;
                        return null;
                    }

                    int minRange;
                    int maxRange;
                    if (!int.TryParse(rangeStr[0], out minRange) || !int.TryParse(rangeStr[1], out maxRange))
                    {
                        error = SelectionParserError.UnexpectedCharacters;
                        return null;
                    }

                    for (int i = minRange; i <= maxRange; i++) result.Add(i);
                }
            }
            catch
            {
                Debug.Assert(true, "Exception in SelectionParser.Parse");
            }

            return result;
        }
    }
}
