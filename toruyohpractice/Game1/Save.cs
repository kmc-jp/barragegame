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

namespace CommonPart {
    static class Save {
        const string SaveDir = "save";
        public const string SaveFile = SaveDir + "/save.dat";        
    }
    
    /// <summary>
    /// 読み込みと書き込みを同じ関数でするためのクラス
    /// </summary>
    class SaveManager {
        BinaryReader reader;
        BinaryWriter writer;
        public bool IsReadMode { get; private set; }
        public SaveManager(BinaryReader r) { reader = r; IsReadMode = true; }
        public SaveManager(BinaryWriter w) { writer = w; IsReadMode = false; }

        public void ReadOrWrite(ref bool value) { if(IsReadMode) value = reader.ReadBoolean(); else writer.Write(value); }
        public void ReadOrWrite(ref byte value) { if(IsReadMode) value = reader.ReadByte(); else writer.Write(value); }
        public void ReadOrWrite(ref byte[] value, int count) {
            if(IsReadMode) value = reader.ReadBytes(reader.ReadInt32()); else { writer.Write(value.Length); writer.Write(value); }
        }
        public void ReadOrWrite(ref char value) { if(IsReadMode) value = reader.ReadChar(); else writer.Write(value); }
        public void ReadOrWrite(ref char[] value, int count) {
            if(IsReadMode) value = reader.ReadChars(reader.ReadInt32()); else { writer.Write(value.Length); writer.Write(value); }
        }
        public void ReadOrWrite(ref decimal value) { if(IsReadMode) value = reader.ReadDecimal(); else writer.Write(value); }
        public void ReadOrWrite(ref double value) { if(IsReadMode) value = reader.ReadDouble(); else writer.Write(value); }
        public void ReadOrWrite(ref short value) { if(IsReadMode) value = reader.ReadInt16(); else writer.Write(value); }
        public void ReadOrWrite(ref int value) { if(IsReadMode) value = reader.ReadInt32(); else writer.Write(value); }
        public void ReadOrWrite(ref long value) { if(IsReadMode) value = reader.ReadInt64(); else writer.Write(value); }
        public void ReadOrWrite(ref sbyte value) { if(IsReadMode) value = reader.ReadSByte(); else writer.Write(value); }
        public void ReadOrWrite(ref float value) { if(IsReadMode) value = reader.ReadSingle(); else writer.Write(value); }
        public void ReadOrWrite(ref string value) { if(IsReadMode) value = reader.ReadString(); else writer.Write(value); }
        public void ReadOrWrite(ref ushort value) { if(IsReadMode) value = reader.ReadUInt16(); else writer.Write(value); }
        public void ReadOrWrite(ref uint value) { if(IsReadMode) value = reader.ReadUInt32(); else writer.Write(value); }
        public void ReadOrWrite(ref ulong value) { if(IsReadMode) value = reader.ReadUInt64(); else writer.Write(value); }
        public void ReadOrWrite(ref Vector value) {
            if(IsReadMode) value = new Vector(reader.ReadDouble(), reader.ReadDouble()); else { writer.Write(value.X); writer.Write(value.Y); }
        }
        //参照渡しできないものの記述を簡単にする用
        public void Write(byte value) { if(!IsReadMode) writer.Write(value); }
        public void Write(short value) { if(!IsReadMode) writer.Write(value); }
        public void Write(int value) { if(!IsReadMode) writer.Write(value); }
        public void Write(long value) { if(!IsReadMode) writer.Write(value); }
        public void Write(sbyte value) { if(!IsReadMode) writer.Write(value); }
        public void Write(ushort value) { if(!IsReadMode) writer.Write(value); }
        public void Write(uint value) { if(!IsReadMode) writer.Write(value); }
        public void Write(ulong value) { if(!IsReadMode) writer.Write(value); }
        public void Write(float value) { if(!IsReadMode) writer.Write(value); }
        public void Write(double value) { if(!IsReadMode) writer.Write(value); }
        public void Write(decimal value) { if(!IsReadMode) writer.Write(value); }
        public void Write(string value) { if(!IsReadMode) writer.Write(value); }
        public void Write(bool value) { if(!IsReadMode) writer.Write(value); }
    }
    [Serializable]
    class SaveLoadException: Exception {
        public SaveLoadException(string mes) : base(mes) { }
        public SaveLoadException(string mes, Exception inner) : base(mes, inner) { }
    }
    [Serializable]
    class SaveLoadException_Inner : Exception {
        public SaveLoadException_Inner(string mes) : base(mes) { }
        public SaveLoadException_Inner(string mes, Exception inner) : base(mes, inner) { }

    }
}
