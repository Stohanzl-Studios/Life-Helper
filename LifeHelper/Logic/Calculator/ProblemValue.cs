using LifeHelper.Logic.Interfaces;
using SharedResources.Enums;

namespace LifeHelper.Logic.Calculator
{
    public class ProblemValue : IProblemValue
    {
        public double Value { get; set; }
        public MathOperation Operation { get; set; }
        public MathOperation Special { get; set; }
    }
}
