using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CommonPart;
using Microsoft.Xna.Framework.Graphics;

namespace CommonPart
{
    enum Unit_Genre { textured = 0, animated = 1, skilled = 2, aniskil = 3, active=4, actaniskied=7 }
    class UnitTypeDataBase
    {
        static int versionOfEditor;

        public List<UnitType> UnitTypeList = new List<UnitType>();
        public Dictionary<string, UnitType> UnitTypeDictionary = new Dictionary<string, UnitType>();
        public UnitTypeDataBase(BinaryReader br)
        {
            setup_from_BinaryReader(br);
            setup_unitType();
        }

        private void setup_from_BinaryReader(BinaryReader br)
        {
            if (br == null) { return; }
            if (br.BaseStream.Length > 1)
            {
                versionOfEditor = br.ReadInt32();
                switch (versionOfEditor)
                {
                    case (int)(DataBase.VersionNumber.SixTeenTenTen):
                        load_br_161010(br);
                        break;
                    default:
                        Console.WriteLine("version Error! " + versionOfEditor);
                        break;
                }
                foreach (UnitType ut in UnitTypeList)
                {
                    UnitTypeDictionary.Add(ut.typename, ut);
                }//Dictionaryをつくる
            }
            else { Console.WriteLine("uts is empty"); return; }
        }// end of setup
        private void load_br_161010(BinaryReader br)
        {
            int n = 0;
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                try
                {
                    bool next_is_str = false;
                    List<int> intdatas = new List<int>();
                    List<string> stringdatas = new List<string>();
                    while (br.BaseStream.Position < br.BaseStream.Length) // load in every int and string data for one unit type 
                    {
                        if (!next_is_str && br.PeekChar() == DataBase.interval_of_each_datatype)//次はstringのデータ
                        //intのデータの一部を読み込み,それがちょうどintervalと一致するすることもあり得る
                        //これはまだ解決されていません。
                        {
                            next_is_str = true;
                            br.ReadChar();
                        }
                        else if (next_is_str && br.PeekChar() == DataBase.interval_of_each_type)//次のUTに移る
                                                                                                //stringdataがなくてもintervalは必ずあります。
                        {

                            next_is_str = false;
                            switch (intdatas[0])
                            {
                                case (int)(Unit_Genre.animated):
                                    UnitTypeList.Add(new AnimatedUnitType(intdatas, stringdatas, n));
                                    break;
                                default:
                                    UnitTypeList.Add(new UnitType(intdatas, stringdatas, n));
                                    break;
                            }
                            n++;
                            br.ReadChar();
                            break;
                        }
                        else
                        {
                            if (next_is_str)//stringを読み込む
                            {

                                stringdatas.Add(br.ReadString());
                            }
                            else
                            {
                                intdatas.Add(br.ReadInt32());
                            }
                        }
                    }// while end. one UnitType has been created and added to the List
                }
                catch (EndOfStreamException e)
                {
                    break;
                }
            }// Finished reading file. List should be alright.
        }
        public void save_into_BinaryWriter(BinaryWriter bw)
        {
            bw.Write(DataBase.ThisSystemVersionNumber);
            foreach (UnitType ut in UnitTypeList)
            {
                foreach (int i in ut.getIntData())
                {
                    bw.Write(i);
                }
                bw.Write(DataBase.interval_of_each_datatype);
                foreach (string str in ut.getStringData())
                {
                    bw.Write(str);
                }
            }
            bw.Write(DataBase.interval_of_each_type);
        }
        private ActiveAniSkiedUnitType lastUT
        {
            get { return ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]); }
        } 
        private void setup_unitType()
        {
            const double normalOmega = Math.PI / 60;
            #region Boss1
            UnitTypeList.Add(new ActiveAniSkiedUnitType("boss1", "boss1", "boss"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).textureTurn = true;
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.mugen, 300, new Vector(100, 100),PointType.pos_on_screen);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 100, new Vector(100, 100));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 100, new Vector(100, 620));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 100, new Vector(1180, 620));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 100, new Vector(1180, 100));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(4, 0, 30, 0, normalOmega,10000, 10);
            //((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("boss10circle-0");
            //((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("5wayshot");
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("boss11wayshot-0");
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("boss11wayshot-3");
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("boss1ransha-0");
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("boss1ransha-1");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("boss1body0", "boss1body0", "boss"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 100, new Vector(50, 50));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).textureTurn = true;
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(0, 0, 30, 0, normalOmega, 1000, 0);
            //((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("boss1wayshot-0");
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("boss11wayshot-1");
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("boss1downshot-0");
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("boss1downshot-1");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("boss1body1", "boss1body1", "boss"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 100, new Vector(50, 50));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).textureTurn = true;
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(0, 0, 30, 0, normalOmega,1000, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("boss1downshot-0");
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("boss1downshot-1");
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("boss11wayshot-1");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("boss1body2", "boss1body2", "boss"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 100, new Vector(50, 50));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).textureTurn = true;
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(0, 0, 30, 0, normalOmega,1000, 0);
            //((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("boss1wayshot-0");
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("boss11wayshot-1");
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("boss1downshot-0");
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("boss1downshot-1");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("boss1tail", "boss1tail", "boss"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 100, new Vector(50, 50));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("boss11wayshot-3");
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(0, 0, 30, 0, normalOmega,1000, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).textureTurn = true;
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);
            #endregion

            #region Boss2
            UnitTypeList.Add(new ActiveAniSkiedUnitType("boss2", "boss2", "boss"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 0, 0, normalOmega,1000, 0);
            lastUT.add_MoveTypeDataSet(MoveType.go_straight, 60, new Vector(DataBase.WindowDefaultSizeX/2, 180), PointType.pos_on_screen);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 0, new Vector());
            //((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("5wayshot");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("funnel", "E6", "boss"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 100, new Vector());
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 50, 0, normalOmega, 1000, 0);
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);
            /*
            UnitTypeList.Add(new ActiveAniSkiedUnitType("funnel1", "E6", "boss"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 100, new Vector());
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("3wayshot-1");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);
            */
            UnitTypeList.Add(new ActiveAniSkiedUnitType("boss2 head", "boss2head", "boss"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 100, new Vector());
            lastUT.setup_standard(0, 0, 100, Math.PI / 2);
            
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("boss2 body7", "boss2body", "boss"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 100, new Vector());
            
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            #endregion

            #region Boss3
            string shot = "boss3onfire-shot", breath = "boss3onfire-breath", hibreath = "boss3onfire-longbreath",
    rshot0 = "boss3onfire-r", rshot1 = "boss3onfire-r^-1", yanagi = "boss3onfire-yanagi", hakkyou = "boss3onfire-hakkyo";
            UnitTypeList.Add(new ActiveAniSkiedUnitType("boss3", "boss3", "boss"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 0, 0, normalOmega, 1000, 0);
            lastUT.add_MoveTypeDataSet(MoveType.go_straight, 25, new Vector(630,250),PointType.pos_on_screen);
            lastUT.add_MoveTypeDataSet(MoveType.noMotion, 1, new Vector());
            lastUT.add_MoveTypeDataSet(MoveType.go_straight, 150, new Vector(800, 300),PointType.pos_on_screen);
            lastUT.add_MoveTypeDataSet(MoveType.go_straight, 240, new Vector(840, 160), PointType.pos_on_screen);
            lastUT.add_MoveTypeDataSet(MoveType.go_straight, 110, new Vector(540, 250), PointType.pos_on_screen);
            lastUT.add_MoveTypeDataSet(MoveType.noMotion, 1, new Vector());
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames(shot);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames(breath);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames(hibreath);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames(rshot0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames(rshot1);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames(yanagi);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames(hakkyou);
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);
            #endregion

            #region test

            UnitTypeList.Add(new ActiveAniSkiedUnitType("testE1", "E1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 120, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("testE2", null, "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 120, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            #endregion
            #region Stage1
            UnitTypeList.Add(new ActiveAniSkiedUnitType("E1a-0", "E2-1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 120, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("3wayshot-0");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E1a-1", "E2-1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 120, new Vector(0, 240));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("20circle-0");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E1a-2", "E1-1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 300, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("3wayshot-1");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E1a-3", "E1-1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 60, new Vector(0, 20));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.rightcircle, 360, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("20circle-0");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E1a-4", "E2-3", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 60, new Vector(0, 70));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.rightcircle, 300, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("zyuzi-0");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E1a-5", "E2-3", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 60, new Vector(0, 70));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.leftcircle, 300, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("zyuzi-0");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E1b-0", "E1-1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 180, new Vector(0, 150));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 300, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("1wayshot-1");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E1b-1", "E2-2", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 240, new Vector(100,200));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 240, new Vector(620, 280));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 240, new Vector(720, 120));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("4wayshot-2");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E1c-0a", "E3-1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 240, new Vector(250, 0));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 300, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("laser-down-1");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E1c-0b", "E3-1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 240, new Vector(-250, 0));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 300, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("laser-down-1");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E1c-1", "E3-1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 60, new Vector(0, 100));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 300, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("laser-once-1");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E1c-2", "E3-1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 60, new Vector(0, 140));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 300, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("laser-once-1");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E2a-0", "E2-f1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 120, new Vector(360, 360));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 120, new Vector(0, 0));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 120, new Vector(360, 360));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 120, new Vector(720, 0));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("downshot-1");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E2a-1", "E2-f1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 120, new Vector(360, 360));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 120, new Vector(720, 0));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 120, new Vector(360, 360));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 120, new Vector(0, 0));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("downshot-1");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E3-0", "E1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 180, new Vector(0, 360));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(720, 0));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("4wayshot-1");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E3-1", "E1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 60, new Vector(0, 220));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(720, 0));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("4wayshot-0");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);
            #endregion

            #region Stage2
            UnitTypeList.Add(new ActiveAniSkiedUnitType("E4-1a", "E4-1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 180, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 180, new Vector(0, 480));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("20circle-0");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E4-2a", "E4-2", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 180, new Vector(0, 480));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 180, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("1chaseShot-1");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E4-1c", "E4-1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 120, new Vector(0, 240));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("3wayshot-1");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E4-2b", "E4-2", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 60, new Vector(0, 80));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 60, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("zyuzi-0");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E5-a", "E5", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 240, new Vector(0, 240));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 240, new Vector(0, -240));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("laser-once-1");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E2-2a", "E2-2", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 180, new Vector(540, 480));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 180, new Vector(180, 0));
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E2-2b", "E2-2", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 180, new Vector(180, 480));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 180, new Vector(540, 0));
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E2-f1", "E2-f1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 120, new Vector(0, -180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 300, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("20circle-0");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E4-2c", "E4-2", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 180, new Vector(-480, 0));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 180, new Vector(480, 0));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("3wayshot-0");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E4-1e", "E4-1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 180, new Vector(720, 270));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 180, new Vector(0, 320));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 180, new Vector(720, 370));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 180, new Vector(0, 320));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("1chaseShot-1");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E4-1f", "E4-1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 180, new Vector(0, 370));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 180, new Vector(720, 320));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 180, new Vector(0, 270));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 180, new Vector(720, 320));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("1chaseShot-1");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E5-c", "E5", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 180, new Vector(0, 660));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 300, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("1chaseShot-1");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E5-d", "E5", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 180, new Vector(0, -60));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 300, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("1chaseShot-1");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E4-2d", "E4-2", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 240, new Vector(0, 360));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 300, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("zyuzi-0");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E5-e", "E5", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 60, new Vector(0, 60));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 300, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("1chaseShot-1");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);
           
            UnitTypeList.Add(new ActiveAniSkiedUnitType("E4-1g", "E4-1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 60, new Vector(0, 60));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 300, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("2wayshot-0");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E4-2e", "E4-2", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 60, new Vector(0, 60));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 300, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("4wayshot-2");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);
         
            UnitTypeList.Add(new ActiveAniSkiedUnitType("E4-2f", "E4-2", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 240, new Vector(20, 720));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 240, new Vector(700, 0));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("1chaseShot-1");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E4-2g", "E4-2", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 240, new Vector(700, 720));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 240, new Vector(20, 0));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("3wayshot-0");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E2-3", "E2-f2", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 60, new Vector(0, 120));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(720, 0));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("laser-1");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E6-a", "E6", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 300, new Vector(0, 720));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 300, new Vector(0, -720));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("3wayshot-1");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E6-b", "E6", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 180, new Vector(0, 360));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 180, new Vector(360, 720));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 180, new Vector(360, 0));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("4wayshot-1");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E6-c", "E6", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 120, new Vector(60, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 300, new Vector(0, 720));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("3wayshot-0");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E6-d", "E6", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 120, new Vector(-60, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 300, new Vector(0, 720));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("3wayshot-0");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E5-f", "E5", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 240, new Vector(720, 0));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 240, new Vector(-720, 0));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("1chaseShot-1");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E5-g", "E5", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 180, new Vector(-720, 0));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 180, new Vector(720, 0));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("3wayshot-0");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("E4-1h", "E4-1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 120, new Vector(0, 60));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(0, 720));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("3wayshot-1");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            #endregion

            #region Stage3
            UnitTypeList.Add(new ActiveAniSkiedUnitType("3E7a", "E7", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 180, new Vector(100,200));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 100, new Vector(640,60));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 100, new Vector(80, 60));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 100, new Vector(620, 200));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("3E7b", "E7", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 180, new Vector(620, 200));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 100, new Vector(80, 60));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 100, new Vector(640, 60));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 100, new Vector(100, 200));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("3E1a", "E1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 30, new Vector(0, 150));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("3E2-f2a", "E2-f2", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 60, new Vector(0, 100));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("3E8a", "E8", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 30, new Vector(0, 50));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("3E1b", "E1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 60, new Vector(100, 150));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("3E1c", "E1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 60, new Vector(620, 150));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("3E7c", "E7", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 100, new Vector(620,0));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 20, new Vector(0, -30));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 100, new Vector(-620, 0));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 20, new Vector(0, -30));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("3E3-1a", "E3-1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 30, new Vector(0, 130));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("3E8b", "E8", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 150, new Vector(0, -580));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("3E4-2a", "E4-2", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 90, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("3E4-1a", "E4-1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 45, new Vector(0, 90));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("3E1d", "E1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 100, new Vector(20,360));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 80, new Vector(360,700));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 80, new Vector(700, 360));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 100, new Vector(0,50));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("3E7d", "E7", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 100, new Vector(700, 360));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 80, new Vector(360, 700));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 80, new Vector(20, 360));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.screen_point_target, 100, new Vector(720, 50));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("3E8c", "E8", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 60, new Vector(0, 160));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("3E5a", "E5", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 50, new Vector(0, 200));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("3E5b", "E5", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 50, new Vector(0, 150));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("3E7e", "E7", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 100, new Vector(0, 150));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("3E7e-N", null, "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 100, new Vector(0, 150));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            #endregion

            #region Stage4
            UnitTypeList.Add(new ActiveAniSkiedUnitType("4E2-f2a", "E2-f2", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 240, new Vector(0, 200));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("4E2-f2b", "E2-f2", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 60, new Vector(0, 150));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("4E4-2a", "E4-2", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 120, new Vector(260, 60));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(0, 180));

            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("1chaseShot-1");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("4E4-1a", "E4-1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 120, new Vector(-260, 60));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.rightcircle, 360, new Vector(0, 180));

            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("1chaseShot-1");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("4E1a", "E1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 180, new Vector(0, 70));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.rightcircle, 300, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("4E1b", "E1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 180, new Vector(0, 70));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.leftcircle, 300, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("4E2-2a", "E2-2", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 600, new Vector(0, -620));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 300, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("1wayshot-1");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);
            
            UnitTypeList.Add(new ActiveAniSkiedUnitType("4E5a", "E5", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 60, new Vector(100, 0));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 300, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("4E5b", "E5", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 60, new Vector(-100, 0));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 300, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("4E3-2a", "E3-2", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 60, new Vector(0, 100));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 300, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("laser-once-1");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("4E2-f1a", "E2-f1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 60, new Vector(0, 100));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 300, new Vector(0, 180));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("1chaseShot-1");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("4E2-1a", "E2-1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 120, new Vector(0, 100));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(720, 0));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("4E2-f2d", "E2-f2", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 60, new Vector(0, 170));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(720, 0));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("20circle-0");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("4E4-2c", "E4-2", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 900, new Vector(0, 700));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(720, 0));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("4E5c", "E5", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 90, new Vector(150, -100));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(720, 0));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("4E5d", "E5", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 90, new Vector(-150, -100));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(720, 0));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("4E1c", "E1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 60, new Vector(0, 80));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(720, 0));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);
            
            UnitTypeList.Add(new ActiveAniSkiedUnitType("4E4-1b", "E4-1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 90, new Vector(0,100 ));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(720, 0));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("4E4-1c", "E4-1", "enemy"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.go_straight, 90, new Vector(0, 100));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_MoveTypeDataSet(MoveType.noMotion, 120, new Vector(720, 0));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(5, 0, 30, 0);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames("");
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);
            #endregion

            #region Boss6
            string _64w1_un50 = "boss6-64way1", _dd_un50 = "boss6-dandan", _32w_un50 = "boss6-32wayransya", _mix2 = "boss6mixbullet2";
            string _hakyo1_ov20 = "boss6-hakkyo1";
            string _czj_ov50 = "boss6-createzyuzi", _czj_sub_ov50 = "boss6-createzyuzifor", _mix1_ov50 = "boss6mixbullet1";
            string _hakyo2_un20 = "boss6-hakkyo2", _mix3 = "boss6mixbullet3";

            UnitTypeList.Add(new ActiveAniSkiedUnitType("boss6", "stage6", "boss"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(10, 0, 10, 0, normalOmega, 10000, 10);

            lastUT.add_MoveTypeDataSet(MoveType.go_straight, 1, new Vector(640, 240), PointType.pos_on_screen);
            lastUT.add_MoveTypeDataSet(MoveType.noMotion, 1, new Vector());
            lastUT.add_MoveTypeDataSet(MoveType.go_straight,1 , new Vector(400,150),PointType.randomRange);
            lastUT.add_MoveTypeDataSet(MoveType.noMotion, 1, new Vector());
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("boss6 up ball", "bulletDL", "boss"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(10, 0, 80, 0, normalOmega, 10000, 10);
            lastUT.add_MoveTypeDataSet(MoveType.noMotion, 0, new Vector());
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames(_hakyo2_un20);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames(_hakyo1_ov20);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames(_czj_sub_ov50);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames(_dd_un50);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames(_mix1_ov50);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames(_mix3);
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);

            UnitTypeList.Add(new ActiveAniSkiedUnitType("boss6 down ball", "bulletDL", "boss"));
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).setup_standard(10, 0, 80, 0, normalOmega, 10000, 10);
            lastUT.add_MoveTypeDataSet(MoveType.noMotion, 0, new Vector());
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames(_64w1_un50);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames(_czj_ov50);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames(_32w_un50);
            ((ActiveAniSkiedUnitType)UnitTypeList[UnitTypeList.Count - 1]).add_skillnames(_mix2);
            UnitTypeDictionary.Add(UnitTypeList[UnitTypeList.Count - 1].typename, UnitTypeList[UnitTypeList.Count - 1]);
            #endregion
        }

        #region method
        public UnitType CreateBlankUt()
        {
            return new UnitType("test " + UnitTypeList.Count.ToString(), DataBase.defaultBlankTextureName, "test", 0, 0);
        }
        public void Add(UnitType ut)
        {
            UnitTypeList.Add(ut);
            UnitTypeDictionary.Add(ut.typename, ut);
        }
        public void Remove(UnitType ut)
        {
            UnitTypeDictionary.Remove(ut.typename);
            UnitTypeList.Remove(ut);
        }
        public void RemoveAt(int id)
        {
            UnitTypeDictionary.Remove(UnitTypeList[id].typename);
            UnitTypeList.RemoveAt(id);
        }
        public UnitType getUnitTypeWithName(string name)
        {
            if (UnitTypeDictionary.ContainsKey(name))
            {
                return UnitTypeDictionary[name];
            }
            else
            {
                Console.WriteLine("UTD error: " + name + " does no exist.");
                return CreateBlankUt();
            }
        }

        #endregion

    }// class UnitTypeDictionary end

    class UnitType
    {
        #region public
        /// <summary>
        /// このUTのジャンルとなる。0-アニメーションしない、1-アニメションする、2-skillを持つアニメ－ションしない
        /// 次は4,8,16..と2の乗数
        /// </summary>
        public int genre = (int)(Unit_Genre.textured);
        /// <summary>
        /// AnimationUnitTypeかどうか
        /// </summary>
        public bool animated { get { return genre % 2 == 1; } }

        public int index_in_List { get; protected set; }
        //public int //something// { get; protected set; }
        public string typename { get; protected set; }

        public string texture_name { get; protected set; }        //UnitTypeDictionaryにペアとなるstringである。他と重複しないように設定する必要がある。
                                                                  //これとは別にUnitは独自のstring変数 name を持っています。
        public string label { get; protected set; } //ラベルは複数のUnitTypeが共通点を表すためにつかいます。stringとしてその部分文字列も使われるので、注意してほしい.
        public int texture_max_id { get; protected set; }
        public int texture_min_id { get; protected set; }
        /// <summary>
        /// animationにアクセスするためのkeyの一部
        /// </summary>
        public string animation_name;
        #endregion

        #region protected
        protected int passableType;
        #endregion

        #region constructor
        /// <summary>
        /// これはAnimation UnitType専用のコンストラクタ です.animation_nameが代入されます
        /// </summary>
        protected UnitType(string _typename, string _texture_name, string _label)
        {
            animation_name = _texture_name;
            texture_name = animation_name;//DataBase.getAniD(animation_name).texture_name;
            typename = _typename; label = _label;
        }

        public UnitType(string _typename, string _texture_name, string label, int texture_max_id, int texture_min_id)
        {
            typename = _typename;
            texture_name = _texture_name;
            this.label = label;
            this.texture_max_id = texture_max_id;
            this.texture_min_id = texture_min_id;
        }
        public UnitType(List<int> intdatas, List<string> stringdatas, int id) // id is the index of the UnitTypeList
        {
            index_in_List = id;

            int n = 0;
            genre = intdatas[n];
            if (animated) { Console.WriteLine("id:" + id + " animated as a UnitType!"); }
            n++;
            texture_max_id = intdatas[n];
            n++;
            texture_min_id = intdatas[n];
            n++;
            passableType = intdatas[n];
            n++;

            n = 0;
            texture_name = stringdatas[n];
            n++;
            typename = stringdatas[n];
            n++;
            label = stringdatas[n];
            n++;
        }
        #endregion

        #region get property in int[] + string[]
        //今のint[],string[]は、必要なデータかつ個数が決まっているのでこの様に書いている。
        //後に、例えば不特定多数のskillを覚えられるとするとskill_ids[]を返すものを作るといいでしょう。
        public virtual int[] getIntData()
        {
            return new int[] {
                genre,
                texture_max_id,
                texture_min_id,
                passableType,   

                //any other int variables should be added here
            };
        }
        public virtual string[] getStringData()
        {
            return new string[] {
                texture_name,
                typename,
                label,

                //any other int variables should be added here
            };
        }
        #endregion
        public virtual void drawIcon(Drawing d, Vector pos)
        {
            d.Draw(pos, DataBase.getTex(texture_name), DataBase.getRectFromTextureNameAndIndex(texture_name, texture_min_id), DepthID.Message);
        }
        #region Method
        public virtual bool passable()
        {
            return true;//要変更
        }
        #endregion
    }
    /// <summary>
    /// genre,passable;texture_name,typename,label,
    /// </summary>
    class AnimatedUnitType : UnitType
    {
        //texture_nameはanimationへのアクセスkeyです

        #region public
        public AnimationAdvanced animation;
        #endregion

        /// <summary>
        /// ほぼUnitTypeにあるAnimationUnitType専用のコンストラクタによって構成される
        /// </summary>
        /// <param name="_texture_name">animationにアクセスするための一部のkeyです</param>
        public AnimatedUnitType(string _typename, string _texture_name, string _label)
            : base(_typename, _texture_name, _label)
        {
            genre = (int)(Unit_Genre.animated);
        }
        public AnimatedUnitType(List<int> intdatas, List<string> stringdatas, int id)
            : this(stringdatas[1], stringdatas[0], stringdatas[2])
        // id is the index of the UnitTypeList
        {
            index_in_List = id;
        }

        #region get property in int[] + string[]
        //今のint[],string[]は、必要なデータかつ個数が決まっているのでこの様に書いている。
        //後に、例えば不特定多数のskillを覚えられるとするとskill_ids[]を返すものを作るといいでしょう。
        public override int[] getIntData()
        {
            return new int[] {
                genre,
                passableType,   

                //any other int variables should be added here
            };
        }
        public override string[] getStringData()
        {
            return new string[] {
                texture_name,
                typename,
                label,

                //any other int variables should be added here
            };
        }
        #endregion

        #region activity
        public override void drawIcon(Drawing d, Vector pos)
        {
            if (animation != null)
            {
                animation.Draw(d, pos, DepthID.Message);
            }
        }
        
        #endregion
    }

    class SkilledAnimatedUnitType : AnimatedUnitType
    {
        public List<string> skillNames = new List<string>(); 
        public SkilledAnimatedUnitType(string _typename, string _texture_name,string _label,string [] _snames):base(_typename,_texture_name,_label)
        {
            add_skillnames(_snames);
            genre = (int)Unit_Genre.animated+(int)Unit_Genre.skilled;
        }
        public SkilledAnimatedUnitType(string _typename, string _texture_name, string _label) : base(_typename, _texture_name, _label)
        {
            genre = (int)Unit_Genre.animated + (int)Unit_Genre.skilled;
        }

        public void add_skillnames(string [] _snames)
        {
            foreach (string _sname in _snames)
            {
                add_skillnames(_sname);
            }
        }
        public void add_skillnames(string _sname)
        {
            if (!skillNames.Contains(_sname) && DataBase.SkillDatasDictionary.ContainsKey(_sname))
            { 
                skillNames.Add(_sname);
            }else
            {
                if (skillNames.Contains(_sname))
                {
                    Console.WriteLine("SkillUnitType ERROR " + _sname);
                }
                else
                {
                    skillNames.Add(_sname);
                    //Console.WriteLine("SkillUnitType DataBase not Exist "+_sname);
                }
            }
        }
    }//SkilledAnimatedUnitType END
}
