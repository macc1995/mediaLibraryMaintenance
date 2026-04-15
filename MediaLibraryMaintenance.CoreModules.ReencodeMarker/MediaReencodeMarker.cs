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

         foreach (var info in mediaFileInfos)
         {
            if (info.VideoCodec != "hvec")
            {
               Console.WriteLine($"File {info.FilePath} is not h265! Marking for re-encode...");
               filesToReencode.Add(info.FilePath);
            }
         }

         if (filesToReencode.Count > 0)
         {
            try
            {
               File.WriteAllLines(outputFile, filesToReencode);
               Console.WriteLine($"\nWrote {filesToReencode.Count} file(s) to {outputFile}");
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
   }

   internal class DryRunStrategy : IRunStrategy
   {
      #region IRunStrategy Members

      public void ExecuteRun(IEnumerable<IMediaFileInfo> mediaFileInfos)
      {
         var count = 0;
         foreach (var info in mediaFileInfos)
         {
            if (info.VideoCodec != "hvec")
            {
               Console.WriteLine($"File {info.FilePath} is not h265!");
               count++;
            }
         }

         if (count > 0)
         {
            Console.WriteLine($"\nFound {count} file(s) that would be marked for re-encoding.");
         }
         else
         {
            Console.WriteLine("\nNo files need re-encoding.");
         }
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