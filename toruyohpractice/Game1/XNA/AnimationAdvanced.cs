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
        {

            animationDataName = name;
            totalFrame = 0;
            frames = new int[_frames.Length];
            for(int i = 0; i < _frames.Length; i++)
            {
                frames[i] = _frames[i];
                totalFrame += frames[i];
            }
            texture_name = _texture_name;

            repeat = _repeat;

            min_texture_index = 0;max_texture_index = frames.Length - 1;

            Texture2Ddata td = DataBase.getTexD(texture_name);
            if (td == null) { xNum = yNum = width = height = -1; Console.Write("Invaild TextureName: " + texture_name); }
            else
            {
                xNum = td.x_max;
                yNum = td.y_max;
                width = td.w_single;
                height = td.h_single;
            }

            if (max_texture_index >= xNum * yNum) { max_texture_index = xNum * yNum - 1; }//限界と突破するのであれば、収まらせる
        }

        /// <summary>
        /// 画像は指定コマから
        /// </summary>
        /// <param name="min_index">指定のコマ</param>
        public AnimationDataAdvanced(string name,int[] _frames, int min_index,string _texture_name,bool _repeat=false)
        {
            animationDataName = name;
            totalFrame = 0;
            frames = new int[_frames.Length];
            for (int i = 0; i < _frames.Length; i++)
            {
                frames[i] = _frames[i];
                totalFrame += frames[i];
            }
            texture_name = _texture_name;

            repeat = _repeat;

            min_texture_index = min_index;

            Texture2Ddata td = DataBase.getTexD(texture_name);
            if (td == null) { xNum = yNum = width = height = -1; Console.Write("Invaild TextureName: " + texture_name); }
            else
            {
                xNum = td.x_max;
                yNum = td.y_max;
                width = td.w_single;
                height = td.h_single;
            }

            if (min_index >= xNum * yNum) { min_index = xNum * yNum - 1; }
            max_texture_index = min_texture_index + frames.Length - 1;
            if (max_texture_index >= xNum * yNum) { max_texture_index = xNum * yNum - 1; }//限界と突破するのであれば、収まらせる
        }

        /// <summary>
        /// DataBaseのTexture2Ddata前提。使われる画像の最初の画像を含め、それからlength個のコマだけを使う。
        /// </summary>
        public AnimationDataAdvanced(string name,int _frame, int length,string _texture_name, bool _repeat = false)
        {

            animationDataName = name;
            frames = new int[length];
            for(int i = 0; i < length; i++) { frames[i] = _frame; }
            totalFrame = length * frames[0];
            texture_name = _texture_name;
            repeat = _repeat;

            min_texture_index = 0;
            max_texture_index = length-1;

            Texture2Ddata td = DataBase.getTexD(texture_name);
            if (td == null) { xNum = yNum = width = height = -1; Console.Write("Invaild TextureName: " + texture_name); }
            else
            {
                xNum = td.x_max;
                yNum = td.y_max;
                width = td.w_single;
                height = td.h_single;
            }
            if (max_texture_index >= xNum * yNum) { max_texture_index = xNum * yNum - 1; }
        }

        /// <summary>
        /// 指定した画像ファイルのコマ番号からアニメーションを作る。
        /// </summary>
        /// <param name="min_index">画像ファイルのコマ番号を指定する</param>
        public AnimationDataAdvanced(string name,int _frame, int length, int min_index,string _texture_name, bool _repeat = false)
        {

            animationDataName = name;
            frames = new int[length];
            for (int i = 0; i < length; i++) { frames[i] = _frame; }
            totalFrame = length * frames[0];
            texture_name = _texture_name;
            repeat = _repeat;

            min_texture_index = min_index;
            
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

            if (min_index >= xNum * yNum) { min_index = xNum * yNum - 1; }
            max_texture_index = min_index + length - 1;
            if (max_texture_index >= xNum * yNum) { max_texture_index = xNum * yNum - 1; }
        }

        #endregion

        #region draw method
        /// <summary>
        /// 
        /// </summary>
        /// <param name="f">今のアニメーションを必要とする物体のframe</param>
        /// <param name="d"></param>
        /// <param name="pos"></param>
        /// <param name="depth"></param>
        /// <param name="size"></param>
        /// <param name="angle"></param>
        public override void Draw(int f, Drawing d, Vector2 pos, DepthID depth, float size = 1, float angle = 0)
        {
            int x = getIndexNow(f);
            //d.Draw(pos, DataBase.getTex(texture_name), new Rectangle(x % xNum * width, x / xNum * height, width, height), depth, size, angle);
            d.Draw(pos, DataBase.getTex(texture_name), DataBase.getRectFromTextureNameAndIndex(texture_name, x), depth, size);
        }
        public override void Draw(int f, Drawing d, Vector2 pos, DepthID depth, Vector2 size, float angle = 0)
        {
            int x = getIndexNow(f);
            // d.Draw(pos, DataBase.getTex(texture_name), new Rectangle(x % xNum * width, x / xNum * height, width, height), depth, size, angle);
            d.Draw(pos, DataBase.getTex(texture_name), DataBase.getRectFromTextureNameAndIndex(texture_name, x), depth, size);
        }
        #endregion

        /// <summary>
        /// このアニメーションデータの続き/前の　アニメーションデータを登録させる。
        /// next=trueにするとこれは続きの登録となり、falseは前の
        /// </summary>
        /// <param name="animation_name">animationDataにアクセス用のkey</param>
        /// <param name="next">true=これは続きのアニメーション、false=これは前の</param>
        public void assignAnimationName(string animation_name,bool next)
        {
            if (next)
            {
                next_animation_name = animation_name;
            }
            else {
                pre_animation_name = animation_name;
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
                for (int s = frames[x]; s < t; s += frames[++x]) ;//ここで x の値が求まるのである
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
        public override float X { get { return data.X; } }
        public override float Y { get { return data.Y; } }
        public new AnimationDataAdvanced data;
        protected bool repeat;
        int frame;
        const bool animateWithUpdate = true;
        public AnimationAdvanced(AnimationDataAdvanced d):base(d)
        { data = d; repeat = data.repeat; }

        /// <summary>
        /// Animationではvirtual修辞していないので、newのupdateになるが、これはAnimationの配列では正しく動かないので、
        /// できればAnimationAdvancedの配列にしてください。
        /// </summary>
        public new void Update()
        {
            if (animateWithUpdate) frame++;
            /*
            if (data.getIndexNow(frame) > data.max_texture_index) {
                if(repeat) {
                    frame = 0;
                }
                else {
                    if (data.next_animation_name != null && data.next_animation_name != AnimationDataAdvanced.notAnimationName) {
                        data = DataBase.getAniD(data.next_animation_name); frame = 0;
                    }
                }
                
            }
            */
        }//update end
        public override void Draw(Drawing d, Vector2 pos, DepthID depth, float size = 1, float angle = 0)
        {
            
            data.Draw(frame, d, pos, depth, size, angle);
            if (!animateWithUpdate && d.Animate)
                frame++;
        }
        public override void Draw(Drawing d, Vector2 pos, DepthID depth, Vector2 size, float angle = 0)
        {
            data.Draw(frame, d, pos, depth, size, angle);
            if (d.Animate)
                frame++;
        }
    }
    
}//namespace end
