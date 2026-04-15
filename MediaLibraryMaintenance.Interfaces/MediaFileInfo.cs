// Done by me, duh.
// Use as you wish

namespace MediaLibraryMaintenance.Interfaces;

public class MediaFileInfo : IMediaFileInfo
{
   #region IMediaFileInfo Members

   public string FilePath { get; set; }

   public string VideoCodec { get; set; }

   public long FileSize { get; set; }

   #endregion

   public override string ToString()
   {
      var fileSizeFormatted = FormatFileSize(FileSize);
      var expectedSizeFormatted = GetExpectedSizeFormatted();

      return $"{FilePath} - Codec: {VideoCodec} - Size: {fileSizeFormatted} - Expected size: {expectedSizeFormatted}";
   }

   private string FormatFileSize(long bytes)
   {
      return bytes switch
      {
         >= 1073741824 => $"{bytes / 1073741824.0:F2} GB",
         >= 1048576 => $"{bytes / 1048576.0:F2} MB",
         _ => $"{bytes} bytes"
      };
   }

   private string GetExpectedSizeFormatted()
   {
      if (VideoCodec == "hvec")
      {
         return FormatFileSize(FileSize);
      }

      var minExpectedSize = (long)(FileSize * 0.15);
      var maxExpectedSize = (long)(FileSize * 0.25);

      if (FileSize >= 1073741824)
      {
         return $"{minExpectedSize / 1073741824.0:F2} GB-{maxExpectedSize / 1073741824.0:F2} GB";
      }
      else if (FileSize >= 1048576)
      {
         return $"{minExpectedSize / 1048576.0:F2} MB-{maxExpectedSize / 1048576.0:F2} MB";
      }
      else
      {
         return $"{minExpectedSize}-{maxExpectedSize} bytes";
      }
   }
}