using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SLXEditor
{
    internal class OnlineUpdater
    {
        private static string SLXEditorExeName = "SLXEditor.exe";
        private static string RemoteVersionUrl = "https://raw.githubusercontent.com/Willicious/SuperLemmixEditor/master/Release/latest-version.txt";
        private static string RemoteExeUrl = "https://williciousmedia.short.gy/SLXEditor";

        private Settings curSettings;

        internal enum UpdateResult
        {
            CheckFailed,
            UpToDate,
            UpdateAvailable,
            UpdateDeclined,
            UpdateStarted,
            UpdateFailed,
            CheckSkipped
        }

        public OnlineUpdater(Settings settings)
        {
            curSettings = settings;
        }

        private string GetExePath()
        {
            return Path.Combine(C.AppPath, SLXEditorExeName);
        }

        private Version GetLocalVersion()
        {
            try
            {
                return Version.Parse(C.Version);
            }
            catch
            {
                return null;
            }
        }

        private async Task<Version> GetRemoteVersion()
        {
            if (string.IsNullOrWhiteSpace(RemoteVersionUrl))
                return null;

            try
            {
                using (var wc = new WebClient())
                {
                    wc.Headers.Add("User-Agent", "SLXEditor");
                    wc.Encoding = Encoding.UTF8;

                    string versionText =
                        (await wc.DownloadStringTaskAsync(RemoteVersionUrl)).Trim();

                    return Version.Parse(versionText);
                }
            }
            catch
            {
                return null;
            }
        }

        private bool ShouldCheckForUpdate()
        {
            return curSettings.LastUpdateCheck == DateTime.MinValue ||
                   (DateTime.Now - curSettings.LastUpdateCheck).TotalDays >= 7;
        }

        private bool StartInstaller(string exePath, string tempPath)
        {
            try
            {
                int processId = Process.GetCurrentProcess().Id;

                string scriptPath = Path.Combine(
                    Path.GetTempPath(),
                    "SLXEditorUpdate_" + processId + ".ps1");

                string backupPath = exePath + ".backup." + processId;

                Func<string, string> quote = delegate (string value)
                {
                    return "'" + value.Replace("'", "''") + "'";
                };

                string script =
                    "$ErrorActionPreference = 'Stop'\r\n" +
                    "$processId = " + processId + "\r\n" +
                    "$exePath = " + quote(exePath) + "\r\n" +
                    "$tempPath = " + quote(tempPath) + "\r\n" +
                    "$backupPath = " + quote(backupPath) + "\r\n" +
                    "$scriptPath = $PSCommandPath\r\n" +
                    "$originalMoved = $false\r\n" +
                    "try {\r\n" +
                    "    while (Get-Process -Id $processId -ErrorAction SilentlyContinue) {\r\n" +
                    "        Start-Sleep -Milliseconds 500\r\n" +
                    "    }\r\n" +
                    "    if (!(Test-Path -LiteralPath $tempPath -PathType Leaf)) {\r\n" +
                    "        throw 'Downloaded update not found.'\r\n" +
                    "    }\r\n" +
                    "    if (Test-Path -LiteralPath $exePath) {\r\n" +
                    "        Move-Item -LiteralPath $exePath -Destination $backupPath\r\n" +
                    "        $originalMoved = $true\r\n" +
                    "    }\r\n" +
                    "    Move-Item -LiteralPath $tempPath -Destination $exePath\r\n" +
                    "    Start-Process -FilePath $exePath\r\n" +
                    "    if ($originalMoved) {\r\n" +
                    "        Remove-Item -LiteralPath $backupPath -Force -ErrorAction SilentlyContinue\r\n" +
                    "    }\r\n" +
                    "}\r\n" +
                    "catch {\r\n" +
                    "    if ($originalMoved -and (Test-Path -LiteralPath $backupPath)) {\r\n" +
                    "        Remove-Item -LiteralPath $exePath -Force -ErrorAction SilentlyContinue\r\n" +
                    "        Move-Item -LiteralPath $backupPath -Destination $exePath -ErrorAction SilentlyContinue\r\n" +
                    "    }\r\n" +
                    "}\r\n" +
                    "finally {\r\n" +
                    "    Remove-Item -LiteralPath $scriptPath -Force -ErrorAction SilentlyContinue\r\n" +
                    "}\r\n";

                File.WriteAllText(
                    scriptPath,
                    script,
                    new UTF8Encoding(false));

                Process.Start(new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = "-NoProfile -ExecutionPolicy Bypass -File \"" +
                                scriptPath + "\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                });

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<UpdateResult> CheckForUpdate(bool manualCheck = false)
        {
            if (!manualCheck && !ShouldCheckForUpdate())
                return UpdateResult.CheckSkipped;

            Version localVersion = GetLocalVersion();
            Version remoteVersion = await GetRemoteVersion();

            if (remoteVersion == null || localVersion == null)
                return UpdateResult.CheckFailed;

            if (remoteVersion <= localVersion)
                return UpdateResult.UpToDate;

            return UpdateResult.UpdateAvailable;
        }

        public async Task<UpdateResult> DownloadAndInstallUpdate()
        {
            string exePath = GetExePath();
            string tempPath = exePath + ".download";

            try
            {
                if (File.Exists(tempPath))
                    File.Delete(tempPath);

                using (var wc = new WebClient())
                {
                    wc.Headers.Add("User-Agent", "SLXEditor");
                    await wc.DownloadFileTaskAsync(new Uri(RemoteExeUrl), tempPath);
                }

                if (StartInstaller(exePath, tempPath))
                    return UpdateResult.UpdateStarted;

                return UpdateResult.UpdateFailed;
            }
            catch
            {
                try
                {
                    if (File.Exists(tempPath))
                        File.Delete(tempPath);
                }
                catch
                {
                    // Fail silently
                }

                return UpdateResult.UpdateFailed;
            }
        }
    }
}