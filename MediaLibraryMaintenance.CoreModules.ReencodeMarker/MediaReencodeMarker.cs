// Done by me, duh.
// Use as you wish

namespace MediaLibraryMaintenance.CoreModules.ReencodeMarker
{
   using System.ComponentModel.Composition;

   using MediaLibraryMaintenance.Interfaces;

   [Export(typeof(ILibraryHandlingModule))]
   public class MediaReencodeMarker : ILibraryHandlingModule
   {
      #region Constants and Fields

      private readonly IMediaFileCollector mediaFileCollector;

      #endregion

      #region Constructors and Destructors

      [ImportingConstructor]
      public MediaReencodeMarker(IMediaFileCollector mediaFileCollector)
      {
         this.mediaFileCollector = mediaFileCollector;
      }

      #endregion

      #region ILibraryHandlingModule Members

      public int MenuOrder { get; } = 100;

      public void PrintMenuItem()
      {
         Console.WriteLine("Mark media for re-encoding");
      }

      public void Execute(string[] args)
      {
         var folders = args.Where(x => !x.StartsWith("--"));
         var files = mediaFileCollector.CollectMediaFiles(folders);
      }

      #endregion
   }

   public class MediaFileCollector : IMediaFileCollector
   {
      #region Constants and Fields

      private string[] extensions = new[] { ".mkv", ".avi", ".mp4" };

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

   public interface IMediaFileCollector
   {
      #region Public Methods and Operators

      public IEnumerable<string> CollectMediaFiles(IEnumerable<string> folders);

      #endregion
   }
}