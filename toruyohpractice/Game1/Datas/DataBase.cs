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
    public enum MoveType {non_target=0,point_target=1,object_target=2 };
    public enum Command { left_and_go_back = -101, nothing = -100, apply_int = 110, apply_string = 111, button_on = 112, button_off = 113, previousPage = 114, nextPage = 115 };
    /// <summary>
    /// 不変なデータをまとめたクラス
    /// </summary>
    class DataBase {
        /// <summary>
        /// このDataBaseなどに使われている読み込み、Editorでのファイルの読み方法が何時の物かの判断に使われる。確実に大きく変化したら更新していくように。
        /// 日付になっている。年月日で, 9月は 09
        /// </summary>
        static readonly int ThisSystemVersionNumber = 160910;
        #region Variable

        /// <summary>
        /// 必ずTexturesDataDictionaryに読み込まれる画像.
        /// </summary>
        public static readonly string defaultBlankTextureName = "None.png";

        /// <summary>
        /// string is its path, maybe from "Content".  and also string key contains a size of texture's single unit
        /// </summary>
        public static  Dictionary<string, Texture2Ddata> TexturesDataDictionary = new Dictionary<string, Texture2Ddata>();

        private static ContentManager Content;

        public static readonly int WindowDefaultSizeX = 1280;
        public static readonly int WindowDefaultSizeY = 960;
        public static readonly int WindowSlimSizeY = 720;

        #endregion
        #region singleton
        public static DataBase database_singleton=new DataBase();
        static DataBase() {
            
        }
        private DataBase() { }
        #endregion
        #region Method
        /// <summary>
        /// TexturesDataDictionaryにTexture2Ddataを追加するメッソド。
        /// </summary>
        private static void tda(string name)
        {
            TexturesDataDictionary.Add(name, new Texture2Ddata(Content.Load<Texture2D>(name), name));
        }
        /// <summary>
        /// Game1からのCotentを使って、DataBaseの内容を埋める
        /// </summary>
        /// <param name="content"></param>
        public static void Load_Contents(ContentManager c)
        {
            Content = c;
            tda(defaultBlankTextureName);
            tda("36 40-enemy1.png");
            tda("36 40-hex1.png");
            tda("18 20-tama1.png");
            tda("testbackground.png");
            tda("16-16 tama1.png");
            tda("leftside1.jpg");
            tda("rightside1.jpg");
        }
        static public Texture2D getTex(string name)
        {
            if (TexturesDataDictionary.ContainsKey(name))
            {
                return TexturesDataDictionary[name].getTex();
            }
            else
            {
                return TexturesDataDictionary[defaultBlankTextureName].getTex();
            }
        }
        public static Rectangle getRectFromTextureNameAndIndex(string name, int id)
        {
            int w = TexturesDataDictionary[name].w_single;
            int h = TexturesDataDictionary[name].h_single;
            int x = id % TexturesDataDictionary[name].x_max  *w;
            int y = id * TexturesDataDictionary[name].x_max * h;
            if (id >= TexturesDataDictionary[name].x_max * TexturesDataDictionary[name].y_max) { x = y = 0; }
            return new Rectangle(x, y, w, h);
        }
       
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
