namespace VisualServoCore.Controller
{
    public interface IController<TInput, TOutput>
    {

        public TOutput Run(TInput input);

    }
}
