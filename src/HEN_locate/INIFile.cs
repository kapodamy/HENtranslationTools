// *******************************
// *** INIFile class V2.1      ***
// *******************************
// *** (C)2009-2013 S.T.A. snc ***
// *******************************
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Settings
{

    internal class INIFile
    {

        #region "Declarations"

        // *** Lock for thread-safe access to file and local cache ***
        private object m_Lock = new object();

        // *** File name ***
        private string m_FileName = null;
        internal string FileName
        {
            get
            {
                return m_FileName;
            }
        }

        // *** Lazy loading flag ***
        private bool m_Lazy = false;

        // *** Automatic flushing flag ***
        private bool m_AutoFlush = false;
        private bool m_AutoRebuild = false;

        // *** Local cache ***
        private Dictionary<string, Dictionary<string, string>> m_Sections = new Dictionary<string, Dictionary<string, string>>();
        private Dictionary<string, Dictionary<string, string>> m_Modified = new Dictionary<string, Dictionary<string, string>>();
        private Dictionary<string, string> m_Comments = new Dictionary<string, string>();

        // *** Local cache modified flag ***
        private bool m_CacheModified = false;

        #endregion

        #region "Methods"

        // *** Constructor ***
        public INIFile(string FileName)
        {
            Initialize(FileName, false, false, false);
        }

        public INIFile(string FileName, bool Lazy, bool AutoFlush)
        {
            Initialize(FileName, Lazy, AutoFlush, false);
        }

        // *** Initialization ***
        private void Initialize(string FileName, bool Lazy, bool AutoFlush, bool AutoRebuild)
        {
            m_FileName = FileName;
            m_Lazy = Lazy;
            m_AutoFlush = AutoFlush;
            m_AutoRebuild = AutoRebuild;
            if (!m_Lazy) Refresh();
        }

        // *** Parse section name ***
        private static string ParseSectionName(string Line)
        {
            if (!Line.StartsWith("[")) return null;
            if (!Line.EndsWith("]")) return null;
            if (Line.Length < 3) return null;
            return Line.Substring(1, Line.Length - 2);
        }

        // *** Parse key+value pair ***
        private static bool ParseKeyValuePair(string Line, ref string Key, ref string Value)
        {
            // *** Check for key+value pair ***
            int i;
            if ((i = Line.IndexOf('=')) <= 0) return false;

            int j = Line.Length - i - 1;
            Key = Line.Substring(0, i).Trim();
            if (Key.Length <= 0) return false;

            Value = (j > 0) ? (Line.Substring(i + 1, j).Trim()) : ("");
            return true;
        }

        // *** Read file contents into local cache ***
        internal void Refresh()
        {
            lock (m_Lock)
            {
                StreamReader sr = null;
                try
                {
                    // *** Clear local cache ***
                    m_Sections.Clear();
                    m_Modified.Clear();

                    // *** Open the INI file ***
                    try
                    {
                        sr = new StreamReader(m_FileName);
                    }
                    catch (FileNotFoundException)
                    {
                        return;
                    }

                    // *** Read up the file content ***
                    Dictionary<string, string> CurrentSection = null;
                    string s;
                    string SectionName;
                    string Key = null;
                    string Value = null;
                    while ((s = sr.ReadLine()) != null)
                    {
                        s = s.Trim();

                        // *** Check for section names ***
                        SectionName = ParseSectionName(s);
                        if (SectionName != null)
                        {
                            // *** Only first occurrence of a section is loaded ***
                            if (m_Sections.ContainsKey(SectionName))
                            {
                                CurrentSection = null;
                            }
                            else
                            {
                                CurrentSection = new Dictionary<string, string>();
                                m_Sections.Add(SectionName, CurrentSection);
                            }
                        }
                        else if (CurrentSection != null)
                        {
                            // *** Check for key+value pair ***
                            if (ParseKeyValuePair(s, ref Key, ref Value))
                            {
                                // *** Only first occurrence of a key is loaded ***
                                if (!CurrentSection.ContainsKey(Key))
                                {
                                    CurrentSection.Add(Key, Value);
                                }
                            }
                        }
                    }
                }
                finally
                {
                    // *** Cleanup: close file ***
                    if (sr != null) sr.Close();
                    sr = null;
                }
            }
        }

        // *** Flush local cache content ***
        internal void Flush()
        {
            lock (m_Lock) { PerformFlush(false); }
        }

        internal void FlushAndRebuild()
        {
            lock (m_Lock) { PerformFlush(true); }
        }

        private void PerformFlush(bool rebuild)
        {
            // *** If local cache was not modified, exit ***
            if (!m_CacheModified) return;
            m_CacheModified = false;

            // *** Check if original file exists ***
            bool OriginalFileExists = File.Exists(m_FileName);

            // *** Get temporary file name ***
            string TmpFileName = Path.ChangeExtension(m_FileName, "$n$");

            // *** Copy content of original file to temporary file, replace modified values ***
            StreamWriter sw = null;

            // *** Create the temporary file ***
            sw = new StreamWriter(TmpFileName);

            // *** prevent blank line ***
            bool first_run = true;

            try
            {
                Dictionary<string, string> CurrentSection = null;

                #region copy structure from original ini
                if (OriginalFileExists && !rebuild)
                {
                    StreamReader sr = null;
                    try
                    {
                        // *** Open the original file ***
                        sr = new StreamReader(m_FileName);

                        // *** Read the file original content, replace changes with local cache values ***
                        string s;
                        string SectionName;
                        string Key = null;
                        string Value = null;
                        bool Unmodified;
                        bool Reading = true;
                        while (Reading)
                        {
                            s = sr.ReadLine();
                            Reading = (s != null);

                            // *** Check for end of file ***
                            if (Reading)
                            {
                                Unmodified = true;
                                s = s.Trim();
                                SectionName = ParseSectionName(s);
                            }
                            else
                            {
                                Unmodified = false;
                                SectionName = null;
                            }

                            // *** Check for section names ***
                            if ((SectionName != null) || (!Reading))
                            {
                                if (CurrentSection != null)
                                {
                                    // *** Write all remaining modified values before leaving a section ****
                                    if (CurrentSection.Count > 0)
                                    {
                                        foreach (string fkey in CurrentSection.Keys)
                                        {
                                            if (CurrentSection.TryGetValue(fkey, out Value))
                                            {
                                                sw.Write(fkey);
                                                sw.Write('=');
                                                sw.WriteLine(Value);
                                            }
                                        }
                                        sw.WriteLine();
                                        CurrentSection.Clear();
                                    }
                                }

                                if (Reading)
                                {
                                    // *** Check if current section is in local modified cache ***
                                    if (!m_Modified.TryGetValue(SectionName, out CurrentSection))
                                    {
                                        CurrentSection = null;
                                    }
                                }
                            }
                            else if (CurrentSection != null)
                            {
                                // *** Check for key+value pair ***
                                if (ParseKeyValuePair(s, ref Key, ref Value))
                                {
                                    if (CurrentSection.TryGetValue(Key, out Value))
                                    {
                                        // *** Write modified value to temporary file ***
                                        Unmodified = false;
                                        CurrentSection.Remove(Key);

                                        sw.Write(Key);
                                        sw.Write('=');
                                        sw.WriteLine(Value);
                                    }
                                }
                            }

                            // *** Write unmodified lines from the original file ***
                            if (Unmodified)
                            {
                                sw.WriteLine(s);
                                string comment;
                                if (SectionName != null && m_Comments.TryGetValue(SectionName, out comment)) { sw.WriteLine(comment); }
                            }
                        }

                        // *** Close the original file ***
                        sr.Close();
                        sr = null;
                    }
                    finally
                    {
                        // *** Cleanup: close files ***                  
                        if (sr != null) sr.Close();
                        sr = null;
                    }
                }
                #endregion

                var tmp_sections = rebuild ? m_Sections : m_Modified;
                if (rebuild) { MergeDictionary(m_Modified, ref tmp_sections); }

                // *** Cycle on all remaining modified values ***
                foreach (KeyValuePair<string, Dictionary<string, string>> SectionPair in tmp_sections)
                {
                    string comment;
                    CurrentSection = SectionPair.Value;
                    if (CurrentSection.Count > 0)
                    {
                        if (first_run)
                        {
                            first_run = false;
                        }
                        else
                        {
                            sw.WriteLine();
                        }

                        // *** Write the section name ***
                        sw.Write('[');
                        sw.Write(SectionPair.Key);
                        sw.WriteLine(']');

                        // *** Write the section name ***
                        if (m_Comments.TryGetValue(SectionPair.Key, out comment)) { sw.WriteLine(comment); }

                        // *** Cycle on all key+value pairs in the section ***
                        foreach (KeyValuePair<string, string> ValuePair in CurrentSection)
                        {
                            // *** Write the key+value pair ***
                            sw.Write(ValuePair.Key);
                            sw.Write('=');
                            sw.WriteLine(ValuePair.Value);
                        }
                        CurrentSection.Clear();
                    }
                }
                m_Modified.Clear();

                // *** Close the temporary file ***
                sw.Close();
                sw = null;

                // *** Rename the temporary file ***
                File.Copy(TmpFileName, m_FileName, true);

                // *** Delete the temporary file ***
                File.Delete(TmpFileName);
            }
            finally
            {
                // *** Cleanup: close files ***                  
                if (sw != null) sw.Close();
                sw = null;
            }
        }

        // *** Read a value from local cache ***
        internal string ExchangeValue(string SectionName, string Key, string DefaultValue)
        {
            return GetValue(SectionName, Key, DefaultValue, true);
        }

        internal int ExchangeValue(string SectionName, string Key, int DefaultValue)
        {
            string StringValue = GetValue(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture), true);
            int Value;
            if (int.TryParse(StringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out Value)) return Value;
            return DefaultValue;
        }

        internal string GetValue(string SectionName, string Key, string DefaultValue)
        {
            return GetValue(SectionName, Key, DefaultValue, false);
        }

        private string GetValue(string SectionName, string Key, string DefaultValue, bool createIfNotExist)
        {
            // *** Lazy loading ***
            if (m_Lazy)
            {
                m_Lazy = false;
                Refresh();
            }

            lock (m_Lock)
            {
                // *** Check if the section exists ***
                Dictionary<string, string> Section;
                if (!m_Sections.TryGetValue(SectionName, out Section))
                {
                    if (createIfNotExist) { SetValue(SectionName, Key, DefaultValue); }
                    return DefaultValue;
                }

                // *** Check if the key exists ***
                string Value;
                if (!Section.TryGetValue(Key, out Value))
                {
                    if (createIfNotExist) { SetValue(SectionName, Key, DefaultValue); }
                    return DefaultValue;
                }
                // *** Return the found value ***
                return Value;
            }
        }

        // *** Insert or modify a value in local cache ***
        internal void SetValue(string SectionName, string Key, string Value)
        {
            // *** Lazy loading ***
            if (m_Lazy)
            {
                m_Lazy = false;
                Refresh();
            }

            lock (m_Lock)
            {
                // *** Flag local cache modification ***
                m_CacheModified = true;

                // *** Check if the section exists ***
                Dictionary<string, string> Section;
                if (!m_Sections.TryGetValue(SectionName, out Section))
                {
                    // *** If it doesn't, add it ***
                    Section = new Dictionary<string, string>();
                    m_Sections.Add(SectionName, Section);
                }

                // *** Modify the value ***
                if (Section.ContainsKey(Key)) Section.Remove(Key);
                Section.Add(Key, Value);

                // *** Add the modified value to local modified values cache ***
                if (!m_Modified.TryGetValue(SectionName, out Section))
                {
                    Section = new Dictionary<string, string>();
                    m_Modified.Add(SectionName, Section);
                }

                if (Section.ContainsKey(Key)) Section.Remove(Key);
                Section.Add(Key, Value);

                // *** Automatic flushing : immediately write any modification to the file ***
                if (m_AutoFlush) PerformFlush(m_AutoRebuild);
            }
        }

        // *** Encode byte array ***
        private static string EncodeByteArray(byte[] Value)
        {
            if (Value == null) return null;

            StringBuilder sb = new StringBuilder();
            foreach (byte b in Value)
            {
                string hex = Convert.ToString(b, 16);
                int l = hex.Length;
                if (l > 2)
                {
                    sb.Append(hex.Substring(l - 2, 2));
                }
                else
                {
                    if (l < 2) sb.Append("0");
                    sb.Append(hex);
                }
            }
            return sb.ToString();
        }

        // *** Decode byte array ***
        private static byte[] DecodeByteArray(string Value)
        {
            if (Value == null) return null;

            int l = Value.Length;
            if (l < 2) return new byte[] { };

            l /= 2;
            byte[] Result = new byte[l];
            for (int i = 0; i < l; i++) Result[i] = Convert.ToByte(Value.Substring(i * 2, 2), 16);
            return Result;
        }

        // *** Sections Getters and Setters*** //
        internal string[] GetSectionsNames(StoreTarget target)
        {
            lock (m_Lock)
            {
                switch (target)
                {
                    case StoreTarget.Original:
                        return ReadDictionaryKeys(m_Sections).ToArray();
                    case StoreTarget.Modified:
                        return ReadDictionaryKeys(m_Modified).ToArray();
                    default:
                        List<string> list = ReadDictionaryKeys(m_Sections);
                        List<string> list2 = ReadDictionaryKeys(m_Modified);

                        foreach (string section in list2) { if (!list.Contains(section)) { list.Add(section); } }
                        return list.ToArray();
                }
            }
        }

        internal bool SectionExists(string section_name, StoreTarget target)
        {
            switch (target)
            {
                case StoreTarget.Modified:
                    return m_Modified.ContainsKey(section_name);
                case StoreTarget.Original:
                    return m_Sections.ContainsKey(section_name);
                default:
                    return m_Sections.ContainsKey(section_name) || m_Modified.ContainsKey(section_name);
            }
        }

        internal void SectionDelete(string section_name, StoreTarget target)
        {
            lock (m_Lock)
            {
                if (target != StoreTarget.Modified && m_Sections.ContainsKey(section_name)) { m_Sections.Remove(section_name); }
                if (target != StoreTarget.Original && m_Modified.ContainsKey(section_name)) { m_Modified.Remove(section_name); }
            }
        }

        internal void SectionRename(string old_name, string new_name, StoreTarget target)
        {
            if (new_name == old_name) { throw new ArgumentException("Both names are equal"); }
            if (new_name == null || new_name.Length < 1) { throw new ArgumentException("Invalid new section name", "new_name"); }
            if (old_name == null || old_name.Length < 1) { throw new ArgumentException("Invalid old section name", "old_name"); }
            //if (!SectionExists(old_name)) { throw new InvalidOperationException("Section \"" + old_name + "\" does not exist."); }

            lock (m_Lock)
            {
                if (target != StoreTarget.Modified && m_Sections.ContainsKey(old_name))
                {
                    Dictionary<string, string> dic = m_Sections[old_name];
                    m_Sections.Remove(old_name);
                    m_Sections.Add(new_name, dic);
                }
                if (target != StoreTarget.Original && m_Modified.ContainsKey(old_name))
                {
                    Dictionary<string, string> dic = m_Modified[old_name];
                    m_Modified.Remove(old_name);
                    m_Modified.Add(new_name, dic);
                }
            }
        }

        internal void SectionCreate(string section_name)
        {
            if (SectionExists(section_name, StoreTarget.BothStores)) { return; }
            m_Modified.Add(section_name, new Dictionary<string, string>());
        }

        internal void SectionSetComment(string section_name, string comment)
        {
            lock (m_Lock)
            {
                if (section_name == null || section_name.Length < 1) { throw new ArgumentException("Invalid section name", "section_name"); }
                if (comment == null) { return; }
                if (comment.Length < 1)
                {
                    m_Comments.Add(section_name, ";");
                    return;
                }

                SectionCreate(section_name);
                StringBuilder sb = new StringBuilder(section_name.Length + 10);
                sb.Append(';');

                for (int i = 0; i < comment.Length; i++)
                {
                    char prev;
                    char next;

                    if (comment[i] == '\r')
                    {
                        prev = '\r';
                        next = '\n';
                    }
                    else if (comment[i] == '\n')
                    {
                        prev = '\n';
                        next = '\r';
                    }
                    else
                    {
                        sb.Append(comment[i]);
                        continue;
                    }

                    if (i + 1 < comment.Length && comment[i + 1] == next)
                    {
                        i++;
                        sb.Append(prev);
                        sb.Append(next);
                    }
                    else
                    {
                        sb.Append(prev);
                    }
                    sb.Append(';');
                }

                m_Comments.Add(section_name, sb.ToString());
            }
        }

        internal List<string> GetKeysNames(StoreTarget target, string section_name)
        {
            lock (m_Lock)
            {
                switch (target)
                {
                    case StoreTarget.Original:
                        return ReadSectionsKeys(m_Sections, section_name);
                    case StoreTarget.Modified:
                        return ReadSectionsKeys(m_Modified, section_name);
                    default:
                        List<string> list = ReadSectionsKeys(m_Sections, section_name);
                        List<string> list2 = ReadSectionsKeys(m_Modified, section_name);

                        foreach (string section in list2) { if (!list.Contains(section)) { list.Add(section); } }
                        return list;
                }
            }
        }

        internal enum StoreTarget { Original, Modified, BothStores }

        protected List<string> ReadSectionsKeys(Dictionary<string, Dictionary<string, string>> dic, string section_name)
        {
            Dictionary<string, string> section;

            if (!dic.TryGetValue(section_name, out section)) return new List<string>();
            var keys = section.Keys;
            List<string> tmp = new List<string>(keys.Count);

            foreach (var key in keys) { tmp.Add(key); }

            return tmp;
        }

        protected List<string> ReadDictionaryKeys(Dictionary<string, Dictionary<string, string>> dic)
        {
            List<string> tmp = new List<string>(m_Sections.Count);
            foreach (KeyValuePair<string, Dictionary<string, string>> pair in dic) { tmp.Add(pair.Key); }

            return tmp;
        }

        protected static void MergeDictionary(Dictionary<string, Dictionary<string, string>> source, ref Dictionary<string, Dictionary<string, string>> target)
        {
            foreach (KeyValuePair<string, Dictionary<string, string>> pair in source)
            {
                if (!target.ContainsKey(pair.Key)) { target.Add(pair.Key, pair.Value); }
            }
        }

        // *** Getters for various types ***
        internal bool GetValue(string SectionName, string Key, bool DefaultValue)
        {
            string StringValue = GetValue(SectionName, Key, DefaultValue.ToString(System.Globalization.CultureInfo.InvariantCulture));
            int Value;
            if (int.TryParse(StringValue, out Value)) return (Value != 0);
            return DefaultValue;
        }

        internal int GetValue(string SectionName, string Key, int DefaultValue)
        {
            string StringValue = GetValue(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture));
            int Value;
            if (int.TryParse(StringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out Value)) return Value;
            return DefaultValue;
        }

        internal uint GetValue(string SectionName, string Key, uint DefaultValue)
        {
            string StringValue = GetValue(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture));
            uint Value;
            if (uint.TryParse(StringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out Value)) return Value;
            return DefaultValue;
        }

        internal long GetValue(string SectionName, string Key, long DefaultValue)
        {
            string StringValue = GetValue(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture));
            long Value;
            if (long.TryParse(StringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out Value)) return Value;
            return DefaultValue;
        }

        internal ulong GetValue(string SectionName, string Key, ulong DefaultValue)
        {
            string StringValue = GetValue(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture));
            ulong Value;
            if (ulong.TryParse(StringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out Value)) return Value;
            return DefaultValue;
        }

        internal byte GetValue(string SectionName, string Key, byte DefaultValue)
        {
            string StringValue = GetValue(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture));
            byte Value;
            if (byte.TryParse(StringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out Value)) return Value;
            return DefaultValue;
        }

        internal double GetValue(string SectionName, string Key, double DefaultValue)
        {
            string StringValue = GetValue(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture));
            double Value;
            if (double.TryParse(StringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out Value)) return Value;
            return DefaultValue;
        }

        internal byte[] GetValue(string SectionName, string Key, byte[] DefaultValue)
        {
            string StringValue = GetValue(SectionName, Key, EncodeByteArray(DefaultValue));
            try
            {
                return DecodeByteArray(StringValue);
            }
            catch (FormatException)
            {
                return DefaultValue;
            }
        }

        internal DateTime GetValue(string SectionName, string Key, DateTime DefaultValue)
        {
            string StringValue = GetValue(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture));
            DateTime Value;
            if (DateTime.TryParse(StringValue, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AssumeLocal, out Value)) return Value;
            return DefaultValue;
        }

        // *** Setters for various types ***
        internal void SetValue(string SectionName, string Key, bool Value)
        {
            SetValue(SectionName, Key, (Value) ? ("1") : ("0"));
        }

        internal void SetValue(string SectionName, string Key, int Value)
        {
            SetValue(SectionName, Key, Value.ToString(CultureInfo.InvariantCulture));
        }

        internal void SetValue(string SectionName, string Key, long Value)
        {
            SetValue(SectionName, Key, Value.ToString(CultureInfo.InvariantCulture));
        }

        internal void SetValue(string SectionName, string Key, ulong Value)
        {
            SetValue(SectionName, Key, Value.ToString(CultureInfo.InvariantCulture));
        }

        internal void SetValue(string SectionName, string Key, double Value)
        {
            SetValue(SectionName, Key, Value.ToString(CultureInfo.InvariantCulture));
        }

        internal void SetValue(string SectionName, string Key, byte[] Value)
        {
            SetValue(SectionName, Key, EncodeByteArray(Value));
        }

        internal void SetValue(string SectionName, string Key, DateTime Value)
        {
            SetValue(SectionName, Key, Value.ToString(CultureInfo.InvariantCulture));
        }

        #endregion

    }

}
