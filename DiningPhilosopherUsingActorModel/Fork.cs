namespace DiningPhilosopherUsingActorModel
{
    public class Fork
    {
        public bool Dirty { get; private set; }
        
        public Fork()
        {
            Dirty = true;
        }

        public void MakeClean()
        {
            Dirty = false;
        }
        
        public void MakeDirty()
        {
            Dirty = true;
        }
    }
}