#define SAVEDATA_NOCHECK
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using Microsoft.Xna.Framework.Input;
using System.Security.Cryptography;
using System.Diagnostics;

namespace CommonPart
{


    /*
        MapDataFile mapfile = new MapDataFile( map_name);
        BinaryReader br= new BinaryReader(mapfile.fs);
        BinaryWriter bw= new BinaryWriter(mapfile.fs); 
        SaveManager rema=new SaveManager(br);
        SaveManager wrma=new SaveManager(bw);

        for(int x=0;x<){
            wrma.ReadOrWrite(  );
        }
    */
    class MapDataFile
    {
        public readonly string FileDir;
        public readonly FileStream fs;
        public MapDataFile(string name)
        {
            FileDir = "Datas/map_" + name + ".dat";
            if (!File.Exists(FileDir))
            {
                File.Create(FileDir);
            }
            fs = new FileStream(FileDir, FileMode.Open);
        }

        public void close()
        {
            fs.Close();
        }
        public void dispose()
        {
            fs.Dispose();
        }
    }

}// namespace end