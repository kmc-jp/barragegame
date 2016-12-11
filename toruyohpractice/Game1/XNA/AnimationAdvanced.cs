using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace CommonPart
{
    // AnimationDataAdvanced はDataBaseの中のAnimationAdDataDictionaryに入っている。

    /// <summary>
    /// dataはrepeat,min,max,length,frames,name,texname,pre,nextで記録される
    /// これもDataBase.TexturesDataDictionaryがある前提となっている。
    /// </summary>
    class AnimationDataAdvanced : AnimationData
    {   // SingleTextureAnimationData class に少しだけ追加の機能をつけたもの

        #region property
        /// <summary>
        /// X,Yで1コマのwidthとheightが返される
        /// </summary>
        public override int X { get { return width; } }
        public override int Y { get { return height; } }
        /// <summary>
        /// 画像ファイルの一行のコマ数と行数。
        /// </summary>
        readonly int xNum, yNum;
        readonly int width;
        readonly int height;

        //以上のデータは、ファイルに保存する必要がないでしょう。Textureの名前から入手させる。
        //以下のデータは、バイナリーファイルに保存する必要がある。 totalframeは不要かな
        /// <summary>
        /// アニメーションが繰り返すか、それとも最後のコマになればそのままになるか
        /// </summary>
        public readonly bool repeat;
        /// <summary>
        /// 画像ファイルの最初の使用するコマ番号と最後のコマ番号
        /// </summary>
        public int min_texture_index, max_texture_index;

        public const string notAnimationName = "notAnimationName";
        public string pre_animation_name { get; protected set; } = notAnimationName;
        public string next_animation_name { get; protected set; } = notAnimationName;
        /// <summary>
        /// このアニメーションが終わるに使われるframe合計、コマ数ではないことに注意.注:ここのframeは物体のframeに依拠する
        /// </summary>
        public readonly int totalFrame;
        /// <summary>
        /// 使う画像ファイルのアクセス用のkey
        /// </summary>
        public string texture_name;
        /// <summary>
        /// このanimationDataがAnimationAdDataDictionaryに登録される時の同梱のkeyである。必ず他と重複しないように。
        /// </summary>
        public string animationDataName;
        /// <summary>
        /// アニメーションの1コマ1コマの間の時間。次のコマになるまでの時間
        /// </summary>
        public readonly int[] frames;
        #endregion

        #region constructor
        /// <summary>
        /// DataBaseのTexture2Ddataを入手して、AnimationDataをつくり上げる.画像ファイルの最初から使用する
        /// </summary>
        /// <param name="_frames">各画像ファイルのコマが表示され続けるframe数の配列</param>
        /// <param name="_texture_name">DataBaseのTexturesDataDictionaryのkeyです</param>
        public AnimationDataAdvanced(string name, int[] _frames,string _texture_name,bool _repeat=false)
            :this(name,0,_texture_name,_repeat)
        {
            totalFrame = 0;
            frames = new int[_frames.Length];
            for(int i = 0; i < _frames.Length; i++)
            {
                frames[i] = _frames[i];
                totalFrame += frames[i];
            }

            max_texture_index = frames.Length - 1;
            if (max_texture_index >= xNum * yNum) { max_texture_index = xNum * yNum - 1; }//限界と突破するのであれば、収まらせる
        }

        /// <summary>
        /// 画像は指定コマから
        /// </summary>
        /// <param name="min_index">指定のコマ</param>
        public AnimationDataAdvanced(string name,int[] _frames, int _min_index,string _texture_name,bool _repeat=false)
            : this(name, _min_index, _texture_name, _repeat)
        {
            totalFrame = 0;
            frames = new int[_frames.Length];
            for (int i = 0; i < _frames.Length; i++)
            {
                frames[i] = _frames[i];
                totalFrame += frames[i];
            }

            max_texture_index = min_texture_index + frames.Length - 1;
            if (max_texture_index >= xNum * yNum) { max_texture_index = xNum * yNum - 1; }//限界と突破するのであれば、収まらせる
        }

        /// <summary>
        /// DataBaseのTexture2Ddata前提。使われる画像の最初の画像を含め、それからlength個のコマだけを使う。
        /// </summary>
        public AnimationDataAdvanced(string name,int _frame, int length,string _texture_name, bool _repeat = false)
            : this(name, 0, _texture_name, _repeat)
        {
            frames = new int[length];
            for(int i = 0; i < length; i++) { frames[i] = _frame; }
            totalFrame = length * frames[0];

            max_texture_index = length-1;
            if (max_texture_index >= xNum * yNum) { max_texture_index = xNum * yNum - 1; }
        }

        /// <summary>
        /// 指定した画像ファイルのコマ番号からアニメーションを作る。
        /// </summary>
        /// <param name="min_index">画像ファイルのコマ番号を指定する</param>
        public AnimationDataAdvanced(string name,int _frame, int length, int min_index,string _texture_name, bool _repeat = false)
            :this(name,min_index,_texture_name,_repeat)
        {
            frames = new int[length];
            for (int i = 0; i < length; i++) { frames[i] = _frame; }
            totalFrame = length * frames[0];

            max_texture_index = min_index + length - 1;
            if (max_texture_index >= xNum * yNum) { max_texture_index = xNum * yNum - 1; }
        }

        /// <summary>
        /// 以上のconstructorの共通部分をまとめたコンストラクターである
        /// </summary>
        /// <param name="_min_index">アニメーションの最初のコマが使う画像ファイル上のコマ番号,0から始まる</param>
        private AnimationDataAdvanced(string _name,int _min_index, string _texture_name,bool _repeat) {
            animationDataName = _name;
            texture_name = _texture_name;
            min_texture_index = _min_index;
            repeat = _repeat;
            #region from texture2Ddata 
            Texture2Ddata td = DataBase.getTexD(texture_name);
            if (td == null) { xNum = yNum = width = height = -1; Console.Write("Invaild TextureName: " + texture_name); }
            else
            {
                xNum = td.x_max;
                yNum = td.y_max;
                width = td.w_single;
                height = td.h_single;
            }
            #endregion
            if (_min_index >= xNum * yNum) { _min_index = xNum * yNum - 1; }
        }
        #endregion
        #region draw method
        /// <summary>
        /// 
        /// </summary>
        /// <param name="f">今のアニメーションを必要とする物体のframe</param>
        /// <param name="d"></param>
        /// <param name="pos">画像の左上の位置。回転はこれを中心としている。</param>
        /// <param name="depth"></param>
        /// <param name="size"></param>
        /// <param name="angle">与えられたposを中心に回転する</param>
        public override void Draw(int f, Drawing d, Vector2 pos, DepthID depth, float size = 1, float angle = 0)
        {
            int x = getIndexNow(f);
            d.Draw(pos, DataBase.getTex(texture_name), new Rectangle(x % xNum * width, x / xNum * height, width, height), depth, size, angle);
        }
        public override void Draw(int f, Drawing d, Vector2 pos, DepthID depth, Vector2 size, float angle = 0)
        {
            int x = getIndexNow(f);
            d.Draw(pos, DataBase.getTex(texture_name), new Rectangle(x % xNum * width, x / xNum * height, width, height), depth, size, angle);
        }
        #endregion

        /// <summary>
        /// DataBaseのAnimationADdataDictionaryの存在が前提になっている. そこから同名アニメーションデータのコピーを作る
        /// </summary>
        /// <returns></returns>
        static public AnimationDataAdvanced getAcopyFromDataBaseByName(string _ani_name) {
            if (DataBase.existsAniD(_ani_name, null)) {
                AnimationDataAdvanced aDad = DataBase.getAniD(_ani_name);
                AnimationDataAdvanced n_aDad=new AnimationDataAdvanced(_ani_name + "_copy", aDad.frames, aDad.min_texture_index, aDad.texture_name, aDad.repeat);
                n_aDad.assignAnimationName(aDad.pre_animation_name,false);
                n_aDad.assignAnimationName(aDad.next_animation_name, true);
                return n_aDad;
            }
            else {
                Console.WriteLine("copy failed. Not Found In Dictionary.");
                return new AnimationDataAdvanced(_ani_name + "_copy",1000,1,DataBase.defaultBlankTextureName); 
            }
        }

        /// <summary>
        /// このアニメーションデータの続き/前の　アニメーションデータを登録させる。
        /// next=trueにするとこれは続きの登録となり、falseは前の
        /// </summary>
        /// <param name="animation_name">animationDataにアクセス用のkey</param>
        /// <param name="next">true=これは続きのアニメーション、false=これは前の</param>
        public void assignAnimationName(string _animation_name,bool next)
        {
            if (next)
            {
                next_animation_name = _animation_name;
            }
            else {
                pre_animation_name = _animation_name;
            }
        }

        /// <summary>
        /// 与えられるf: 現在の物体で更新されているframe
        /// </summary>
        /// <param name="f">現在の物体で更新されているframe</param>
        /// <returns></returns>
        public int getIndexNow(int f) {
            int t = 0;
            if (repeat)
            {
                t = f % totalFrame;
            }else { t = f; }
            int x = 0;// x は今描かれるべきコマの番号の変数である。ここではまだ求まっていない
            //またこれはmin_texture_indexを考慮していない。
            if (!repeat && t > totalFrame) {
                x = frames.Length - 1;
            }
            else
            {
                for (int s = frames[0]; s < t; s += frames[++x]) ;//ここで x の値が求まるのである
            }
            x += min_texture_index;
            if (x > max_texture_index) { x = max_texture_index; }
            return x;
        }

        /// <summary>
        /// repeat は含まれていないことに注意
        /// </summary>
        /// <returns></returns>
        public int[] getIntsData()
        {
            //min,max,frames.Length
            int[] ints = new int[2+  1+frames.Length];
            int z = 0;
            ints[z] = min_texture_index;
            z++;
            ints[z] = max_texture_index;
            z++;
            ints[z] = frames.Length; //z=2? here
            z++; //z = 3 here;
            for(int n = 0; n < frames.Length; n++)
            {
                ints[z + n]=frames[n];
            }
            z += frames.Length;

            return ints;
        }
        //tex name, pre anime name, next anime name, 
        public string[] getStringsData()
        {
            return new string[] { animationDataName,texture_name, pre_animation_name, next_animation_name };
        }
    }

    
    class AnimationAdvanced : Animation
    {
        public override float X { get { if (data == null) return 0;else return data.X; } }
        public override float Y { get { if (data == null) return 0; else return data.Y; } }
        new AnimationDataAdvanced data;
        int frame;
        const bool animateWithUpdate = true;
        protected bool repeat=false;
        public AnimationAdvanced(AnimationDataAdvanced d):base(d)// data= dとしているだけ。
        { data = d; if(data!=null)repeat = data.repeat; }

        /// <summary>
        /// アニメーションのループをたどり、最初のアニメーションを見つけるか、このアニメーションにまたループして戻っている場合は自分を見つける。
        /// </summary>
        public void backToTop() {
            if (data == null) return;
            if (data.pre_animation_name == null ||data.pre_animation_name==AnimationDataAdvanced.notAnimationName) {
                frame = 0;
                return;
            }
            string data2_name=data.pre_animation_name; // このループの最初のアニメーションを見つけて記録するための変数
            string data1_name = data.animationDataName;//この変数は上の変数の次のアニメーションの名前を記録している。
            while (data2_name != null &&data2_name!=AnimationDataAdvanced.notAnimationName&& DataBase.getAniD(data2_name) != null &&
                    DataBase.getAniD(data2_name).next_animation_name == data1_name )
            {
                data1_name = data2_name;
                data2_name = DataBase.getAniD(data2_name).pre_animation_name;
                if (data2_name == data.animationDataName) { break; }
            }
            if (data2_name != data.animationDataName)
            {
                data = DataBase.getAniD(data2_name);
            }
            frame = 0;
        }
        /// <summary>
        /// Animationではvirtual修辞していないので、newのupdateになるが、これはAnimationの配列では正しく動かないので、
        /// できればAnimationAdvancedの配列にしてください。
        /// </summary>
        public new void Update()
        {
            if (data == null) return;
            if (animateWithUpdate) frame++;
            if (frame>data.totalFrame)
            {
                //Console.WriteLine(data.animationDataName+"come to the last frame!");
                if (repeat && data.next_animation_name==AnimationDataAdvanced.notAnimationName) {

                    frame = 0;
                }
                else {
                    if (data.next_animation_name != null && data.next_animation_name != AnimationDataAdvanced.notAnimationName)
                    {
                        data = DataBase.getAniD(data.next_animation_name);
                        frame = 0;
                    }
                }
            }
        }
        public override void Draw(Drawing d, Vector2 pos, DepthID depth, float size = 1, float angle = 0)
        {
            if (data == null) return;
            data.Draw(frame, d, pos, depth, size, angle);
            if (!animateWithUpdate && d.Animate)
                frame++;
        }
        public override void Draw(Drawing d, Vector2 pos, DepthID depth, Vector2 size, float angle = 0)
        {
            if (data == null) return;
            data.Draw(frame, d, pos, depth, size, angle);
            if (d.Animate)
                frame++;
        }
    }
    
}//namespace end
