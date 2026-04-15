// Done by me, duh.
// Use as you wish

namespace MediaLibraryMaintenance
{
   using System.ComponentModel.Composition;
   using System.ComponentModel.Composition.Hosting;
   using System.Diagnostics;
   using System.Reflection;
   using System.Text.Json;

   using MediaLibraryMaintenance.Interfaces;

   internal class Program
   {
      #region Properties

      [ImportMany] private IEnumerable<ILibraryHandlingModule> LibraryHandlingModules { get; set; }

      #endregion

      #region Methods

      private static async Task<string?> GetVideoCodecAsync(string filePath)
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

            if (firstStream.TryGetProperty("codec_name", out var codecName))
            {
               return codecName.GetString();
            }

            return null;
         }
         catch
         {
            return null;
         }
      }

      static async Task Main(string[] args)
      {
         var prog = new Program();
         await prog.Run(args);
      }

      private async Task Run(string[] args)
      {
         Console.WriteLine("Bonjour");
         Console.WriteLine();
         Console.WriteLine("Use --apply to wet run");
         Console.WriteLine();

         var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
         var catalog = new DirectoryCatalog(path);
         var container = new CompositionContainer(catalog);
         container.ComposeParts(this);

         var modules = LibraryHandlingModules.OrderBy(x => x.MenuOrder).ToList();
         var selectedIndex = 0;

         while (true)
         {
            Console.Clear();
            Console.WriteLine("Available modules (use ↑↓ arrows, press Enter to select):");
            Console.WriteLine();

            for (var i = 0; i < modules.Count; i++)
            {
               if (i == selectedIndex)
               {
                  Console.ForegroundColor = ConsoleColor.Black;
                  Console.BackgroundColor = ConsoleColor.White;
                  Console.Write("► ");
               }
               else
               {
                  Console.Write("  ");
               }

               modules[i].PrintMenuItem();
               Console.ResetColor();
            }

            var key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.UpArrow)
            {
               selectedIndex = (selectedIndex - 1 + modules.Count) % modules.Count;
            }
            else if (key.Key == ConsoleKey.DownArrow)
            {
               selectedIndex = (selectedIndex + 1) % modules.Count;
            }
            else if (key.Key == ConsoleKey.Enter)
            {
               Console.Clear();
               Console.WriteLine($"Executing module {selectedIndex + 1}...");
               var selectedModule = modules[selectedIndex];
               selectedModule.Execute(args);
               // Execute the selected module
               break;
            }
            else if (key.Key == ConsoleKey.Escape)
            {
               Console.WriteLine("Cancelled.");
               return;
            }
         }
      }

      #endregion
   }
}