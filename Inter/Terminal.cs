using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Inter
{
    class Terminal
    {
        private string data;
        /// <summary>
        /// Разбиение на строки
        /// </summary>
        private  List<string> podStr;
        public Terminal(string vhod)
        {
            this.data = vhod;
        }
        public void formStr(string vhod)
        {

        }
        public void otsek(TextBlock Error)
        {
            // убираем отступы
            data = data.Trim();
            string[] words = data.Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries);
            Regex regex = new Regex("Program ");
            MatchCollection matches = regex.Matches(words[0]);
            podStr = new List<string>();
            if (0 == matches.Count)
            {
              Error.Text = "Отсутсвие объявления ключевого слова Program";
            }  
            else
            {  
                foreach (string s in words)
                {
                    // исключаем подстроку с наименованием программы
                    if (s.StartsWith("Program"))
                    {
                    }
                    else
                    {                
                        podStr.Add(s);
                    }
                }
            }
        }

        private List<string> Parse(TextBlock Error, List<string> podStr, string request)
        {
            List<string> kort = new List<string>();
            foreach (string s in podStr)
            {
                string perem;
                perem = s.Trim();
                Regex regex = new Regex(request);
                MatchCollection matches = regex.Matches(perem);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        Regex regex1 = new Regex("var");
                        string result = regex1.Replace(match.Value, "");
                        kort.Add(result);
                        Console.WriteLine(result);
                    }
                }
                else
                {
                    Error.Text = "Отсутсвие объявления ключевого слова var";
                }
            }
            return kort;
        }

        public List<string> Parsing(TextBlock Error, List<string> podStr, string request)
        {
            List<string> kort = new List<string>();
            foreach (string s in podStr)
            {
                string perem;
                perem = s.Trim();
                Regex regex = new Regex(request);
                MatchCollection matches = regex.Matches(perem);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        kort.Add(match.Value);
                    }
                }
                else
                {
                    Error.Text = "Отсутсвие объявления";
                }
            }
            return kort;
        }

        public List<string> VarGet(TextBlock Error, List<string> podStr)
        {
            List<string> kort = new List<string>();
            kort = Parse(Error, podStr, @"var(\w|\D)+");
            return kort;
        }

        public List<string> PeremandType(TextBlock Error,  List<string> podStr)
        {
            List<string> kort = new List<string>();
            kort = Parsing(Error, podStr, @"(\w|,| )+:( |\w)+(,| |)+");
            return kort;
        }

        public List<string> ot(TextBlock Error, List<string> podStr)
        {
            List<string> kort = new List<string>();
            kort = Parsing(Error, podStr, @"(\w|,| )+");
            return kort;
        }


        public List<string> getpodStr()
        {
            return podStr;
        }
    }
    /*
     * Program dfs; 
     * var a,d: int;
     */
}
