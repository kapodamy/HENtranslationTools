using Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace HEN_locate
{
    public partial class Form1 : Form
    {
        private const byte PADDING = 2;

        private string lastPath = null;
        private INIFile config = null;

        private List<Hen_String> list_strings = new List<Hen_String>()
        {
            new Hen_String("welcome message", 0x1208, "welcome_message", true),
            new Hen_String("downloading message", 0x1368, "downloading_message", false)
        };

        FileStream src = null;

        public Form1()
        {
            InitializeComponent();
            textBox_path.Select(0, 0);
        }


        private void Button_open_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "HEN Plugin|HENplugin.prx;*.prx";
                ofd.Title = "Select the PS3HEN file...";
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                if (lastPath != null) ofd.InitialDirectory = lastPath;
                var res = ofd.ShowDialog();

                if (res == DialogResult.OK)
                {
                    // TODO: load the file
                    lastPath = ofd.InitialDirectory;
                    file_load(ofd.FileName);
                }
            }
        }


        private void file_load(string path)
        {
            if (src != null)
            {
                src.Close();
            }

            try
            {
                src = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                foreach (var str in list_strings)
                {
                    src.Position = str.OFFSET;

                    byte[] buffer = new byte[0xffff];
                    int read = src.Read(buffer, 0, buffer.Length);
                    int i = 0;
                    int suffix_size = 0;

                    for (; i < read; i++)
                    {
                        if (buffer[i] == 0x00)// search end of string
                        {
                            str.max_size = i;
                            if (str.suffix != null)
                            {
                                for (; i > 0; i--)
                                {
                                    if (buffer[i] == 0x20)// search space char
                                    {
                                        suffix_size = str.max_size - i;
                                        break;
                                    }

                                }
                                if (i < 1)
                                {
                                    throw new Exception("Invalid string found for " + str.DISPLAY_TEXT);
                                }
                            }

                            break;
                        }
                        else if (buffer[i] < 0x0A)// check for control-char
                        {
                            throw new Exception("String not found for " + str.DISPLAY_TEXT);
                        }
                    }

                    if (i >= read)
                    {
                        throw new Exception("Invalid string size for " + str.DISPLAY_TEXT);
                    }

                    src.Position = str.OFFSET;

                    // read string
                    str.text = Encoding.UTF8.GetString(buffer, 0, str.max_size - suffix_size);
                    str.text_orig = Encoding.UTF8.GetString(buffer, 0, str.max_size);

                    if (suffix_size > 0)
                    {
                        str.suffix = Encoding.UTF8.GetString(buffer, str.max_size - suffix_size, suffix_size);
                    }

                    // find extra space, but with a padding.
                    // this (in theory) prevent overwritten any integer
                    // especially the downloading message because is near the elf ro data limit
                    for (i = str.max_size + 1; i < buffer.Length && i < read; i++)
                    {
                        if (buffer[i] != 0x00)
                        {
                            int value = i - PADDING - 1/* end of the string mark*/;
                            if (value < str.max_size) break;
                            str.max_size = value;
                            break;
                        }
                    }
                }

                // enable editing
                comboBox_src.Enabled = true;
                button_apply.Enabled = true;
                comboBox_src.SelectedIndex = -1;
                textBox_path.Text = path;

            }
            catch (Exception e)
            {
                textBox_custom.Enabled = false;
                comboBox_src.Enabled = false;
                button_apply.Enabled = false;
                textBox_path.Text = "";

                if (src != null)
                {
                    src.Close();
                }

                src = null;

                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            finally
            {
                comboBox_src.SelectedIndex = -1;
                textBox_orig.Text = "";
                textBox_custom.Text = "";
                textBox_res.Text = "";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            config = new INIFile(AppDomain.CurrentDomain.BaseDirectory + "hen_locale.ini");

            foreach (var str in list_strings)
            {
                Int64.TryParse(
                    config.ExchangeValue("string offsets", str.CONFIG + "_offset", str.OFFSET.ToString()),
                    out str.OFFSET
                );
            }

            config.Flush();
        }


        internal class Hen_String
        {

            internal Hen_String(string d, long o, string c, bool s)
            {
                CONFIG = c;
                OFFSET = o;
                DISPLAY_TEXT = d;

                text = null;
                suffix = s ? "" : null;// if not has suffix set to null
                text_orig = null;
            }

            internal readonly string DISPLAY_TEXT;
            internal long OFFSET;
            internal readonly string CONFIG;

            internal string text;
            internal string text_orig;
            internal string suffix;
            internal int max_size;
        }

        private void ComboBox_src_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_src.SelectedIndex < 0)
            {
                textBox_res.Text = "";
                textBox_orig.Text = "";
                textBox_custom.Text = "";
                textBox_custom.Enabled = false;
            }

            var str = list_strings[comboBox_src.SelectedIndex];
            textBox_res.Text = str.text + str.suffix;
            textBox_orig.Text = str.text_orig;
            textBox_custom.Text = str.text;
            textBox_custom.Enabled = true;
        }

        private void TextBox_custom_TextChanged(object sender, EventArgs e)
        {
            if (comboBox_src.SelectedIndex < 0) return;

            Hen_String str = list_strings[comboBox_src.SelectedIndex];

            string res = textBox_custom.Text;
            string current = textBox_custom.Text;

            if (current.Length > 0)
            {
                while (current.Length > 0)
                {
                    res = current + str.suffix;
                    int size = Encoding.UTF8.GetByteCount(res);

                    if (size <= str.max_size) break;

                    current = current.Substring(0, current.Length - 1);
                }

                if (current.Length < 1)
                {
                    MessageBox.Show("Cannot compute the string", "error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    textBox_res.Text = str.text + str.suffix;
                    textBox_custom.Text = str.text;
                    return;
                }
            }
            else
            {
                res += str.suffix;
            }

            str.text = current;
            textBox_res.Text = res;

            if (current.Length != textBox_custom.Text.Length)
            {
                textBox_custom.Text = current;
                textBox_custom.SelectionStart = current.Length;

                label_warn.Visible = true;
                timer_warn_fadeOut.Enabled = true;
                timer_warn_fadeOut.Start();
            }
        }

        private void Button_apply_Click(object sender, EventArgs e)
        {
            foreach (var str in list_strings)
            {
                src.Position = str.OFFSET;

                byte[] buffer = Encoding.UTF8.GetBytes(str.text + str.suffix);

                src.Write(buffer, 0, buffer.Length);
                src.WriteByte(0x00);

                if (buffer.Length < str.max_size)
                {
                    // fill with zeros
                    for (int i = str.max_size - buffer.Length; i > 0; i--)
                    {
                        src.WriteByte(0x00);
                    }
                }
            }

            src.Flush();
            MessageBox.Show("¡Changes applied!", "success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (src != null)
            {
                src.Close();
            }
        }

        private void Timer_warn_fadeOut_Tick(object sender, EventArgs e)
        {
            label_warn.Visible = false;
            timer_warn_fadeOut.Stop();
        }
    }
}
