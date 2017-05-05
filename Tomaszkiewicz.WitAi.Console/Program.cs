using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Tomaszkiewicz.WitAi.Exceptions;

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
            var baseColor = System.Console.ForegroundColor;

            var persistence = new InMemoryWitPersistence();

            System.Console.WriteLine("Ready to start a conversation - just type a message :)");

            while (true)
            {
                var dispatcher = new WitDispatcher("VZQQJVTB2E5DBMXL2RXSS73CX75H4ABO", persistence);

                dispatcher.SetDefaultHandler(new ConsoleHandler());
                dispatcher.RegisterIntentHandler(new GreetingsHandler());
                dispatcher.RegisterIntentHandler(new WeatherHandler());
                dispatcher.RegisterIntentHandler(new ThanksHandler());

                System.Console.ForegroundColor = ConsoleColor.Magenta;
                System.Console.Write("> ");

                var text = System.Console.ReadLine()?.Trim();

                System.Console.ForegroundColor = baseColor;

                if (string.IsNullOrWhiteSpace(text))
                    continue;

                if (text == "exit")
                    break;
                
                try
                {
                    await dispatcher.Dispatch(text);
                }
                catch (NoIntentDetectedException)
                {
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine("< No intent detected.");
                    System.Console.ForegroundColor = baseColor;
                }
                catch (Exception ex)
                {
                    System.Console.ForegroundColor = ConsoleColor.DarkRed;
                    System.Console.WriteLine(ex.Message);
                    System.Console.WriteLine(ex.StackTrace);
                    System.Console.ForegroundColor = baseColor;
                }
            }
        }
    }
}