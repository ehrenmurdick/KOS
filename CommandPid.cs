using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace kOS
{
    [CommandAttribute(@"^CAP ([a-zA-Z][a-zA-Z0-9_]*?) INT TO ([^,]+)$")]
    public class CommandCapInt : Command
    {
        public CommandCapInt(Match regexMatch, ExecutionContext context) : base(regexMatch, context) { }

        public override void Evaluate()
        {
            string varname = RegexMatch.Groups[1].Value;
            float c = new Expression(RegexMatch.Groups[2].Value, ParentContext).Float();

            PidController controller = ParentContext.GetLock(varname) as PidController;


            State = ExecutionState.DONE;
        }
    }

    [CommandAttribute(@"^TUNE ([a-zA-Z][a-zA-Z0-9_]*?) TO ([^,]+),([^,]+),([^,]+)$")]
    public class CommandTune : Command
    {
        public CommandTune(Match regexMatch, ExecutionContext context) : base(regexMatch, context) { }

        public override void Evaluate()
        {
            string varname = RegexMatch.Groups[1].Value;
            float p = new Expression(RegexMatch.Groups[2].Value, ParentContext).Float();
            float i = new Expression(RegexMatch.Groups[3].Value, ParentContext).Float();
            float d = new Expression(RegexMatch.Groups[4].Value, ParentContext).Float();

            PidController controller = ParentContext.GetLock(varname) as PidController;

            controller.kp = p;
            controller.ki = i;
            controller.kp = d;

            State = ExecutionState.DONE;
        }
    }

    [CommandAttribute(@"^CONTROL ([a-zA-Z][a-zA-Z0-9_]*?) TO (.+?)$")]
    public class CommandPid : Command
    {
        public CommandPid(Match regexMatch, ExecutionContext context) : base(regexMatch, context) { }

        public override void Evaluate()
        {
            string varname = RegexMatch.Groups[1].Value;
            Expression expression = new Expression(RegexMatch.Groups[2].Value, ParentContext);

            PidController controller = new PidController(expression);


            ParentContext.Unlock(varname);
            ParentContext.Lock(varname, controller);

            State = ExecutionState.DONE;
        }
    }
}
