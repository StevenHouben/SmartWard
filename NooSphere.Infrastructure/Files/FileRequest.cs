using System;
using ABC.Model;


namespace ABC.Infrastructure.Files
{
    public class FileRequest
    {
        public FileResource Resouce { get; set; }
        public String Bytes { get; set; }
    }
}