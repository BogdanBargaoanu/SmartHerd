using Assets.Scripts.AI.BehaviourTree;

namespace Assets.Scripts.Wolf
{
    public class WanderNode : Node
    {
        private Wolf wolf;

        public WanderNode(Wolf wolf)
        {
            this.wolf = wolf;
        }

        public override bool Execute()
        {
            wolf.Wander();
            return true;
        }
    }
}
