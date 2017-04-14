using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Tomaszkiewicz.WitAi.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Debug.Listeners.Add(new TextWriterTraceListener(System.Console.Out));

            try
            {
                Run().Wait();
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.InnerExceptions)
                {
                    System.Console.WriteLine(ex.Message);
                    System.Console.WriteLine(ex.StackTrace);
                }

                System.Console.ReadLine();
            }
        }

        static async Task Run()
        {
            var dispatcher = new WitDispatcher("VZQQJVTB2E5DBMXL2RXSS73CX75H4ABO");

            dispatcher.RegisterIntentHandler("greetings", new GreetingsHandler());
            dispatcher.RegisterIntentHandler("weather", new WeatherHandler());

            while (true)
            {
                var text = System.Console.ReadLine()?.Trim();

                if (text == null)
                    continue;

                if (text == "exit")
                    break;

                System.Console.WriteLine($"> {text}");

                await dispatcher.Dispatch(text);

                System.Console.WriteLine("Dispatch completed.");
            }
        }
    }
}