﻿using StableDiffusionGui.Controls;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace StableDiffusionGui.Io
{
    internal class ConfigParser
    {

        public enum StringMode { Any, Int, Float }

        public static void SaveGuiElement(TextBox textbox, string key, StringMode stringMode = StringMode.Any)
        {
            switch (stringMode)
            {
                case StringMode.Any: Config.Set(key, textbox.Text); break;
                case StringMode.Int: Config.Set(key, textbox.Text.GetInt().ToString()); break;
                case StringMode.Float: Config.Set(key, textbox.Text.GetFloat().ToString()); break;
            }
        }

        public static void SaveGuiElement(ComboBox comboBox, string key, StringMode stringMode = StringMode.Any)
        {
            switch (stringMode)
            {
                case StringMode.Any: Config.Set(key, comboBox.Text); break;
                case StringMode.Int: Config.Set(key, comboBox.Text.GetInt().ToString()); break;
                case StringMode.Float: Config.Set(key, comboBox.Text.GetFloat().ToStringDot()); break;
            }
        }

        public static void SaveGuiElement(CheckBox checkbox, string key)
        {
            Config.Set(key, checkbox.Checked.ToString());
        }

        public static void SaveGuiElement(NumericUpDown upDown, string key, StringMode stringMode = StringMode.Any)
        {
            switch (stringMode)
            {
                case StringMode.Any: Config.Set(key, ((float)upDown.Value).ToStringDot()); break;
                case StringMode.Int: Config.Set(key, ((int)upDown.Value).ToString()); break;
                case StringMode.Float: Config.Set(key, ((float)upDown.Value).ToStringDot()); break;
            }
        }

        public static void SaveGuiElement(HTAlt.WinForms.HTSlider slider, string key, SaveValueAs convertMode = SaveValueAs.Unchanged, float convertValue = 1f)
        {
            float value = slider is CustomSlider ? ((CustomSlider)slider).ActualValueFloat : slider.Value;

            if (convertMode == SaveValueAs.Multiplied)
                value = value * convertValue;

            if (convertMode == SaveValueAs.Divided)
                value = value / convertValue;

            Config.Set(key, value.ToStringDot());
        }

        public static void SaveComboxIndex(ComboBox comboBox, string key)
        {
            Config.Set(key, comboBox.SelectedIndex.ToString());
        }

        public static void LoadGuiElement(ComboBox comboBox, string key, string suffix = "")
        {
            comboBox.Text = Config.Get<string>(key) + suffix;
        }

        public static void LoadGuiElement(TextBox textbox, string key, string suffix = "")
        {
            textbox.Text = Config.Get<string>(key) + suffix; ;
        }

        public static void LoadGuiElement(CheckBox checkbox, string key)
        {
            checkbox.Checked = Config.Get<bool>(key);
        }

        public static void LoadGuiElement(NumericUpDown upDown, string key)
        {
            upDown.Value = Convert.ToDecimal(Config.Get<float>(key).Clamp((float)upDown.Minimum, (float)upDown.Maximum));
        }

        public enum SaveValueAs { Unchanged, Multiplied, Divided }

        public static void LoadGuiElement(HTAlt.WinForms.HTSlider slider, string key)
        {
            var value = Config.Get<float>(key);

            if (slider is CustomSlider)
                ((CustomSlider)slider).ActualValue = (decimal)value.Clamp((float)((CustomSlider)slider).ActualMinimum, (float)((CustomSlider)slider).ActualMaximum);
            else
                slider.Value = value.RoundToInt().Clamp(slider.Minimum, slider.Maximum);
        }

        public static void LoadComboxIndex(ComboBox comboBox, string key)
        {
            if (comboBox.Items.Count == 0)
                return;

            comboBox.SelectedIndex = Config.Get<int>(key).Clamp(0, comboBox.Items.Count - 1);
        }
    }
}
