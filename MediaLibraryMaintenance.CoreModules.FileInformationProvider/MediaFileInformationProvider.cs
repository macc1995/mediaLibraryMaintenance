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

         Console.WriteLine($"\nAnalyzing {files.Count} file(s) in parallel...\n");
         
         var processedCount = 0;
         var lockObj = new object();

         var options = new ParallelOptions
         {
            MaxDegreeOfParallelism = Environment.ProcessorCount
         };

         var infos = new List<MediaFileInfo>();

         await Parallel.ForEachAsync(files, options, async (file, cancellationToken) =>
         {
            var factory = new MediaFileInfoFactory();
            var info = await factory.Create(file);
            
            lock (lockObj)
            {
               infos.Add(info);
               processedCount++;
               Console.WriteLine($"[{processedCount}/{files.Count}] Processed: {Path.GetFileName(file)}");
            }
         });

         Console.WriteLine($"\nCompleted analyzing {infos.Count} file(s).\n");
         return infos;
      }

      #endregion
   }
}