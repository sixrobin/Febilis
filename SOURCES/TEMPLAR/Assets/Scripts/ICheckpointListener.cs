namespace Templar
{
    public interface ICheckpointListener
    {
        void OnCheckpointInteracted(Interaction.Checkpoint.CheckpointController checkpointCtrl);
    }
}