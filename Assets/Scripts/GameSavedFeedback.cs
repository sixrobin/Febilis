namespace Templar
{
    using UnityEngine;

    public class GameSavedFeedback : MonoBehaviour
    {
        private static GameSavedFeedback s_instance;
        
        public const string SAVED = "Saved";
        
        [SerializeField] private Animator _animator = null;
        
        private int _savedStateHash;
        
        private void OnSaveDone(Templar.Manager.SaveManager.SaveDoneEventArgs args)
        {
            // Save animation is already playing.
            if (_animator.GetCurrentAnimatorStateInfo(0).shortNameHash == _savedStateHash)
                return;

            // Game is saved on load but we don't want to show feedback for this save.
            if (args.OnLoad)
                return;

            // Saving failed.
            if (!args.Success)
                return;
            
            _animator.SetTrigger(SAVED);
        }
        
        private void Awake()
        {
            if (s_instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }
            
            s_instance = this;
            DontDestroyOnLoad(gameObject);
            
            Manager.SaveManager.SaveDone += OnSaveDone;
            _savedStateHash = Animator.StringToHash("Saved");
        }

        private void OnDestroy()
        {
            Manager.SaveManager.SaveDone -= OnSaveDone;
        }
    }
}
