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
         var files = mediaFileCollector.CollectMediaFiles(path);
         var infos = new List<MediaFileInfo>();
         foreach (var file in files)
         {
            var factory = new MediaFileInfoFactory();

            var info = await factory.Create(file);
            infos.Add(info);
         }

         return infos;
      }

      #endregion
   }
}