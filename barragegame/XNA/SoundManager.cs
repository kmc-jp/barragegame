using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace barragegame {
    /// <summary>
    /// 音楽、SEを管理するクラス
    /// </summary>
    class SoundManager {

        public static MusicPlayer2 Music = new MusicPlayer2();
        public static SEPlayer SE = new SEPlayer();

        public static void Update() {
            SE.Update();
        }
        
        /// <param name="volume">0～100</param>
        /// <param name="pitch">倍率 0～2</param>
        public static void PlaySE(SoundEffectID id, int volume = 50, float pitch = 1) {
            SE.Play(id, volume, pitch);
        }
    }
}
