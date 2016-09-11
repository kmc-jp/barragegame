using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CommonPart;

namespace CommonPart
{
    class UnitTypeDataBase
    {
        static char interval_of_each_type = ';';//ut1;ut2
        static char interval_of_each_datatype = ',';// int ...ij,string kl...
        public List<UnitType> UnitTypeList = new List<UnitType>();
        public Dictionary<string, UnitType> UnitTypeDictionary = new Dictionary<string, UnitType>();
        public UnitTypeDataBase(BinaryReader br)
        {
            setup_from_BinaryReader(br);
        }

        public void setup_from_BinaryReader(BinaryReader br)
        {
            if (br == null) { return; }
            int n = 0;
            while (true)
            {
                try
                {
                    bool next_is_str = false;
                    List<int> intdatas = new List<int>();
                    List<string> stringdatas = new List<string>();
                    while (true) // load in every int and string data for one unit type 
                    {
                        if (!next_is_str && br.PeekChar() == interval_of_each_datatype)
                        //intのデータの一部を読み込み,それがちょうどintervalと一致するすることもあり得る
                        //これはまだ解決されていません。
                        {
                            next_is_str = true;
                            br.ReadChar();
                        }
                        else if (next_is_str && br.PeekChar() == interval_of_each_type) //stringdataがなくてもintervalは必ずあります。
                        {

                            next_is_str = false;
                            UnitTypeList.Add(new UnitType(intdatas, stringdatas, n));
                            n++;
                            br.ReadChar();
                            break;
                        }
                        else
                        {
                            if (next_is_str)
                            {
                                stringdatas.Add(br.ReadString());
                            }
                            else
                            {
                                intdatas.Add(br.ReadInt16());
                            }
                        }
                    }// while end. one UnitType has been created and added to the List
                }
                catch (EndOfStreamException e)
                {
                    break;
                }
            }// Finished reading file. List should be alright.
            foreach (UnitType ut in UnitTypeList)
            {
                UnitTypeDictionary.Add(ut.getTypename(), ut);
            }//Dictionaryをつくる
        }// end of setup

        public void save_into_BinaryWriter(BinaryWriter bw)
        {
            foreach (UnitType ut in UnitTypeList)
            {
                foreach (int i in ut.getIntData())
                {
                    bw.Write(i);
                }
                bw.Write(interval_of_each_datatype);
                foreach (string str in ut.getStringData())
                {
                    bw.Write(str);
                }
            }
            bw.Write(interval_of_each_type);
        }

        #region method
        public UnitType CreateBlankUt()
        {
            return new UnitType("test " + UnitTypeList.Count.ToString(), DataBase.defaultBlankTextureName, "test", 1, 1, 0,0);
        }
        public void Add(UnitType ut)
        {
            UnitTypeList.Add(ut);
            UnitTypeDictionary.Add(ut.getTypename(), ut);
        }
        public void Remove(UnitType ut)
        {
            UnitTypeDictionary.Remove(ut.getTypename());
            UnitTypeList.Remove(ut);
        }
        public void RemoveAt(int id)
        {
            UnitTypeDictionary.Remove(UnitTypeList[id].getTypename());
            UnitTypeList.RemoveAt(id);
        }
        public UnitType getUnitTypeWithName(string name)
        {
            return UnitTypeDictionary[name];
        }
        #endregion

    }// class UnitTypeDictionary end

    class UnitType
    {
        #region public
        public int index_in_List { get; private set; }
        public int maxhp { get; private set; }
        public int maxatk { get; private set; }
        //public int //something// { get; private set; }
        public string texture_name { get; private set; }
        public int texture_max_id { get; private set; }
        public int texture_min_id { get; private set; }
        #endregion

        #region protected
        protected string typename; //UnitTypeDictionaryにペアとなるstringである。他と重複しないように設定する必要がある。
                                 //これとは別にUnitは独自のstring変数 name を持っています。
        protected string label;   //ラベルは複数のUnitTypeが共通点を表すためにつかいます。stringとしてその部分文字列も使われるので、注意してほしい。
        protected int passableType;
        #endregion

        #region constructor
        public UnitType(string typename, string _texture_name,string label, int maxhp, int maxatk, int texture_max_id, int texture_min_id)
        {
            this.typename = typename;
            this.label = label;
            this.maxhp = maxhp;
            this.maxatk = maxatk;
            this.texture_max_id = texture_max_id;
            this.texture_min_id = texture_min_id;
        }
        public UnitType(List<int> intdatas, List<string> stringdatas,int id) // id is the index of the UnitTypeList
        {
            index_in_List = id;

            int n = 0;
            texture_max_id = intdatas[n]; //0th
            texture_min_id = intdatas[n];
            n++;
            maxhp = intdatas[n];
            n++;
            maxatk = intdatas[n];
            n++;
            passableType = intdatas[n];
            n++;

            n = 0;
            texture_name= stringdatas[n];
            n++;
            typename = stringdatas[n];
            n++;
            label = stringdatas[n];
            n++;
        }
        #endregion

        #region get property in int[] + string[]
        public virtual int[] getIntData()
        {
            return new int[] {
                texture_max_id, //0th
                texture_min_id, 
                maxhp,          
                maxatk,         
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
        #region Method
        public virtual bool passable()
        {
            return true;//要変更
        }

        public string getTypename()
        {
            return typename;
        }

        public string getlabel()
        {
            return label;
        }
        #endregion
    }
}
