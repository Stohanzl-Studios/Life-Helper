using SharedResources.Interfaces.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedResources.Interfaces
{
    public interface IResponseDelegateBase : IDelegateBase
    {
        IResponse Response { get; }
    }
}
