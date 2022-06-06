using LifeHelper.Logic.Interfaces;
using SharedResources.Enums;

namespace LifeHelper.Logic.Calculator
{
    public class Problem : IProblem
    {
        public Problem() { }

        public IMathValue[]? Values { get; set; }
        public MathOperation Operation { get; set; }
        public MathOperation Special { get; set; }

        public IProblem Deserialize(string data)
        {
            string temp = "";
            List<IMathValue> values = new List<IMathValue>();
            MathOperation specialOperation = MathOperation.None;
            int doneProblems = 1;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == '√')
                {
                    specialOperation = specialOperation | MathOperation.SquareRoot;
                    continue;
                }
                else if (data[i] == '²')
                {
                    specialOperation = specialOperation | MathOperation.Square;
                }
                if (int.TryParse(data[i] + "", out int tempp) || data[i] == ',')
                    temp += data[i];
                else if (data[i] == '(')
                {
                    //(2+7)+(2+7)
                    string problemSerialized = data.Remove(0, i + 1);
                    IProblem problem = Core.CreateIProblem().Deserialize(problemSerialized);
                    problem.Special = specialOperation;
                    string[] problems = data.Split(')');
                    i = 0;
                    for (int inner = 0; inner < GetIndexOfProblems(problem) + doneProblems; inner++)
                    {
                        i += problems[inner].Length + 1;
                    }
                    MathOperation op = i < data.Length ? EnumHelper.GetMathOperation(data[i]) : MathOperation.None;
                    if (op == MathOperation.Square)
                    {
                        problem.Special = problem.Special | MathOperation.Square;
                        problem.Operation = i + 1 < data.Length ? EnumHelper.GetMathOperation(data[i + 1]) : MathOperation.None;
                    }
                    values.Add(problem);
                    doneProblems++;
                    specialOperation = MathOperation.None;
                }
                else if (data[i] == ')')
                {
                    values.Add(Core.CreateIProblemValue(EnumHelper.GetMathOperation(data[i - temp.Length]), double.Parse(temp), specialOperation));
                    Operation = i + 1 < data.Length ? EnumHelper.GetMathOperation(data[i + 1]) : MathOperation.None;
                    if (Operation == MathOperation.Square)
                    {
                        Special = Special | MathOperation.Square;
                        Operation = i + 2 < data.Length ? EnumHelper.GetMathOperation(data[i + 2]) : MathOperation.None;
                    }
                    break;
                }
                else if (temp != string.Empty)
                {
                    double result = double.Parse(temp);
                    MathOperation op = EnumHelper.GetMathOperation(data[i]);
                    if (op.HasFlag(MathOperation.Square) && i + 1 < data.Length)
                        values.Add(Core.CreateIProblemValue(EnumHelper.GetMathOperation(data[i + 1]), result, specialOperation));
                    else if (op != MathOperation.Square)
                        values.Add(Core.CreateIProblemValue(EnumHelper.GetMathOperation(data[i]), result, specialOperation));
                    else
                        values.Add(Core.CreateIProblemValue(MathOperation.None, result, specialOperation));
                    temp = "";
                    specialOperation = MathOperation.None;
                }
                if (i + 1 == data.Length && temp.Length > 0 && temp != string.Empty)
                    values.Add(Core.CreateIProblemValue(EnumHelper.GetMathOperation(data[i]), double.Parse(temp), specialOperation));
            }
            Values = values.ToArray();
            return this;
        }
        public string Serialize(bool solve = false)
        {
            string result = "";
            if (Values == null)
                return result;
            if (solve)
                return $"{Solve()}";
            foreach (IMathValue value in Values)
            {
                if (value is IProblem)
                {
                    result += value.Special.HasFlag(MathOperation.SquareRoot) ? $"√(" : "(";
                    result += ((IProblem)value).Serialize();
                    result += (value.Special.HasFlag(MathOperation.Square) ? $")²" : ")") + EnumHelper.GetOperation(value.Operation);
                }
                else if (value is IProblemValue)
                {
                    IProblemValue problemValue = (IProblemValue)value;
                    result += (problemValue.Special.HasFlag(MathOperation.SquareRoot) ? "√" : "") + problemValue.Value + (problemValue.Special.HasFlag(MathOperation.Square) ? "²" : "") + EnumHelper.GetOperation(problemValue.Operation);
                }
            }
            return result;
        }
        public double Solve()
        {
            if (Values == null)
                return 0;
            double result = 0;
            MathOperation nextOperation = MathOperation.None;
            List<IMathValue> values = new List<IMathValue>();
            values.AddRange(Values);
            int mathIndex = values.FindIndex(x => x.Operation == MathOperation.Multiplication || x.Operation == MathOperation.Division || x.Operation == MathOperation.Modulus || x.Special != MathOperation.None || x is IProblem);
            while (mathIndex >= 0)
            {
                if (values[mathIndex] is IProblem)
                {
                    values[mathIndex] = Core.CreateIProblemValue(values[mathIndex].Operation, ((IProblem)values[mathIndex]).Solve(), values[mathIndex].Special);
                    mathIndex = values.FindIndex(x => x.Operation == MathOperation.Multiplication || x.Operation == MathOperation.Division || x is IProblem);
                }
                else if (values[mathIndex] is IProblemValue)
                {
                    if (mathIndex + 1 < values.Count)
                    {
                        values[mathIndex + 1] = MakeOperation(values[mathIndex], values[mathIndex + 1], values[mathIndex].Operation);
                        values.RemoveAt(mathIndex);
                    }
                    else
                        values[mathIndex] = MakeOperation(values[mathIndex], null, values[mathIndex].Operation);
                    mathIndex = values.FindIndex(x => x.Operation == MathOperation.Multiplication || x.Operation == MathOperation.Division || x.Special != MathOperation.None || x is IProblem);
                }
            }
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] is IProblem)
                {
                    IProblem problem = (IProblem)values[i];
                    double problemResult = problem.Solve();
                    if (problem.Special.HasFlag(MathOperation.Square))
                        problemResult = problemResult * problemResult;
                    if (problem.Special.HasFlag(MathOperation.SquareRoot))
                        problemResult = Math.Sqrt(problemResult);
                    switch (nextOperation)
                    {
                        case MathOperation.Addition: result += problemResult; break;
                        case MathOperation.Subtraction: result -= problemResult; break;
                        case MathOperation.Division: result = result / problemResult; break;
                        case MathOperation.Multiplication: result = result * problemResult; break;
                    }
                    nextOperation = problem.Operation;
                }
                else if (values[i] is IProblemValue)
                {
                    IProblemValue problemValue = (IProblemValue)values[i];
                    double value = problemValue.Value;
                    if (problemValue.Special.HasFlag(MathOperation.Square))
                        value = value * value;
                    if (problemValue.Special.HasFlag(MathOperation.SquareRoot))
                        value = Math.Sqrt(value);
                    switch (nextOperation)
                    {
                        case MathOperation.None: if (i == 0) result = value; break;
                        case MathOperation.Addition: result += value; break;
                        case MathOperation.Subtraction: result -= value; break;
                        case MathOperation.Division: result = result / value; break;
                        case MathOperation.Multiplication: result = result * value; break;
                    }
                    nextOperation = problemValue.Operation;
                }
            }
            return result;
        }

        private IProblemValue MakeOperation(IMathValue value1, IMathValue? value2, MathOperation operation)
        {
            IProblemValue solvedValue = Core.CreateIProblemValue();

            if (value1 is IProblemValue)
            {
                IProblemValue value1Problem = (IProblemValue)value1;
                double value = value1Problem.Value;
                if (value1Problem.Special.HasFlag(MathOperation.Square))
                    value = value * value;
                if (value1Problem.Special.HasFlag(MathOperation.SquareRoot))
                    value = Math.Sqrt(value);
                solvedValue.Value = value;
                solvedValue.Operation = value1Problem.Operation;
            }
            else if (value1 is IProblem)
            {
                IProblem problem1 = (IProblem)value1;
                solvedValue.Value = problem1.Solve();
                if (problem1.Special.HasFlag(MathOperation.Square))
                    solvedValue.Value = solvedValue.Value * solvedValue.Value;
                if (problem1.Special.HasFlag(MathOperation.SquareRoot))
                    solvedValue.Value = Math.Sqrt(solvedValue.Value);
                solvedValue.Operation = problem1.Operation;
            }
            if (value2 != null)
            {
                if (value2 is IProblemValue)
                {
                    IProblemValue value2PRoblem = (IProblemValue)value2;
                    double value = value2PRoblem.Value;
                    if (value2PRoblem.Special.HasFlag(MathOperation.Square))
                        value = value * value;
                    if (value2PRoblem.Special.HasFlag(MathOperation.SquareRoot))
                        value = Math.Sqrt(value);
                    switch (solvedValue.Operation)
                    {
                        case MathOperation.Addition: solvedValue.Value += value; break;
                        case MathOperation.Subtraction: solvedValue.Value -= value; break;
                        case MathOperation.Division: solvedValue.Value = solvedValue.Value / value; break;
                        case MathOperation.Multiplication: solvedValue.Value = solvedValue.Value * value; break;
                        case MathOperation.Modulus: solvedValue.Value = solvedValue.Value % value; break;
                    }
                    solvedValue.Operation = value2PRoblem.Operation;
                }
                else if (value2 is IProblem)
                {
                    IProblem problem2 = (IProblem)value2;
                    switch (solvedValue.Operation)
                    {
                        case MathOperation.Addition: solvedValue.Value += problem2.Solve(); break;
                        case MathOperation.Subtraction: solvedValue.Value -= problem2.Solve(); break;
                        case MathOperation.Division: solvedValue.Value = solvedValue.Value / problem2.Solve(); break;
                        case MathOperation.Multiplication: solvedValue.Value = solvedValue.Value * problem2.Solve(); break;
                        case MathOperation.Modulus: solvedValue.Value = solvedValue.Value % problem2.Solve(); break;
                    }
                    if (problem2.Special.HasFlag(MathOperation.Square))
                        solvedValue.Value = solvedValue.Value * solvedValue.Value;
                    if (problem2.Special.HasFlag(MathOperation.SquareRoot))
                        solvedValue.Value = Math.Sqrt(solvedValue.Value);
                    solvedValue.Operation = problem2.Operation;
                }
            }
            solvedValue.Special = MathOperation.None;
            return solvedValue;
        }
        private int GetIndexOfProblems(IProblem root)
        {
            int result = 0;
            List<IMathValue> childrens = new List<IMathValue>();
            childrens.AddRange(root.Values ?? new IMathValue[1]);
            childrens.RemoveAll(x => x is IProblemValue == true);
            foreach (IMathValue problem in childrens)
            {
                int index = GetIndexOfProblems((IProblem)problem);
                result += index + 1;
            }
            return result;
        }
    }
}