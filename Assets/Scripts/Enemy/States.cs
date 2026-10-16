namespace MobSpawner.Enemy
{
    public class IdleState : IState
    {
        private readonly EnemyBrain brain;
        private readonly StateMachine machine;
        public IdleState(EnemyBrain b, StateMachine m) { brain = b; machine = m; }

        public void Enter() => brain.Agent.ResetPath();
        public void Tick()
        {
            if (brain.CanSeeTarget) machine.ChangeState(brain.Chase);
        }
        public void Exit() { }
    }

    public class ChaseState : IState
    {
        private readonly EnemyBrain brain;
        private readonly StateMachine machine;
        public ChaseState(EnemyBrain b, StateMachine m) { brain = b; machine = m; }

        public void Enter() => brain.Agent.isStopped = false;
        public void Tick()
        {
            if (!brain.CanSeeTarget) { machine.ChangeState(brain.Idle); return; }
            if (brain.DistanceToTarget <= brain.AttackRange) { machine.ChangeState(brain.Attack); return; }
            brain.Agent.SetDestination(brain.Target.position);
        }
        public void Exit() { }
    }

    public class AttackState : IState
    {
        private readonly EnemyBrain brain;
        private readonly StateMachine machine;
        public AttackState(EnemyBrain b, StateMachine m) { brain = b; machine = m; }

        public void Enter() => brain.Agent.isStopped = true;
        public void Tick()
        {
            brain.FaceTarget();
            // Legs will attack from here later.
            if (brain.DistanceToTarget > brain.AttackExitRange) machine.ChangeState(brain.Chase);
        }
        public void Exit() => brain.Agent.isStopped = false;
    }
}