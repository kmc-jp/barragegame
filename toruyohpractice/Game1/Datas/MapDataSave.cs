#define SAVEDATA_NOCHECK
using System.IO;
using System;
using System.Collections.Generic;

namespace CommonPart
{
   
    /// <summary>
    /// 必ずnewしてから使ってください。mapの基本属性を保持してどこからでも取得できるためのclass
    /// </summary>
    class MapDataSave
    {
        #region variables concerns FileStream
        private FileStream MapFileStream;
        public string fileName;
        public string mapName;
        /// <summary>
        /// starts from 0 !, maximum (x,y) on the map
        /// </summary>
        public int max_x, max_y;
        private BinaryReader br;
        private BinaryWriter bw;
        #endregion

        #region variable concerns Map in the screen 
        /// <summary>
        /// マップの1マスの長さはスクリーンのXrate、マップの1マスの高さはスクリーンのYrateだけに相当する。
        /// ect.マップの(0,0)座標はスクリーンの(Xrate長,Yrate高)の長方形にあたる。
        /// </summary>
        static public float Xrate = 4, Yrate = 4;
        /// <summary>
        /// Sceneにおいて、マップの描画が始まる左上の画面座標
        /// </summary>
        static public double leftsideX=0,topsideY=0;
        /// <summary> 
        /// Sceneから更新されるべきのスクリーン上に見えるmapの左上と右下の map上の座標
        /// </summary>
        static public int ltx=0, lty=0, rbx=0, rby=0;

        /// <summary>
        /// unitのListである。
        /// </summary>
        static public List<Unit> utList = new List<Unit>();
        #endregion

        #region constructor
        /// <summary>
        /// This Does Not make a File! use CreateFile() to save the MapData
        /// </summary>
        /// <param name="_fileName">マップがファイルとしての名前</param>
        /// <param name="_mapName">マップの名前</param>
        /// <param name="mx">マップの最大x座標</param>
        /// <param name="my">マップの最大y座標</param>
        public MapDataSave(string _fileName,string _mapName,int mx,int my,int _xrate,int _yrate,int _leftsideX,int _topsideY)
        {
            fileName = _fileName;
            mapName = _mapName;
            max_x = mx; max_y = my;
            Xrate = _xrate;
            Yrate = _yrate;
            leftsideX = _leftsideX;
            topsideY = _topsideY;
            createFiletoWrite();
            WriteAll();
        }
        /// <summary>
        /// !! only you already have the File whose name is _fileName.
        /// </summary>
        /// <param name="_fileName"></param>
        public MapDataSave(string _fileName)
        {
            DataBase.goToFolderDatas();
            if (File.Exists(_fileName))
            {
                MapFileStream = File.Open(_fileName, FileMode.Open,FileAccess.Read);
                MapFileStream.Position = 0;
                br = new BinaryReader(MapFileStream);
                int version = br.ReadInt32();
                #region switch version
                switch (version)
                {
                    case (int)(DataBase.VersionNumber.SixTeenTenTen):
                        mapName = br.ReadString();
                        max_x = br.ReadInt32();
                        max_y = br.ReadInt32();
                        Xrate = br.ReadSingle();
                        Yrate = br.ReadSingle();
                        leftsideX = br.ReadDouble();
                        topsideY = br.ReadDouble();
                        while (br.BaseStream.Position < br.BaseStream.Length)
                        {
                            bool it_is_int_now = true;
                            List<int> ints = new List<int>();
                            List<string> strings = new List<string>();
                            while (br.BaseStream.Position < br.BaseStream.Length)
                            {
                                if(br.PeekChar() == DataBase.interval_of_each_type)//unitとunitのデータの狭間 
                                {
                                    br.ReadChar();
                                    break;
                                }else if(it_is_int_now==true && br.PeekChar()==DataBase.interval_of_each_datatype)//intとstringの狭間
                                {
                                    it_is_int_now = false;// it is string now!
                                    br.ReadChar();
                                }else if (it_is_int_now)
                                {
                                    ints.Add(br.ReadInt32());
                                }else// it_is_sstring_now
                                {
                                    strings.Add(br.ReadString());
                                }
                            }// while reading one unit end
                            if (ints.Count > 0 && strings.Count > 0)
                            {
                                //utList.Add(new Unit(ints, strings));
                            }else { Console.WriteLine("MapDataS: ints "+ints.Count+" strs "+strings.Count); }
                        }//while reading all unit end

                        // BinaryReader has Read All
                        br.Close();
                        break;
                    default:
                        Console.Write("Unknown Datas here");
                        break;
                }
                #endregion
                br.Close();
                MapFileStream.Close();
            }else
            {
                Console.WriteLine(fileName + " is UnFound. Loading Map failed");
            }

        }// Load MapSaveData 
        #endregion

        static public void update_map_xy(int _ltx,int _lty,int _rbx,int _rby,int _leftsideX,int _topsideY)
        {
            ltx = _ltx;
            lty = _lty;
            rbx = _rbx;
            rby = _rby;
            leftsideX = _leftsideX;
            topsideY = _topsideY;
        }
        public void changeTo(string _fileName,string _mapName,int mx,int my,int _xrate,int _yrate) {
            mapName = _mapName;
            fileName = _fileName;
            max_x = mx;
            max_y = my;
            Xrate = _xrate; Yrate = _yrate;
        }

        public double ScreenPosXToMapPosX(double sx)
        {
            return (sx - leftsideX) / Xrate + ltx;
        }
        public double ScreenPosYToMapPosY(double sy)
        {
            return lty - (sy - topsideY) / Yrate;
        }
        public Vector ScreenPosToMapPos(Vector spos)
        {
            return new Vector(
                ScreenPosXToMapPosX(spos.X), ScreenPosYToMapPosY(spos.Y)
                );
        }
        public Vector ScreenPosToMapPos(double sx,double sy){
            return new Vector(
                ScreenPosXToMapPosX(sx), ScreenPosYToMapPosY(sy)
                );
        }
        public bool PosInsideMap(Vector pos)
        {
            if(pos.X>=0 && pos.X<=max_x && pos.Y>=0 && pos.Y <= max_y)
            {
                return true;
            }else
            {
                return false;
            }
        }
        
        /// <summary>
        /// Mapの情報を書き込むファイルを作る
        /// </summary>
        public void createFiletoWrite()
        {
            DataBase.goToFolderDatas();
            MapFileStream = File.Open(fileName, FileMode.OpenOrCreate,FileAccess.Write);
        }
        /// <summary>
        /// ファイルが作られている前提で、すべてのデータを書き込む
        /// </summary>
        public void WriteAll() {
            MapFileStream.Position = 0;
            bw = new BinaryWriter(MapFileStream);
            bw.Write(DataBase.ThisSystemVersionNumber);
            bw.Write(mapName);
            bw.Write(max_x);bw.Write(max_y);
            bw.Write(Xrate);bw.Write(Yrate);
            bw.Write(leftsideX);bw.Write(topsideY);
            #region Write Units
            if (utList.Count > 0) {
                Console.WriteLine("no Units On the MapDataS");
                /*
                for(int i = 0; i < utList.Count; i++)
                {
                    if (PosInsideMap(new Vector(utList[i].x_index, utList[i].y_index)))
                    {
                        //List<int> ints = utList[i].getListIntData();
                        //string[] strings = utList[i].getStringData();
                        for (int j = 0; j < ints.Count; j++)
                        {
                            bw.Write(ints[j]);
                        }
                        bw.Write(DataBase.interval_of_each_datatype);//like intintint,stringstringstring
                        for (int k = 0; k < strings.Length; k++)
                        {
                            bw.Write(strings[k]);
                        }
                        bw.Write(DataBase.interval_of_each_type);// like Unit[i] , Unit[i+1],
                    }
                }
                */
            }
            #endregion
            bw.Close();
            MapFileStream.Close();
        }
        public void close() // contains Dispose()
        {
            MapFileStream.Close();
            mapName = null;
            fileName = null;
        }


    }

}// namespace end