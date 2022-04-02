using SharpRay.Eventing;

namespace SnakeEvents
{
    public struct SnakeGameOver : IGameEvent { public int Score { get; init; } }
}
