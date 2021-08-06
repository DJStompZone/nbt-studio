﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using NbtStudio.UI;

namespace NbtStudio
{
    public class Studio
    {
        public MainForm Form { get; private set; }
        public readonly NbtTreeModel Tree = new();

        private readonly string[] CommandLineArguments;

        public Studio(string[] args)
        {
            CommandLineArguments = args;
            if (Properties.Settings.Default.RecentFiles is null)
                Properties.Settings.Default.RecentFiles = new();
            if (Properties.Settings.Default.CustomIconSets is null)
                Properties.Settings.Default.CustomIconSets = new();
        }

        public void LaunchForm()
        {
            // https://stackoverflow.com/a/13228495
            if (Environment.OSVersion.Version.Major >= 6)
                SetProcessDPIAware();
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            Form = new MainForm(this, CommandLineArguments);
            System.Windows.Forms.Application.Run(Form);
        }

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }
}