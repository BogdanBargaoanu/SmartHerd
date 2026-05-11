using Assets.Scripts.AI.FSM;
using UnityEngine;

namespace Assets.Scripts.AI.Utility
{
    public class WolfUtilityAI : MonoBehaviour
    {
        [Header("Needs")]
        public float hunger = 50f;
        public float stamina = 100f;
        public float health = 100f;

        [Header("Curves")]
        public AnimationCurve hungerCurve;
        public AnimationCurve staminaCurve;
        public AnimationCurve healthCurve;

        public float HuntScore()
        {
            return hungerCurve.Evaluate(hunger / 100f);
        }

        public float RestScore()
        {
            return staminaCurve.Evaluate(1f - stamina / 100f);
        }

        public float RetreatScore()
        {
            return healthCurve.Evaluate(1f - health / 100f);
        }

        public WolfState DecideState()
        {
            float hunt = HuntScore();
            float retreat = RetreatScore();
            float rest = RestScore();

            float max = Mathf.Max(hunt, retreat, rest);

            if (max == retreat)
                return WolfState.Retreat;

            if (max == hunt)
                return WolfState.Hunt;

            return WolfState.Idle;
        }
    }
}
