using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Microsoft.Xna.Framework;

namespace CommonPart {

    public enum Unit_state { fadeout=0,dead=1,out_of_window=2 };
    public enum MoveType {non_target=0,point_target=1,object_target=2,go_straight,mugen,rightcircle,leftcircle,stop,chase_angle };
    public enum Command
    {
        left_and_go_back = -101, nothing = -100, apply_int = 110, apply_string = 111,
        button_on = 112, button_off = 113, previousPage = 114, nextPage = 115, Scroll = 116,
        openUTD = 200, UTDutButtonPressed = 201, closeUTD = 204,
        openAniD = 202, addTex = 203, closeAniD = 205,// open animation DataBase, add Texture
        CreateNewMapFile = 1001, LoadMapFile = 1002,
    };

    /// <summary>
    /// Commandとそれと対応した整数、文字列データを格納するための物
    /// </summary>
    class CommandData
    {
        
        public readonly Command c;
        public readonly int[] ints;
        public readonly string[] strings;
        public CommandData(Command _c, int[] its = null, string[] strs = null)
        {
            c = _c;
            if (its != null) { ints = new int[its.Length]; for (int i = 0; i < its.Length; i++) ints[i] = its[i]; }
            if (strs != null) { strings = new string[strs.Length]; for (int j = 0; j < strs.Length; j++) strings[j] = strs[j]; }
        }
        public CommandData(Command _c, string[] strs) : this(_c, null, strs) { }
    }

    /// <summary>
    /// 不変なデータをまとめたクラス
    /// </summary>
    class DataBase : IDisposable
    {
        #region about Editor

        /// <summary>
        /// このDataBaseなどに使われている読み込み、Editorでのファイルの読み方法が何時の物かの判断に使われる。確実に大きく変化したら更新していくように。
        /// 日付になっている。年月日で, 9月は 09
        /// </summary>
        public static readonly int ThisSystemVersionNumber = 161010;
        public enum VersionNumber { SixTeenTenTen = 161010, };
        /// <summary>
        /// used between UnitType, etc.-- ut1;ut2
        /// </summary>
        public static char interval_of_each_type = ';';
        /// <summary>
        /// used between int and string , etc.--int ...ij,string kl...
        /// </summary>
        public static char interval_of_each_datatype = ',';
        /// <summary>
        /// used before every array , etc.--int ...ij&int k-z&intA-F;string GH&string I-P...
        /// </summary>
        public static char interval_of_array = '&';
        #endregion

        #region UTD
        public static string utFileName = "uts.dat";
        public static FileStream ut_file;
        public static readonly UnitTypeDataBase utDataBase;
        #endregion
        #region Textures
        public static readonly string texDFileName = "texNames.dat";

        /// <summary>
        /// 必ずTexturesDataDictionaryに読み込まれる画像.
        /// </summary>
        public const string defaultBlankTextureName = "None";

        /// <summary>
        /// string is its path, maybe from "Content".  and also string key contains a size of texture's single unit
        /// </summary>
        //keyを使って読み込みできるので、class化していない。そのままバイナリ-ファイルからkeyを読み取り、Content.Loadをする。
        public static Dictionary<string, Texture2Ddata> TexturesDataDictionary = new Dictionary<string, Texture2Ddata>();

        /// <summary>
        /// TexturesDataDictionaryにTexture2Ddataを追加するメッソド。
        /// </summary>
        private static void tda(string name)
        {

            /*Directory.SetCurrentDirectory(Directory.GetParent(Directory.GetCurrentDirectory()).FullName);
            Console.WriteLine(Directory.GetCurrentDirectory());
            Directory.SetCurrentDirectory(Content.RootDirectory);
            Console.WriteLine(Directory.GetCurrentDirectory());
            Console.WriteLine(File.Exists(name));*/
            try
            {
                Texture2D t = Content.Load<Texture2D>(name);
                TexturesDataDictionary.Add(name, new Texture2Ddata(t, name));
            }
            catch { Console.WriteLine("tda: load error" + name); return; }
        }
        /// <summary>
        /// TexturesDataDictionaryにTexture2Ddataを追加するメッソド。
        /// </summary>
        public static void tdaA(string name)
        {
            try
            {
                Texture2D t = Content.Load<Texture2D>(name);
                TexturesDataDictionary.Add(name, new Texture2Ddata(t, name));
            }
            catch { Console.WriteLine("tdaA: load error" + name); return; }
            /*
             * Directory.SetCurrentDirectory(DirectoryWhenGameStart);
            if (Directory.Exists("Content"))
            {
                Directory.SetCurrentDirectory("Content");
                if (File.Exists(name))
                {
                    Console.WriteLine("Found in " + Directory.GetCurrentDirectory());
                    TexturesDataDictionary.Add(name, new Texture2Ddata(Content.Load<Texture2D>(name), name));
                }
                else
                {
                    Console.WriteLine("Tex " + name + "is Null. Maybe Not Found directly in the Content?");
                }
            }else {
                Console.WriteLine("tdaA: CurrentDirectory -" + Directory.GetCurrentDirectory());
                TexturesDataDictionary.Add(name, new Texture2Ddata(Content.Load<Texture2D>(name), name));
            }//Contentを見つけていない場合
            */
        }

        #endregion
        #region Animation
        public static AnimationDataAdvanced defaultBlankAnimationData;
        public const string defaultAnimationNameAddOn = "-stand";
        public static string aniDFileName = "animationNames.dat";
        public static Dictionary<string, AnimationDataAdvanced> AnimationAdDataDictionary = new Dictionary<string, AnimationDataAdvanced>();

        /// <summary>
        /// TexturesDataDictionaryが構成できてからこれをcall/使用してください。
        /// </summary>
        private static void setup_Animation()
        {
            defaultBlankAnimationData = new AnimationDataAdvanced("defaultblank", 5, 1, defaultBlankTextureName);
            /*FileStream aniD_file = File.Open(aniDFileName, FileMode.OpenOrCreate);
            aniD_file.Position = 0;
            BinaryReader aniD_br = new BinaryReader(aniD_file);
            while (aniD_br.BaseStream.Position < aniD_br.BaseStream.Length)
            {
                try
                {
                    bool repeat = aniD_br.ReadBoolean();
                    int min_index = aniD_br.ReadInt32();
                    int max_index = aniD_br.ReadInt32();
                    int length = aniD_br.ReadInt32();
                    int[] frames = new int[length];
                    for (int i = 0; i < length; i++) { frames[i] = aniD_br.ReadInt32(); }
                    //ints end, strings start
                    string animeName = aniD_br.ReadString();
                    string textureName = aniD_br.ReadString();
                    string preName = aniD_br.ReadString();
                    string nexN = aniD_br.ReadString();
                    AnimationAdDataDictionary.Add(animeName, new AnimationDataAdvanced(animeName, frames, textureName, repeat));
                }
                catch (EndOfStreamException e) { break; }
            }
            aniD_br.Close(); aniD_file.Close();*/
           
        }
        private void save_Animation()
        {
            FileStream aniD_file = File.Open(aniDFileName, FileMode.Create);
            aniD_file.Position = 0;

            BinaryWriter aniD_bw = new BinaryWriter(aniD_file);
            foreach (AnimationDataAdvanced ad in AnimationAdDataDictionary.Values)
            {
                aniD_bw.Write(ad.repeat);
                foreach (int d in ad.getIntsData()) { aniD_bw.Write(d); }
                foreach (string str in ad.getStringsData()) { aniD_bw.Write(str); }
            }
            aniD_bw.Close(); aniD_file.Close();
        }

        public static AnimationDataAdvanced getAniD(string name, string addOn = null)
        {
            Console.WriteLine("DataBase:"+name + addOn);
            if (addOn == null && AnimationAdDataDictionary.ContainsKey(name))
            {
                return AnimationAdDataDictionary[name];

            }
            else if (AnimationAdDataDictionary.ContainsKey(name + addOn))
            {
                return AnimationAdDataDictionary[name + addOn];
            }

            if (AnimationAdDataDictionary.ContainsKey(name + defaultAnimationNameAddOn))
            {
                return AnimationAdDataDictionary[name + defaultAnimationNameAddOn];
            }
            else
            {
                return defaultBlankAnimationData;
            }
        }

        #endregion

        private static ContentManager Content;
        public static string DirectoryWhenGameStart;

        #region about Coloum
        public const string BlankDefaultContent = "ClickAndType";
        public const string ButtonDefaultContent = "Click";
        public const int InvaildColoumContentReply_int = -99999;
        public const string InvaildColoumContentReply_string = "fobagnufabo";
        #endregion

        /// <summary>
        /// string is its path, maybe from "Content".  and also string key contains a size of texture's single unit
        /// </summary>
        #region SkillData
        public static Dictionary<string, SkillData> SkillDatasDictionary = new Dictionary<string, SkillData>();
        public static void setupSkillData()
        {
            SkillDatasDictionary.Add("shot",new SingleShotSkillData("shot", 60, -1, 0.3, 0,5,0,1,50,10));
            SkillDatasDictionary.Add("circle", new SingleShotSkillData("circle", 60, 5, 0, Math.PI/10, 5, 0, 1, 50, 10));
            SkillDatasDictionary.Add("laser", new LaserTopData("laser", 1000, 5, 0, Math.PI/2, 8, 0, 1, 10, 10,0.005, Color.MediumVioletRed));

        }
        public static SkillData getSkillData(string skillName)
        {
            return SkillDatasDictionary[skillName];
        }
        #endregion
        #region GameScreen
        public static readonly int WindowDefaultSizeX = 1280;
        public static readonly int WindowDefaultSizeY = 960;
        public static readonly int WindowSlimSizeY = 720;

        #endregion
       
        /// <summary>
        /// Game1からのCotentを使って、DataBaseの内容を埋める
        /// </summary>
        /// <param name="content"></param>
        /// 
        public static void Load_Contents(ContentManager c)
        {
            Content = c;
            #region textures
            FileStream texD_file = File.Open(texDFileName, FileMode.OpenOrCreate, FileAccess.Read);
            texD_file.Position = 0;
            BinaryReader texD_br = new BinaryReader(texD_file);
            while (texD_br.BaseStream.Position < texD_br.BaseStream.Length)
            {
                try
                {
                    string n = texD_br.ReadString();
                    if (n != defaultBlankTextureName)
                    {
                        tda(n);
                    }
                }
                catch (EndOfStreamException e) { break; }
            }
            texD_br.Close(); texD_file.Close();
            tda(defaultBlankTextureName);
            #endregion
            #region animation
            setup_Animation();
            #endregion
            /*
                        tda("36-40 enmey1");
                        tda("36 40-enemy1");
                        tda("36 40-hex1");
                        tda("18 20-tama1");
                        tda("testbackground");
                        tda("16-16 tama1");
                        tda("leftside1");
                        tda("rightside1");
                        tda("60 105-player");
                        tda("background3");
                        tda("testlife");
                        tda("140 220-enemy1");
                        tda("16-16_tama2");
                        */
            tda("120 68-enemy2");
            tda("rightside2");
            tda("rightside3");
            tda("rightside4");
            tda("background1");
            tda("background2");
            tda("background4");
            tda("ougi");
            tda("720×174 sword");
            
            setupSkillData();

            AnimationAdDataDictionary.Add("boss1" + defaultAnimationNameAddOn, new AnimationDataAdvanced("boss1" + defaultAnimationNameAddOn,
                10, 12, 0, "36-40 enemy1", true));
            AnimationAdDataDictionary.Add("boss1atk", new AnimationDataAdvanced("boss1atk", new int[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 },
                "36-40 enemy1"));
            AnimationAdDataDictionary.Add("enemy2" + defaultAnimationNameAddOn, new AnimationDataAdvanced("enmey2" + defaultAnimationNameAddOn,
                 10, 4, 0, "120 68-enemy2", true));
        }

        #region Unload And Save
        public void Dispose()
        {
            Console.WriteLine("DisposeDataBase");
            Directory.SetCurrentDirectory(DirectoryWhenGameStart);
            if (!Directory.Exists("Datas")) { Directory.CreateDirectory("Datas"); }
            Directory.SetCurrentDirectory("Datas");
            Console.WriteLine(Directory.GetCurrentDirectory());
            ut_file.Close();
            #region texture
            FileStream texD_file = File.Open(texDFileName, FileMode.Create, FileAccess.Write);
            texD_file.Position = 0;

            BinaryWriter texD_bw = new BinaryWriter(texD_file);
            foreach (string s in TexturesDataDictionary.Keys)
            {
                texD_bw.Write(s);
            }
            texD_bw.Close(); texD_file.Close();
            #endregion
            #region anime
            save_Animation();
            #endregion
            AnimationAdDataDictionary.Clear();
            TexturesDataDictionary.Clear();

            Content = null;
        }
        #endregion
        #region singleton and setup
        public static DataBase database_singleton = new DataBase();
        //public DataBase get() { return database_singleton; }
        static DataBase()
        {
            DirectoryWhenGameStart = Directory.GetCurrentDirectory();

            if (Directory.GetCurrentDirectory() == "Datas") { }
            else if (!Directory.Exists("Datas")) { Directory.CreateDirectory("Datas"); }
            Directory.SetCurrentDirectory("Datas");
            Console.WriteLine(Directory.GetCurrentDirectory());
            ut_file = File.Open(utFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            BinaryReader ut_br = new BinaryReader(ut_file);
            utDataBase = new UnitTypeDataBase(ut_br);
            ut_br.Close();
        }
        private DataBase() { }
        #endregion

        #region Method
        public static Texture2D getTex(string name)
        {
            if (TexturesDataDictionary.ContainsKey(name))
            {
                return TexturesDataDictionary[name].texture;
            }
            else
            {
                Console.WriteLine("TexDictionary " + name + " is not added");
                return TexturesDataDictionary[defaultBlankTextureName].texture;
            }
        }
        public static Texture2Ddata getTexD(string name)
        {
            if (name == null || name == "" || !TexturesDataDictionary.ContainsKey(name)) { return TexturesDataDictionary[defaultBlankTextureName]; }
            else { return TexturesDataDictionary[name]; }
        }
        public static Rectangle getRectFromTextureNameAndIndex(string name, int id)
        {
            int w = TexturesDataDictionary[name].w_single;
            int h = TexturesDataDictionary[name].h_single;
            int x = id % TexturesDataDictionary[name].x_max * w;
            int y = id / TexturesDataDictionary[name].x_max * h;
            if (id >= TexturesDataDictionary[name].x_max * TexturesDataDictionary[name].y_max) { x = y = 0; }
            return new Rectangle(x, y, w, h);
        }
       
        public static UnitType getUnitType(string typename)
        {
            if (typename == null)
            {
                if (utDataBase.UnitTypeList.Count > 0) return utDataBase.UnitTypeList[0];
                else return utDataBase.CreateBlankUt();
            }
            else { return utDataBase.getUnitTypeWithName(typename); }
        }
        public static int getUTDcount() { return utDataBase.UnitTypeList.Count; }
        #endregion

    }// DataBase end

    class Texture2Ddata
    {
        /// <summary>
        /// Textureのファイル名を使って得た、画像の1コマの width, height
        /// </summary>
        public int w_single=0 , h_single=0;
        public int x_max , y_max;
        public Texture2D texture; public string texName;
        public Texture2Ddata(Texture2D tex, string name)
        {
            texture = tex;
            texName = name;
            int r = 0;//nameのstringとしての位置　変数。
            while (r < name.Length)
            {
                if (!char.IsNumber(name[r])) { r++; }
                else { break; }
            }//最初の数字のところまで行く。
            while (r < name.Length)
            {
                if (char.IsNumber(name[r])) { w_single = w_single * 10 + (int)name[r] - (int)'0'; r++; }
                else { r++; break; }
            }//widthを読む
            while (r < name.Length)
            {
                if (char.IsNumber(name[r])) { h_single = h_single * 10 + (int)name[r] - (int)'0'; r++; }
                else { break; }
            }//heightを読む
            if (w_single == 0) { w_single = texture.Width; }
            if (h_single == 0) { h_single = texture.Height; }
            x_max = texture.Width / w_single;
            y_max = texture.Height / h_single;
        }
        public Texture2D getTex()
        {
            return texture;
        }

    }
}// namespace end
