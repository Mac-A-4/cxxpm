using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace cxxpm.Utility {

    internal class ConsoleUtil {

        public string Query(string query) {
            UsingColor(ConsoleColor.Yellow, () => {
                Console.Write(query);
            });
            return Console.ReadLine()!;
        }

        public T QueryEnum<T>(string query) where T : struct, Enum {
            var options = Enum.GetValues<T>();
            for (var i = 0; i < options.Length; ++i) {
                var value = options[i];
                Info($"{i}: {value}");
            }
            return options[int.Parse(Query(query))];
        }

        public void i(string data) {
            Info(data);
        }

        public void e(string data) {
            Error(data);
        }

        public void s(string data) {
            Success(data);
        }

        public void Info(string data) {
            WriteLine(data);
        }

        public void Error(string data) {
            WriteLine(data, ConsoleColor.Red);
        }

        public void Success(string data) {
            WriteLine(data, ConsoleColor.Green);
        }

        private void WriteLine(string data, ConsoleColor color = ConsoleColor.Gray) {
            UsingColor(color, () => {
                Console.WriteLine(data);
            });
        }

        private void UsingColor(ConsoleColor color, Action action) {
            var previous = Console.ForegroundColor;
            Console.ForegroundColor = color;
            action.Invoke();
            Console.ForegroundColor = previous;
        }

    }

}
