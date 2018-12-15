using System;
using System.Threading;
using Akka.Actor;

namespace DiningPhilosopherUsingActorModel
{
    public class Philosopher : ReceiveActor
    {
        private IActorRef _left;
        private IActorRef _right;
        private Fork _leftFork;
        private Fork _rightFork;
        private PhilosopherStatus _status;

        public Philosopher(string left, string right, ref Fork leftFork, ref Fork rightFork)
        {
            _left = Context.ActorSelection($"user/{left}").Anchor;
            _right = Context.ActorSelection($"user/{right}").Anchor;
            _rightFork = rightFork;
            _leftFork = leftFork;
            _status = PhilosopherStatus.Thinking;
            Receive<string>(message => ProcessRequest(message));
        }

        private void ChangeStatus(PhilosopherStatus status)
        {
            _status = status;
            Context.ActorSelection($"/user/{nameof(MonitorActor)}").Tell(_status.ToString(), Self);
        }

        private void ProcessRequest(string message)
        {
            if (message == Constants.Start)
            {
                Think();
                RequestLeftFork();
            }

            if (message == Constants.ForkResponse && Sender.Equals(_left) &&
                _status == PhilosopherStatus.WaitingForLeftFork)
                RequestRightFork();

            if (message == Constants.ForkResponse && Sender.Equals(_right) &&
                _status == PhilosopherStatus.WaitingForRightFork)
                Eat();

            if (message == Constants.ForkRequest && Sender.Equals(_left) && _leftFork.Dirty)
                ResponseLeftFork();

            if (message == Constants.ForkRequest && Sender.Equals(_right) && _rightFork.Dirty)
                ResponseRightFork();
        }

        private void ResponseLeftFork()
        {
            _leftFork.MakeClean();
            Sender.Tell(Constants.ForkResponse, Self);
        }

        private void ResponseRightFork()
        {
            _rightFork.MakeClean();
            Sender.Tell(Constants.ForkResponse, Self);
        }

        private void RequestLeftFork()
        {
            ChangeStatus(PhilosopherStatus.WaitingForLeftFork);
            _left.Tell(Constants.ForkRequest, Self);
        }

        private void RequestRightFork()
        {
            ChangeStatus(PhilosopherStatus.WaitingForRightFork);
            _right.Tell(Constants.ForkRequest, Self);
        }

        private void Think()
        {
            ChangeStatus(PhilosopherStatus.Thinking);
            Thread.Sleep(new Random().Next(0, 100));
        }

        private void Eat()
        {
            ChangeStatus(PhilosopherStatus.Eating);
            Thread.Sleep(new Random().Next(0, 100));
            ChangeStatus(PhilosopherStatus.Thinking);
            Self.Tell(Constants.Start);
            _leftFork.MakeDirty();
            _rightFork.MakeDirty();
        }
    }
}