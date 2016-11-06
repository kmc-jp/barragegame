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
    public enum MoveType {non_target=0,point_target=1,object_target=2,go_straight,mugen,rightcircle,leftcircle,stop,
        chase_angle,screen_point_target };
    public enum Command
    {
        exit = -1000,
        left_and_go_back = -101, nothing = -100,
        apply_int = 110, apply_string = 111,
        button_on = 112, button_off = 113, previousPage = 114, nextPage = 115, Scroll = 116, tru_fals = 117,
        selectInScroll = 118, closeThis = 119, reloadScroll = 120, buttonPressed1 = 121, buttonPressed2 = 122,
        openUTD = 200, UTDutButtonPressed = 201,
        openAniD = 202, addTex = 203, playAnimation = 206, newAniD = 207, applyAniD = 208,// open animation DataBase, add Texture,play animation,
        openMusicGallery = 204, openMapEditor = 205,
        specialIntChange1 = 301, specialIntChange2 = 302,
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
        public static readonly string defaultBlankTextureName = "None";

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
            try
            {
                Texture2D t = Content.Load<Texture2D>(name);
                if (!TexturesDataDictionary.ContainsKey(name))
                {
                    TexturesDataDictionary.Add(name, new Texture2Ddata(t, name));
                }
                else
                {
                    Console.WriteLine("tda:Exist in Dictionary: " + name);
                }
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
                if (!TexturesDataDictionary.ContainsKey(name))
                {
                    TexturesDataDictionary.Add(name, new Texture2Ddata(t, name));
                    Console.WriteLine(name + " added.");
                }
                else
                {
                    Console.WriteLine("tdaA:Exist in Dictionary: " + name);
                }
            }
            catch { Console.WriteLine("tdaA: load error" + name); return; }
        }

        #endregion
        #region Animation
        public static AnimationDataAdvanced defaultBlankAnimationData;
        public const string defaultAnimationNameAddOn = "-stand";
        static string aniDFileName = "animationNames.dat";
        public static Dictionary<string, AnimationDataAdvanced> AnimationAdDataDictionary = new Dictionary<string, AnimationDataAdvanced>();

        /// <summary>
        /// TexturesDataDictionaryが構成できてからこれをcall/使用してください。
        /// </summary>
        private static void setup_Animation()
        {
            defaultBlankAnimationData = new AnimationDataAdvanced("defaultblank", 1000, 1, defaultBlankTextureName);
            FileStream aniD_file = File.Open(aniDFileName, FileMode.OpenOrCreate,FileAccess.Read);
            aniD_file.Position = 0;
            BinaryReader aniD_br = new BinaryReader(aniD_file);
            Console.WriteLine("in setup_Animation");
            Console.WriteLine(Directory.GetCurrentDirectory());
            Console.WriteLine("file exits?: "+File.Exists(aniDFileName));
            Console.WriteLine("Filelength:"+aniD_br.BaseStream.Length);
            while (aniD_br.BaseStream.Position < aniD_br.BaseStream.Length)
            {
                try
                {
                    bool repeat = aniD_br.ReadBoolean();
                    Console.WriteLine("repeat:"+repeat);
                    int min_index = aniD_br.ReadInt32();
                    Console.WriteLine("min:" + min_index.ToString());
                    int max_index = aniD_br.ReadInt32();
                    Console.WriteLine("max:" + max_index.ToString());
                    int length = aniD_br.ReadInt32();
                    Console.WriteLine("l:" + length.ToString());
                    int[] frames = new int[length];
                    for (int i = 0; i < length; i++) { frames[i] = aniD_br.ReadInt32(); Console.WriteLine(i+":" + frames[i].ToString()); }
                    //ints end, strings start
                    string animeName = aniD_br.ReadString();
                    Console.WriteLine("name:" + animeName);
                    string textureName = aniD_br.ReadString();
                    Console.WriteLine("texname:" + textureName);
                    string preName = aniD_br.ReadString();
                    Console.WriteLine("prename:" + preName);
                    string nexN = aniD_br.ReadString();
                    Console.WriteLine("nexname:" + nexN);
                    addAniD(new AnimationDataAdvanced(animeName, frames, min_index,textureName, repeat));
                    getAniD(aniDFileName).assignAnimationName(preName, false);
                    getAniD(aniDFileName).assignAnimationName(nexN, true);
                    Console.WriteLine(aniD_br.BaseStream.Position);
                }
                catch (EndOfStreamException e) { Console.WriteLine("setup animation: EndOfStream"); break; }
            }
            Console.WriteLine(aniD_br.BaseStream.Position);
            aniD_br.Close(); aniD_file.Close();

        }
        private void save_Animation()
        {
            FileStream aniD_file = File.Open(aniDFileName, FileMode.Create,FileAccess.Write);
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
        #endregion

        private static ContentManager Content;
        public static string DirectoryWhenGameStart;
        /// <summary>
        /// ゲーム初期化後にゲームが見ているDirectoryからDatasのフォルダを開くか作って開く
        /// </summary>
        public static void goToFolderDatas()
        {
            Directory.SetCurrentDirectory(DirectoryWhenGameStart);
            if (!Directory.GetCurrentDirectory().EndsWith("Datas"))
            {
                if (!Directory.Exists("Datas")) { Directory.CreateDirectory("Datas"); }
                Directory.SetCurrentDirectory("Datas");
            }
        }
        public static void goToStartDirectory()
        {
            Directory.SetCurrentDirectory(DirectoryWhenGameStart);
        }
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
        private const int low_speed=2;
        private const int middle_speed=4;
        private const int high_speed=7;
        private const int big_radius=10;
        private const int small_radius=5;
        private const int high_cd1 = 5; private const int high_cd2 = 8; private const int high_cd3 = 20; private const int high_cd4 = 30;
        private const int middle_cd1 = 45; private const int middle_cd2 = 60;
        private const int low_cd1 = 90; private const int low_cd2 = 100; private const int low_cd3 = 120;
        private const double highangle1 = Math.PI / 10;
        private const double middleangle1 = Math.PI / 6; private const double middleangle2= Math.PI / 5;
        private const double lowangle1 = Math.PI / 2;

        
        public static Dictionary<string, SkillData> SkillDatasDictionary = new Dictionary<string, SkillData>();
        public static void setupSkillData()
        {
            addSkillData(new SingleShotSkillData("shot",SkillGenreS.shot,MoveType.go_straight,"bullet1", middle_cd1, -1, 0.3, 0,5,0,1,50,10));
            addSkillData(new SingleShotSkillData("16circle-0",SkillGenreS.circle,MoveType.go_straight,"bullet1", middle_cd2, 5, 0, highangle1, 5, 0, 1, 50, 10));
            addSkillData(new LaserTopData("laser",MoveType.chase_angle,"bullet1", 100000, 5, 0, lowangle1, high_cd2, 0.008, Color.MediumVioletRed));
            addSkillData(new GenerateUnitSkillData("createbullet",SkillGenreS.shot,MoveType.go_straight,"bullet1", low_cd3, 2, 0, -Math.PI/2, 8,"yanagi"));
            addSkillData(new SingleShotSkillData("yanagi",SkillGenreS.yanagi ,MoveType.go_straight,"bullet1", low_cd1, 2, 0.2, 8, 0.25));
            addSkillData(new WayShotSkillData("5wayshot", SkillGenreS.wayshot,MoveType.go_straight,"bullet1", high_cd3, 6, 0, highangle1, 8, 5));
            addSkillData(new WayShotSkillData("3wayshot-0", SkillGenreS.wayshot,MoveType.go_straight,"bullet1", middle_cd1,middle_speed, 0, middleangle2, small_radius, 3));
            addSkillData(new WayShotSkillData("3wayshot-1", SkillGenreS.wayshot, MoveType.go_straight, "bullet1", middle_cd2, middle_speed, 0, middleangle2, small_radius, 3));
            addSkillData(new SingleShotSkillData("16circle-0", SkillGenreS.circle, MoveType.go_straight, "bullet1", low_cd2, low_speed, 0, highangle1, small_radius));
            addSkillData(new SingleShotSkillData("downshot-0", SkillGenreS.circle,MoveType.go_straight, "bullet1", middle_cd1, middle_speed, 0, lowangle1, small_radius));
            addSkillData(new SingleShotSkillData("1wayshot-0",SkillGenreS.shot,MoveType.go_straight,"bullet1", middle_cd2, middle_speed, 0, 0,small_radius));
            addSkillData(new WayShotSkillData("4wayshot-0", SkillGenreS.wayshot, MoveType.go_straight, "bullet1", middle_cd1, low_speed, 0, middleangle1, big_radius, 4));
            addSkillData(new WayShotSkillData("4wayshot-1", SkillGenreS.wayshot, MoveType.go_straight, "bullet1", low_cd1, low_speed, 0, middleangle1, big_radius, 4));
            addSkillData(new WayShotSkillData("4wayshot-2", SkillGenreS.wayshot, MoveType.go_straight, "bullet1", high_cd3, middle_speed, 0, middleangle1, small_radius, 4));
            addSkillData(new LaserTopData("laser-0", MoveType.go_straight, "bullet1", low_cd3, high_speed, 0, lowangle1, small_radius, 0, Color.Maroon));
            addSkillData(new LaserTopData("laser-1", MoveType.chase_angle, "bullet1", low_cd3, high_speed, 0, lowangle1, small_radius, 0.005, Color.MediumVioletRed));
            addSkillData(new SingleShotSkillData("zyuzi-0",SkillGenreS.zyuzi ,MoveType.go_straight,"bullet1", middle_cd2, low_speed, 0, 0,small_radius));

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
            goToFolderDatas();
            Console.WriteLine(Directory.GetCurrentDirectory());
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
            goToStartDirectory();
            /*
                        tda("36-40 enmey1");
                        tda("36 40-enemy1");
                        tda("36 40-hex1");
                        tda("18 20-tama1");
                        tda("16-16 tama1");
                        tda("leftside1");
                        tda("60 105-player");
                        tda("33x60バッテリーアイコン");
                        tda("16-16_tama2");
            tda("120 68-enemy2");
            tda("stageselect");
            tda("400x50タイトル画面文字");
            tda("rightside1");
            tda("rightside2");
            tda("rightside3");
            tda("rightside4");
            tda("background1");
            tda("background2");
            tda("background3");
            tda("background4");
            tda("ougi");
            tda("720×174 sword");
            tda("90 98-boss1_body0");
            tda("90 78-boss1_body1");
            tda("90 77-boss1_body2");
            tda("90 270-boss1");
            
            tda("Boss1_tail");
            tda("120×68 E1-1");
            
            tda("タイトル画面NF");
            */
            setupSkillData();
            /*
            addAniD( new AnimationDataAdvanced("boss1" + defaultAnimationNameAddOn,
                10, 3, "90 270-boss1", true));
            addAniD(new AnimationDataAdvanced("boss1atk", new int[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 },
                "36-40 enemy1",true));
            addAniD(new AnimationDataAdvanced("boss1body0" + defaultAnimationNameAddOn,
                10, 3, "90 98-boss1_body0", true));
            addAniD( new AnimationDataAdvanced("boss1body1" + defaultAnimationNameAddOn,
                10, 3, "90 78-boss1_body1", true));
            addAniD(new AnimationDataAdvanced("boss1body2" + defaultAnimationNameAddOn,
                10, 3, "90 77-boss1_body2", true));
            addAniD( new AnimationDataAdvanced("boss1tail" + defaultAnimationNameAddOn,
                10, 1, "Boss1_tail", true));
            //
            addAniD( new AnimationDataAdvanced("enmey2" + defaultAnimationNameAddOn,
                 10, 4, 0, "120 68-enemy2", true));
            addAniD(new AnimationDataAdvanced("bullet1" + defaultAnimationNameAddOn,
                10, 3, "16-16 tama1",true));
            addAniD(new AnimationDataAdvanced("heal1" + defaultAnimationNameAddOn,
                10, 3, "16-16_tama2",true));
            addAniD( new AnimationDataAdvanced("swordgauge" + defaultAnimationNameAddOn,
                10, 1,1, "720×174 sword",true));
            addAniD(new AnimationDataAdvanced("swordgauge" + "high",
                10, 4, 2, "720×174 sword",true));
            addAniD( new AnimationDataAdvanced("swordgauge" + "max",
                10,10, 6, "720×174 sword",true));
            //
            
            addAniD(new AnimationDataAdvanced("E1-1" + defaultAnimationNameAddOn,
                10, 4, 0, "120×68 E1-1", true));
            */
            
        }


        #region Unload And Save
        public void Dispose()
        {
            Console.WriteLine("DisposeDataBase");
            goToFolderDatas();
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

            if (!Directory.GetCurrentDirectory().EndsWith("Datas"))
            {
                if (!Directory.Exists("Datas")) { Directory.CreateDirectory("Datas"); }
                Directory.SetCurrentDirectory("Datas");
            }
            Console.WriteLine(Directory.GetCurrentDirectory());
            ut_file = File.Open(utFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            BinaryReader ut_br = new BinaryReader(ut_file);
            utDataBase = new UnitTypeDataBase(ut_br);
            ut_br.Close();
            goToStartDirectory();
        }
        private DataBase() { }
        #endregion

        #region Method
        /// <summary>
        /// ファイルパスからファイル名を取得する
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string getFileNameFromFilePath(string filePath, char da = '/', char db = '.')
        {
            if (filePath == null) { Console.WriteLine("getFileName: filePath is null."); return filePath; }
            int ia = 0, ib = 0;
            for (int r = 0; r < filePath.Length; r++)
            {
                if (filePath[r] == da)
                {
                    ia = r;
                    Console.WriteLine("Found / " + r.ToString() + " ");
                }
                else if (filePath[r] == db)
                {
                    ib = r;
                    Console.WriteLine("Found . " + r.ToString() + " ");
                }
            }
            ia++;
            if (ia <= 1) { Console.WriteLine("getFileName:" + filePath + " ia not Found?or the First. da is " + da); }
            if (ib <= ia) { Console.WriteLine("getFileName:" + filePath + " ib<=ia; da is " + da + " db is " + db); }
            if (ib == 0) { Console.WriteLine("getFileName:" + filePath + " not Found " + db); ib = ia + 1; }
            if (ib >= filePath.Length) { Console.WriteLine("getFileName:" + filePath + " ib>=length (ia is the last?)"); }
            return filePath.Substring(ia, ib - ia);
        }
        public static void addSkillData(SkillData skdata)
        {
            if (existSkillDataName(skdata.skillName))
            {
                Console.WriteLine("addSkillData:" + skdata.skillName + " already exists");
            }
            else
            {
                SkillDatasDictionary.Add(skdata.skillName, skdata);
            }
        }
        public static bool existSkillDataName(string skillName)
        {
            return SkillDatasDictionary.ContainsKey(skillName);
        }
        public static SkillData getSkillData(string skillName)
        {
            if (SkillDatasDictionary.ContainsKey(skillName))
            {
                return SkillDatasDictionary[skillName];
            }
            else
            {
                Console.WriteLine("getSkillData: " + skillName + " does not exist in Dictionary.");
                return null;
            }
        }
        public static void addAniD(AnimationDataAdvanced ad)
        {
            Console.WriteLine(ad.animationDataName);
            if (AnimationAdDataDictionary.ContainsKey(ad.animationDataName))
            {
                Console.WriteLine("addAniD: " + ad.animationDataName + " exists.");
            }
            else
            {
                AnimationAdDataDictionary.Add(ad.animationDataName, ad);
            }
        }
        public static void RemoveAniD(string name, string addOn)
        {
            if (addOn == null)
            {
                AnimationAdDataDictionary.Remove(name);
            }
            else
            {
                AnimationAdDataDictionary.Remove(name + addOn);
            }
        }
        public static bool existsAniD(string name, string addOn)
        {
            if (addOn == null) return AnimationAdDataDictionary.ContainsKey(name);
            else return AnimationAdDataDictionary.ContainsKey(name + addOn);
        }
        public static AnimationDataAdvanced getAniD(string name, string addOn = null)
        {
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
        public static Texture2D getTex(string name)
        {
            if (TexturesDataDictionary.ContainsKey(name))
            {
                return TexturesDataDictionary[name].texture;
            }
            else
            {
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
            if (w_single <= 2) { w_single = texture.Width; }
            if (h_single <= 2) { h_single = texture.Height; }
            x_max = texture.Width / w_single;
            y_max = texture.Height / h_single;
            if (x_max == 0) { Console.WriteLine("Texture2Ddata: x_max=0 Error! : "+texName); }
            if (y_max == 0) { Console.WriteLine("Texture2Ddata: y_max=0 Error! : " + texName); }
        }
        public Texture2D getTex()
        {
            return texture;
        }

    }
}// namespace end
