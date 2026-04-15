// Done by me, duh.
// Use as you wish

namespace MediaLibraryMaintenance.CoreModules.ReencodeMarker
{
   using System.ComponentModel.Composition;
   using System.Diagnostics;
   using System.Text.Json;

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

      public async Task Execute(string[] args)
      {
         var folders = args.Where(x => !x.StartsWith("--"));
         var files = mediaFileCollector.CollectMediaFiles(folders);
         var mediaFileInfos = new List<MediaFileInfo>();

         foreach (var file in files)
         {
            var mediaFileInfo = await GetVideoCodecAsync(file);
            if (mediaFileInfo != null)
            {
               mediaFileInfos.Add(mediaFileInfo);
            }
         }

         var groups = mediaFileInfos.GroupBy(x => x.VideoCodec);
         foreach (var group in groups)
         {
            Console.WriteLine($"---------------{group.Key}------------");
            foreach (var mediaFileInfo in group)
            {
               Console.WriteLine(Path.GetFileName(mediaFileInfo.FilePath));
            }
         }
      }

      #endregion

      #region Methods

      private async Task<MediaFileInfo?> GetVideoCodecAsync(string filePath)
      {
         var startInfo = new ProcessStartInfo
         {
            FileName = "ffprobe",
            Arguments = $"-v error -select_streams v:0 -show_entries stream=codec_name -of json \"{filePath}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
         };

         using var process = new Process { StartInfo = startInfo };
         process.Start();

         var stdout = await process.StandardOutput.ReadToEndAsync();
         var stderr = await process.StandardError.ReadToEndAsync();

         await process.WaitForExitAsync();

         if (process.ExitCode != 0)
         {
            Console.WriteLine($"ffprobe failed for: {filePath}");
            Console.WriteLine(stderr);
            return null;
         }

         try
         {
            using var doc = JsonDocument.Parse(stdout);

            if (!doc.RootElement.TryGetProperty("streams", out var streams) || (streams.ValueKind != JsonValueKind.Array)
                                                                            || (streams.GetArrayLength() == 0))
            {
               return null;
            }

            var firstStream = streams[0];

            string? codecName = null;
            if (firstStream.TryGetProperty("codec_name", out var codecNameElement))
            {
               codecName = codecNameElement.GetString();
            }

            return new MediaFileInfo { FilePath = filePath, VideoCodec = codecName };
         }
         catch
         {
            return null;
         }
      }

      #endregion
   }

   [Export(typeof(IMediaFileCollector))]
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

   public class MediaFileInfo
   {
      #region Public Properties

      public string FilePath { get; set; }

      public string? VideoCodec { get; set; }

      #endregion
   }
}