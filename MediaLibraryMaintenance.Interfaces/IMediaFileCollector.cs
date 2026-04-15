// Done by me, duh.
// Use as you wish

namespace MediaLibraryMaintenance.Interfaces;

public interface IMediaFileCollector
{
   #region Public Methods and Operators

   public IEnumerable<string> CollectMediaFiles(IEnumerable<string> folders);

   #endregion
}