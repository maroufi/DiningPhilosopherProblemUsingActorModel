using System;
using System.Linq;
using System.Threading;
using Akka.Actor;

namespace DiningPhilosopherUsingActorModel
{
    public class MonitorActor : ReceiveActor
    {
        private string[] statuses;
        public MonitorActor(int numberOfPhilosopher)
        {
            statuses = Enumerable.Range(0, numberOfPhilosopher).Select(s => "Thinking").ToArray();
            Receive<string>(message => PrintMessages(message));
        }

        private void PrintMessages(string message)
        {
            Console.Clear();
            statuses[int.Parse(Sender.Path.Name) - 1] = message;
            Console.WriteLine(string.Join(' ', statuses));
            Thread.Sleep(500);
        }
    }
}