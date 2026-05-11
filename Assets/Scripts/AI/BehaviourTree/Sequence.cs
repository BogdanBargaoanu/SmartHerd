using System.Collections.Generic;

namespace Assets.Scripts.AI.BehaviourTree
{
    public class Sequence : Node
    {
        private List<Node> children;

        public Sequence(List<Node> children)
        {
            this.children = children;
        }

        public override bool Execute()
        {
            foreach (Node child in children)
            {
                if (!child.Execute())
                    return false;
            }

            return true;
        }
    }
}
