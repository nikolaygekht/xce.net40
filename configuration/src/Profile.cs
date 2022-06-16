using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace gehtsoft.xce.configuration
{
    public class ProfileKey
    {
        private string mName;
        private string mOrgName;
        private string mValue;

        public string Name
        {
            get
            {
                return mName;
            }
        }

        internal string OrgName
        {
            get
            {
                return mOrgName;
            }
        }

        public string Value
        {
            get
            {
                return mValue;
            }
            set
            {
                mValue = value;
            }
        }

        bool mIsKey;
        public bool IsKey
        {
            get
            {
                return mIsKey;
            }
        }

        internal ProfileKey(string name, string value, bool isKey)
        {
            mOrgName = name;
            mName = name;
            mValue = value;
            mIsKey = isKey;
        }

        internal ProfileKey(string orgName, string name, string value, bool isKey)
        {
            mOrgName = orgName;
            mName = name;
            mValue = value;
            mIsKey = isKey;
        }
    }

    public class ProfileSection : IEnumerable<ProfileKey>
    {
        string mName;
        List<ProfileKey> mKeys = new List<ProfileKey>();

        internal ProfileSection(string name)
        {
            mName = name;
        }

        public int Count
        {
            get
            {
                return mKeys.Count;
            }
        }

        public string Name
        {
            get
            {
                return mName;
            }
        }

        public ProfileKey this[int index]
        {
            get
            {
                return mKeys[index];
            }
        }

        public string this[string name]
        {
            get
            {
                return this[name, 0];
            }
            set
            {
                this[name, 0] = value;
            }
        }

        public string this[string name, int occurrence]
        {
            get
            {
                int found;
                int idx = Find(name, occurrence, out found);
                if (idx >= 0)
                    return mKeys[idx].Value;
                else
                    return null;
            }
            set
            {
                int found;
                int idx = Find(name, occurrence, out found);
                if (idx >= 0)
                    mKeys[idx].Value = value;
                if (found == occurrence)
                    Add(name, value);
                else
                    throw new ArgumentOutOfRangeException("occurrence");
            }
        }

        public string this[string name, string defaultValue]
        {
            get
            {
                string s = this[name, 0];
                if (s == null)
                    return defaultValue;
                else
                    return s;
            }
        }

        public string this[string name, int occurrence, string defaultValue]
        {
            get
            {
                string s = this[name, occurrence];
                if (s == null)
                    return defaultValue;
                else
                    return s;
            }
        }

        public void Add(string name, string value)
        {
            mKeys.Add(new ProfileKey(name, value, true));
        }

        internal void Add(string orgName, string name, string value, bool isKey)
        {
            mKeys.Add(new ProfileKey(orgName, name, value, isKey));
        }

        public void Remove(int index)
        {
            mKeys.RemoveAt(index);
        }

        public void Remove(string name, int occurrence)
        {
            int found;
            int idx = Find(name, occurrence, out found);
            if (idx >= 0)
                Remove(idx);
            else
                throw new ArgumentOutOfRangeException("occurrence");
        }

        public void Remove(string name)
        {
            Remove(name, 0);
        }

        public bool Exists(string name)
        {
            int found;
            return Find(name, 0, out found) >= 0;
        }

        public int CountOf(string name)
        {
            int found;
            Find(name, Int32.MaxValue, out found);
            return found;
        }

        public void Clear()
        {
            mKeys.Clear();
        }

        private int Find(string name, int occurrence, out int found)
        {
            int _occurence = 0;
            for (int i = 0; i < mKeys.Count; i++)
            {
                if (mKeys[i].IsKey && name.Equals(mKeys[i].Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (_occurence == occurrence)
                    {
                        found = _occurence;
                        return i;
                    }
                    else
                        _occurence++;
                }
            }
            found = _occurence;
            return -1;
        }

        public IEnumerator<ProfileKey> GetEnumerator()
        {
            return mKeys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mKeys.GetEnumerator();
        }
    }

    public class Profile : IEnumerable<ProfileSection>
    {
        List<ProfileSection> mSections = new List<ProfileSection>();

        public int Count
        {
            get
            {
                return mSections.Count;
            }
        }

        public ProfileSection this[int index]
        {
            get
            {
                return mSections[index];
            }
        }

        public ProfileSection this[string name]
        {
            get
            {
                for (int i = 0; i < mSections.Count; i++)
                    if (name.Equals(mSections[i].Name, StringComparison.InvariantCultureIgnoreCase))
                        return mSections[i];
                return null;
            }
        }

        public string this[string section, string key]
        {
            get
            {
                return this[section, key, 0];
            }
            set
            {
                this[section, key, 0] = value;
            }
        }

        public string this[string section, string key, string defaultValue]
        {
            get
            {
                string s = this[section, key, 0];
                if (s == null)
                    return defaultValue;
                else
                    return s;
            }
        }

        public string this[string section, string key, int occurrence, string defaultValue]
        {
            get
            {
                string s = this[section, key, occurrence];
                if (s == null)
                    return defaultValue;
                else
                    return s;
            }
        }

        public string this[string section, string key, int occurrence]
        {
            get
            {
                ProfileSection _section = this[section];
                if (_section == null)
                    return null;
                return _section[key, occurrence];
            }
            set
            {
                ProfileSection _section = this[section];
                if (_section == null)
                {
                    if (occurrence != 0)
                        throw new ArgumentOutOfRangeException("occurrence");
                    _section = new ProfileSection(section);
                    mSections.Add(_section);
                }
                _section[key, occurrence] = value;
            }
        }

        public IEnumerator<ProfileSection> GetEnumerator()
        {
            return mSections.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mSections.GetEnumerator();
        }

        public Profile()
        {
        }

        static Regex mRegSection = new Regex("^\\s*\\[([^\\s\\]]+)\\]\\s*$");
        static Regex mRegKey = new Regex("^(\\s*([^\\s\\=]([^\\=]*[^\\s\\=])?)\\s*)=(.*)$");

        public void Load(string fileName)
        {
            mSections.Clear();

            ProfileSection section = new ProfileSection("");
            mSections.Add(section);

            using (StreamReader reader = new StreamReader(fileName, Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Match m;
                    m = mRegSection.Match(line);
                    if (m.Success)
                    {
                        section = new ProfileSection(m.Groups[1].Value);
                        mSections.Add(section);
                    }
                    else
                    {
                        m = mRegKey.Match(line);
                        if (m.Success)
                        {
                            section.Add(m.Groups[1].Value, m.Groups[2].Value, m.Groups[4].Value, true);
                        }
                        else
                            section.Add("", "", line, false);
                    }
                }
                reader.Close();
            }
        }

        public void Save(string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                for (int i = 0; i < mSections.Count; i++)
                {
                    ProfileSection section = mSections[i];
                    if (section.Name.Length > 0)
                        writer.WriteLine("[{0}]", section.Name);
                    for (int j = 0; j < section.Count; j++)
                    {
                        ProfileKey key = section[j];
                        if (key.IsKey)
                            writer.WriteLine("{0}={1}", key.OrgName, key.Value);
                        else
                            writer.WriteLine("{0}", key.Value);
                    }
                }
            }
        }

        public ProfileSection AddSection(string name)
        {
            ProfileSection s = new ProfileSection(name);
            mSections.Add(s);
            return s;
        }
    }
}
