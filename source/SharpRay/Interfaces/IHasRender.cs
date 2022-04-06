namespace SharpRay.Interfaces
{
    public interface IHasRender
    {
        string RenderLayer { get; set; }
        void Render();
    }
}
