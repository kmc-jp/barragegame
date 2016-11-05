using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace CommonPart
{
    abstract class BasicEditorScene :Scene
    {
        static protected List<Window> windows = new List<Window>();
        static public bool ready { get; private set; } = false;

        public BasicEditorScene(SceneManager scene) : base(scene) { }

        abstract protected void setup_windows();
        protected virtual void close()
        {
            Delete = true;
            windows.Clear();
        }
        protected virtual void addTex()
        {
            Console.WriteLine("Type in the path from Content correctly.");
            Console.WriteLine("Type in End1024 to back to Editor.");
            string str = Console.ReadLine();
            if (str == "End1024") { return; }
            else if (DataBase.TexturesDataDictionary.ContainsKey(str))
            {
                Console.Write(" already inside the DataBase\n");
            }
            else
            {
                DataBase.tdaA(str);
                addTex();
            }
        }
        /// <summary>
        /// windowsのすべてのdrawとmousePositionを表示
        /// </summary>
        /// <param name="d"></param>
        public override void SceneDraw(Drawing d)
        {
            foreach (Window w in windows) { w.draw(d); }
            Vector MousePosition = mouse.MousePosition();
            new RichText("x:" + MousePosition.X + " y:" + MousePosition.Y, FontID.Medium).Draw(d, new Vector(10, Game1._WindowSizeY - 40), DepthID.Message);
        }//SceneDraw
        abstract protected void switch_windowsIcommand(int i);
        /// <summary>
        /// sceneのbaseのupdate()とwindowsのupdate
        /// </summary>
        public override void SceneUpdate()
        {
            base.SceneUpdate();
            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].PosInside(mouse.MousePosition()))
                {
                    windows[i].update((KeyManager)Input, mouse);
                    switch_windowsIcommand(i);
                    if (windows.Count > i) { windows[i].commandForTop = Command.nothing; }
                }
                else
                {
                    windows[i].update();
                }
            }
        }//SceneUpdate() end
    }//class BasicEditorScene end

}//namespace CommonPart end
