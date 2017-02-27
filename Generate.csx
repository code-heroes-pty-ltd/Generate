using System;
using System.Diagnostics;
using System.IO;

static string inkscape = @"C:\Program Files\Inkscape\inkscape.exe";
static string pngOut = @"C:\Software\PNGOut\pngout.exe";
static int pngOutUnableToCompressFurther = 2;
static string srcDir = @"..\Src\";
static string androidResourcesDir = @"..\Src\UI.Droid\Resources\";
static string iOSResourcesDir = @"..\Src\UI.iOS\Resources\";
static string uwpResourcesDir = @"..\Src\UI.UWP\";
static bool optimizeImages = true;
static bool force = Environment.GetCommandLineArgs().Contains("-force");

if (!File.Exists(inkscape))
{
    WriteError("Inkscape executable not found.");
    return;
}

if (optimizeImages && !File.Exists(pngOut))
{
    WriteError("PNGOut executable not found.");
    return;
}

// add your SVG file names (without extension) to the appropriate array, depending on what type of image you want to generate
new[]
    {
        "hamburger",
    }
    .ToList()
    .ForEach(ExportToolBarItem);

new[]
    {
    }
    .ToList()
    .ForEach(ExportMediumIcon);

new[]
    {
    }
    .ToList()
    .ForEach(ExportSmallIcon);

new[]
    {
    }
    .ToList()
    .ForEach(ExportErrorImage);

new[]
      {
      }
      .ToList()
      .ForEach(ExportSplashImage);

new[]
    {
    }
    .ToList()
    .ForEach(ExportTinyIcon);

new[]
    {
    }
    .ToList()
    .ForEach(ExportLargeIcon);

// uncomment this to generate an application icon
//ExportAppIcon("app_icon_ios", "app_icon_android", "app_icon_uwp");

private static void ExportToolBarItem(string name) =>
    ExportItem(name, "toolbar item", 24, 22, 20);

private static void ExportTinyIcon(string name) =>
    ExportItem(name, "medium icon", 6, 4, 4);

private static void ExportSmallIcon(string name) =>
    ExportItem(name, "small icon", 19, 18, 12);

private static void ExportMediumIcon(string name) =>
    ExportItem(name, "medium icon", 32, 30, 20);

private static void ExportLargeIcon(string name) =>
    ExportItem(name, "large icon", 100, 82, 80);

private static void ExportSplashImage(string name) =>
    ExportItem(name, "splash image", 150, 120, 120);

private static void ExportItem(string name, string type, int androidBaseSize, int iOSBaseSize, int uwpBaseSize)
{
    var input = name + ".svg";
    var outputBaseName = name + ".png";

    WriteInfo($"Exporting {input} as {type}...");

    WriteInfo("  Android");
    Export(input, Path.Combine(androidResourcesDir, "drawable-hdpi", outputBaseName),  Scale.Hdpi.ScaleValue(androidBaseSize));
    Export(input, Path.Combine(androidResourcesDir, "drawable-xhdpi", outputBaseName), Scale.Xhdpi.ScaleValue(androidBaseSize));
    Export(input, Path.Combine(androidResourcesDir, "drawable-xxhdpi", outputBaseName), Scale.Xxhdpi.ScaleValue(androidBaseSize));
    Export(input, Path.Combine(androidResourcesDir, "drawable-xxxhdpi", outputBaseName), Scale.Xxxhdpi.ScaleValue(androidBaseSize));

    WriteInfo("  iOS");
    Export(input, Path.Combine(iOSResourcesDir, outputBaseName), iOSBaseSize);
    Export(input, Path.Combine(iOSResourcesDir, WithSuffix(outputBaseName, "@2x")), Scale.At2x.ScaleValue(iOSBaseSize));
    Export(input, Path.Combine(iOSResourcesDir, WithSuffix(outputBaseName, "@3x")), Scale.At3x.ScaleValue(iOSBaseSize));

    WriteInfo("  UWP");
    Export(input, Path.Combine(uwpResourcesDir, WithSuffix(outputBaseName, ".scale-100")), uwpBaseSize);
    Export(input, Path.Combine(uwpResourcesDir, WithSuffix(outputBaseName, ".scale-125")), Scale.Scale125.ScaleValue(uwpBaseSize));
    Export(input, Path.Combine(uwpResourcesDir, WithSuffix(outputBaseName, ".scale-150")), Scale.Scale150.ScaleValue(uwpBaseSize));
    Export(input, Path.Combine(uwpResourcesDir, WithSuffix(outputBaseName, ".scale-200")), Scale.Scale200.ScaleValue(uwpBaseSize));
    Export(input, Path.Combine(uwpResourcesDir, WithSuffix(outputBaseName, ".scale-400")), Scale.Scale400.ScaleValue(uwpBaseSize));
}

private static void ExportAppIcon(string iOSname, string androidName, string uwpName)
{
    var iosInput = iOSname + ".svg";
    var androidInput = androidName + ".svg";
    var uwpInput = uwpName + ".svg";
    var outputBaseName = "icon.png";
    var iosOutputBaseName = "Icon.png";
    var androidBase = 72;
    var uwpBase = 44;
    var iPhone4n6Splotlight = 29;
    var ios7n8Spotlight = 40;
    var iPad5n6Spotlight = 50;
    var iPhone5n6 = 57;
    var iPhone7n8 = 60;
    var iPad5n6 = 72;
    var iPad7n8 = 76;
    var iPadPro9 = 83.5m;

    WriteDebug($"Exporting {androidInput} as app icon...");
    Export(androidInput, Path.Combine(androidResourcesDir, "drawable-hdpi", outputBaseName),  Scale.Hdpi.ScaleValue(androidBase));
    Export(androidInput, Path.Combine(androidResourcesDir, "drawable-xhdpi", outputBaseName), Scale.Xhdpi.ScaleValue(androidBase));
    Export(androidInput, Path.Combine(androidResourcesDir, "drawable-xxhdpi", outputBaseName), Scale.Xxhdpi.ScaleValue(androidBase));
    Export(androidInput, Path.Combine(androidResourcesDir, "drawable-xxxhdpi", outputBaseName), Scale.Xxxhdpi.ScaleValue(androidBase));

    WriteDebug($"Exporting {iosInput} as app icon...");
    Export(iosInput, Path.Combine(iOSResourcesDir, WithSuffix(iosOutputBaseName, $"-Small")), iPhone4n6Splotlight);
    Export(iosInput, Path.Combine(iOSResourcesDir, WithSuffix(iosOutputBaseName, $"-Small@2x")), Scale.At2x.ScaleValue(iPhone4n6Splotlight));
    Export(iosInput, Path.Combine(iOSResourcesDir, WithSuffix(iosOutputBaseName, $"-Small-{ios7n8Spotlight}")), ios7n8Spotlight);
    Export(iosInput, Path.Combine(iOSResourcesDir, WithSuffix(iosOutputBaseName, $"-Small-{ios7n8Spotlight}@2x")), Scale.At2x.ScaleValue(ios7n8Spotlight));
    Export(iosInput, Path.Combine(iOSResourcesDir, WithSuffix(iosOutputBaseName, $"-{iPad5n6Spotlight}")), iPad5n6Spotlight);
    Export(iosInput, Path.Combine(iOSResourcesDir, WithSuffix(iosOutputBaseName, $"-{iPad5n6Spotlight}@2x")), Scale.At2x.ScaleValue(iPad5n6Spotlight));
    Export(iosInput, Path.Combine(iOSResourcesDir, iosOutputBaseName), iPhone5n6);
    Export(iosInput, Path.Combine(iOSResourcesDir, WithSuffix(iosOutputBaseName, $"@2x")), Scale.At2x.ScaleValue(iPhone5n6));
    Export(iosInput, Path.Combine(iOSResourcesDir, WithSuffix(iosOutputBaseName, $"-{iPhone7n8}@2x")), Scale.At2x.ScaleValue(iPhone7n8));
    Export(iosInput, Path.Combine(iOSResourcesDir, WithSuffix(iosOutputBaseName, $"-{iPad5n6}")), iPad5n6);
    Export(iosInput, Path.Combine(iOSResourcesDir, WithSuffix(iosOutputBaseName, $"-{iPad5n6}@2x")), Scale.At2x.ScaleValue(iPad5n6));
    Export(iosInput, Path.Combine(iOSResourcesDir, WithSuffix(iosOutputBaseName, $"-{iPad7n8}")), iPad7n8);
    Export(iosInput, Path.Combine(iOSResourcesDir, WithSuffix(iosOutputBaseName, $"-{iPad7n8}@2x")), Scale.At2x.ScaleValue(iPad7n8));
    Export(iosInput, Path.Combine(iOSResourcesDir, WithSuffix(iosOutputBaseName, $"-{iPadPro9}@2x")), Scale.At2x.ScaleValue(iPadPro9));

    WriteDebug($"Exporting {uwpInput} as app icon...");
    Export(uwpInput, Path.Combine(uwpResourcesDir, WithSuffix(outputBaseName, ".scale-100")), uwpBase);
    Export(uwpInput, Path.Combine(uwpResourcesDir, WithSuffix(outputBaseName, ".scale-125")), Scale.Scale125.ScaleValue(uwpBase));
    Export(uwpInput, Path.Combine(uwpResourcesDir, WithSuffix(outputBaseName, ".scale-150")), Scale.Scale150.ScaleValue(uwpBase));
    Export(uwpInput, Path.Combine(uwpResourcesDir, WithSuffix(outputBaseName, ".scale-200")), Scale.Scale200.ScaleValue(uwpBase));
    Export(uwpInput, Path.Combine(uwpResourcesDir, WithSuffix(outputBaseName, ".scale-400")), Scale.Scale400.ScaleValue(uwpBase));
}

private static void Export(string input, string output, decimal size) =>
    Export(input, output, size, size);

private static void Export(string input, string output, decimal width, decimal height)
{
    WriteDebug($"    -> {output} at {width}x{height}");

    var inputFile = new FileInfo(input);
    var outputFile = new FileInfo(output);

    if (!force && outputFile.Exists && inputFile.LastWriteTimeUtc < outputFile.LastWriteTimeUtc)
    {
        WriteDebug("       up to date - skipping");
        return;
    }

    var startInfo = new ProcessStartInfo(inkscape, $"--file=\"{input}\" --export-png=\"{output}\" --export-width={width} --export-height={height} --export-area-page")
    {
        WorkingDirectory = Environment.CurrentDirectory,
        CreateNoWindow = true,
        UseShellExecute = false
    };
    var process = Process.Start(startInfo);
    var exited = process.WaitForExit(60000);

    if (!exited)
    {
        WriteError("Inkscape process failed to exit!");
        return;
    }

    if (process.ExitCode != 0)
    {
        WriteError($"Inkscape process returned exit code {process.ExitCode}. Process started was:");
        WriteError($"{startInfo.FileName} {startInfo.Arguments}");
        return;
    }

    if (optimizeImages)
    {
        startInfo = new ProcessStartInfo(pngOut, $"\"{output}\" \"{output}\" /y")
        {
            WorkingDirectory = Environment.CurrentDirectory,
            CreateNoWindow = true,
            UseShellExecute = false
        };
        process = Process.Start(startInfo);
        exited = process.WaitForExit(60000);

        if (!exited)
        {
            WriteWarn("PNGOut process failed to exit!");
            return;
        }

        if (process.ExitCode != 0 && process.ExitCode != pngOutUnableToCompressFurther)
        {
            WriteWarn($"PNGOut process returned exit code {process.ExitCode}. Process started was:");
            WriteWarn($"{startInfo.FileName} {startInfo.Arguments}");
            return;
        }
    }
}

private static string WithSuffix(string file, string suffix) =>
    Path.GetFileNameWithoutExtension(file) + suffix + Path.GetExtension(file);

private enum Scale
{
    // Android
    Mdpi,
    Hdpi,
    Xhdpi,
    Xxhdpi,
    Xxxhdpi,

    // iOS
    At1x,
    At2x,
    At3x,

    // UWP
    Scale100,
    Scale125,
    Scale150,
    Scale200,
    Scale400
}

private static decimal ScaleValue(this Scale @this, decimal value, Scale? sourceScale = null)
{
    var targetMultiplier = GetMultiplier(@this);
    var sourceMultiplier = sourceScale == null ? 1 : GetMultiplier(sourceScale.Value);
    var multiplier = targetMultiplier / sourceMultiplier;

    return (decimal)(value * multiplier);
}

private static decimal GetMultiplier(this Scale @this)
{
    switch (@this)
    {
        case Scale.Mdpi:
            return 1m;
        case Scale.Hdpi:
            return 1.5m;
        case Scale.Xhdpi:
            return 2m;
        case Scale.Xxhdpi:
            return 3m;
        case Scale.Xxxhdpi:
            return 4m;
        case Scale.At1x:
            return 1m;
        case Scale.At2x:
            return 2m;
        case Scale.At3x:
            return 3m;
        case Scale.Scale100:
            return 1m;
        case Scale.Scale125:
            return 1.25m;
        case Scale.Scale150:
            return 1.5m;
        case Scale.Scale200:
            return 2m;
        case Scale.Scale400:
            return 4m;
        default:
            throw new NotSupportedException($"Unsupported scale: {@this}.");
    }
}

private enum Level
{
    Debug,
    Info,
    Warn,
    Error
}

private static void WriteDebug(string message, params object[] args) =>
    Write(Level.Debug, message, args);

private static void WriteInfo(string message, params object[] args) =>
    Write(Level.Info, message, args);

private static void WriteWarn(string message, params object[] args) =>
    Write(Level.Warn, message, args);

private static void WriteError(string message, params object[] args) =>
    Write(Level.Error, message, args);

private static void Write(Level level, string message, params object[] args)
{
    Console.ForegroundColor = GetForegroundColorFor(level);
    Console.WriteLine(message, args);
}

private static ConsoleColor GetForegroundColorFor(Level level)
{
    switch (level)
    {
        case Level.Debug:
            return ConsoleColor.Gray;
        case Level.Warn:
            return ConsoleColor.Yellow;
        case Level.Error:
            return ConsoleColor.Red;
        default:
            return ConsoleColor.White;
    }
}