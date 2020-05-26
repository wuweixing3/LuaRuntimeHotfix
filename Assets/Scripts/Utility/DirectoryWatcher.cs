using System.IO;

public static class DirectoryWatcher
{
    public static void CreateWatch(string dirPath, FileSystemEventHandler handler)
    {
        if (!Directory.Exists(dirPath)) {
            return;
        }

        var watcher = new FileSystemWatcher {
            IncludeSubdirectories = true,
            Path = dirPath,
            NotifyFilter = NotifyFilters.LastWrite,
            Filter = "*.lua"
        };
        //includeSubdirectories;
        watcher.Changed += handler;
        watcher.EnableRaisingEvents = true;
        watcher.InternalBufferSize = 10240;
    }
}
