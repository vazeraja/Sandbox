using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Aarthificial.Reanimation.Nodes
{
    public abstract class ReanimatorNode : ScriptableObject {
        
        [HideInInspector] public string guid;
        [TextArea] public string title = string.Empty;
        [HideInInspector] public Vector2 position;

        [field: NonSerialized]
        public virtual bool needsAnimationPreview { get; set; } = false;

        public abstract TerminationNode Resolve(IReadOnlyReanimatorState previousState, ReanimatorState nextState);

        protected void AddTrace(ReanimatorState nextState)
        {
#if UNITY_EDITOR
            nextState.AddTrace(this);
#endif
        }

        public virtual ReanimatorNode Copy()
        {
            return Instantiate(this);
        }
    }
}