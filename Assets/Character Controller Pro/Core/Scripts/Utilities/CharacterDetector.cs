using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Demo
{

    public abstract class CharacterDetector : MonoBehaviour
    {
        protected Dictionary<Transform, CharacterActor> characters = new Dictionary<Transform, CharacterActor>();
        protected List<int> onEnterDirtyTransforms = new List<int>();
        protected List<int> onStayDirtyTransforms = new List<int>();
        protected List<int> onExitDirtyTransforms = new List<int>();

        protected virtual void ProcessEnterAction(CharacterActor characterActor) { }
        protected virtual void ProcessStayAction(CharacterActor characterActor) { }
        protected virtual void ProcessExitAction(CharacterActor characterActor) { }

        /// <summary>
        /// Gets the number of characters (CharacterActor) currently inside this trigger.
        /// </summary>
        public int CharactersNumber { get; private set; }

        void FixedUpdate()
        {
            if (onEnterDirtyTransforms.Count != 0)
                onEnterDirtyTransforms.Clear();

            if (onStayDirtyTransforms.Count != 0)
                onStayDirtyTransforms.Clear();

            if (onExitDirtyTransforms.Count != 0)
                onExitDirtyTransforms.Clear();
        }

        void ProcessAction(Transform transform, List<int> characterActorsIDList, System.Action<CharacterActor> Action)
        {
            // If the component is not active then don't do anything.
            if (!this.enabled)
                return;

            CharacterActor characterActor = characters.GetOrRegisterValue<Transform, CharacterActor>(transform);

            if (characterActor == null)
                return;

            // We don't want to trigger the logic more than once (due to multiple colliders interacting with the trigger).
            // Check if we already have this actor registered using the ID.
            int characterActorID = characterActor.GetInstanceID();

            if (characterActorsIDList.Contains(characterActorID))
                return;

            // The actor is not registered, add the ID to the list.
            characterActorsIDList.Add(characterActorID);

            CharactersNumber++;

            if (Action != null)
                Action(characterActor);
        }

        // Enter ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        void OnTriggerEnter(Collider collider)
        {
            ProcessAction(collider.transform, onEnterDirtyTransforms, ProcessEnterAction);
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            ProcessAction(collider.transform, onEnterDirtyTransforms, ProcessEnterAction);
        }

        // Stay ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        void OnTriggerStay(Collider collider)
        {
            ProcessAction(collider.transform, onStayDirtyTransforms, ProcessStayAction);
        }

        void OnTriggerStay2D(Collider2D collider)
        {
            ProcessAction(collider.transform, onStayDirtyTransforms, ProcessStayAction);
        }


        // Enter ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        void OnTriggerExit(Collider collider)
        {
            ProcessAction(collider.transform, onExitDirtyTransforms, ProcessExitAction);
        }

        void OnTriggerExit2D(Collider2D collider)
        {
            ProcessAction(collider.transform, onExitDirtyTransforms, ProcessExitAction);
        }

    }

}
