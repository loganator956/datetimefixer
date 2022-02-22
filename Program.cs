// check for any major problems that will prevent the operation and exit if find any
if (args.Length != 1)
{
    Console.WriteLine($"Unexpected number of arguments specified. Expected 1, received {args.Length}");
    return;
}
if (args[0].ToLower().Contains("help"))
{
    Console.WriteLine("This app will set the CreateDate and ModifiedDates to become identical (Picks whichever date is earliest out of the two)");
    Console.WriteLine("Only takes one argument which is the path of the directory to search and process");
    return;
}
if (!Directory.Exists(args[0]))
{
    Console.WriteLine($"The specified directory does not exist. {args[0]}");
    return;
}

List<string> files = GetFilesRecursive(args[0]);

foreach (string s in files)
{
    FileInfo file = new FileInfo(s);
    DateTime earliestDateTime = file.CreationTime;
    if (file.LastWriteTime < earliestDateTime) { earliestDateTime = file.LastWriteTime; };
    file.LastWriteTime = file.CreationTime = earliestDateTime;
    Console.WriteLine($"Updated file {s} to {earliestDateTime.ToString()}");
}


List<string> GetFilesRecursive(string dir)
{
    List<string> files = new List<string>();
    files.AddRange(Directory.GetFiles(dir));
    foreach (string subDir in Directory.GetDirectories(dir))
    {
        files.AddRange(GetFilesRecursive(subDir));
    }
    return files;
}