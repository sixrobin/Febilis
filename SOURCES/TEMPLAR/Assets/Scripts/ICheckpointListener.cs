namespace Templar
{
    public interface ICheckpointListener
    {
        void OnCheckpointInteracted(Interaction.CheckpointController checkpointCtrl);
    }
}