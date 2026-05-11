using Assets.Scripts.AI.BehaviourTree;

namespace Assets.Scripts.Wolf
{
    public class ChaseNode : Node
    {
        private Wolf wolf;

        public ChaseNode(Wolf wolf)
        {
            this.wolf = wolf;
        }

        public override bool Execute()
        {
            wolf.ChaseTarget();
            return true;
        }
    }
}
