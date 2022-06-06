using LifeHelper.Delegates;
using LifeHelper.Logic.Enums;

namespace LifeHelper.Logic.Managers
{
    public interface IStateManager
    {
        AppState ApplicationState { get; set; }
        OnAppStateChanged? StateChanged { set; }
    }
}
