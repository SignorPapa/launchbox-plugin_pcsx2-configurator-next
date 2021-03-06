﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Win32;
using Unbroken.LaunchBox.Plugins.Data;

namespace PCSX2_Configurator_Next.Core
{
    public class ConfiguratorModel
    {
        public string PluginDir => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public string LaunchBoxDir => Directory.GetParent($"{Directory.GetParent(PluginDir)}").ToString();
        public string RemoteConfigsUrl => "https://github.com/Zombeaver/PCSX2-Configs/trunk/Game%20Configs";
        public string RemoteConfigsDir => _remoteConfigsDir ?? (_remoteConfigsDir = GetRemoteConfigsDir());
        public string RemoteConfigDummyFileName => "remote";
        public string Pcsx2UiFileName => "PCSX2_ui.ini";
        public string SvnDir => $"{LaunchBoxDir}\\SVN";
        public string Pcsx2CommandLine => _pcsx2Emulator.CommandLine;
        public string Pcsx2RelativeAppPath => _pcsx2RelativeAppPath ?? (_pcsx2RelativeAppPath = GetPcsx2AppPath(absolutePath: false));
        public string Pcsx2AbsoluteAppPath => _pcsx2AbsoluteAppPath ?? (_pcsx2AbsoluteAppPath = GetPcsx2AppPath(absolutePath: true));
        public string Pcsx2RelativeDir => _pcsx2RelativeDir ?? (_pcsx2RelativeDir = Path.GetDirectoryName(Pcsx2RelativeAppPath));
        public string Pcsx2AbsoluteDir => _pcsx2AbsoluteDir ?? (_pcsx2AbsoluteDir = Path.GetDirectoryName(Pcsx2AbsoluteAppPath));
        public string Pcsx2InisDir => _pcsx2InisDir ?? (_pcsx2InisDir = GetPcsx2InisDir());
        public string Pcsx2BaseUiFilePath => _pcsx2BaseUiFilePath ?? (_pcsx2BaseUiFilePath = $"{Pcsx2InisDir}\\{Pcsx2UiFileName}");

        private readonly IEmulator _pcsx2Emulator;
        public ConfiguratorModel(string buildTitle)
        {
            var pcsx2Emulators = Utils.LaunchBoxFindEmulatorsByTitle("PCSX2");
            _pcsx2Emulator = pcsx2Emulators.Length > 1
                ? Utils.LaunchBoxFindEmulatorsByTitle(buildTitle).FirstOrDefault()
                : pcsx2Emulators.First();
            _pcsx2Emulator = _pcsx2Emulator ?? (pcsx2Emulators.Length > 0 ? pcsx2Emulators.First() : null);
        }

        private string _remoteConfigsDir;
        private string GetRemoteConfigsDir()
        {
            var remoteConfigDir = $"{PluginDir}\\remote";

            if (!Directory.Exists(remoteConfigDir))
            {
                Directory.CreateDirectory(remoteConfigDir).Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }

            return remoteConfigDir;
        }

        private string _pcsx2RelativeAppPath;
        private string _pcsx2AbsoluteAppPath;
        private string _pcsx2RelativeDir;
        private string _pcsx2AbsoluteDir;
        private string GetPcsx2AppPath(bool absolutePath)
        {
            var appPath = _pcsx2Emulator.ApplicationPath;
            var absolutAppPath = Utils.LaunchBoxRelativePathToAbsolute(appPath);

            appPath = absolutePath ? absolutAppPath : appPath;

            return File.Exists(absolutAppPath) ? appPath : null;
        }

        private string _pcsx2InisDir;
        private string _pcsx2BaseUiFilePath;
        private string GetPcsx2InisDir()
        {
            var pcsx2InisDir = File.Exists($"{Pcsx2AbsoluteDir}\\portable.ini")
                ? $"{Pcsx2AbsoluteDir}\\inis"
                : Registry.GetValue("HKEY_CURRENT_USER\\Software\\PCSX2", "SettingsFolder", null).ToString();

            if (string.IsNullOrEmpty(pcsx2InisDir))
            {
                pcsx2InisDir = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\PCSX2\\inis";
            }

            return pcsx2InisDir;
        }
    }
}
