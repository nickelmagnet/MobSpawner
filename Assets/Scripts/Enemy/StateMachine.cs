namespace MobSpawner.Enemy
{
    public interface IState
    {
        void Enter();
        void Tick();
        void Exit();
    }
    public class StateMachine
    {
        public IState Current { get; private set; }

        public void ChangeState(IState next)
        {
            if (next == Current) return;
            Current?.Exit();
            Current = next;
            Current?.Enter();
        }

        public void Tick() => Current?.Tick();
    }
}