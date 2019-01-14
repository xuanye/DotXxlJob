namespace Hessian.Net
{
    public interface IObjectSerializer
    {
        void Serialize(HessianOutputWriter writer, object graph);

        object Deserialize(HessianInputReader reader);
    }
}