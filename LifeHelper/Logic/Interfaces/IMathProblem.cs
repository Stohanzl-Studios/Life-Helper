using SharedResources.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeHelper.Logic.Interfaces
{
    public interface IMathValue
    {
        MathOperation Operation { get; set; }
        MathOperation Special { get; set; }
    }
}
