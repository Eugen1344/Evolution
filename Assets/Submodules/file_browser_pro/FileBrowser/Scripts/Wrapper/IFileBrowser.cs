namespace Crosstales.FB.Wrapper
{
   /// <summary>Interface for all file browsers.</summary>
   public interface IFileBrowser
   {
      #region Properties

      /// <summary>Indicates if this wrapper can open multiple files.</summary>
      /// <returns>Wrapper can open multiple files.</returns>
      bool canOpenMultipleFiles { get; }

      /// <summary>Indicates if this wrapper can open multiple folders.</summary>
      /// <returns>Wrapper can open multiple folders.</returns>
      bool canOpenMultipleFolders { get; }

      /// <summary>Indicates if this wrapper is supporting the current platform.</summary>
      /// <returns>True if this wrapper supports current platform.</returns>
      bool isPlatformSupported { get; }

      /// <summary>Indicates if this wrapper is working directly inside the Unity Editor (without 'Play'-mode).</summary>
      /// <returns>True if this wrapper is working directly inside the Unity Editor.</returns>
      bool isWorkingInEditor { get; }

      #endregion


      #region Methods

      /// <summary>Open native file browser for a single file.</summary>
      /// <param name="title">Dialog title</param>
      /// <param name="directory">Root directory</param>
      /// <param name="defaultName">Default file name (currently only supported under Windows standalone)</param>
      /// <param name="extensions">List of extension filters. Filter Example: new ExtensionFilter("Image Files", "jpg", "png")</param>
      /// <returns>Returns a string of the chosen file. Empty string when cancelled</returns>
      string OpenSingleFile(string title, string directory, string defaultName, params ExtensionFilter[] extensions);

      /// <summary>Open native file browser for multiple files.</summary>
      /// <param name="title">Dialog title</param>
      /// <param name="directory">Root directory</param>
      /// <param name="defaultName">Default file name (currently only supported under Windows standalone)</param>
      /// <param name="multiselect">Allow multiple file selection</param>
      /// <param name="extensions">List of extension filters. Filter Example: new ExtensionFilter("Image Files", "jpg", "png")</param>
      /// <returns>Returns array of chosen files. Zero length array when cancelled</returns>
      string[] OpenFiles(string title, string directory, string defaultName, bool multiselect, params ExtensionFilter[] extensions);

      /// <summary>Open native folder browser for a single folder.</summary>
      /// <param name="title">Dialog title</param>
      /// <param name="directory">Root directory</param>
      /// <returns>Returns a string of the chosen folder. Empty string when cancelled</returns>
      string OpenSingleFolder(string title, string directory);

      /// <summary>Open native folder browser for multiple folders.</summary>
      /// <param name="title">Dialog title</param>
      /// <param name="directory">Root directory</param>
      /// <param name="multiselect">Allow multiple folder selection</param>
      /// <returns>Returns array of chosen folders. Zero length array when cancelled</returns>
      string[] OpenFolders(string title, string directory, bool multiselect);

      /// <summary>Open native save file browser.</summary>
      /// <param name="title">Dialog title</param>
      /// <param name="directory">Root directory</param>
      /// <param name="defaultName">Default file name</param>
      /// <param name="extensions">List of extension filters. Filter Example: new ExtensionFilter("Image Files", "jpg", "png")</param>
      /// <returns>Returns chosen file. Empty string when cancelled</returns>
      string SaveFile(string title, string directory, string defaultName, params ExtensionFilter[] extensions);

      /// <summary>Asynchronously opens native file browser for multiple files.</summary>
      /// <param name="title">Dialog title</param>
      /// <param name="directory">Root directory</param>
      /// <param name="defaultName">Default file name (currently only supported under Windows standalone)</param>
      /// <param name="multiselect">Allow multiple file selection</param>
      /// <param name="extensions">List of extension filters. Filter Example: new ExtensionFilter("Image Files", "jpg", "png")</param>
      /// <param name="cb">Callback for the async operation.</param>
      /// <returns>Returns array of chosen files. Zero length array when cancelled</returns>
      void OpenFilesAsync(string title, string directory, string defaultName, bool multiselect, ExtensionFilter[] extensions, System.Action<string[]> cb);

      /// <summary>Asynchronously opens native folder browser for multiple folders.</summary>
      /// <param name="title">Dialog title</param>
      /// <param name="directory">Root directory</param>
      /// <param name="multiselect">Allow multiple folder selection</param>
      /// <param name="cb">Callback for the async operation.</param>
      /// <returns>Returns array of chosen folders. Zero length array when cancelled</returns>
      void OpenFoldersAsync(string title, string directory, bool multiselect, System.Action<string[]> cb);

      /// <summary>Asynchronously opens native save file browser.</summary>
      /// <param name="title">Dialog title</param>
      /// <param name="directory">Root directory</param>
      /// <param name="defaultName">Default file name</param>
      /// <param name="extensions">List of extension filters. Filter Example: new ExtensionFilter("Image Files", "jpg", "png")</param>
      /// <param name="cb">Callback for the async operation.</param>
      /// <returns>Returns chosen file. Empty string when cancelled</returns>
      void SaveFileAsync(string title, string directory, string defaultName, ExtensionFilter[] extensions, System.Action<string> cb);

      //TODO add NEW methods
/*
      /// <summary>Open native file browser for a single file and reads the data.</summary>
      /// <param name="title">Dialog title</param>
      /// <param name="directory">Root directory</param>
      /// <param name="defaultName">Default file name (currently only supported under Windows standalone)</param>
      /// <param name="extensions">List of extension filters. Filter Example: new ExtensionFilter("Image Files", "jpg", "png")</param>
      /// <returns>Returns a string of the chosen file. Empty string when cancelled</returns>
      string OpenAndReadSingleFile(string title, string directory, string defaultName, params ExtensionFilter[] extensions);

      /// <summary>Open native file browser for multiple files and reads the data.</summary>
      /// <param name="title">Dialog title</param>
      /// <param name="directory">Root directory</param>
      /// <param name="defaultName">Default file name (currently only supported under Windows standalone)</param>
      /// <param name="multiselect">Allow multiple file selection</param>
      /// <param name="extensions">List of extension filters. Filter Example: new ExtensionFilter("Image Files", "jpg", "png")</param>
      /// <returns>Returns array of chosen files. Zero length array when cancelled</returns>
      string[] OpenAndReadFiles(string title, string directory, string defaultName, bool multiselect, params ExtensionFilter[] extensions);

      /// <summary>Open native save file browser and writes the data.</summary>
      /// <param name="title">Dialog title</param>
      /// <param name="directory">Root directory</param>
      /// <param name="defaultName">Default file name</param>
      /// <param name="extensions">List of extension filters. Filter Example: new ExtensionFilter("Image Files", "jpg", "png")</param>
      /// <returns>Returns chosen file. Empty string when cancelled</returns>
      string SaveAndWriteFile(string title, string directory, string defaultName, params ExtensionFilter[] extensions);

      /// <summary>Asynchronously opens native file browser for multiple files and reads the data.</summary>
      /// <param name="title">Dialog title</param>
      /// <param name="directory">Root directory</param>
      /// <param name="defaultName">Default file name (currently only supported under Windows standalone)</param>
      /// <param name="multiselect">Allow multiple file selection</param>
      /// <param name="extensions">List of extension filters. Filter Example: new ExtensionFilter("Image Files", "jpg", "png")</param>
      /// <param name="cb">Callback for the async operation.</param>
      /// <returns>Returns array of chosen files. Zero length array when cancelled</returns>
      void OpenAndReadFilesAsync(string title, string directory, string defaultName, bool multiselect, ExtensionFilter[] extensions, System.Action<string[]> cb);

      /// <summary>Asynchronously opens native save file browser and writes the data.</summary>
      /// <param name="title">Dialog title</param>
      /// <param name="directory">Root directory</param>
      /// <param name="defaultName">Default file name</param>
      /// <param name="extensions">List of extension filters. Filter Example: new ExtensionFilter("Image Files", "jpg", "png")</param>
      /// <param name="cb">Callback for the async operation.</param>
      /// <returns>Returns chosen file. Empty string when cancelled</returns>
      void SaveAndWriteFileAsync(string title, string directory, string defaultName, ExtensionFilter[] extensions, System.Action<string> cb);
*/
      #endregion
   }
}
// © 2018-2021 crosstales LLC (https://www.crosstales.com)