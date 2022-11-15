namespace SharpRay.Interfaces
{
    public interface IHasUpdate
    {
        /// <summary>
        /// Delta time is the interval since last render frame in ticks!
        /// </summary>
        /// <param name="deltaTime"></param>
        void Update(double deltaTime);
    }
}
