using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#if DEBUG
namespace barragegame {
    class SoundTest : Scene {
        MusicPlayer2 music;
        static readonly int length = Function.GetEnumLength<BGMID>() - 1;
        BGMID id;
        public SoundTest(SceneManager s) :base(s) { music = SoundManager.Music; id = music.GetPlayingID; }
        public override void SceneUpdate() {
            var key = Keyboard.GetState();
            if(key.IsKeyDown(Keys.A)) music.SetPosition(-5000, 102);
            if(key.IsKeyDown(Keys.Z)) music.SetPosition(-1000, 102);
            if(key.IsKeyDown(Keys.S)) music.SetPosition(0, 100);
            if(key.IsKeyDown(Keys.X)) Delete = true;
            if(Input.GetKeyPressed(KeyID.Up)) music.PlayBGM(id = (BGMID)(((int)id - 1 + length) % length), true);
            if(Input.GetKeyPressed(KeyID.Down)) music.PlayBGM(id = (BGMID)(((int)id + 1 + length) % length), true);
        }
        public override void SceneDraw(Drawing d) {
            string[] str = new string[] {
                "Position:", music.GetNowTime(),
                "Length:", music.GetLength(),
                "LoopBegin:", music.LoopBegin(),
                "LoopEnd:", music.LoopEnd(),
                "ID:", id.ToString(),
            };
            StringBuilder bld = new StringBuilder(256);
            for(int i = 0; i < str.Length;) {
                bld.Append(str[i++].PadRight(12)).Append(str[i++]).Append('\n');
            }
            new RichText(bld.ToString()).Draw(d, new Vector2(20, 20), DepthID.Debug);
            new RichText("A：ループ5秒前にワープ Z：ループ1秒前にワープ \nS：曲の最初にワープ X：戻る ↑↓:曲選択", FontID.Medium).Draw(d, new Vector2(20, 420), DepthID.Debug, 0.7f);

            if(music.GetPlayingSet == null) return;
            new FilledBox(new Vector2(600, 18), Color.DarkGray).Draw(d, new Vector2(20, 180), DepthID.Debug);
            new FilledBox(new Vector2(600 * music.GetPlayingSet.Stream.Position / music.GetPlayingSet.Stream.Length, 18), Color.Orange).Draw(d, new Vector2(20, 180), DepthID.Debug);
            new FilledBox(new Vector2(1, 18), Color.Red).Draw(d, new Vector2(20 + (int)(600 * music.GetPlayingSet.Stream.LoopBegin / music.GetPlayingSet.Stream.Length), 180), DepthID.Debug);
            new FilledBox(new Vector2(1, 18), Color.Green).Draw(d, new Vector2(20 + (int)(600 * music.GetPlayingSet.Stream.LoopEnd / music.GetPlayingSet.Stream.Length), 180), DepthID.Debug);
        }
    }
}
#endif