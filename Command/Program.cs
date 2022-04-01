using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Command
{
    public class SmartController
    {
        Stack<ICommand> historyCommand;
        ICommand[] command;
        // ICommand lastCommand;

        public SmartController()
        {
            command = new ICommand[4];
            for (int i = 0; i < command.Length; i++)
            {
                command[i] = new NoCommand();
            }
            historyCommand = new Stack<ICommand>();
        }

        public void SetCommand(uint n, ICommand c)
        {
            command[n] = c;
        }

        public void Run(uint n)
        {
            command[n].Execute();
            foreach (ICommand c in command[n].GetCommands())
                historyCommand.Push(c);
        }

        public void Cancel()
        {
            if (historyCommand.Count > 0)
                historyCommand.Pop().Undo();
        }

    }
    public class Microwave
    {
        public uint Time;

        public void Start(uint time)
        {
            Time = time;
            Console.WriteLine("Microwave is starting now");
            Task.Delay((int)Time).GetAwaiter().GetResult();
            Stop();
        }

        public void Stop()
        {
            Console.WriteLine("MicrowAVE is stopping");
        }
    }
    public class Conditioner
    {
        public ConditionLevel level;

        public Conditioner()
        {
            level = new ConditionLevel();
        }

        public void On()
        {
            Console.WriteLine("Condition is on now. With level - " + level.Level);
        }

        public void Off()
        {
            Console.WriteLine("Condition is off now");
        }
    }
    public class ConditionLevel
    {
        public const int MIN = 0;
        public const int MAX = 10;
        public int Level { get; private set; }

        public ConditionLevel()
        {
            Level = (MIN + MAX) / 2;
        }

        public void RaiseLevel()
        {
            if (Level < MAX)
                Level++;
            Console.WriteLine("Condition level is {0}", Level);
        }

        public void DropLevel()
        {
            if (Level > MIN)
                Level--;
            Console.WriteLine("Condition level is {0}", Level);
        }
    }
    public interface ICommand
    {
        public void Execute();
        public void Undo();

        public List<ICommand> GetCommands()
        {
            List<ICommand> l = new List<ICommand>();
            l.Add(this);
            return l;
        }
    }
    public class NoCommand : ICommand
    {
        public void Execute()
        { }
        public void Undo()
        { }
    }

    public class MacroConditionerCommand : ICommand
    {
        public List<ICommand> listCommand;

        public MacroConditionerCommand(List<ICommand> lc)
        {
            listCommand = new List<ICommand>(lc);
        }

        public void Execute()
        {
            foreach (ICommand c in listCommand)
                c.Execute();
        }

        public List<ICommand> GetCommands()
        {
            return new List<ICommand>(listCommand);
        }

        public void Undo()
        {
            if (!(listCommand.Count == 0))
            {
                listCommand.Reverse();
                foreach (ICommand c in listCommand)
                    c.Undo();
                listCommand.Reverse();
            }
        }
    }
    public class ConditionerLevelCommand : ICommand
    {
        ConditionLevel level;

        public ConditionerLevelCommand(Conditioner c)
        {
            level = c.level;
        }

        public void Execute()
        {
            level.RaiseLevel();
        }

        public void Undo()
        {
            level.DropLevel();
        }
    }

    public class ConditionerCommand : ICommand
    {
        Conditioner conditioner;

        public ConditionerCommand(Conditioner c)
        {
            conditioner = c;
        }

        public void Execute()
        {
            conditioner.On();
        }

        public void Undo()
        {
            conditioner.Off();
        }
    }
    public class MicrowaveCommand : ICommand
    {
        Microwave microwave;
        uint time;

        public MicrowaveCommand(Microwave m, uint t)
        {
            microwave = m;
            time = t;
        }

        public void Execute()
        {
            microwave.Start(time);
        }

        public void Undo()
        {
            microwave.Stop();
        }
    }

    class InvokerPerson
    {
        static void Main(string[] args)
        {
            Microwave Sony = new Microwave();
            Conditioner Philips = new Conditioner();
            SmartController controller1 = new SmartController();
            controller1.SetCommand(0, new MicrowaveCommand(Sony, 2000));
            controller1.SetCommand(1, new ConditionerCommand(Philips));
            controller1.Run(0);
            controller1.Run(1);
            List<ICommand> commandsForConditioner = new List<ICommand>();
            commandsForConditioner.Add(new ConditionerCommand(Philips));
            commandsForConditioner.Add(new ConditionerLevelCommand(Philips));
            commandsForConditioner.Add(new ConditionerLevelCommand(Philips));
            controller1.SetCommand(3, new MacroConditionerCommand(commandsForConditioner));
            controller1.Run(3);
            controller1.Cancel();
            controller1.Cancel();
            controller1.Cancel();
            controller1.Cancel();
            controller1.Cancel();
        }
    }
}
