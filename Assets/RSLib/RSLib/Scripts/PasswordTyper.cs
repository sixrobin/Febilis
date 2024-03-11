namespace RSLib
{
    using UnityEngine;
    #if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
    #endif
    
    public class PasswordTyper : MonoBehaviour
    {
        #if ODIN_INSPECTOR
        [BoxGroup("Data")]
        #endif
        [SerializeField] private string _password = string.Empty;
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Callbacks")]
        #endif
        [SerializeField] private UnityEngine.Events.UnityEvent _onUnlock = null;
        
        #if ODIN_INSPECTOR
        [FoldoutGroup("Callbacks")]
        #endif
        [SerializeField] private UnityEngine.Events.UnityEvent _onLock = null;

        private bool _unlocked;
        private string _currentWord;
        
        private void Update()
        {
            string inputString = Input.inputString;
            _currentWord += inputString;

            if (!_password.StartsWith(_currentWord))
            {
                _currentWord = inputString; // Reset current word but register current input.
            }
            else if (_password == _currentWord)
            {
                _unlocked = !_unlocked;
                _currentWord = string.Empty;
                
                if (_unlocked)
                    _onUnlock?.Invoke();
                else
                    _onLock?.Invoke();
            }
        }
    }
}
