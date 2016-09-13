using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace CommonPart {
    /// <summary>
    /// メニューのカーソル移動処理を書いたクラス
    /// </summary>
    abstract class MenuScene: Scene {
        protected int Index = 0;
        protected int MaxIndex;
        protected MenuScene(SceneManager s, int indexes)
            : base(s) {
            MaxIndex = indexes;
        }
        public override void SceneUpdate() {
            if(Input.IsPressedForMenu(KeyID.Up, 30, 8)) {
                SoundManager.PlaySE(SoundEffectID.Cursor_Move);
                Index--;
                if(Index < 0) Index = MaxIndex - 1;
            } else if(Input.IsPressedForMenu(KeyID.Down, 30, 8)) {
                SoundManager.PlaySE(SoundEffectID.Cursor_Move);
                Index++;
                if(Index >= MaxIndex) Index = 0;
            }
            if(Input.GetKeyPressed(KeyID.Select)) {
                SoundManager.PlaySE(SoundEffectID.Cursor_OK);
                Choosed(Index);
            }
        }
        protected abstract void Choosed(int i);
    }
    /// <summary>
    /// 複数ページ版メニュー
    /// </summary>
    abstract class MultiPagedMenu: Scene {
        /// <summary>
        /// 現在のページ
        /// </summary>
        protected int Page = 0;
        /// <summary>
        /// 最大ページ
        /// </summary>
        protected int MaxPage;
        /// <summary>
        /// 1ページ当たりの数
        /// </summary>
        protected int PageLength;
        /// <summary>
        /// 現在のページの長さ
        /// </summary>
        protected int PageLengthNow;
        /// <summary>
        /// 現在のページでの位置
        /// </summary>
        protected int Index = 0;
        /// <summary>
        /// 全体の要素数
        /// </summary>
        protected int MaxIndex;

        protected int NowIndex { get { return Index + Page * PageLength; } }

        public MultiPagedMenu(SceneManager s) : base(s) { }
        public MultiPagedMenu(SceneManager s, int length, int pageLeng) : base(s) {
            SetIndexes(length, pageLeng);
        }
        protected void SetIndexes(int length, int pageLeng) {
            PageLength = pageLeng;
            MaxIndex = length;
            MaxPage = (MaxIndex - 1) / PageLength + 1;
            Page = Math.Min(MaxPage - 1, Page);
            PageLengthNow = Math.Min(PageLength, MaxIndex - Page * PageLength);
            Index = Math.Min(PageLengthNow - 1, Index);
        }
        public override void SceneUpdate() {
            if(MaxIndex == 0) return;
            if(Input.IsPressedForMenu(KeyID.Up, 30, 8)) {
                SoundManager.PlaySE(SoundEffectID.Cursor_Move);
                Index--;
                if(Index < 0) Index = PageLengthNow - 1;
            } else if(Input.IsPressedForMenu(KeyID.Down, 30, 8)) {
                SoundManager.PlaySE(SoundEffectID.Cursor_Move);
                Index++;
                if(Index >= PageLengthNow) Index = 0;
            }
            if(MaxPage != 0) {
                if(Input.GetKeyPressed(KeyID.Left)) {
                    SoundManager.PlaySE(SoundEffectID.Cursor_Move);
                    Page--;
                    if(Page < 0) Page = MaxPage - 1;
                    PageLengthNow = Math.Min(PageLength, MaxIndex - Page * PageLength);
                    if(Index >= PageLengthNow) Index = PageLengthNow - 1;
                } else if(Input.GetKeyPressed(KeyID.Right)) {
                    SoundManager.PlaySE(SoundEffectID.Cursor_Move);
                    Page++;
                    if(Page >= MaxPage) Page = 0;
                    PageLengthNow = Math.Min(PageLength, MaxIndex - Page * PageLength);
                    if(Index >= PageLengthNow) Index = PageLengthNow - 1;
                }
            }
            if(Input.GetKeyPressed(KeyID.Select)) {
                SoundManager.PlaySE(SoundEffectID.Cursor_OK);
                Choosed(NowIndex);
            }
        }
        protected abstract void Choosed(int i);
    }
}
