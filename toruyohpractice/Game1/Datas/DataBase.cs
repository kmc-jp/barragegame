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

    /// <summary>
    /// projectionなどに使われる exist_timesの各インデックスが代表するもの
    /// </summary>
    public enum existTimesIndex{    InvisibleStill = 0, InvisibleActive, VisibleStill, VisibleActive    }
    public enum Unit_state { fadeout=0,dead,out_of_window,bulletDamagedPlayer,exist_timeOut };
    public enum MoveType {noMotion=0,
        screen_point_target = 1,//その点に近づこうとする、正の時間が指定されていると動き始めるとき速度が[距離/時間]変わります。
        player_target =2,//速度のみを使った追いかけ,方向転換は一瞬
        go_straight,//これは実際は方向を決めて、その向きに突っ走るだけです。
        mugen,rightcircle,leftcircle,stop, //周期が必要

        chase_player_target,//omega角速度使用の追いかけ
        rotateAndGo,//角速度で回転しながら今自分の向きに進む
        chase_enemy_target//発射したenemyについていく skilledbullet用
    };
    /// <summary>
    /// MoveTypeを持ち、なんらかのposをも持っている時、そのposの意味
    /// </summary>
    public enum PointType { notused = -1, //使われていない
        displacement,//全部の変位、1 fpsでの変位ではない
        pos_on_screen,//画面上の座標を示す
        player_pos, //プレイヤーの座標を指す。
        randomRange,//ベクトルがx方向の正負変位,yの正負変位を表しているが、値はその変位内の乱数
        randomDirection,//ベクトルは意味を持たない?初期角度にランダム角度足した方向へ移動する
        Direction,//決まった方向
    }
    public enum Command
    {
        exit = -1000,
        left_and_go_back = -101, nothing = -100,
        apply_int = 110, apply_string = 111,
        button_on = 112, button_off = 113, previousPage = 114, nextPage = 115, Scroll = 116, tru_fals = 117,
        selectInScroll = 118, closeThis = 119, reloadScroll = 120, buttonPressed1 = 121, buttonPressed2 = 122, buttonPressed3 = 123,
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

        #region about player character / boss character / map stage / enemy motion 
        public const string halfBlackTextureName = "1280x2000背景用グレー画像";

        public const int basicFramePerSecond = 60; 
        public static string charaName = "chara1";
        public static string charaCutInTexName = "カットインfin";
        public const string bossLifeBar_default_aniName = "1280x150体力ゲージ";
        /// <summary>
        /// すべてのstop_timeとかに使われる。普通0より小さくならないtimeがこの値だと無限と認識する。Conditionでは99999と認識する
        /// </summary>
        public const int motion_inftyTime =-99999;
        /// <summary>
        /// この角度はその時点でのプレイヤーへの向きを意味する。
        /// </summary>
        public const double AngleToPlayer = -666;
        /// <summary>
        /// この角度はその時点での自分の向きを意味する。
        /// </summary>
        public const double SelfAngle = -888;
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
        public const string aniNameAddOn_spell = "-spell", aniNameAddOn_spellOff = "-spell off",
            aniNameAddOn_evadeL= "-evadeL", aniNameAddOn_evadeR = "-evadeR",  aniNameAddOn_alterOn="-alterOn",aniNameAddOn_alterOff="-alterOff";
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
                    //Console.WriteLine("repeat:"+repeat);
                    int min_index = aniD_br.ReadInt32();
                    //Console.WriteLine("min:" + min_index.ToString());
                    int max_index = aniD_br.ReadInt32();
                    //Console.WriteLine("max:" + max_index.ToString());
                    int length = aniD_br.ReadInt32();
                    //Console.WriteLine("l:" + length.ToString());
                    int[] frames = new int[length];
                    for (int i = 0; i < length; i++) { frames[i] = aniD_br.ReadInt32();
                        //Console.WriteLine(i+":" + frames[i].ToString());
                    }
                    //ints end, strings start
                    string animeName = aniD_br.ReadString();
                    //Console.WriteLine("name:" + animeName);
                    string textureName = aniD_br.ReadString();
                    //Console.WriteLine("texname:" + textureName);
                    string preName = aniD_br.ReadString();
                    //Console.WriteLine("prename:" + preName);
                    string nexN = aniD_br.ReadString();
                    //Console.WriteLine("nexname:" + nexN);
                    addAniD(new AnimationDataAdvanced(animeName, frames, min_index,textureName, repeat));
                    if(preName!=AnimationDataAdvanced.notAnimationName || nexN != AnimationDataAdvanced.notAnimationName)
                    {
                        //Console.Write(preName+" "+nexN+" ; ");
                    }
                    getAniD(animeName).assignAnimationName(preName, false);
                    getAniD(animeName).assignAnimationName(nexN, true);
                    //Console.Write(aniD_br.BaseStream.Position+" ");
                }
                catch (EndOfStreamException e) { Console.WriteLine("setup animation: EndOfStream"); break; }
            }
            Console.WriteLine(aniD_br.BaseStream.Position);
            //Console.WriteLine("AnimationDataAdvanced setup finished.");
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
                foreach (string str in ad.getStringsData()) {
                    //if (str != AnimationDataAdvanced.notAnimationName) { Console.Write(str); }
                    aniD_bw.Write(str);
                }
            }
            aniD_bw.Close();
            aniD_file.Close();
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

        #region SkillData
        private const double low_speed1=1.5; private const double low_speed2 = 2.5;
        private const double middle_speed1=4; private const double middle_speed2 = 5.5;
        private const double high_speed1=7.5; private const double high_speed2=9;
        private const double big_radius=10;
        private const double small_radius=5;
        private const int high_cd1 = 5; private const int high_cd2 = 8; private const int high_cd3 = 20; private const int high_cd4 = 30;
        private const int middle_cd1 = 45; private const int middle_cd2 = 60;
        private const int low_cd1 = 90; private const int low_cd2 = 100; private const int low_cd3 = 120; private const int low_cd6 = 240;
        private const double highangle0 = Math.PI / 8; private const double highangle1 = Math.PI / 10; private const double highangle2 = Math.PI / 16; private const double highangle3 = Math.PI / 32;
        private const double middleangle1 = Math.PI / 6; private const double middleangle2= Math.PI / 5; private const double middleangle4 = Math.PI / 4;
        private const double lowangle1 = Math.PI / 2; private const double lowangle2 = Math.PI / 4;

        
        public static Dictionary<string, SkillData> SkillDatasDictionary = new Dictionary<string, SkillData>();
        public static void setupSkillData()
        {
            Vector nv = new Vector(); double tPI = 2*Math.PI;
            Motion goStraightToPlayer = new Motion(MoveType.go_straight,PointType.player_pos,nv,middle_speed1,0,0);
            Motion goStraightWithDirection = new Motion(MoveType.go_straight, PointType.Direction, nv, low_speed1, 0,0);
            Motion goStraightWithDownDirection = new Motion(MoveType.go_straight, PointType.Direction, nv, high_speed1, 0,0);

            Motion rCircle = new Motion(MoveType.rightcircle, PointType.notused, new Vector(), low_speed1, 0, 60, Math.PI / 30);
            Motion rotateAndGoDirection = new Motion(MoveType.rotateAndGo, PointType.Direction, nv, low_speed1, 0, 0);

            Motion rotateAndGoRandom = new Motion(MoveType.rotateAndGo, PointType.randomDirection, new Vector(tPI/4,0), low_speed1, 0, 0);
            Motion fire = new Motion(MoveType.rotateAndGo, PointType.randomDirection, new Vector(tPI / 24, 0), low_speed1, 0, 0);
            Motion chaseEnemy = new Motion(MoveType.chase_enemy_target, PointType.notused, nv, 0, 0, 0);
            //addSkillData(new WaySkilledBulletsData("createbullet",null,SkillGenreS.wayshot,null,low_cd3,goStraightToPlayer,small_radius,"yanagi-s",1,60));
            const string bulletTimeOut = Condition.hP + "<0";
            const string hppBelowFifty = Condition.hPp + "<=50";
            const string hppOverFifty = Condition.hPp + ">50";
            addSkillData(new WaySkilledBulletsData("createbullet",null, SkillGenreS.wayshot, "bulletsmall", low_cd3, goStraightWithDirection,small_radius,"cs",1,10));
            addSkillData(new WaySkilledBulletsData("createbullet2way", null, SkillGenreS.wayshot, "bulletsmall", low_cd3, goStraightWithDirection, small_radius, "1wayshot-4", 2, 70,Math.PI));//this is basic of 2way.
            addSkillData(new WaySkilledBulletsData("createbullet2way-2", null, SkillGenreS.wayshot, "bulletsmall", low_cd3, goStraightWithDirection, small_radius, new string[] { "1wayshot-4", "7way*3shot" , "32-circle-random" }, 2, 30, Math.PI));

            addSkillData(new WaySkilledBulletsData("createbullet", null, SkillGenreS.wayshot, "bulletsmall", low_cd3, goStraightWithDirection, small_radius, "cs", 1, 10));
            addSkillData(new WaySkilledBulletsData("createbullet2way", null, SkillGenreS.wayshot, "bulletsmall", low_cd3, goStraightWithDirection, small_radius, "1wayshot-4", 2, 70, Math.PI));//this is basic of 2way.
            addSkillData(new WaySkilledBulletsData("7way*3shot*2", null, SkillGenreS.wayshot, "bulletsmall", low_cd3, goStraightWithDirection, small_radius, "7way*3shot", 2, 80, Math.PI));
            addSkillData(new WayShotSkillData("yanagi-s", null, SkillGenreS.yanagi ,MoveType.go_straight,"bulletsmall",15,middle_speed1, 0.2,lowangle1,small_radius,4,motion_inftyTime,1));
            addSkillData(new WaySkilledBulletsData("5way*3shot", null, SkillGenreS.wayshot, "bulletsmall", low_cd3, goStraightWithDirection,0,0,AngleToPlayer,0,0, big_radius,new string[] { "5wayshot-test" }, 1, high_cd2*3+1));
            addSkillData(new WaySkilledBulletsData("5way*5shot", null, SkillGenreS.wayshot, "bulletsmall", low_cd3, goStraightWithDirection, 0, 0, AngleToPlayer, 0, 0, big_radius, new string[] { "5wayshot-test" }, 1, high_cd1 * 5 + 1));
            addSkillData(new WaySkilledBulletsData("5way*10shot-star", null, SkillGenreS.wayshot, "bulletsmall", low_cd3, goStraightWithDirection, middle_speed2, 0, AngleToPlayer, 0, 0, big_radius, new string[] { "5wayshot-test-1" }, 1, high_cd2 * 10 + 1));
            addSkillData(new WayShotSkillData("5wayshot-test", null ,SkillGenreS.wayshot, "bulletsmall", high_cd1, goStraightWithDirection, middle_speed2,0.01,SelfAngle,0,0,small_radius,5 , middleangle1));
            addSkillData(new WayShotSkillData("5wayshot-test-1", null, SkillGenreS.wayshot, "bulletsmall", high_cd2, goStraightWithDirection, small_radius, 5, middleangle1));
            addSkillData(new WayShotSkillData("4wayshot-test", null, SkillGenreS.wayshot, "bulletsmall", low_cd6, MoveType.rotateAndGo,PointType.player_pos,nv,0,low_speed1,0.1,SelfAngle,tPI/100, small_radius, 4, lowangle1));
            addSkillData(new WaySkilledBulletsData("32-circle*8shot", null, SkillGenreS.wayshot, null, low_cd3, rotateAndGoRandom, 0, 0, AngleToPlayer, highangle3, 0, big_radius, new string[] { "32-circle-random" }, 1, high_cd2 * 4 + 1));
            addSkillData(new WaySkilledBulletsData("20-circle*5shot", null, SkillGenreS.wayshot, null, low_cd3, goStraightWithDirection, 0, 0, AngleToPlayer, 0, 0, big_radius, new string[] { "20circle-0" }, 1, high_cd2 * 3 + 1));
            addSkillData(new WaySkilledBulletsData("16-circle*5shot", null, SkillGenreS.wayshot, null, low_cd3, goStraightWithDirection, 0, 0, AngleToPlayer, highangle3, 0, big_radius, new string[] { "16circle-0" }, 1, high_cd2 * 3 + 1));
            addSkillData(new WayShotSkillData("32circle-0", null, SkillGenreS.wayshot, MoveType.go_straight, "bulletsmall", high_cd1, middle_speed1, 0, highangle2, small_radius, (int)(tPI / highangle2), highangle2));
            addSkillData(new WayShotSkillData("16circle-0", null, SkillGenreS.wayshot, "bulletsmall", high_cd1, goStraightWithDirection, middle_speed1, 0, highangle0, highangle3, 0, small_radius, (int)(tPI / highangle0), highangle0));
            addSkillData(new WayShotSkillData("16circle-1", null, SkillGenreS.wayshot, "bulletsmall", 7, goStraightWithDirection, low_speed2, 0.01, SelfAngle, highangle3, 0, small_radius, (int)(tPI / highangle0), highangle0));
            addSkillData(new WayShotSkillData("8circle-0", null, SkillGenreS.wayshot, "bulletsmall", high_cd1, goStraightWithDirection, middle_speed1, 0, SelfAngle, middleangle4, 0, small_radius, (int)(tPI / middleangle4), middleangle4));
            addSkillData(new WayShotSkillData("9circle-0", bulletTimeOut, SkillGenreS.wayshot, "bulletsmall", low_cd1, goStraightWithDirection, middle_speed2, 0, AngleToPlayer, 0, 0, small_radius, 9,middleangle4));
            addSkillData(new WayShotSkillData("1wayshot-3", null, SkillGenreS.wayshot, MoveType.go_straight, "bulletsmall", middle_cd2, low_speed1, 0, 0, small_radius, 1));
            addSkillData(new WaySkilledBulletsData("ransya-0", null, SkillGenreS.wayshot, "bulletlarge", 100,rotateAndGoDirection,0,0,lowangle2, highangle3, 0, small_radius,new string[] { "16circle-1" }, 1, 25));
            addSkillData(new WayShotSkillData("Directshot", null, SkillGenreS.wayshot, MoveType.go_straight, "bulletsmall", high_cd2, middle_speed1, 0, 0, small_radius, 1,SelfAngle));
            addSkillData(new WaySkilledBulletsData("ransya-1", null, SkillGenreS.wayshot, "bulletlarge", 100, rotateAndGoRandom, middle_speed1, 0, lowangle2, highangle3, 0, small_radius, new string[] { "hakkyou-1" }, 1, 100));
            addSkillData(new WaySkilledBulletsData("createbullet3way", null, SkillGenreS.wayshot, "bulletLL", low_cd2, goStraightWithDirection, middle_speed1,0,lowangle1,0,0,small_radius, new string[] { "9circle-0" }, 3, 45,lowangle2));//this is basic of 2way.

            addSkillData(new WaySkilledBulletsData("1shotfrom2point", null, SkillGenreS.wayshot, null, middle_cd2, goStraightWithDirection, small_radius, "1wayshot-5", 2, 20, Math.PI));//this is basic of 2way.
            addSkillData(new WaySkilledBulletsData("laserfrom2point", null, SkillGenreS.wayshot, "bulletLL", middle_cd2, goStraightWithDirection, small_radius, "laser-way1", 2, 30, Math.PI));//this is basic of 2way.
            addSkillData(new WaySkilledBulletsData("1downshotfrom2point", null, SkillGenreS.wayshot, null, high_cd4, goStraightWithDirection, small_radius, "1wayshot-6", 2, 20, Math.PI));//this is basic of 2way.
            addSkillData(new WayShotSkillData("1wayshot-5", bulletTimeOut, SkillGenreS.wayshot, MoveType.go_straight, "bulletrice", low_cd2, high_speed1, 0, 0, small_radius, 1));
            addSkillData(new WayShotSkillData("1wayshot-6", bulletTimeOut, SkillGenreS.wayshot, "bulletrice", low_cd2, goStraightWithDirection,low_speed2, 0,lowangle1, 0,0,small_radius, 1));
            addSkillData(new WayShotSkillData("yanagi-0", null, SkillGenreS.yanagi, MoveType.go_straight, "bulletsmall", low_cd2, middle_speed1, 0.2, lowangle1, small_radius, 8, motion_inftyTime, 1));

            addSkillData(new WaySkilledBulletsData("ransya-3", null, SkillGenreS.wayshot, null, 100, rotateAndGoDirection, 0, 0, lowangle2, highangle3, 0, small_radius, new string[] { "16circle-1" }, 1, 25));
            addSkillData(new WaySkilledBulletsData("ransya-4", null, SkillGenreS.wayshot, null, 100, rotateAndGoDirection, 0, 0, lowangle2, Math.PI/180, 0, small_radius, new string[] { "8circle-0" }, 1, 25));
            addSkillData(new WaySkilledBulletsData("ransya-2", null, SkillGenreS.wayshot, "bulletlarge", 100, rotateAndGoDirection, middle_speed1, 0, lowangle2, highangle3, 0, small_radius, new string[] { "16circle-0" }, 1, 100));
            addSkillData(new WayShotSkillData("laser-way1", bulletTimeOut, SkillGenreS.laser, "bulletsmall", 180, MoveType.go_straight, PointType.player_pos, new Vector(), 0, high_speed1, 0.03, lowangle2, 0, small_radius*10, Color.Aquamarine, 1, 0, 120));
            addSkillData(new WaySkilledBulletsData("ransya-3^-1", null, SkillGenreS.wayshot, null, 100, rotateAndGoDirection, 0, 0, lowangle2, -highangle3, 0, small_radius, new string[] { "16circle-1" }, 1, 25));

            #region boss3
            addSkillData(new WaySkilledBulletsData("boss3onfire-shot", Condition.hPp + ">=50", SkillGenreS.wayshot, null, middle_cd2, goStraightWithDirection, 0, 0, SelfAngle, 0, 0, big_radius, new string[] { "firesmall", "firelarge", "firemiddle" }, 1, high_cd1 * 5 + 1));
            addSkillData(new WaySkilledBulletsData("boss3onfire-bless", Condition.hPp + ">=50", SkillGenreS.wayshot, null, low_cd3, goStraightWithDirection, 0, 0, SelfAngle, 0, 0, big_radius, new string[] { "firesmall", "firelarge", "firemiddle" }, 1, 100));
            addSkillData(new WaySkilledBulletsData("boss3onfire-longbless", Condition.hPp + ">50", SkillGenreS.wayshot, null, 10000, goStraightWithDirection, 0, 0, SelfAngle, 0, 0, big_radius, new string[] { "firesmall2", "firelarge2", "firemiddle2" }, 1, 10000));
            addSkillData(new WaySkilledBulletsData("boss3onfire-hakkyo", Condition.hPp+"<=20", SkillGenreS.wayshot, null, 60, rotateAndGoDirection, 0, 0, lowangle2, Math.PI / 360, 0, small_radius, new string[] { "boss3onfire-rotate" }, 1, 1000));
            addSkillData(new WaySkilledBulletsData("boss3onfire-r", Condition.hPp + ">20", SkillGenreS.wayshot, null, 1000, rotateAndGoDirection, 0, 0, lowangle2, Math.PI / 45, 0, small_radius, new string[] { "boss3onfire-rotate2" }, 1, 1000));
            addSkillData(new WaySkilledBulletsData("boss3onfire-r^-1", Condition.hPp + ">20", SkillGenreS.wayshot, null, 1000, rotateAndGoDirection, 0, 0, lowangle2, -Math.PI / 45, 0, small_radius, new string[] { "boss3onfire-rotate2" }, 1, 1000));//上の弾幕と同時使用
            addSkillData(new WaySkilledBulletsData("boss3onfire-yanagi", Condition.hPp + "<50", SkillGenreS.wayshot, null, middle_cd2, goStraightWithDirection, 0, 0, AngleToPlayer, 0, 0, big_radius, new string[] { "boss3onfire-yanagi0", "ransya-3-boss3" }, 1, high_cd2 * 3 + 1));

            #region forboss3
            addSkillData(new WayShotSkillData("firesmall", null, SkillGenreS.wayshot, "smallonfire", high_cd1, fire, high_speed1, 0, AngleToPlayer, 0, 0, small_radius, 1, middleangle1));
            addSkillData(new WayShotSkillData("firemiddle", null, SkillGenreS.wayshot, "middleonfire", high_cd1, fire, high_speed1, 0, AngleToPlayer, 0, 0, small_radius, 1, middleangle1));
            addSkillData(new WayShotSkillData("firelarge", null, SkillGenreS.wayshot, "largeonfire", high_cd1, fire, high_speed1, 0, AngleToPlayer, 0, 0, small_radius, 1, middleangle1));
            addSkillData(new WayShotSkillData("firesmall1", null, SkillGenreS.wayshot, "smallonfire", high_cd1, fire, high_speed1, 0, SelfAngle, 0, 0, small_radius, 1, middleangle1));
            addSkillData(new WayShotSkillData("firemiddle1", null, SkillGenreS.wayshot, "middleonfire", high_cd1, fire, high_speed1, 0, SelfAngle, 0, 0, small_radius, 1, middleangle1));
            addSkillData(new WayShotSkillData("firelarge1", null, SkillGenreS.wayshot, "largeonfire", high_cd1, fire, high_speed1, 0, SelfAngle, 0, 0, small_radius, 1, middleangle1));
            addSkillData(new WayShotSkillData("firesmall2", null, SkillGenreS.wayshot, "smallonfire", high_cd1, fire, middle_speed1, 0, AngleToPlayer, 0, 0, small_radius, 1, middleangle1));
            addSkillData(new WayShotSkillData("firemiddle2", null, SkillGenreS.wayshot, "middleonfire", high_cd1, fire, middle_speed1, 0, AngleToPlayer, 0, 0, small_radius, 1, middleangle1));
            addSkillData(new WayShotSkillData("firelarge2", null, SkillGenreS.wayshot, "largeonfire", high_cd1, fire, middle_speed1, 0, AngleToPlayer, 0, 0, small_radius, 1, middleangle1));
            addSkillData(new WayShotSkillData("boss3onfire-yanagi0", null, SkillGenreS.yanagi, MoveType.go_straight, "middleonfire", low_cd2, middle_speed1, 0.2, 8, small_radius, 6, motion_inftyTime, 1));
            addSkillData(new WaySkilledBulletsData("boss3onfire-rotate", null, SkillGenreS.wayshot, null, middle_cd1, goStraightWithDirection, 0, 0, SelfAngle, 0, 0, big_radius, new string[] { "firesmall1", "firelarge1", "firemiddle1" }, 1, high_cd1 * 1 + 1));
            addSkillData(new WaySkilledBulletsData("boss3onfire-rotate2", null, SkillGenreS.wayshot, null, high_cd3, goStraightWithDirection, 0, 0, SelfAngle, 0, 0, big_radius, new string[] { "firesmall1", "firelarge1", "firemiddle1" }, 1, high_cd1 * 1 + 1));
            addSkillData(new WaySkilledBulletsData("ransya-3-boss3", null, SkillGenreS.wayshot, null, 100, rotateAndGoDirection, 0, 0, lowangle2, highangle3, 0, small_radius, new string[] { "16circle-boss3" }, 1, 25));
            addSkillData(new WayShotSkillData("16circle-boss3", null, SkillGenreS.wayshot, "bulletlightred-yellow", 7, goStraightWithDirection, low_speed2, 0.01, SelfAngle, highangle3, 0, small_radius, (int)(tPI / highangle0), highangle0));

            #endregion

            #endregion

            #region boss1
            addSkillData(new WayShotSkillData("boss11wayshot-0", hppBelowFifty, SkillGenreS.wayshot, MoveType.go_straight, "bulletlarge", high_cd4, high_speed1, 0, 0, big_radius,1));
            addSkillData(new WayShotSkillData("boss11wayshot-1", hppBelowFifty, SkillGenreS.wayshot, MoveType.go_straight, "bulletlarge", high_cd4, high_speed1, 0, 0, big_radius,1));
            addSkillData(new WayShotSkillData("boss11wayshot-2", hppBelowFifty, SkillGenreS.wayshot, MoveType.go_straight, "bulletsmall", high_cd4, high_speed1, 0, 0, small_radius,1));
            addSkillData(new WayShotSkillData("boss11wayshot-3", hppBelowFifty, SkillGenreS.wayshot, MoveType.go_straight, "bulletLL", low_cd1, high_speed2, 0, 0, big_radius,1));
            addSkillData(new WaySkilledBulletsData("boss1ransha-0", hppOverFifty, SkillGenreS.wayshot, "bulletsmall", low_cd6, chaseEnemy, small_radius, "boss1preransha", 1, 100));
            addSkillData(new WaySkilledBulletsData("boss1ransha-1", hppBelowFifty, SkillGenreS.wayshot, "bulletsmall", low_cd3, chaseEnemy, small_radius, "boss1preransha", 1, 100));
            addSkillData(new WaySkilledBulletsData("boss1preransha", null, SkillGenreS.wayshot, null, high_cd1, goStraightWithDirection, 0, 0, SelfAngle, 0, 0, big_radius, new string[] { "1wayshotofsmall", "1wayshotoflarge", }, 1, high_cd1 * 3 + 1));
            addSkillData(new WayShotSkillData("boss1downshot-0",hppOverFifty, SkillGenreS.wayshot, "bulletsmall", low_cd1, goStraightWithDirection, middle_speed1, 0.0, lowangle1, 0, 0, small_radius));
            addSkillData(new WayShotSkillData("boss1downshot-1", hppOverFifty, SkillGenreS.wayshot, "bulletsmall", low_cd1, goStraightWithDirection, middle_speed1, 0.0, lowangle1+Math.PI, 0, 0, small_radius));
            addSkillData(new WayShotSkillData("boss1laser-0", hppBelowFifty, SkillGenreS.laser, "bulletsmall", low_cd6, MoveType.chase_player_target, PointType.player_pos, new Vector(), 0, high_speed1, 0, lowangle1, 0.003, small_radius, Color.Chocolate, 1, 0, 140));
            #endregion

            #region boss2
            addSkillData(new WayShotSkillData("b2-3wayshot-1", null, SkillGenreS.wayshot, MoveType.go_straight, "bulletlight", middle_cd1, middle_speed1, 0, middleangle2, small_radius, 3));
            addSkillData(new WayShotSkillData("b2-1wayshot-0.75", null, SkillGenreS.wayshot, MoveType.go_straight, "bulletmiddle", middle_cd1, high_speed1, 0, 0, small_radius, 1));
            addSkillData(new WayShotSkillData("b2-2wayshot-0.75", null, SkillGenreS.wayshot, MoveType.go_straight, "bulletlightblue-green", middle_cd1, high_speed1, 0, 0, small_radius, 1));
            addSkillData(new WaySkilledBulletsData("b2-x2shot-1",null,SkillGenreS.wayshot,null,60, goStraightWithDownDirection,small_radius,"b2-sub-1wayshot-1",2, 15 ,Math.PI));
            addSkillData(new WayShotSkillData("b2-laserDown1-1",null,SkillGenreL.generation,SkillGenreS.laser,"bulletsmall",600,goStraightWithDownDirection,small_radius,Color.Goldenrod,1,0,600));
            addSkillData(new WayShotSkillData("b2-1wayDown-0.5",null,SkillGenreS.wayshot,"bullethalf",high_cd4,goStraightWithDownDirection,small_radius));
            addSkillData(new WayShotSkillData("b2-blaserDown1", null, SkillGenreL.generation, SkillGenreS.laser, "bulletlightred-yellow", 700, goStraightWithDownDirection,big_radius*2, Color.Goldenrod, 1, 0, 480));
            addSkillData(new WayShotSkillData("b2-laser-1", null,  SkillGenreS.laser, "bulletsmall", 600, MoveType.go_straight,PointType.pos_on_screen,new Vector(640,550),0,middle_speed1,0,0,0,big_radius, Color.Goldenrod, 1, 0, 600));

            #region for boss2
            addSkillData(new WayShotSkillData("b2-sub-1wayshot-1", bulletTimeOut, SkillGenreS.wayshot, "bulletline", 10, goStraightWithDownDirection, small_radius));
            #endregion
            #endregion

            #region boss6
            string _64w1_un50 = "boss6-64way1", _dd_un50="boss6-dandan",_32w_un50="boss6-32wayransya",_mix2="boss6mixbullet2";
            string _hakyo1_ov20 = "boss6-hakkyo1";
            string _czj_ov50 = "boss6-createzyuzi", _czj_sub_ov50="boss6-createzyuzifor", _mix1_ov50="boss6mixbullet1";
            string _hakyo2_un20="boss6-hakkyo2",_mix3="boss6mixbullet3"; 
            
            string rs = Condition.route_set;
            string and = "&";
            addSkillData(new WaySkilledBulletsData("boss6-64way1", Condition.hPp+"<50" + and + rs + "=5", SkillGenreS.wayshot, null, low_cd3, goStraightWithDirection, 0, 0, AngleToPlayer, 0, 0, big_radius, new string[] { "64circle-0", "1wayshot-2" }, 1, high_cd2 * 3 + 1));
            addSkillData(new WaySkilledBulletsData("boss6-hakkyo2", Condition.hPp + "<20"+and+rs+"=1", SkillGenreS.wayshot, null, high_cd1, goStraightWithDirection, 0, 0, SelfAngle, 0, 0, big_radius, new string[] { "1wayshotofsmall", "1wayshotoflarge", "1wayshotofLL" }, 1, high_cd2 * 3 + 1));
            addSkillData(new WaySkilledBulletsData("boss6-hakkyo1", Condition.hPp + ">=20" + and + rs + "=1", SkillGenreS.wayshot, null, high_cd1, goStraightWithDirection, 0, 0, SelfAngle, 0, 0, big_radius, new string[] { "1wayshotoflarge-1"}, 1, high_cd2 * 3 + 1));
            addSkillData(new WaySkilledBulletsData("boss6-createzyuzi", Condition.hPp+">=50" + and + rs + "=2", SkillGenreS.wayshot, null, middle_cd2, goStraightWithDirection, low_speed1, 0, lowangle1, 0, 0, small_radius, new string[] { "createzyuzi-1",}, (int)(tPI / lowangle1), 1, lowangle1));
            //この弾幕と同時に、createzyuziforを出す見えない敵を出現させる。
            addSkillData(new WaySkilledBulletsData("boss6-createzyuzifor", Condition.hPp+">=50" + and + rs + "=2", SkillGenreS.wayshot, "bulletsmall", low_cd3, goStraightWithDirection, small_radius, "1wayshot-4", 2, 70, Math.PI));
            addSkillData(new WaySkilledBulletsData("boss6-dandan", Condition.hPp+"<50" + and + rs + "=2", SkillGenreS.wayshot, "bulletsmall", low_cd3, goStraightWithDirection, small_radius, new string[] { "cs" }, 1, 10));
            addSkillData(new WaySkilledBulletsData("boss6-32wayransya", Condition.hPp+"<50"+and + rs + "=3", SkillGenreS.wayshot, null, low_cd3, rotateAndGoRandom, 0, 0, SelfAngle, 0, 0, big_radius, new string[] { "32-circle-random" }, 1, high_cd2 * 4 + 2));
            addSkillData(new WaySkilledBulletsData("boss6mixbullet1", Condition.hPp+">=50"+and + rs + "=3", SkillGenreS.wayshot, null, low_cd3, goStraightWithDirection, small_radius, new string[] { "createbullet2way-2", "1wayshot-5", "20-circle-random" }, 1, 10));
            addSkillData(new WaySkilledBulletsData("boss6mixbullet2", Condition.hPp + "<50"+">=20" + and + rs + "=4", SkillGenreS.wayshot,"bulletLL", low_cd3, goStraightWithDirection, small_radius, new string[] {  "1wayshot-5", "20-circle-random" }, 2, 150,Math.PI));
            addSkillData(new WaySkilledBulletsData("boss6mixbullet3", Condition.hPp + "<20"+and + rs + "=4", SkillGenreS.wayshot,null, low_cd3, goStraightWithDirection, small_radius, new string[] { "createbullet2way-2", "1wayshot-5", "20-circle-random", "laser-way17" }, 2, 20, Math.PI));

            #region forboss6
            addSkillData(new WayShotSkillData("1wayshotofsmall", null, SkillGenreS.wayshot, "bulletsmall", middle_cd2, rotateAndGoRandom, high_speed1, 0, AngleToPlayer, 0, 0, small_radius, 1, middleangle1));
            addSkillData(new WayShotSkillData("1wayshotoflarge-1", null, SkillGenreS.wayshot, "bulletlarge", middle_cd1, rotateAndGoRandom, middle_speed2, 0.01, AngleToPlayer, 0, 0, small_radius, 3, middleangle1));
            addSkillData(new WayShotSkillData("1wayshotoflarge", null, SkillGenreS.wayshot, "bulletlarge", middle_cd2, rotateAndGoRandom, high_speed1, 0, AngleToPlayer, 0, 0, small_radius, 1, middleangle1));
            addSkillData(new WayShotSkillData("1wayshotofLL", null, SkillGenreS.wayshot, "bulletLL", middle_cd2, rotateAndGoRandom, high_speed1, 0, AngleToPlayer, 0, 0, small_radius, 1, middleangle1));
            addSkillData(new WayShotSkillData("64circle-0", null, SkillGenreS.wayshot, MoveType.go_straight, "bulletrice", low_cd1, low_speed1, 0, highangle3, small_radius, (int)(tPI / highangle3), highangle3));
            addSkillData(new WayShotSkillData("1wayshot-2", null, SkillGenreS.wayshot, MoveType.go_straight, "bulletLL", middle_cd2, middle_speed1, 0, 0, small_radius, 1));
            addSkillData(new WaySkilledBulletsData("createzyuzi-1", bulletTimeOut, SkillGenreS.wayshot, "bulletsmall", middle_cd2, goStraightWithDirection, middle_speed2, 0, AngleToPlayer, 0, 0, small_radius, new string[] { "createzyuzi-1" }, 4, 60, lowangle2, 0, 10));
            addSkillData(new WayShotSkillData("1wayshot-4", bulletTimeOut, SkillGenreS.wayshot, MoveType.go_straight, "bulletmiddle", low_cd2, middle_speed1, 0, 0, small_radius, 1));
            addSkillData(new WaySkilledBulletsData("cs", bulletTimeOut, SkillGenreS.wayshot, "bulletsmall", low_cd1, goStraightWithDirection, small_radius, new string[] { "cs" }, 2, 40, lowangle1));
            addSkillData(new WayShotSkillData("32-circle-random", null, SkillGenreS.wayshot, "bulletlarge", high_cd1, rotateAndGoRandom, high_speed1, 0, AngleToPlayer, 0, 0, small_radius, 32, highangle2));

            addSkillData(new WayShotSkillData("20-circle-random", null, SkillGenreS.wayshot, "bulletsmall", low_cd1, rotateAndGoRandom, high_speed1, 0, AngleToPlayer, 0, 0, small_radius, 20, highangle1));
            addSkillData(new WaySkilledBulletsData("createbullet2way-2", bulletTimeOut, SkillGenreS.wayshot, "bulletsmall", low_cd3, goStraightWithDirection, small_radius, new string[] { "1wayshot-4", "7way*3shot" }, 2, 30, Math.PI));
            addSkillData(new WayShotSkillData("1wayshot-5", null, SkillGenreS.wayshot, MoveType.go_straight, "bulletLL", middle_cd2, low_speed2, 0.05, 0, small_radius, 1));
            addSkillData(new WaySkilledBulletsData("7way*3shot", bulletTimeOut, SkillGenreS.wayshot, "bulletsmall", low_cd3, goStraightWithDirection, 0, 0, AngleToPlayer, 0, 0, big_radius, new string[] { "7wayshot" }, 1, high_cd2 * 3 + 1));
            addSkillData(new WayShotSkillData("7wayshot", null, SkillGenreS.wayshot, "bulletrice", high_cd2, goStraightWithDirection, high_speed1, 0.02, AngleToPlayer, 0, 0, small_radius, 7, highangle1));
            addSkillData(new WayShotSkillData("laser-way17", null, SkillGenreS.laser, "bulletsmall", 180, MoveType.go_straight, PointType.player_pos, new Vector(), 0, high_speed1, 0.03, lowangle2, 0, small_radius, Color.Purple, 17, Math.PI/7, 120));

            #endregion

            #endregion

            #region 2016/12/10まで
            addSkillData(new WayShotSkillData("5wayshot", null, SkillGenreS.wayshot,MoveType.go_straight,"bulletsmall", high_cd3, middle_speed1, 0, highangle1, small_radius,5));
            addSkillData(new WayShotSkillData("3wayshot-0", null, SkillGenreS.wayshot,MoveType.go_straight,"bulletsmall", middle_cd1,middle_speed1, 0, middleangle2, small_radius, 3));
            addSkillData(new WayShotSkillData("3wayshot-1", null, SkillGenreS.wayshot, MoveType.go_straight, "bulletsmall", middle_cd2, middle_speed1, 0, middleangle2, small_radius, 3));
            addSkillData(new WayShotSkillData("boss1wayshot-0", null, SkillGenreS.wayshot, MoveType.go_straight, "bulletsmall", 200, middle_speed1, 0, middleangle2, small_radius, 3));
            addSkillData(new WayShotSkillData("boss1wayshot-1", null, SkillGenreS.wayshot, MoveType.go_straight, "bulletlarge", 270, middle_speed1, 0, middleangle2, small_radius, 3));
            addSkillData(new WayShotSkillData("boss2wayshot-0", null, SkillGenreS.wayshot, MoveType.go_straight, "bulletsmall", 270, middle_speed1, 0, middleangle2, small_radius,3));
            addSkillData(new WayShotSkillData("20circle-0", null,SkillGenreS.wayshot, MoveType.go_straight, "bulletlight", low_cd2, low_speed1, 0, highangle1, small_radius,(int)(tPI/highangle1),lowangle1));

            addSkillData(new WayShotSkillData("boss10circle-0", null, SkillGenreS.wayshot, MoveType.go_straight, "bulletsmall", low_cd3, low_speed1, 0, middleangle2, small_radius, (int)(tPI / middleangle2), lowangle1));
            addSkillData(new WayShotSkillData("downshot-0", null, SkillGenreS.wayshot, "bulletsmall", low_cd1, goStraightWithDirection, middle_speed1,0.0,lowangle1,0, 0,small_radius));
            addSkillData(new WayShotSkillData("downshot-1", null, SkillGenreS.wayshot, "bullethalf", middle_cd1, goStraightWithDirection, middle_speed1, 0, lowangle1, 0,0, small_radius));

            addSkillData(new WayShotSkillData("1wayshot-1", null, SkillGenreS.wayshot, MoveType.go_straight, "bulletsmall", middle_cd2, high_speed1, 0, 0, small_radius,1));
            addSkillData(new WayShotSkillData("1chaseShot-1", null, SkillGenreS.wayshot,MoveType.player_target,"bulletsmall", middle_cd2, middle_speed1, 0, 0,small_radius,1,120));
            addSkillData(new WayShotSkillData("1chaseShot-2", null, SkillGenreS.wayshot, MoveType.player_target, "bulletsmall", middle_cd2, high_speed1, 0, 0, small_radius,1,120));
            addSkillData(new WayShotSkillData("2wayshot-0", null, SkillGenreS.wayshot, MoveType.go_straight, "bulletlarge", middle_cd1, middle_speed1, 0, middleangle1, big_radius, 2));
            addSkillData(new WayShotSkillData("4wayshot-0", null, SkillGenreS.wayshot, MoveType.go_straight, "bulletlarge", middle_cd1, middle_speed1, 0, middleangle1, big_radius, 4));

            addSkillData(new WayShotSkillData("4wayshot-1", null, SkillGenreS.wayshot, MoveType.go_straight, "bulletlight", low_cd1, middle_speed1, 0, middleangle2, big_radius, 4));
            addSkillData(new WayShotSkillData("4wayshot-2", null, SkillGenreS.wayshot, MoveType.go_straight, "bulletsmall", high_cd3, middle_speed1, 0, middleangle1, small_radius,4));
            addSkillData(new WayShotSkillData("laser-once-1", null,SkillGenreS.laser, "bulletsmall", 600, MoveType.go_straight,PointType.player_pos,new Vector(),0, high_speed1, 0, lowangle1, 0,small_radius, Color.Maroon,1,0,600));

            addSkillData(new WayShotSkillData("laser-down-1", null, SkillGenreS.laser,  "bulletsmall", 360, MoveType.go_straight, PointType.Direction, new Vector(),0, high_speed1, 0, lowangle1, 0,small_radius, Color.Maroon, 1,0,180));


            addSkillData(new WayShotSkillData("laser-1", null, SkillGenreS.laser, "bulletsmall", low_cd6, MoveType.chase_player_target, PointType.player_pos, new Vector(),0,high_speed1, 0, lowangle1, 0.003,small_radius, Color.Chocolate,1,0,140));
            addSkillData(new WayShotSkillData("zyuzi-0", null, SkillGenreS.wayshot ,MoveType.go_straight,"bullethalf", middle_cd2, low_speed1, 0, lowangle1, small_radius, (int)(tPI / lowangle1), middleangle4));

            #endregion
        }
        #endregion
        #region GameScreen
        public const int WindowDefaultSizeX = 1280;
        public const int WindowDefaultSizeY = 960;
        public const int WindowSlimSizeY = 720;

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
            #region tda as program
            /*
            tda(bossLifeBar_default_aniName);
            tda(charaCutInTexName);
            tda("150x150Mapアイコン");
            tda("1280x2000背景用グレー画像");
            tda("1100x270メッセージウィンドゥ");
            tda("333x226扇ゲージ");
            tda("25x145必殺技２");
            tda("167x15必殺技エフェクトsample");
            tda("130 149-player");
            tda("130x149右横回避");
            tda("130x149左横回避");
            tda("130x149刀モーション");
                        tda("16-16 tama1");
                        tda("leftside1");
                        tda("130 149-player");
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
            #endregion
            setupSkillData();
            addAniD(new AnimationDataAdvanced("bulletDL" + defaultAnimationNameAddOn,5, 1, 4, "100x100_B15", false));
            /*
            addAniD(new AnimationDataAdvanced("stageSelectButton"+defaultAnimationNameAddOn,14,4,0,"150x150Mapアイコン",true));
            addAniD(new AnimationDataAdvanced("stageSelectButton" + aniNameAddOn_spell, 14, 4, 4, "150x150Mapアイコン", true));
            addAniD(new AnimationDataAdvanced("stageSelectButton" + aniNameAddOn_spellOff, 14, 4, 8, "150x150Mapアイコン", true));
            
            addAniD(new AnimationDataAdvanced(charaName +aniNameAddOn_evadeL,new int[] { 2,2,8,2,2},"130x149左横回避"));
            addAniD(new AnimationDataAdvanced(charaName + aniNameAddOn_evadeR, new int[] { 2, 2, 8, 2, 2 }, "130x149右横回避"));
            getAniD(charaName + aniNameAddOn_evadeR).assignAnimationName(charaName + defaultAnimationNameAddOn, true);
            getAniD(charaName + aniNameAddOn_evadeL).assignAnimationName(charaName + defaultAnimationNameAddOn, true);
            addAniD(new AnimationDataAdvanced(charaName + aniNameAddOn_spell, new int[] { 4, 2, 2,15 }, "130x149刀モーション"));
            addAniD(new AnimationDataAdvanced("swordSkilltoBossDash", 1, 40, "167x15必殺技エフェクトsample"));
            addAniD(new AnimationDataAdvanced("swordSkilltoBossSlash", 1, 18, "25x145必殺技２"));
            addAniD(new AnimationDataAdvanced(charaName + defaultAnimationNameAddOn, 10, 1, "130 149-player", false));
            addAniD(new AnimationDataAdvanced(bossLifeBar_default_aniName+defaultAnimationNameAddOn,10,1,bossLifeBar_default_aniName));
            addAniD(new AnimationDataAdvanced(bossLifeBar_default_aniName + aniNameAddOn_spell, 10, 4,2, bossLifeBar_default_aniName));
            getAniD(bossLifeBar_default_aniName + aniNameAddOn_spell).assignAnimationName(bossLifeBar_default_aniName + defaultAnimationNameAddOn, true);
            */
            #region addAniD in program
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
            #endregion
            addAniD(new AnimationDataAdvanced(charaName + defaultAnimationNameAddOn, 10, 1, "130 149-player", false));

            
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
            database_singleton = null;
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
        public static bool timeExceedMaxDuration( int time, int MaxDuration)
        {
            return (MaxDuration == time) || (MaxDuration != motion_inftyTime && time >= MaxDuration);
        }
        /// <summary>
        /// 渡された時間が無限を意味するか0より大きいならtrue, 0以下で無限でないならfalse
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static bool timeEffective(int time)
        {
            return (time == motion_inftyTime || time > 0);
        }
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
            //Console.WriteLine(ad.animationDataName);
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
            if (addOn == null) return AnimationAdDataDictionary.ContainsKey(name) || AnimationAdDataDictionary.ContainsKey(name+defaultAnimationNameAddOn);
            else return AnimationAdDataDictionary.ContainsKey(name + addOn);
        }
        public static AnimationDataAdvanced getAniD(string name, string addOn = null)
        {
            if (name == null) return null;
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
            else if (AnimationAdDataDictionary.ContainsKey(name))
            {
                return AnimationAdDataDictionary[name];
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
