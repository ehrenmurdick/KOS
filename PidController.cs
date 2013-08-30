using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace kOS
{
    public class PidController : Expression
    {
        Expression errorExpression;
        float err;
        float prevErr;
        float max = float.MaxValue;
        float min = float.MinValue;

        public float kp = 0.05f;
        public float ki = 0.000001f;
        public float kd = 0.05f;

        float i;
        float p;
        float d;

        public PidController(Expression errorExpression)
        {
            this.errorExpression = errorExpression;
        }

        public void Update(float time)
        {
            err = errorExpression.Float();

            i += err * time;
            float action = (kp * err) + (ki * i) + (kd * (err - prevErr) / time);
            float clamped = Math.Max(min, Math.Min(max, action));
            if (clamped != action)
            {
                i -= err * time;
            }

            prevErr = err;

            Value = action;
        }

    }
}
