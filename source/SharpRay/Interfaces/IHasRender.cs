namespace SharpRay.Interfaces
{
    public interface IHasRender
    {
        int RenderLayer { get; set; }
        void Render();
    }
}
