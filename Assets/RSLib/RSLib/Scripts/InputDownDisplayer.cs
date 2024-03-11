namespace RSLib
{
    using UnityEngine;
    #if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
    #endif

    /// <summary>
    /// Offers the possibility to enable some gameObjects on input down, and disable on input up.
    /// </summary>
    [DisallowMultipleComponent]
    public class InputDownDisplayer : MonoBehaviour
    {
        #if ODIN_INSPECTOR
        [BoxGroup("Input")]
        #endif
        [Tooltip("Set as None so that gameObjects display can not be triggered without removing the script instance.")]
        [SerializeField] private KeyCode _key = KeyCode.None;

#pragma warning disable CS0414
        #if ODIN_INSPECTOR
        [BoxGroup("Data")]
        #endif
        [Tooltip("The script instance will be destroyed in build on awake if this is set to true.")]
        [SerializeField] private bool _editorOnly = false;
#pragma warning restore CS0414

        #if ODIN_INSPECTOR
        [BoxGroup("Data")]
        #endif
        [Tooltip("GameObjects instances that should be displayed by the input.")]
        [SerializeField] private GameObject[] _objectsToDisplay = null;

        private void Display(bool state)
        {
            for (int i = _objectsToDisplay.Length - 1; i >= 0; --i)
                _objectsToDisplay[i].SetActive(state);
        }

        private void Awake()
        {
#if !UNITY_EDITOR
            if (_editorOnly)
                Destroy(this);
#endif

            Display(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(_key))
                Display(true);
            else if (Input.GetKeyUp(_key))
                Display(false);
        }
    }
}