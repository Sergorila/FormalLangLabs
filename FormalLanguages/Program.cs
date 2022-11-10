using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FormalLanguages
{
    class Program
    {
        static List<string> codeStrings = new List<string>
            {
                "do while a < b and t < e or a <> b",
                "c = d + e",
                "f = g - 10",
                "output t",
                "loop"
            };

        static List<string> codeStrings2 = new List<string>
            {
               "do while a >= 20",
                   "	a = 12",
                   "	a = a+ 5",
               "	output a",
               "loop"
           };

        static void Main(string[] args)
        {
            
            Task4();
        }

        static void Task1()
        {
            Console.Write("Код::");
            var code = "do while a < 5 and b < 8 or a <> b or a not b a = a + b + c + 2 output";
            Console.WriteLine(code);
            var analyser = new Lexical();
            analyser.Run(string.Join(Environment.NewLine, code));
            Console.WriteLine("Результат:");
            for (int i = 0; i < analyser.Lexemes.Count; i++)
            {
                Console.WriteLine($"Индекс: {i + 1}, Класс: {analyser.Lexemes[i].Class}, Тип: {analyser.Lexemes[i].Type}, Значение {analyser.Lexemes[i].Value}");
            }
        }

        static void Task2()
        {
            var analyser = new SyntaxAnalyzer();
            try
            {
                var result = analyser.Run(string.Join(Environment.NewLine, codeStrings));
                Console.WriteLine("Result:");
                Console.WriteLine(result ? "Okay" : "It is not a while statement");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void Task3()
        {
            var analyser = new AnalyzerPOLIZ();
            try
            {
                var result = analyser.Run(string.Join(Environment.NewLine, codeStrings2), out List<Entry> entryList);
                Console.WriteLine("Result:");
                Console.WriteLine(result ? "Okay" : "It is not a while statement");
                foreach (var entry in entryList)
                {
                    if (entry.EntryType == EntryType.Var) FormatOut(entry.Value);
                    else if (entry.EntryType == EntryType.Const) FormatOut(entry.Value);
                    else if (entry.EntryType == EntryType.Cmd) FormatOut(entry.Cmd.ToString());
                    else if (entry.EntryType == EntryType.CmdPtr) FormatOut($"{entry.CmdPtr}");
                }
                Console.WriteLine();
                for (int i = 0; i < entryList.Count + 1; i++)
                {
                    FormatOut($"{i}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine();
        }

        static void Task4()
        {
            using var stream = new StreamReader(@"C:\Users\Sergey\Desktop\SSU.FLTT\SSU.FLTT.Lab1\codeStrings.txt");
            var code = stream.ReadToEnd();
            Console.WriteLine("Code from file:");
            Console.WriteLine(code);

            Interpreter interpreter = new();
            try
            {
                interpreter.Run(code);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void FormatOut(string str)
        {
            Console.Write("{0, 6} ", str);
        }
    }
}
