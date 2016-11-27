using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace CommonPart
{
#if WINDOWS
    /// <summary>
    /// プログラム自動生成の本体を起動するためのコード。普通触らない。
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (Debugger.IsAttached)
            {
                //デバッガがあるならそちらに任せる
                MainProcess(args);
            }
            else
            {
                //ないなら分かりやすくエラーを表示する
                try
                {
                    MainProcess(args);
                }
                catch (Exception e)
                {
                    bool flag = false;
                    try
                    {
                        using (StreamWriter w = new StreamWriter("error.log", true))
                        {
                            w.WriteLine(DateTime.Now.ToString());
                            w.WriteLine(e.ToString());
                            w.WriteLine();
                        }
                    }
                    catch
                    {
                        flag = true;
                    }
                    SoundManager.Music.Close();
                    if (flag)
                        MessageBox.Show("エラーが発生しました。強制終了します。\n内容 : " + e.Message + "\nエラーログ書き出しに失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    else
                        MessageBox.Show("エラーが発生しました。強制終了します。\n内容 : " + e.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }
        static void MainProcess(string[] args)
        {
            if (File.Exists(UpdateScene.TempExe)) File.Delete(UpdateScene.TempExe);
            if (File.Exists(UpdateScene.TempExe + ".config")) File.Delete(UpdateScene.TempExe + ".config");
            using (Game1 game = new Game1())
            {
                game.Run();
            }
            SoundManager.Music.Close();
        }
    }
#endif
}

