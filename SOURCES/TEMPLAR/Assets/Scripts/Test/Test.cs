namespace Templar
{
    using RSLib.Extensions;
    using UnityEngine;
    using System.Linq;

    public class Test : MonoBehaviour
    {
        public RSLib.Audio.ClipProvider[] clips;
        
        private void Start()
        {
            RSLib.Audio.AudioManager.PlaySounds(clips);
        }

        private void Update()
        {
        }
    }
}