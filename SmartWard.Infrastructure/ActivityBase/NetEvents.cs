namespace SmartWard.Infrastructure.ActivityBase
{
    public enum ActivityEvent
    {
        ActivityAdded,
        ActivityRemoved,
        ActivityChanged,
        ActivitySwitched
    }
    public enum DeviceEvent
    {
        DeviceAdded,
        DeviceRemoved,
        DeviceRoleChanged
    }
    public enum FileEvent
    {
        FileDownloadRequest,
        FileUploadRequest,
        FileDeleteRequest
    }
    public enum UserEvent
    {
        UserAdded,
        UserRemoved,
        UserUpdate,
        UserOnline,
        UserOffline
    }
}
