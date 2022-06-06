using SharedResources.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedResources.Interfaces
{
    public interface IDelegateBase
    {
        Result Result { get; }
        string? Error { get; }
        string? Message { get; }
    }
}
