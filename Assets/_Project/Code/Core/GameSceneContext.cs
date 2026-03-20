using _Project.Code.Features.Player.MB;
using _Project.Code.Features.Time;

namespace _Project.Code
{
    public class GameSceneContext
    {
        private static GameSceneContext _instance;
        public static GameSceneContext Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameSceneContext();
                }
                return _instance;
            }
        }
        
        public Player Player;
        public TimeManager TimeManager;
        
        private GameSceneContext() { }
    }
}