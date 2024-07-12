using NbtStudio.UI;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using fNbt;
using TryashtarUtils.Nbt;

namespace NbtStudio
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "--GetTag")
            {
                var rootCommand = new RootCommand
                {
                    new Option<string>(
                        "--GetTag",
                        "Get a tag from the specified NBT file")
                        {
                            IsRequired = true
                        },
                    new Argument<string>("nbt_filename", "The NBT file to read from")
                };

                rootCommand.SetHandler((string getTag, string nbt_filename) =>
                {
                    GetTag(getTag, nbt_filename);
                },
                new Option<string>("--GetTag"),
                new Argument<string>("nbt_filename"));

                rootCommand.InvokeAsync(args).Wait();
            }
            else
            {
                if (Environment.OSVersion.Version.Major >= 6)
                    SetProcessDPIAware();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm(args));
            }
        }

        private static void GetTag(string tag, string nbtFilename)
        {
            if (!File.Exists(nbtFilename))
            {
                Console.WriteLine($"File not found: {nbtFilename}");
                return;
            }

            var nbtFile = new fNbt.NbtFile();
            nbtFile.LoadFromFile(nbtFilename);

            if (nbtFile.RootTag is not NbtCompound rootTag)
            {
                Console.WriteLine("Root tag is not a compound tag.");
                return;
            }

            var targetTag = rootTag[tag];

            if (targetTag != null)
            {
                Console.WriteLine(targetTag.ToSnbt(new SnbtOptions(), include_name: true));
            }
            else
            {
                Console.WriteLine($"Tag not found: {tag}");
            }
        }

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }
}
