using System.Diagnostics;

const string VersionTag = "v1.1_dev";
Console.WriteLine($"datetimefixer {VersionTag}");
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

// check optional extra features

// check if exiftool executable can be found #1
bool useExifTool = false;

try
{
    Console.WriteLine($"Testing to find exiftool");
    Process p = Process.Start("exiftool", "-ver");
    p.WaitForExit();
    useExifTool = true;
}
catch (System.ComponentModel.Win32Exception)
{
    useExifTool = false;
    ConsoleColor oldColor = Console.ForegroundColor;
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"exiftool not found. Proceeding without exiftool");
    Console.ForegroundColor = oldColor;
}


// process files

List<string> files = GetFilesRecursive(args[0]);

int iteration = 0;
foreach (string s in files)
{
    Console.WriteLine($"{iteration}/{files.Count}");
    FileInfo file = new FileInfo(s);
    DateTime earliestDateTime = file.CreationTime;
    if (file.LastWriteTime < earliestDateTime) { earliestDateTime = file.LastWriteTime; };
    if (useExifTool)
    {
        // iterate through output of exiftool
        ProcessStartInfo exifToolStartInfo = new ProcessStartInfo("exiftool");
        exifToolStartInfo.ArgumentList.Add("-time:all");
        exifToolStartInfo.ArgumentList.Add(s);
        exifToolStartInfo.RedirectStandardOutput = true;
        Process? exifTool = Process.Start(exifToolStartInfo);
        if (exifTool != null)
        {
            exifTool.WaitForExit();
            List<string> lines = new List<string>();
            while (true)
            {
                string? newLine = exifTool.StandardOutput.ReadLine();
                if (newLine != null)
                {
                    if (newLine.Contains("0000:00:00 00:00:00")) { continue; }; // skipping empty datetimes
                    if (newLine.Contains("Profile Date Time")) { continue; }; // skipping unreliable or irelevant properties
                    if (newLine.Contains("Offset Time")) { continue; }; // skipping unreliable or irelevant properties
                    if (newLine.Contains("Sub Sec Time")) { continue; }; // skipping unreliable or irelevant properties
                    lines.Add(newLine);
                    // Console.WriteLine(newLine);
                }
                else
                {
                    break;
                }
            }
        }
    }
    // file.LastWriteTime = file.CreationTime = earliestDateTime;
    // Console.WriteLine($"Updated file {s} to {earliestDateTime.ToString()}");
    iteration++;
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