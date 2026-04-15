// Done by me, duh.
// Use as you wish

namespace MediaLibraryMaintenance
{
   using System.ComponentModel.Composition;
   using System.ComponentModel.Composition.Hosting;
   using System.Reflection;

   using MediaLibraryMaintenance.Interfaces;

   internal class Program
   {
      #region Constants and Fields

      private bool isWetRun;

      #endregion

      #region Properties

      [ImportMany] private IEnumerable<ILibraryHandlingModule>? LibraryHandlingModules { get; set; }

      #endregion

      #region Methods

      static async Task Main(string[] args)
      {
         var prog = new Program();
         var list = args.ToList();
         list.Add("D:\\Plex\\Movies");
         list.Add("--apply");
         args = list.ToArray();
         await prog.Run(args);
      }

      private async Task Run(string[] args)
      {
         if (args.Any(x => x.StartsWith("--apply")))
         {
            isWetRun = true;
            ;
         }

         Console.WriteLine("Bonjour");
         Console.WriteLine();
         Console.WriteLine("Use --apply to wet run");
         Console.WriteLine();

         var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
         if (path is null)
         {
            return;
         }

         var catalog = new DirectoryCatalog(path);
         var container = new CompositionContainer(catalog);
         container.ComposeParts(this);

         if (LibraryHandlingModules is null || !LibraryHandlingModules.Any())
         {
            return;
         }

         foreach (var libraryHandlingModule in LibraryHandlingModules)
         {
            libraryHandlingModule.Init(isWetRun);
         }

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
               await selectedModule.Execute(args);
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