using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CommandLine;
using QuickAccess;
using System.Collections;
using System.Threading;

namespace QuickAccessConsole
{
    internal class Program
    {
        [Verb("list", HelpText = "List current quick acess or supported languages.")]
        class ListOptions
        {
            [Option('q', "quick-access", Required = false, HelpText = "List quick access.")]
            public bool IsListQuickAccess { get; set; }

            [Option('r', "recent-files", Required = false, HelpText = "List recent files.")]
            public bool IsListRecentFiles { get; set; }

            [Option('f', "frequent-folders", Required = false, HelpText = "List frequent folders.")]
            public bool IsListFrequentFolders { get; set; }

            [Option('u', "ui-culture", Required = false, HelpText = "List system ui culture name.")]
            public bool IsListUICulture { get; set; }
        }

        [Verb("remove", HelpText = "Remove items from quick access.")]
        class RemoveOptions
        {
            [Value(0, HelpText = "Targets to remove.")]
            public IEnumerable<string> RemoveItems { get; set; }

            [Option('m', "menunames", Required = false, HelpText = "Add unsported menu names")]
            public IEnumerable<string> MenuNames { get; set; }
        }

        [Verb("check", HelpText = "Check whether in quick access or show quick access or supported language.")]
        class CheckOptions
        {
            [Value(0, HelpText = "Targets to check.")]
            public IEnumerable<string> Target { get; set; }

            [Option('q', "quick-access", Required = false, HelpText = "Check whether in quick access.")]
            public bool IsInQuickAccess { get; set; }

            [Option('s', "supported", Required = false, HelpText = "Check whether current system is supported")]
            public bool IsSupportedSystem { get; set; }

            [Option('m', "menunames", Required = false, HelpText = "Add unsported menu names")]
            public IEnumerable<string> MenuNames { get; set; }
        }

        [Verb("empty", HelpText = "Empty quick access.")]
        class EmptyOptions
        {
            [Option('a', "all", Required = false, HelpText = "Empty all quick access items.")]
            public bool IsEmptyAll { get; set; }

            [Option('r', "recent-files", Required = false, HelpText = "Empty recent files.")]
            public bool IsEmptyRecentFiles { get; set; }

            [Option('f', "frequent-folders", Required = false, HelpText = "Empty frequent folders.")]
            public bool IsEmptyFrequentFolders { get; set; }
        }

        //[Verb("test", HelpText = "Test some feature.")]
        //class TestOptions
        //{
        //    [Option('t', "test", Required = false, HelpText = "Test with option.")]
        //    public bool IsTestOption { get; set; }
        //}

        private class OutputData
        {
            public Dictionary<string, string> DictData { get; set; }
            public ArrayList ArrData { get; set; }
            public string Type { get; set; }
            public ArrayList Debug { get; set; }
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            QuickAccessHandler _handler = new QuickAccessHandler();

            Parser.Default.ParseArguments<ListOptions, RemoveOptions, CheckOptions, EmptyOptions/*, TestOptions*/>(args)
                .MapResult(
                    (ListOptions _option) => HandleListOptions(_option, _handler),
                    (RemoveOptions _option) => HandleRemoveOptions(args, _option, _handler),
                    (CheckOptions _option) => HandleCheckOptions(args, _option, _handler),
                    (EmptyOptions _option) => HandleEmptyOptions(_option, _handler),
                    //(TestOptions _option) => HandleTestOptions(_option, _handler),
                    error => -1
                );
        }

        private static string BuildOutputData(Dictionary<string, string> dict, ArrayList arr, string type, ArrayList debug)
        {
            var output = new OutputData
            {
                DictData = dict,
                ArrData = arr,
                Type = type,
                Debug = debug
            };

            return JsonConvert.SerializeObject(output);
        }

        private static int HandleListOptions(ListOptions options, QuickAccessHandler handler)
        {
            Dictionary<string, string> dictRes = new Dictionary<string, string>() { };
            ArrayList arrRes = new ArrayList() { };
            ArrayList debugRes = new ArrayList() { };

            if (options.IsListRecentFiles)
            {
                dictRes = handler.GetRecentFilesDict();
            }
            else if (options.IsListFrequentFolders)
            {
                dictRes = handler.GetFrequentFoldersDict();
            }
            else if (options.IsListQuickAccess)
            {
                dictRes = handler.GetQuickAccessDict();
            }
            else if (options.IsListUICulture)
            {
                string code = handler.GetSystemUICultureCode();
                arrRes.Add(code);
            }

            Console.WriteLine(BuildOutputData(dictRes, arrRes, "list", debugRes));

            return 0;
        }

        private static int HandleRemoveOptions(IEnumerable<string> args, RemoveOptions options, QuickAccessHandler handler)
        {
            Dictionary<string, string> dictRes = new Dictionary<string, string>() { };
            ArrayList arrRes = new ArrayList() { };
            ArrayList debugRes = new ArrayList() { };

            List<string> menuNameList = options.MenuNames.ToList<string>();
            List<string> removeList = options.RemoveItems.ToList<string>();

            if (menuNameList.Count > 0)
            {
                foreach (var name in menuNameList)
                {
                    handler.AddQuickAccessMenuName(name);
                }
            }

            CancellationTokenSource taskCancelToken = new CancellationTokenSource();
            var task = Task.Factory.StartNew(() =>
            {
                handler.RemoveFromQuickAccess(removeList);
            });
            if (!task.Wait(10 * 1000, taskCancelToken.Token))
            {
                debugRes.Add("remove timeout");
                Console.Error.WriteLine("Remove function timeout");

                Console.WriteLine(BuildOutputData(dictRes, arrRes, "remove", debugRes));
                return -1;
            }

            foreach (var item in removeList)
            {
                bool res = handler.IsInQuickAccess(item);

                arrRes.Add(res);
            }

            Console.WriteLine(BuildOutputData(dictRes, arrRes, "remove", debugRes));

            return 0;
        }

        private static int HandleCheckOptions(IEnumerable<string> args, CheckOptions options, QuickAccessHandler handler)
        {
            Dictionary<string, string> dictRes = new Dictionary<string, string>() { };
            ArrayList arrRes = new ArrayList() { };
            ArrayList debugRes = new ArrayList() { };

            List<string> menuNameList = options.MenuNames.ToList<string>();

            if (menuNameList.Count > 0)
            {
                foreach (var name in menuNameList)
                {
                    handler.AddQuickAccessMenuName(name);
                }
            }

            if (options.IsInQuickAccess)
            {
                foreach (var item in options.Target)
                {
                    bool isInQuickAccess = handler.IsInQuickAccess(item);
                    arrRes.Add(isInQuickAccess);
                }
            }
            else if (options.IsSupportedSystem)
            {
                bool isSupported = handler.IsSupportedSystem();

                arrRes.Add(isSupported);
            }

            Console.WriteLine(BuildOutputData(dictRes, arrRes, "check", debugRes));

            return 0;
        }

        private static int HandleEmptyOptions(EmptyOptions options, QuickAccessHandler handler)
        {
            if (options.IsEmptyAll)
            {
                handler.EmptyQuickAccess();
            }
            else if (options.IsEmptyRecentFiles)
            {
                handler.EmptyRecentFiles();
            }
            else if (options.IsEmptyFrequentFolders)
            {
                handler.EmptyFrequentFolders();
            }

            return 0;
        }

        //private static int HandleTestOptions(TestOptions options)
        //{
        //    CultureInfo ci = CultureInfo.CurrentCulture;
        //    Console.WriteLine("Current Language Info: {0}", ci.Name);

        //    ArrayList CurSupportLanguageList = new ArrayList(_handler.GetSupportLanguages());
        //    if (CurSupportLanguageList.Contains(ci.Name))
        //    {
        //        Console.WriteLine("Support cur system lang");
        //    } else
        //    {
        //        Console.WriteLine("Unsupported cur system lang");
        //    }

        //    return 0;
        //}
    }
}
