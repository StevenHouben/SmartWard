namespace SmartWard.Infrastructure.ActivityBase
{
    public interface IActivityNode
    {
        event InitializedHandler Initialized;
        event ConnectionEstablishedHandler ConnectionEstablished;

    }
}
