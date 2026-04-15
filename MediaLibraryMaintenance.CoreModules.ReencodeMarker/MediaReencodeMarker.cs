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

      private readonly IMediaFileInfoProvider mediaFileInfoProvider;

      private bool isLive;

      #endregion

      #region Constructors and Destructors

      [ImportingConstructor]
      public MediaReencodeMarker(IMediaFileInfoProvider mediaFileInfoProvider)
      {
         this.mediaFileInfoProvider = mediaFileInfoProvider;
      }

      #endregion

      #region ILibraryHandlingModule Members

      public int MenuOrder { get; } = 100;

      public void Init(bool isWetRun)
      {
         isLive = isWetRun;
      }

      public void PrintMenuItem()
      {
         Console.WriteLine("Mark media for re-encoding");
      }

      public async Task Execute(string[] args)
      {
         var path = args.Where(x => !x.Contains("--")).ToList();
         if ((path == null) || (path.Count == 0))
         {
            return;
         }

         var outputFileArg = args.FirstOrDefault(x => x.StartsWith("--output="));
         var outputFile = outputFileArg?.Replace("--output=", "") ?? "reencode-list.txt";

         var mediaInfo = await mediaFileInfoProvider.GetMediaFileInfos(path);
         IRunStrategy strategy = isLive ? new LiveRunStrategy(outputFile) : new DryRunStrategy();
         strategy.ExecuteRun(mediaInfo);
      }

      #endregion
   }

   internal class LiveRunStrategy : IRunStrategy
   {
      #region Constants and Fields

      private readonly string outputFile;

      #endregion

      #region Constructors and Destructors

      public LiveRunStrategy(string outputFile)
      {
         this.outputFile = outputFile;
      }

      #endregion

      #region IRunStrategy Members

      public void ExecuteRun(IEnumerable<IMediaFileInfo> mediaFileInfos)
      {
         var filesToReencode = new List<string>();
         long totalOriginalSize = 0;
         long totalExpectedSize = 0;

         foreach (var info in mediaFileInfos)
         {
            if (info.VideoCodec != "hvec")
            {
               Console.WriteLine($"File {info.FilePath} is not h265! Marking for re-encode...");
               filesToReencode.Add(info.ToString());

               totalOriginalSize += info.FileSize;
               totalExpectedSize += (long)(info.FileSize * 0.25);
            }
         }

         if (filesToReencode.Count > 0)
         {
            try
            {
               var spaceSavings = totalOriginalSize - totalExpectedSize;
               
               var outputLines = new List<string>(filesToReencode);
               outputLines.Add("");
               outputLines.Add("=== Space Savings Estimate (using maximum 25% size) ===");
               outputLines.Add($"Current total size: {FormatBytes(totalOriginalSize)}");
               outputLines.Add($"Expected total size: {FormatBytes(totalExpectedSize)}");
               outputLines.Add($"Space saved: {FormatBytes(spaceSavings)}");
               
               File.WriteAllLines(outputFile, outputLines);
               Console.WriteLine($"\nWrote {filesToReencode.Count} file(s) to {outputFile}");

               Console.WriteLine("\nSpace savings estimate (using maximum 25% size):");
               Console.WriteLine($"  Current total size: {FormatBytes(totalOriginalSize)}");
               Console.WriteLine($"  Expected total size: {FormatBytes(totalExpectedSize)}");
               Console.WriteLine($"  Space saved: {FormatBytes(spaceSavings)}");
            }
            catch (Exception ex)
            {
               Console.WriteLine($"Error writing output file: {ex.Message}");
            }
         }
         else
         {
            Console.WriteLine("\nNo files need re-encoding.");
         }
      }

      #endregion

      #region Methods

      private string FormatBytes(long bytes)
      {
         if (bytes >= 1073741824)
         {
            return $"{bytes / 1073741824.0:F2} GB";
         }

         if (bytes >= 1048576)
         {
            return $"{bytes / 1048576.0:F2} MB";
         }

         return $"{bytes} bytes";
      }

      #endregion
   }

   internal class DryRunStrategy : IRunStrategy
   {
      #region IRunStrategy Members

      public void ExecuteRun(IEnumerable<IMediaFileInfo> mediaFileInfos)
      {
         var count = 0;
         long totalOriginalSize = 0;
         long totalExpectedSize = 0;

         foreach (var info in mediaFileInfos)
         {
            if (info.VideoCodec != "hvec")
            {
               Console.WriteLine($"File {info.FilePath} is not h265!");
               count++;

               totalOriginalSize += info.FileSize;
               totalExpectedSize += (long)(info.FileSize * 0.25);
            }
         }

         if (count > 0)
         {
            Console.WriteLine($"\nFound {count} file(s) that would be marked for re-encoding.");

            var spaceSavings = totalOriginalSize - totalExpectedSize;
            Console.WriteLine("\nSpace savings estimate (using maximum 25% size):");
            Console.WriteLine($"  Current total size: {FormatBytes(totalOriginalSize)}");
            Console.WriteLine($"  Expected total size: {FormatBytes(totalExpectedSize)}");
            Console.WriteLine($"  Space saved: {FormatBytes(spaceSavings)}");
         }
         else
         {
            Console.WriteLine("\nNo files need re-encoding.");
         }
      }

      #endregion

      #region Methods

      private string FormatBytes(long bytes)
      {
         if (bytes >= 1073741824)
         {
            return $"{bytes / 1073741824.0:F2} GB";
         }

         if (bytes >= 1048576)
         {
            return $"{bytes / 1048576.0:F2} MB";
         }

         return $"{bytes} bytes";
      }

      #endregion
   }

   internal interface IRunStrategy
   {
      #region Public Methods and Operators

      void ExecuteRun(IEnumerable<IMediaFileInfo> mediaFileInfos);

      #endregion
   }
}