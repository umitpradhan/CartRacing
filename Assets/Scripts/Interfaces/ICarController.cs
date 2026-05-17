namespace CarRacing.Interface
{
    public interface ICarController
    {
        void Move(float steering, bool accelerate, bool brake, bool uTurn, bool drift);
    }
}