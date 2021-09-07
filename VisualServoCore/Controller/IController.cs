namespace VisualServoCore.Controller
{
    public interface IController<TImage>
    {

        public (double Speed, double Steer) Run(TImage input);

    }
}
