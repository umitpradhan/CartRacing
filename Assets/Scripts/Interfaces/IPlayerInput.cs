
namespace CarRacing.Interface
{
    public interface IPlayerInput
    {
        float GetSteering();
        bool GetAccelerate();
        bool GetBrake();
        bool GetUTurn();
        bool GetDrift();
    }
}