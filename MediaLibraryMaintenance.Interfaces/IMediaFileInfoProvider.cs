// Done by me, duh.
// Use as you wish

namespace MediaLibraryMaintenance.Interfaces;

public interface IMediaFileInfoProvider
{
   #region Public Methods and Operators

   Task<IEnumerable<IMediaFileInfo>> GetMediaFileInfos(IEnumerable<string> path);

   #endregion
}