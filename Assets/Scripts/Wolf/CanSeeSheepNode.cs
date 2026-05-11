using Assets.Scripts.AI.BehaviourTree;

namespace Assets.Scripts.Wolf
{
    public class CanSeeSheepNode : Node
    {
        private Wolf wolf;

        public CanSeeSheepNode(Wolf wolf)
        {
            this.wolf = wolf;
        }

        public override bool Execute()
        {
            return wolf.FindNearestSheep() != null;
        }
    }
}
