using LifeHelper.Delegates;
using LifeHelper.Logic.Enums;

namespace LifeHelper.Logic.Managers
{
    public class StateManager : IStateManager
    {
        private StateManager() { }
        public static StateManager Instance { get; } = new StateManager();

        private AppState _ApplicationState = AppState.Normal;
        public AppState ApplicationState
        {
            get { return _ApplicationState; }
            set { _ApplicationState = value; if (StateChanged != null) StateChanged.Invoke(value); }
        }

        public OnAppStateChanged? StateChanged { private get; set; }
    }
}
