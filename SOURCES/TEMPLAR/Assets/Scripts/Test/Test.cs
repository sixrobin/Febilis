namespace Templar
{
    using System.Collections.Generic;
    using RSLib.Yield;
    using UnityEngine;
    using System.Linq;

    public class Test : MonoBehaviour
    {
        private System.Collections.IEnumerator Start()
        {
            RSLib.Yield.CustomCoroutine a = this.RunCustomCoroutine(FirstCoroutine(), _ => CoroutineCallback());
            RSLib.Yield.CustomCoroutine b = this.RunCustomCoroutine(SecondCoroutine());

            while (!a.IsDone && !b.IsDone)
                yield return null;
            
            Debug.Log("A or B done.");
        }

        void CoroutineCallback()
        {
            Debug.Log("Callback");
        }
        
        System.Collections.IEnumerator FirstCoroutine()
        {
            yield return new WaitForSeconds(1f);
            Debug.Log("A");
        }
        
        System.Collections.IEnumerator SecondCoroutine()
        {
            yield return new WaitForSeconds(3f);
            Debug.Log("B");
        }
    }
}