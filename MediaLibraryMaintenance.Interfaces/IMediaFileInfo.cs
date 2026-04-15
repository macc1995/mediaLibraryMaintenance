// Done by me, duh.
// Use as you wish

namespace MediaLibraryMaintenance.Interfaces;

public interface IMediaFileInfo
{
   #region Public Properties

   string FilePath { get; set; }

   public long FileSize { get; set; }

   string VideoCodec { get; set; }

   #endregion
}