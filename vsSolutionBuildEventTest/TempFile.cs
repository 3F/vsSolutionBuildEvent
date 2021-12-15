using System;
using System.IO;
using Xunit;

namespace net.r_eg.vsSBE.Test
{
    internal sealed class TempFile: IDisposable
    {
        public string Dir { get; private set; }

        public string FullPath { get; private set; }

        public TempFile(bool insideDir = false, string ext = null)
        {
            string path = Path.GetTempPath();
            string name = Guid.NewGuid().ToString();

            if(ext != null) {
                name += ext;
            }

            FullPath = Path.Combine(path, name);
            if(insideDir)
            {
                Dir         = Directory.CreateDirectory(FullPath).FullName;
                FullPath    = Path.Combine(Dir, name);
            }
            using FileStream f = File.Create(FullPath);
        }

        #region IDisposable

        private bool disposed;

        private void Dispose(bool _)
        {
            if(!disposed)
            {
                try
                {
                    File.Delete(FullPath);
                    if(Dir != null) Directory.Delete(Dir);
                }
                catch(Exception ex)
                {
                    Assert.True(false, $"Failed disposing: {ex.Message}");
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
