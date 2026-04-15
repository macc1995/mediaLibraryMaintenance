// Done by me, duh.
// Use as you wish

namespace MediaLibraryMaintenance.Interfaces;

public class MediaFileInfo : IMediaFileInfo
{
   #region IMediaFileInfo Members

   public string FilePath { get; set; }

   public string VideoCodec { get; set; }

   #endregion

   #region Public Properties

   public long FileSize { get; set; }

   #endregion
}