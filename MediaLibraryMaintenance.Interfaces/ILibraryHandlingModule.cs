// Done by me, duh.
// Use as you wish

namespace MediaLibraryMaintenance.Interfaces
{
   public interface ILibraryHandlingModule
   {
      #region Public Properties

      int MenuOrder { get; }

        #endregion

        #region Public Methods and Operators

        Task Execute(string[] args);

      void PrintMenuItem();

      #endregion
   }
}