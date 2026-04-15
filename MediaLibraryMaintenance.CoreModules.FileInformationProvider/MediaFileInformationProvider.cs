// Done by me, duh.
// Use as you wish

namespace MediaLibraryMaintenance.CoreModules.FileInformationProvider
{
   using System.ComponentModel.Composition;

   using MediaLibraryMaintenance.Interfaces;

   [Export(typeof(IMediaFileInfoProvider))]
   public class MediaFileInformationProvider : IMediaFileInfoProvider
   {
      #region Constants and Fields

      private readonly IMediaFileCollector mediaFileCollector;

      #endregion

      #region Constructors and Destructors

      [ImportingConstructor]
      public MediaFileInformationProvider(IMediaFileCollector mediaFileCollector)
      {
         this.mediaFileCollector = mediaFileCollector;
      }

      #endregion

      #region IMediaFileInfoProvider Members

      public async Task<IEnumerable<IMediaFileInfo>> GetMediaFileInfos(IEnumerable<string> path)
      {
         var files = mediaFileCollector.CollectMediaFiles(path).ToList();
         
         if (files.Count == 0)
         {
            Console.WriteLine("No media files found.");
            return new List<MediaFileInfo>();
         }

         Console.WriteLine($"\nAnalyzing {files.Count} file(s)...\n");
         
         var infos = new List<MediaFileInfo>();
         var processedCount = 0;

         foreach (var file in files)
         {
            processedCount++;
            Console.Write($"[{processedCount}/{files.Count}] Processing: {Path.GetFileName(file)}...");
            
            var factory = new MediaFileInfoFactory();
            var info = await factory.Create(file);
            infos.Add(info);
            
            Console.WriteLine(" Done");
         }

         Console.WriteLine($"\nCompleted analyzing {infos.Count} file(s).\n");
         return infos;
      }

      #endregion
   }
}