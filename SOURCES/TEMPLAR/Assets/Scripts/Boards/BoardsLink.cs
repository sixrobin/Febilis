namespace Templar.Boards
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class BoardsLink : MonoBehaviour
    {
        [SerializeField] private BoardDirection _exitDir = BoardDirection.NONE;

        public BoardDirection ExitDir => _exitDir;
        public BoardDirection EnterDir => _exitDir.Opposite();

        public Board OwnerBoard { get; private set; }

        public void SetOwnerBoard(Board board)
        {
            OwnerBoard = board;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            UnityEngine.Assertions.Assert.IsFalse(
                _exitDir == BoardDirection.NONE,
                $"Boards Link instance on {transform.name} exit direction has an invalid value {_exitDir.ToString()}.");

            if (other.GetComponent<Unit.Player.PlayerController>())
                Manager.BoardsManager.TriggerLink(this);
        }
    }
}