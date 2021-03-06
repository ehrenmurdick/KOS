﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace kOS
{
    public class Volume
    {
        public int Capacity = -1;
        public String Name = "";
        protected List<File> files = new List<File>();

        public bool Renameable = true;

        public virtual File GetByName(String name)
        {
            foreach (File p in files)
            {
                if (p.Filename.ToUpper() == name.ToUpper()) return p;
            }

            return null;
        }

        public virtual void DeleteByName(String name)
        {
            foreach (File p in files)
            {
                if (p.Filename.ToUpper() == name.ToUpper())
                {
                    files.Remove(p);
                    return;
                }
            }
        }

        public virtual bool SaveFile(File file)
        {
            DeleteByName(file.Filename);
            files.Add(file);

            return true;
        }

        public virtual int GetFreeSpace() { return -1; }
        public virtual bool IsRoomFor(File newFile) { return true; }
        public virtual void LoadPrograms(List<File> programsToLoad) { }
        public virtual ConfigNode Save(string nodeName) { return new ConfigNode(nodeName); }

        public virtual List<FileInfo> GetFileList()
        {
            List<FileInfo> retList = new List<FileInfo>();

            foreach (File file in files)
            {
                retList.Add(new FileInfo(file.Filename, file.GetSize()));
            }

            return retList;
        }
    }

    public class Archive : Volume
    {
        public string ArchiveFolder = GameDatabase.Instance.PluginDataFolder + "/Plugins/PluginData/Archive/";

        public Archive()
        {
            Renameable = false;
            Name = "Archive";

            loadAll();
        }

        public override bool IsRoomFor(File newFile)
        {
            return true;
        }

        private void loadAll()
        {
            Directory.CreateDirectory(ArchiveFolder);

            // Attempt to migrate files from old archive drive
            if (KSP.IO.File.Exists<File>(HighLogic.fetch.GameSaveFolder + "/arc"))
            {
                var reader = KSP.IO.BinaryReader.CreateForType<File>(HighLogic.fetch.GameSaveFolder + "/arc");

                int fileCount = reader.ReadInt32();

                for (int i = 0; i < fileCount; i++)
                {
                    try
                    {
                        String filename = reader.ReadString();
                        String body = reader.ReadString();

                        File file = new File(filename);
                        file.Deserialize(body);

                        Debug.Log("**** " + filename);

                        files.Add(file);
                        SaveFile(file);
                    }
                    catch (EndOfStreamException e)
                    {
                        break;
                    }
                }

                reader.Close();

                KSP.IO.File.Delete<File>(HighLogic.fetch.GameSaveFolder + "/arc");
            }
        }

        public override File GetByName(string name)
        {
            try
            {
                using (StreamReader infile = new StreamReader(ArchiveFolder + name + ".txt", true))
                {
                    String fileBody = infile.ReadToEnd().Replace("\r\n", "\n") ;

                    File retFile = new File(name);
                    retFile.Deserialize(fileBody);
                    
                    base.DeleteByName(name);
                    files.Add(retFile);

                    return retFile;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public override bool SaveFile(File file)
        {
            base.SaveFile(file);

            Directory.CreateDirectory(ArchiveFolder);

            try
            {
                using (StreamWriter outfile = new StreamWriter(ArchiveFolder + file.Filename + ".txt", false))
                {
                    String fileBody = file.Serialize();

                    if (Application.platform == RuntimePlatform.WindowsPlayer)
                    {
                        // Only evil windows gets evil windows line breaks
                        fileBody = fileBody.Replace("\n", "\r\n");
                    }

                    outfile.Write(fileBody);
                }
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public override void DeleteByName(string name)
        {
            System.IO.File.Delete(ArchiveFolder + name + ".txt");
        }

        public override List<FileInfo> GetFileList()
        {
            var retList = new List<FileInfo>();

            try
            {
                foreach (var file in Directory.GetFiles(ArchiveFolder, "*.txt"))
                {
                    var sysFileInfo = new System.IO.FileInfo(file);
                    var fileInfo = new kOS.FileInfo(sysFileInfo.Name.Substring(0, sysFileInfo.Name.Length - 4), (int)sysFileInfo.Length);

                    retList.Add(fileInfo);
                }
            }
            catch (DirectoryNotFoundException e)
            {
            }

            return retList;
        }
    }
}
