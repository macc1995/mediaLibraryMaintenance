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

         var mediaInfo = await mediaFileInfoProvider.GetMediaFileInfos(path);
         foreach (var info in mediaInfo)
         {
            if (info.VideoCodec != "hvec")
            {
               Console.WriteLine($"File {info.FilePath} is not h265!");
            }
         }
      }

      #endregion
   }
}