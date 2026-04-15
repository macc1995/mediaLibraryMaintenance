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
      Console.WriteLine("Scanning directories for media files...");
      var fileCount = 0;

      foreach (var folder in folders)
      {
         if (!Directory.Exists(folder))
         {
            Console.WriteLine($"Directory does not exist: {folder}");
            continue;
         }

         Console.WriteLine($"Scanning: {folder}");

         var files = Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories)
            .Where(x => extensions.Contains(Path.GetExtension(x), StringComparer.OrdinalIgnoreCase));
         
         foreach (var file in files)
         {
            fileCount++;
            Console.WriteLine($"Found file #{fileCount}: {Path.GetFileName(file)}");
            yield return file;
         }
      }

      Console.WriteLine($"\nTotal files found: {fileCount}");
   }

   #endregion
}