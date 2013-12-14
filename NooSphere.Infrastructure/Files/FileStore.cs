using ABC.Infrastructure.Helpers;
using ABC.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;


namespace ABC.Infrastructure.Files
{
    public class FileStore : IFileStore
    {
        #region Events

        public event FileAddedHandler FileAdded;
        public event FileChangedHandler FileChanged;
        public event FileAddedHandler FileCopied;
        public event FileRemovedHandler FileRemoved;

        #endregion


        #region Properties

        public string BasePath { get; set; }

        #endregion


        #region Private Members

        readonly HttpClient _httpClient = new HttpClient();
        readonly Dictionary<string, FileResource> _files = new Dictionary<string, FileResource>();
        readonly object _lookUpLock = new object();
        readonly object _fileLock = new object();

        #endregion


        #region Public Methods

        public FileStore( string path )
        {
            BasePath = path;
        }

        public void AddFile( FileResource resource, byte[] fileInBytes, FileSource source )
        {
            //Check if we have a valid file
            Check( resource, fileInBytes );

            //See if we have file 
            if ( _files.ContainsKey( resource.Id ) )
            {
                if ( IsNewer( _files[ resource.Id ], resource ) )
                    UpdateFile( resource, fileInBytes, source );
                else return;
            }
            else
            {
                SaveToDisk( fileInBytes, resource );
                _files.Add( resource.Id, resource );
            }

            //Check what the source is and who we should inform
            switch ( source )
            {
                case FileSource.ActivityManager:
                    if ( FileAdded != null )
                        FileAdded( this, new FileEventArgs( resource ) );
                    break;
                case FileSource.ActivityClient:
                    if ( FileCopied != null )
                        FileCopied( this, new FileEventArgs( resource ) );
                    break;
            }
            Log.Out( "FileStore", string.Format( "Added file {0} to store", resource.Name ), LogCode.Log );
        }

        public void DownloadFile( FileResource resource, string path, FileSource source, string _connectionId = null )
        {
            Rest.DownloadStream( path, _connectionId ).ContinueWith( stream =>
            {
                Log.Out( "FileStore", string.Format( "Finished download for {0}", resource.Name ), LogCode.Log );
                AddFile( resource, stream.Result, source );
            } );
            Log.Out( "FileStore", string.Format( "Started download for {0}", resource.Name ), LogCode.Log );
        }

        bool IsNewer( FileResource resourceInFileStore, FileResource requestedResource )
        {
            return false;
        }

        public void AddFile( FileResource resource, Stream stream, FileSource source )
        {
            AddFile( resource, GetBytesFromStream( resource, stream ), source );
        }

        public void UpdateFile( FileResource resource, Stream stream, FileSource source )
        {
            UpdateFile( resource, GetBytesFromStream( resource, stream ), source );
        }

        public void UpdateFile( FileResource resource, byte[] fileInBytes, FileSource source )
        {
            SaveToDisk( fileInBytes, resource );
            _files[ resource.Id ] = resource;

            switch ( source )
            {
                case FileSource.ActivityManager:
                    if ( FileChanged != null )
                        FileChanged( this, new FileEventArgs( resource ) );
                    break;
            }
            Log.Out( "FileStore", string.Format( "Updated file {0} to store", resource.Name ), LogCode.Log );
        }

        public void RemoveFile( FileResource resource )
        {
            _files.Remove( resource.Id );
            File.Delete( BasePath + resource.RelativePath );
            if ( FileRemoved != null )
                FileRemoved( this, new FileEventArgs( resource ) );
            Log.Out( "FileStore", string.Format( "FileStore: Removed file {0} from store", resource.Name ), LogCode.Log );
        }

        public bool LookUp( string id )
        {
            lock ( _lookUpLock )
                return _files.ContainsKey( id );
        }

        public Stream GetStreamFromFile( FileResource resource )
        {
            lock ( _fileLock )
                return new FileStream( BasePath + resource.RelativePath, FileMode.Open, FileAccess.Read, FileShare.Read );
        }

        public byte[] GetBytesFromFile( FileResource resource )
        {
            var fi = new FileInfo( BasePath + resource.RelativePath );
            var buffer = new byte[fi.Length];

            lock ( _fileLock )
                using ( var fs = new FileStream( fi.FullName, FileMode.Open, FileAccess.Read, FileShare.Read ) )
                    fs.Read( buffer, 0, (int)fs.Length );

            return buffer;
        }

        public void Updatefile( FileResource resource, byte[] fileInBytes )
        {
            Task.Factory.StartNew(
                delegate
                {
                    _files[ resource.Id ] = resource;
                    SaveToDisk( fileInBytes, resource );
                    if ( FileChanged != null )
                        FileChanged( this, new FileEventArgs( resource ) );
                    Log.Out( "FileStore", string.Format( "FileStore: Updated file {0} in store", resource.Name ), LogCode.Log );
                } );
        }

        /// <summary>
        /// Initializes a directory for future file saving. It uses the implicitly
        /// called ToString() method to convert the object to a path
        /// </summary>
        /// <param name="relative"> </param>
        public void IntializePath( object relative )
        {
            var path = Path.Combine( BasePath, relative.ToString() );
            //In case the activity path does not exist yet, we'll create one
            if ( !Directory.Exists( path ) )
            {
                var dInfo = Directory.CreateDirectory( path );
            }
        }

        public void CleanUp( string path )
        {
            if ( Directory.Exists( Path.Combine( BasePath, path ) ) )
                Directory.Delete( Path.Combine( BasePath, path ), true );
        }

        #endregion


        #region Private Methods

        byte[] GetBytesFromStream( FileResource resource, Stream stream )
        {
            var buffer = new byte[resource.Size];
            var ms = new MemoryStream();
            int bytesRead;
            do
            {
                bytesRead = stream.Read( buffer, 0, buffer.Length );
                ms.Write( buffer, 0, bytesRead );
            } while ( bytesRead > 0 );
            ms.Close();
            return buffer;
        }

        void Check( FileResource resource, byte[] fileInBytes )
        {
            if ( _files == null )
                throw new Exception( "Filestore: Not initialized" );
            if ( resource == null )
                throw new Exception( ( "Filestore: Resource not found" ) );
            if ( fileInBytes == null )
                throw new Exception( ( "Filestore: Bytearray null" ) );
            if ( fileInBytes.Length == 0 )
                throw new Exception( ( "Filestore: Bytearray empty" ) );
        }

        void SaveToDisk( byte[] fileInBytes, FileResource resource )
        {
            var path = Path.Combine( BasePath, resource.RelativePath );
            var dir = Path.GetDirectoryName( path );
            if ( dir != null && !Directory.Exists( dir ) ) Directory.CreateDirectory( dir );

            lock ( _fileLock )
            {
                if ( !File.Exists( @path ) )
                {
                    using ( var fileToupload = new FileStream( @path, FileMode.OpenOrCreate ) )
                    {
                        fileToupload.Write( fileInBytes, 0, fileInBytes.Length );
                        fileToupload.Close();
                        fileToupload.Dispose();

                        //File.SetCreationTimeUtc(path, DateTime.Parse(resource.CreationTime));
                        //File.SetLastWriteTimeUtc(path, DateTime.Parse(resource.LastWriteTime));
                        Console.WriteLine( "FileStore: Saved file {0} to disk at {1}", resource.Name,
                                           path );
                        Log.Out( "FileStore", string.Format( "FileStore: Saved file {0} to disk at {1}", resource.Name,
                                                             path ), LogCode.Log );
                    }
                }
                else
                    Log.Out( "FileStore", string.Format( "FileStore: file {0} already in store", resource.Name,
                                                         path ), LogCode.Log );
            }
        }

        #endregion
    }

    class DownloadState
    {
        public FileResource Resource { get; set; }
        public FileSource FileSource { get; set; }

        public DownloadState( FileResource resource, FileSource fileSource )
        {
            Resource = resource;
            FileSource = fileSource;
        }
    }
}