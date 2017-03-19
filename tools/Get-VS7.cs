﻿// Copyright 2017 - Refael Ackermann
// Distributed under MIT style license
// See accompanying file LICENSE at https://github.com/node4good/windows-autoconf

// Usage:
// powershell -ExecutionPolicy Unrestricted -Version "2.0" -Command "&{ Add-Type -Path GetVS2017Configuration.cs; [VisualStudioConfiguration.Main]::Query()}"
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace VisualStudioConfiguration
{
    [Flags]
    public enum InstanceState : uint
    {
        None = 0,
        Local = 1,
        Registered = 2,
        NoRebootRequired = 4,
        NoErrors = 8,
        Complete = 4294967295,
    }

    [Guid("6380BCFF-41D3-4B2E-8B2E-BF8A6810C848")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    public interface IEnumSetupInstances
    {

        void Next([MarshalAs(UnmanagedType.U4), In] int celt,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Interface), Out] ISetupInstance[] rgelt,
            [MarshalAs(UnmanagedType.U4)] out int pceltFetched);

        void Skip([MarshalAs(UnmanagedType.U4), In] int celt);

        void Reset();

        [return: MarshalAs(UnmanagedType.Interface)]
        IEnumSetupInstances Clone();
    }

    [Guid("42843719-DB4C-46C2-8E7C-64F1816EFD5B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    public interface ISetupConfiguration
    {
    }

    [Guid("26AAB78C-4A60-49D6-AF3B-3C35BC93365D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    public interface ISetupConfiguration2 : ISetupConfiguration
    {

        [return: MarshalAs(UnmanagedType.Interface)]
        IEnumSetupInstances EnumInstances();

        [return: MarshalAs(UnmanagedType.Interface)]
        ISetupInstance GetInstanceForCurrentProcess();

        [return: MarshalAs(UnmanagedType.Interface)]
        ISetupInstance GetInstanceForPath([MarshalAs(UnmanagedType.LPWStr), In] string path);

        [return: MarshalAs(UnmanagedType.Interface)]
        IEnumSetupInstances EnumAllInstances();
    }

    [Guid("B41463C3-8866-43B5-BC33-2B0676F7F42E")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    public interface ISetupInstance
    {
    }

    [Guid("89143C9A-05AF-49B0-B717-72E218A2185C")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    public interface ISetupInstance2 : ISetupInstance
    {
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetInstanceId();

        [return: MarshalAs(UnmanagedType.Struct)]
        System.Runtime.InteropServices.ComTypes.FILETIME GetInstallDate();

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetInstallationName();

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetInstallationPath();

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetInstallationVersion();

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetDisplayName([MarshalAs(UnmanagedType.U4), In] int lcid);

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetDescription([MarshalAs(UnmanagedType.U4), In] int lcid);

        [return: MarshalAs(UnmanagedType.BStr)]
        string ResolvePath([MarshalAs(UnmanagedType.LPWStr), In] string pwszRelativePath);

        [return: MarshalAs(UnmanagedType.U4)]
        InstanceState GetState();

        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)]
        ISetupPackageReference[] GetPackages();

        ISetupPackageReference GetProduct();

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetProductPath();

        [return: MarshalAs(UnmanagedType.VariantBool)]
        bool IsLaunchable();

        [return: MarshalAs(UnmanagedType.VariantBool)]
        bool IsComplete();

        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)]
        ISetupPropertyStore GetProperties();

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetEnginePath();
    }

    [Guid("DA8D8A16-B2B6-4487-A2F1-594CCCCD6BF5")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    public interface ISetupPackageReference
    {

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetId();

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetVersion();

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetChip();

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetLanguage();

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetBranch();

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetType();

        [return: MarshalAs(UnmanagedType.BStr)]
        string GetUniqueId();

        [return: MarshalAs(UnmanagedType.VariantBool)]
        bool GetIsExtension();
    }

    [Guid("c601c175-a3be-44bc-91f6-4568d230fc83")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    public interface ISetupPropertyStore
    {

        [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)]
        string[] GetNames();

        object GetValue([MarshalAs(UnmanagedType.LPWStr), In] string pwszName);
    }

    [Guid("42843719-DB4C-46C2-8E7C-64F1816EFD5B")]
    [CoClass(typeof(SetupConfigurationClass))]
    [ComImport]
    public interface SetupConfiguration : ISetupConfiguration2, ISetupConfiguration
    {
    }

    [Guid("177F0C4A-1CD3-4DE7-A32C-71DBBB9FA36D")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComImport]
    public class SetupConfigurationClass
    {
    }

    public class VSInstance : Dictionary<string, object>
    {
        public string ToJSON() {
             List<string> outStrings = new List<string>();
             foreach (string key in this.Keys) {
                 string line = '"' + key + "\": ";
                 switch (this[key].GetType().Name) {
                     case "String":
                        string str = (String)this[key];
                        if (str.IndexOf('{') == 0) line += str;
                        else line += '"' + (String)this[key] + '"';
                     break;

                     case "String[]":
                        line += "[\n    " + String.Join(",\n    ", (string[])this[key]) + "    \n    ]";
                     break;

                     case "Boolean":
                        line += (Boolean)this[key] ? "true" : "false";
                     break;

                     default:
                        line = null;
                     break;
                 }
                 if (line != null) outStrings.Add(line);
             }
             return "{\n        " + String.Join(",\n        ", outStrings.ToArray()) + "\n    }";
        }
        public static string ToJSON(IEnumerable<VSInstance> insts){
             List<string> outStrings = new List<string>();
             foreach (VSInstance inst in insts) {
                outStrings.Add(inst.ToJSON());
             }
             return "    [\n    " + String.Join(",\n    ", outStrings.ToArray()) + "    \n    ]";
        }
    }

    public static class Main
    {
        public static void Query()
        {
            List<VSInstance> insts = QueryEx(null);
            Console.Write(VSInstance.ToJSON(insts));
        }


        public static List<VSInstance> QueryEx(string[] args)
        {
            List<VSInstance> insts = new List<VSInstance>();
			ISetupConfiguration query = new SetupConfiguration();
			ISetupConfiguration2 query2 = (ISetupConfiguration2) query;
			IEnumSetupInstances e = query2.EnumAllInstances();
			ISetupInstance2[] rgelt = new ISetupInstance2[1];
			int pceltFetched;
            e.Next(1, rgelt, out pceltFetched);
			while (pceltFetched > 0)
			{
                ISetupInstance2 raw = rgelt[0];
                insts.Add(ParseInstance(raw));
                e.Next(1, rgelt, out pceltFetched);
			}
            return insts;
        }

        private static VSInstance ParseInstance(ISetupInstance2 setupInstance2)
        {
            VSInstance inst = new VSInstance();
            string[] prodParts = setupInstance2.GetProduct().GetId().Split('.');
            Array.Reverse(prodParts);
            inst["Product"] = prodParts[0];
            inst["Version"] = setupInstance2.GetInstallationVersion();
            inst["InstallationPath"] = setupInstance2.GetInstallationPath().Replace("\\", "\\\\");
            inst["IsComplete"] = setupInstance2.IsComplete();
            inst["IsLaunchable"] = setupInstance2.IsLaunchable();
            inst["CmdPath"] = (inst["InstallationPath"] + @"\\Common7\\Tools\\VsDevCmd.bat");

            inst["Win8SDK"] = "";
            inst["SDK10"] = "";
            inst["VisualCppTools"] = "";
            List<string> packs = new List<String>();
            foreach (ISetupPackageReference package in setupInstance2.GetPackages())
            {
                string id = package.GetId();

                string ver = package.GetVersion();
                string detail = "{\"id\": \"" + id + "\", \"version\":\"" + ver + "\"}";
                packs.Add("        " + detail);

                if (id.Contains("Component.MSBuild")) {
                    inst["MSBuild"] = detail;
                    inst["MSBuildVer"] = ver;
                } else if (id.Contains("Microsoft.VisualCpp.Tools.Core")) {
                    inst["VisualCppTools"] = ver;
                    inst["VCTools"] = detail;
                } else if (id.Contains("Microsoft.Windows.81SDK")) {
                    if (inst["Win8SDK"].ToString().CompareTo(ver) > 0) continue;
                    inst["Win8SDK"] = ver;
                } else if (id.Contains("Win10SDK_10")) {
                    if (inst["SDK10"].ToString().CompareTo(ver) > 0) continue;
                    inst["SDK10"] = ver;
                }
            }
            packs.Sort();
            inst["Packages"] = packs.ToArray();

            string[] sdk10Parts = inst["SDK10"].ToString().Split('.');
            sdk10Parts[sdk10Parts.Length - 1] = "0";
            inst["SDK10"] = String.Join(".", sdk10Parts);
            inst["SDK"] = inst["SDK10"].ToString() != "0" ? inst["SDK10"] : inst["SDK8"];
            Regex trimmer = new Regex(@"\.\d+$");
            if (inst.ContainsKey("MSBuild")) {
                string ver = trimmer.Replace(trimmer.Replace((string)inst["MSBuildVer"], ""), "");
                inst["MSBuildToolsPath"] = inst["InstallationPath"] + @"\\MSBuild\\" + ver + @"\\Bin\\";
                inst["MSBuildPath"] = inst["MSBuildToolsPath"] + "MSBuild.exe";
            }
            if (inst.ContainsKey("VisualCppTools")) {
                string ver = trimmer.Replace((string)inst["VisualCppTools"], "");
                inst["VisualCppToolsX64"] = inst["InstallationPath"] + @"\\VC\\Tools\\MSVC\\" + ver + @"\\bin\\HostX64\\x64\\";
                inst["VisualCppToolsX86"] = inst["InstallationPath"] + @"\\VC\\Tools\\MSVC\\" + ver + @"\\bin\\HostX86\\x86\\";
            }

            return inst;
        }
    }
}

public static class Program
{
    public static int Main(string[] args)
    {
        VisualStudioConfiguration.Main.Query();
        return 0;
    }
}
