namespace Templar
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class BoardsLink : MonoBehaviour
    {
        [SerializeField] private CardinalDirection _exitDir = CardinalDirection.NONE;

        public CardinalDirection ExitDir => _exitDir;
        public CardinalDirection EnterDir => _exitDir.Opposite();

        private void OnTriggerEnter2D(Collider2D other)
        {
            UnityEngine.Assertions.Assert.IsFalse(
                _exitDir == CardinalDirection.NONE,
                $"Boards Link instance on {transform.name} exit direction has an invalid value {_exitDir.ToString()}.");

            if (other.GetComponent<Unit.Player.PlayerController>())
                Manager.BoardsManager.TriggerLink(this);
        }
    }
}