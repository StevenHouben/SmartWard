using ABC.Model.Resources;


namespace ABC.Infrastructure.ActivityBase
{
    public class ResourceEventArgs
    {
        public IResource Resource { get; set; }
        public ResourceEventArgs() {}

        public ResourceEventArgs(IResource resource)
        {
            Resource = resource;
        }
    }

    public class ResourceRemovedEventArgs
    {
        public string Id { get; set; }
        public ResourceRemovedEventArgs() {}

        public ResourceRemovedEventArgs(string id)
        {
            Id = id;
        }
    }
}