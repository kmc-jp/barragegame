using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace cellgame
{
    /// <summary>
    /// マウスやキーボードからの入力を行うクラス
    /// </summary>
    abstract class InputManager
    {
        public bool IsEnd { get; protected set; }
        public abstract bool IsKeyDown(KeyID k);
        public abstract bool IsKeyDownOld(KeyID k);
        static readonly int keys = Function.GetEnumLength(typeof(KeyID));
        int[] times = new int[keys];
        public int KeyDownTime(KeyID k)
        {
            return times[(int)k];
        }

        public bool IsPressedForMenu(KeyID id, int wait, int interval)
        {
            return GetKeyPressed(id) || (KeyDownTime(id) >= wait && KeyDownTime(id) % interval == 0);
        }

        public bool IsAnyKeyDown()
        {
            foreach (KeyID id in Enum.GetValues(typeof(KeyID)))
                if (IsKeyDown(id))
                    return true;
            return false;
        }

        protected static Keys[] KeyIDKeys;

        static InputManager()
        {
            KeyIDKeys = new Keys[Enum.GetNames(typeof(KeyID)).Length];
            SetDefault();
        }
        public static void SetDefault()
        {
            SetKey(KeyID.Up, Keys.Up);
            SetKey(KeyID.Down, Keys.Down);
            SetKey(KeyID.Left, Keys.Left);
            SetKey(KeyID.Right, Keys.Right);
            SetKey(KeyID.Select, Keys.Z);
            SetKey(KeyID.Cancel, Keys.X);
            SetKey(KeyID.Wait, Keys.C);
            SetKey(KeyID.Skip, Keys.LeftControl);
            SetKey(KeyID.N, Keys.N);
            SetKey(KeyID.Slow, Keys.LeftShift);
        }
        public static void SetKey(KeyID id, Keys k)
        {
            KeyIDKeys[(int)id] = k;
        }
        public static Keys GetKey(KeyID id) { return KeyIDKeys[(int)id]; }
        public static Keys[] GetKeys() { return KeyIDKeys; }
        public static void SetKeys(Keys[] keys) { KeyIDKeys = keys; }

        public void Update()
        {
            Update_();
            for (int i = 0; i < keys; i++)
                if (IsKeyDown((KeyID)i)) times[i]++;
                else times[i] = 0;
        }
        public abstract void Update_();
        public virtual void End() { }
        /// <summary>
        /// キーの状態を調べる
        /// </summary>
        /// <param name="key">調べるキー</param>
        /// <returns>そのフレームに押されたかなどの情報</returns>
        public KeyState GetKeyState(KeyID key)
        {
            if (IsKeyDown(key))
                if (IsKeyDownOld(key))
                    return KeyState.Down;
                else
                    return KeyState.Press;
            else
                if (IsKeyDownOld(key))
                return KeyState.Pull;
            else
                return KeyState.Up;
        }
        /// <summary>
        /// GeKeyState == KeyState.Pressと同義
        /// </summary>
        public bool GetKeyPressed(KeyID key) { return times[(int)key] == 1; }
    }
    class KeyManager : InputManager
    {
        KeyboardState now;
        KeyboardState old;
        int index;
        public override void Update_()
        {
            old = now;
            now = Keyboard.GetState();
            index++;
        }
        public override bool IsKeyDown(KeyID k) { return now.IsKeyDown(GetKey(k)); }
        public override bool IsKeyDownOld(KeyID k) { return old.IsKeyDown(GetKey(k)); }
    }

    /// <summary>
    /// キーの状態（4種）
    /// </summary>
    enum KeyState
    {
        /// <summary>キーに触られていない状態</summary>
        Up,
        /// <summary>キーが離された瞬間</summary>
        Pull,
        /// <summary>キーを押した瞬間</summary>
        Press,
        /// <summary>キーが押されている状態</summary>
        Down
    }
    enum KeyID
    {
        Up, Down, Left, Right,
        Select, Cancel, Wait, Skip,Slow,
        //以下デバッグ用
        N,
    }
    /// <summary>
    /// マウスのボタン（未実装）
    /// </summary>
    enum MouseButton
    {
        Left, Middle, Right
    }
}