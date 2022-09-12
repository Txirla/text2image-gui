﻿using Microsoft.WindowsAPICodePack.Dialogs;
using StableDiffusionGui.Io;
using StableDiffusionGui.Os;
using StableDiffusionGui.Ui;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StableDiffusionGui.Forms
{
    public partial class SettingsForm : Form
    {
        private bool _ready = false;

        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            _ready = false;
            comboxSdModel.Items.Clear();
            IoUtils.GetFileInfosSorted(Paths.GetModelsPath(), true, "*.ckpt").ToList().ForEach(x => comboxSdModel.Items.Add(x.Name));
            LoadSettings();
            Task.Run(() => LoadGpus());
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_ready)
            {
                e.Cancel = true;
                return;
            }

            SaveSettings();
            Program.MainForm.RefreshAfterSettingsChanged();
        }

        private async Task LoadGpus()
        {
            comboxCudaDevice.Items.Clear();
            comboxCudaDevice.Items.Add("Loading...");
            comboxCudaDevice.SelectedIndex = 0;

            var gpus = await GpuUtils.GetCudaGpus();

            comboxCudaDevice.Items.Clear();
            comboxCudaDevice.Items.Add("CPU (Experimental, may not work at all)");

            foreach (var g in gpus)
                comboxCudaDevice.Items.Add($"GPU {g.Value} ({g.Key})");

            ConfigParser.LoadComboxIndex(comboxCudaDevice);
            _ready = true;
        }

        void LoadSettings()
        {
            ConfigParser.LoadGuiElement(checkboxOptimizedSd);
            ConfigParser.LoadGuiElement(checkboxFullPrecision);
            ConfigParser.LoadGuiElement(checkboxFolderPerPrompt);
            ConfigParser.LoadGuiElement(checkboxAdvancedMode);
            ConfigParser.LoadGuiElement(checkboxMultiPromptsSameSeed);
            ConfigParser.LoadGuiElement(checkboxPromptInFilename);
            ConfigParser.LoadGuiElement(textboxOutPath);
            ConfigParser.LoadGuiElement(comboxSdModel);
            // ConfigParser.LoadComboxIndex(comboxCudaDevice);
        }

        void SaveSettings()
        {
            ConfigParser.SaveGuiElement(checkboxOptimizedSd);
            ConfigParser.SaveGuiElement(checkboxFullPrecision);
            ConfigParser.SaveGuiElement(checkboxFolderPerPrompt);
            ConfigParser.SaveGuiElement(checkboxAdvancedMode);
            ConfigParser.SaveGuiElement(checkboxMultiPromptsSameSeed);
            ConfigParser.SaveGuiElement(checkboxPromptInFilename);
            ConfigParser.SaveGuiElement(textboxOutPath);
            ConfigParser.SaveGuiElement(comboxSdModel);
            ConfigParser.SaveComboxIndex(comboxCudaDevice);
        }

        private void checkboxFolderPerPrompt_CheckedChanged(object sender, EventArgs e)
        {
            panelPromptInFilename.Visible = !checkboxFolderPerPrompt.Checked;
        }

        private void btnOutPathBrowse_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog { InitialDirectory = textboxOutPath.Text, IsFolderPicker = true };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                textboxOutPath.Text = dialog.FileName;
        }

        private void btnOpenModelsFolder_Click(object sender, EventArgs e)
        {
            Process.Start("explorer", Paths.GetModelsPath().Wrap());
        }
    }
}
