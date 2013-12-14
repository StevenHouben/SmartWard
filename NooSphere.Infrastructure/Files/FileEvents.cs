using ABC.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ABC.Infrastructure.Files
{
    /// <summary>
    /// File Events used by local File Server
    /// </summary>
    public delegate void FileChangedHandler( Object sender, FileEventArgs e );

    public delegate void FileAddedHandler( Object sender, FileEventArgs e );

    public delegate void FileRemovedHandler( Object sender, FileEventArgs e );

    public delegate void FileLockedHandler( Object sender, FileEventArgs e );

    public class FileEventArgs
    {
        public FileResource Resource { get; set; }
        public string LocalPath { get; set; }
        public FileEventArgs() {}

        public FileEventArgs( FileResource resource )
        {
            Resource = resource;
        }

        public FileEventArgs( FileResource resource, string localPath )
        {
            Resource = resource;
            LocalPath = localPath;
        }
    }

    public class GenericEventArgs<T>
    {
        public T Generic { get; set; }
        public GenericEventArgs() {}

        public GenericEventArgs( T generic )
        {
            Generic = generic;
        }
    }
}