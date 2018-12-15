using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Util.Internal;

namespace DiningPhilosopherUsingActorModel
{
    class Program
    {
        static void Main(string[] args)
        {
            var system = ActorSystem.Create("ActorSystem");

            var numberOfPhilosopher = int.Parse(Console.ReadLine());

            Fork[] forks = new Fork[numberOfPhilosopher];
            IActorRef[] actors = new IActorRef[numberOfPhilosopher];

            for (int i = 0; i < numberOfPhilosopher; i++)
            {
                forks[i] = new Fork();
            }

            for (int i = 1; i < numberOfPhilosopher + 1; i++)
            {
                var leftPhilosopher = i == 1 ? numberOfPhilosopher : i - 1;
                var rightPhilosopher = i == numberOfPhilosopher ? 1 : i + 1;

                var leftFork = i - 1;
                var rightFork = i == numberOfPhilosopher ? 0 : i;

                var props = Props.Create(() =>
                    new Philosopher(leftPhilosopher.ToString(), rightPhilosopher.ToString(),ref forks[leftFork],
                       ref forks[rightFork]));

                actors[i - 1] = system.ActorOf(props, i.ToString());
                
            }

            foreach (var actor in actors)
            {
                actor.Tell(Constants.Start);
            }
            
            var monitorProps = Props.Create(() =>
                new MonitorActor(numberOfPhilosopher));
            system.ActorOf(monitorProps,nameof(MonitorActor));

            Console.ReadKey();
        }
    }
}