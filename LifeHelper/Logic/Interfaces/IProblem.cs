using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeHelper.Logic.Interfaces
{
    public interface IProblem : IMathValue
    {
        IMathValue[]? Values { get; set; }

        IProblem Deserialize(string data);
        string Serialize(bool solve = false);
        double Solve();
    } //2+ 2+ 2+ 2+ -25- 56* 25- 1+ 1
}
