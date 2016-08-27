using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace barragegame {
    /// <summary>
    /// パッド動作を確認するシーン
    /// </summary>
    class PadTestScene: Scene {
        public PadTestScene(SceneManager s) : base(s) { s.BackSceneNumber++; JoyPadManager.GetPad(); }
        JoyWrapper.JOYINFOEX info;
        JoyWrapper.JOYCAPS caps;
        int rcount = 0;
        int lcount = 0;
        public override void SceneUpdate() {
            KeyboardState state = Keyboard.GetState();
            if(state.IsKeyDown(Keys.X) || state.IsKeyDown(Keys.Escape)) Delete = true;

            //遊びの調整
            if(Keyboard.GetState().IsKeyDown(Keys.Left)) {
                if(lcount == 0 || lcount >= 15 && lcount % 2 == 0)
                    JoyPadManager.JoyPlay--;
                lcount++;
            }
            else lcount = 0;
            if(Keyboard.GetState().IsKeyDown(Keys.Right)) {
                if(rcount == 0 || rcount >= 15 && rcount % 2 == 0)
                    JoyPadManager.JoyPlay++;
                rcount++;
            }
            else rcount = 0;
            if(JoyPadManager.JoyPlay < 5) JoyPadManager.JoyPlay = 5;
            if(JoyPadManager.JoyPlay > 95) JoyPadManager.JoyPlay = 95;

            //1秒ごとにパッドの再取得を試みる
            if(!JoyPadManager.Enable() && Frame % 60 != 0) return;
            JoyPadManager.GetPad();
            info = JoyPadManager.PadState();
            caps = JoyPadManager.PadCaps();
        }
        public override void SceneDraw(Drawing d) {
            TalkWindow.DrawMessageBack(d, new Vector2(Game1.WindowSizeX - 20, Game1.WindowSizeY - 20), new Vector2(10, 10), DepthID.BackGroundWall);
            new RichText("パッド : " + (JoyPadManager.Enable() ? "有効" : "無効")).Draw(d, new Vector2(40, 40), DepthID.Message);
            new RichText("[Esc]キーor[X]キーで戻る。[←]キー、[→]キーで遊びの調整。").Draw(d, new Vector2(40, 420), DepthID.Message);
            new RichText("遊び : " + JoyPadManager.JoyPlay + "%").Draw(d, new Vector2(40, 380), DepthID.Message);
            if(!JoyPadManager.Enable()) { return; }
            double x = (double)(info.dwXpos - caps.wXmin) / (caps.wXmax - caps.wXmin);
            double y = (double)(info.dwYpos - caps.wYmin) / (caps.wYmax - caps.wYmin);
            new RichText("x : " + x.ToString("0.00000") + (x < 0.5 - JoyPadManager.JoyPlay * 0.005 ? "  ←" : (x > 0.5 + JoyPadManager.JoyPlay * 0.005 ? "  →" : ""))).Draw(d, new Vector2(40, 300), DepthID.Message);
            new RichText("y : " + y.ToString("0.00000") + (y < 0.5 - JoyPadManager.JoyPlay * 0.005 ? "  ↑" : (y > 0.5 + JoyPadManager.JoyPlay * 0.005 ? "  ↓" : ""))).Draw(d, new Vector2(240, 300), DepthID.Message);
            new FilledBox(new Vector2(200, 200), Color.DarkRed).Draw(d, new Vector2(40, 80), DepthID.Message);
            new FilledBox(new Vector2(200, JoyPadManager.JoyPlay * 2), Color.Blue * 0.5f).Draw(d, new Vector2(40, 180 - JoyPadManager.JoyPlay), DepthID.Message);
            new FilledBox(new Vector2(JoyPadManager.JoyPlay * 2, 200), Color.Blue * 0.5f).Draw(d, new Vector2(140 - JoyPadManager.JoyPlay, 80), DepthID.Message);
            new FilledBox(new Vector2(1, 200), Color.GreenYellow).Draw(d, new Vector2(140, 80), DepthID.Message);
            new FilledBox(new Vector2(200, 1), Color.GreenYellow).Draw(d, new Vector2(40, 180), DepthID.Message);
            new FilledBox(new Vector2(5, 5), Color.Red).Draw(d, new Vector2(38 + (float)(x * 200), 78 + (float)(y * 200)), DepthID.Message);

            if((caps.wCaps & JoyWrapper.JoyCaps.HasPov) != 0) {
                new FilledBox(new Vector2(1, 120), Color.GreenYellow).Draw(d, new Vector2(340, 120), DepthID.Message);
                new FilledBox(new Vector2(120, 1), Color.GreenYellow).Draw(d, new Vector2(280, 180), DepthID.Message);
                new FilledBox(new Vector2(5, 5), Color.Red).Draw(d, new Vector(338 + (info.dwPOV == 65535 ? 0 : 60 * Function.Sin(info.dwPOV / 100.0)), 178 - (info.dwPOV == 65535 ? 0 : 60 * Function.Cos(info.dwPOV / 100.0))), DepthID.Message);
            }
            new RichText("POV : " + ((caps.wCaps & JoyWrapper.JoyCaps.HasPov) != 0 ? info.dwPOV.ToString() : "無効")).Draw(d, new Vector2(280, 80), DepthID.Message);

            StringBuilder sb = new StringBuilder();
            sb.Append("Buttons : \n ");
            for(int i = 0; i < caps.wNumButtons; i++) {
                sb.Append((1 << i & info.dwButtons) != 0 ? "#ffff00" : "#808080").Append((i + 1).ToString("00 "));
                if(i % 12 == 11) sb.Append("\n");
            }
            new RichText(sb.ToString()).NoNum().Draw(d, new Vector2(40, 330), DepthID.Message);
        }
    }
}
