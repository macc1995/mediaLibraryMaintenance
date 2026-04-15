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

      

      static async Task Main(string[] args)
      {
         var prog = new Program();
            args = new[] { "D:\\Plex\\Movies" }.ToArray();
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