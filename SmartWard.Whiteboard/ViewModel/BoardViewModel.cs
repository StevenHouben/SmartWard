using SmartWard.Infrastructure;

namespace SmartWard.Whiteboard.ViewModel
{
    internal class BoardViewModel:ViewModelBase
    {
        public WardNode WardNode { get; set; }

        public BoardViewModel()
        {
            WardNode = WardNode.StartWardNodeAsSystem(WebConfiguration.DefaultWebConfiguration);
        }
    }
}
