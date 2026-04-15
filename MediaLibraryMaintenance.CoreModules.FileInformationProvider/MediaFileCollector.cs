// Done by me, duh.
// Use as you wish

namespace MediaLibraryMaintenance.CoreModules.FileInformationProvider;

using System.ComponentModel.Composition;

using MediaLibraryMaintenance.Interfaces;

[Export(typeof(IMediaFileCollector))]
public class MediaFileCollector : IMediaFileCollector
{
   #region Constants and Fields

   private readonly string[] extensions = [".mkv", ".avi", ".mp4"];

   #endregion

   #region IMediaFileCollector Members

   public IEnumerable<string> CollectMediaFiles(IEnumerable<string> folders)
   {
      foreach (var folder in folders)
      {
         if (!Directory.Exists(folder))
         {
            Console.WriteLine($"Directory does not exist: {folder}");
            continue;
         }

         var files = Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories)
            .Where(x => extensions.Contains(Path.GetExtension(x), StringComparer.OrdinalIgnoreCase));
         foreach (var file in files)
         {
            yield return file;
         }
      }
   }

   #endregion
}