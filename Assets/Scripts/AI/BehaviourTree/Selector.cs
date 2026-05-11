using System.Collections.Generic;

namespace Assets.Scripts.AI.BehaviourTree
{
    public class Selector : Node
    {
        private List<Node> children;

        public Selector(List<Node> children)
        {
            this.children = children;
        }

        public override bool Execute()
        {
            foreach (Node child in children)
            {
                if (child.Execute())
                    return true;
            }

            return false;
        }
    }
}
