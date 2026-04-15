
using MediaLibraryMaintenance.Interfaces;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MediaLibraryMaintenance
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var prog = new Program();
            prog.Run(args);
            
            
            
        }

        [ImportMany]
        IEnumerable<ILibraryHandlingModule> libraryHandlingModules { get; }
        private void Run(string[] args)
        {
            Console.WriteLine("Bonjour");
            Console.WriteLine();
            Console.WriteLine("Use --apply to wet run");

            Console.WriteLine("Hello, World!");
            var path = Assembly.GetExecutingAssembly().FullName
            var catalog = new DirectoryCatalog(path);
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);


        }
    }
}
