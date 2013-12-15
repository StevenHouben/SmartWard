namespace ABC.Infrastructure.Events
{
    public enum NotificationType
    {
        ActivityAdded,
        ActivityChanged,
        ActivityRemoved,
        DeviceAdded,
        DeviceChanged,
        DeviceRemoved,
        UserAdded,
        UserChanged,
        UserRemoved,
        ResourceAdded,
        ResourceChanged,
        ResourceRemoved,
        FileDownload,
        FileUpload,
        FileDelete,
        UserConnected,
        UserDisconnected,
        UserStatusChanged,
        Message,
        None,
        ParticipantAdded,
        ParticipantRemoved
    }
}