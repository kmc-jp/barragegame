using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CommonPart {
    /// <summary>
    /// 3D用。ただし今のところテクスチャ（と、フォグ）のみ対応
    /// </summary>
    class Drawing3D {
        GraphicsDevice dev;

        BasicEffect effect;
        public Drawing3D(GraphicsDevice d) {
            dev = d;

            //描画方法のセット
            effect = new BasicEffect(dev);
            //カメラの位置
            effect.View = Matrix.CreateLookAt(new Vector3(0, 0, 300), new Vector3(0, 0, 0), Vector3.Up);
            effect.TextureEnabled = true;
            //カメラの視野角、アスペクト比、描画する距離の範囲
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(Function.ToRadian(45), 0.75f, 1, 1000);

            /*var rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            dev.RasterizerState = rs;//*/
        }

        /// <summary>
        /// 画面への描画範囲の設定
        /// </summary>
        /// <param name="v">範囲</param>
        public void SetViewport(Viewport v) { dev.Viewport = v; }

        /// <summary>
        /// フォグの設定
        /// </summary>
        /// <param name="c">色（nullでフォグなし）</param>
        /// <param name="start">開始距離</param>
        /// <param name="end">終了距離</param>
        public void SetFogColor(Color? c, float start = 400, float end = 1000) {
            if(c == null) { effect.FogEnabled = false; return; }
            effect.FogEnabled = true;
            effect.FogColor = c.Value.ToVector3();
            effect.FogStart = start;
            effect.FogEnd = end;
        }
        /// <summary>
        /// テクスチャの描画
        /// </summary>
        /// <param name="pos">位置ベクトル</param>
        /// <param name="x1">pos基準左上の位置ベクトル</param>
        /// <param name="x2">pos基準右上の位置ベクトル</param>
        /// <param name="texture">テクスチャ</param>
        public void DrawTexture(Vector3 pos, Vector3 x1, Vector3 x2, Texture2D texture) {
            VertexPositionTexture[] vpos = new VertexPositionTexture[4];
            vpos[0] = new VertexPositionTexture(pos + x1, new Vector2(0, 0));
            vpos[1] = new VertexPositionTexture(pos + x2, new Vector2(1, 0));
            vpos[2] = new VertexPositionTexture(pos - x2, new Vector2(0, 1));
            vpos[3] = new VertexPositionTexture(pos - x1, new Vector2(1, 1));

            effect.Texture = texture;

            foreach(EffectPass pass in effect.CurrentTechnique.Passes) {
                pass.Apply();

                dev.DrawUserPrimitives(PrimitiveType.TriangleStrip, vpos, 0, 2);
            }
        }
    }
}
