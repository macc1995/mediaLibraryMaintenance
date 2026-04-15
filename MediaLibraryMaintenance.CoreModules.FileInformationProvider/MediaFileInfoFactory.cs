// Done by me, duh.
// Use as you wish

namespace MediaLibraryMaintenance.CoreModules.FileInformationProvider;

using System.Diagnostics;
using System.Text.Json;

using MediaLibraryMaintenance.Interfaces;

public class MediaFileInfoFactory
{
   #region Constants and Fields

   private MediaFileInfo info = new MediaFileInfo();

   #endregion

   #region Public Methods and Operators

   public async Task<MediaFileInfo> Create(string filePath)
   {
      info.FilePath = filePath;
      var codec = await GetVideoCodecAsync(filePath);
      info.VideoCodec = codec;
      var fileInfo = new FileInfo(filePath);
      info.FileSize = fileInfo.Length;
      return info;
   }

   public async Task<string?> GetVideoCodecAsync(string filePath)
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

      using var process = new Process();
      process.StartInfo = startInfo;
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

         return codecName ?? "Unknown";
      }
      catch
      {
         return null;
      }
   }

   #endregion
}