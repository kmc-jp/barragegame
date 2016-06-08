using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace barragegame {
#if MANAGED_DIRECTX_INPUT   //ManagedDirectX版　要：参照の追加、app.configの変更、コードが古いので若干変更
using DInput = Microsoft.DirectX.DirectInput;
    class JoyPadManager {
        static DInput.Device pad;
        static DInput.JoystickState padstate;
        static DInput.JoystickState padstateold;
        static JoyPadManager() {
            ResetJoypad();
        }
        static void ResetJoypad() {
            DInput.DeviceList devList = DInput.Manager.GetDevices(DInput.DeviceClass.GameControl
                , DInput.EnumDevicesFlags.AttachedOnly);

            foreach(DInput.DeviceInstance dev in devList) {
                pad = new DInput.Device(dev.InstanceGuid);
                break;
            }
            if(pad == null) return;

            foreach(DInput.DeviceObjectInstance obj in pad.Objects)
                if((obj.ObjectId & (int)DInput.DeviceObjectTypeFlags.Axis) != 0)
                    pad.Properties.SetRange(DInput.ParameterHow.ById, obj.ObjectId, new DInput.InputRange(-100, 100));

            pad.Properties.AxisModeAbsolute = true;
            pad.SetCooperativeLevel(null, DInput.CooperativeLevelFlags.NonExclusive | DInput.CooperativeLevelFlags.Background);
            pad.Acquire();
        }
        public static void GetPad() {
            if(pad == null) ResetJoypad();
        }
        public static void Update() {
            if(pad == null) return;
            try {
                pad.Poll();
                padstateold = padstate;
                padstate = pad.CurrentJoystickState;
            } catch(DInput.InputException) {
                pad = null;
            }
        }
        static bool IsKeyPressed(DInput.JoystickState state, KeyID id, byte[] buttons){
            byte[] bts = state.GetButtons();
            if(pad == null || bts == null) return false;
            int pov = -1;
            if(pad.Caps.NumberPointOfViews > 0)
                pov = state.GetPointOfView()[0];
            switch(id) {
                case KeyID.Left: return state.X < -50 || (22500 <= pov && pov <= 31500);
                case KeyID.Right: return state.X > 50 || (4500 <= pov && pov <= 13500);
                case KeyID.Up: return state.Y < -50 || (0 <= pov && pov <= 4500) || (31500 <= pov && pov <= 36000);
                case KeyID.Down: return state.Y > 50 || (13500 <= pov && pov <= 22500);
                default:
                    if(buttons.Length <= (int)id || bts.Length <= buttons[(int)id]) return false;
                    return bts[buttons[(int)id]] != 0;
            }
        }
        public static bool IsKeyPressed(KeyID id) {
            return IsKeyPressed(padstate, id, SaveData.JoyButtons);
        }
        public static bool IsKeyPressedOld(KeyID id) {
            return IsKeyPressed(padstateold, id, SaveData.JoyButtons);
        }
        public static int FirstPressedKey() {
            byte[] bts = padstate.GetButtons();
            byte[] btso = padstateold.GetButtons();
            if(pad == null || bts == null || btso == null) return -1;
            for(int i = 0; i < Math.Min(bts.Length, pad.Caps.NumberButtons); i++)
                if(bts[i] != 0 && btso[i] == 0) return i;
            return -1;
        }
    }
#else
    using System.Runtime.InteropServices;
    /// <summary>
    /// JoyPadの管理
    /// </summary>
    class JoyPadManager {
        /// <summary>
        /// 現在のPad状態
        /// </summary>
        static JoyWrapper.JOYINFOEX padstate;
        /// <summary>
        /// 1フレーム前のPad状態
        /// </summary>
        static JoyWrapper.JOYINFOEX padstateold;
        /// <summary>
        /// 使用中PadのID
        /// </summary>
        static int padid = -1;
        /// <summary>
        /// Padの情報
        /// </summary>
        static JoyWrapper.JOYCAPS padcaps;
        /// <summary>
        /// 遊びの割合（%）
        /// </summary>
        public static int JoyPlay = 40;
        /// <summary>
        /// デフォルトの設定
        /// </summary>
        public static byte[] JoyButtons = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        static JoyPadManager() {
            GetPad();
        }
        public static bool Enable() { return padid >= 0; }
        public static JoyWrapper.JOYINFOEX PadState() { return padstate; }
        public static JoyWrapper.JOYCAPS PadCaps() { return padcaps; }
        /// <summary>
        /// Padをセットする　失敗すると重いので必要なときのみ呼ぶ
        /// </summary>
        public static void GetPad() {
            padid = -1;
            padcaps = new JoyWrapper.JOYCAPS();
            int l = JoyWrapper.joyGetNumDevs();
            for(int i = 0; i < l; i++) {
                JoyWrapper.JoyError j = JoyWrapper.joyGetDevCaps(i, ref padcaps, JoyWrapper.JoyCapsSize);
                if(j == JoyWrapper.JoyError.NoError) {
                    padid = i;
                    break;
                }
            }
        }
        public static void Update() {
            if(padid < 0) return;
            padstateold = padstate;
            padstate = new JoyWrapper.JOYINFOEX();
            padstate.dwFlags = JoyWrapper.JoyReturns.All;
            padstate.dwSize = JoyWrapper.JoyInfoExSize;
            if(JoyWrapper.joyGetPosEx(padid, ref padstate) == JoyWrapper.JoyError.NoError) {
                return;
            }
            //失敗
            padid = -1;
            padstateold = new JoyWrapper.JOYINFOEX();
        }
        static bool IsKeyPressed(JoyWrapper.JOYINFOEX state, KeyID id, byte[] buttons) {
            double play = JoyPlay * 0.005;
            if(padid < 0) return false;
            uint pov = state.dwPOV;
            double x;
            switch(id) {
                case KeyID.Left:
                    x = (double)(state.dwXpos - padcaps.wXmin) / (padcaps.wXmax - padcaps.wXmin);
                    return x < 0.5 - play
                    || ((padcaps.wCaps & JoyWrapper.JoyCaps.HasPov) != 0 && 22500 <= pov && pov <= 31500);  //2スティック型の場合、十字キーはPOVに0.01度単位（でも8方位）になることが多いのでそちらも方向入力とする
                case KeyID.Right:
                    x = (double)(state.dwXpos - padcaps.wXmin) / (padcaps.wXmax - padcaps.wXmin);
                    return x > 0.5 + play
                    || ((padcaps.wCaps & JoyWrapper.JoyCaps.HasPov) != 0 && 4500 <= pov && pov <= 13500);
                case KeyID.Up:
                    x = (double)(state.dwYpos - padcaps.wYmin) / (padcaps.wYmax - padcaps.wYmin);
                    return x < 0.5 - play
                    || ((padcaps.wCaps & JoyWrapper.JoyCaps.HasPov) != 0 && 0 <= pov && pov <= 4500) || (31500 <= pov && pov <= 36000);
                case KeyID.Down:
                    x = (double)(state.dwYpos - padcaps.wYmin) / (padcaps.wYmax - padcaps.wYmin);
                    return x > 0.5 + play
                    || ((padcaps.wCaps & JoyWrapper.JoyCaps.HasPov) != 0 && 13500 <= pov && pov <= 22500);
                default:
                    int i = (int)id - 4;
                    if(buttons.Length <= i || 32 <= buttons[i]) return false;
                    return (state.dwButtons & Pow2(buttons[i])) != 0;
            }
        }
        public static bool IsKeyPressed(KeyID id) {
            return IsKeyPressed(padstate, id, JoyButtons);
        }
        public static bool IsKeyPressedOld(KeyID id) {
            return IsKeyPressed(padstateold, id, JoyButtons);
        }
        /// <summary>
        /// 前のフレームに押されてなくて、現在のフレームに押されているボタンのうち最もボタン番号が若いものを得る
        /// </summary>
        public static int FirstPressedKey() {
            if(padid < 0) return -1;
            for(int i = 0, j = 1; i < 32; i++, j <<= 1)
                if((padstate.dwButtons & j) != 0 && (padstateold.dwButtons & j) == 0) return i;
            return -1;
        }
        static uint Pow2(int n) {
            return 1u << n;
        }
    }
    /// <summary>
    /// dllからの関数を使うクラス
    /// </summary>
    class JoyWrapper {
        //dllから読み込み
        [DllImport("winmm.dll")]
        public static extern JoyError joyGetPosEx(int uJoyID, ref JOYINFOEX pji);
        [DllImport("winmm.dll")]
        public static extern JoyError joyGetDevCaps(int uJoyID, ref JOYCAPS pjc, int cbjc);
        [DllImport("winmm.dll")]
        public static extern int joyGetNumDevs();

        //必要な定数
        public static readonly int JoyInfoExSize = Marshal.SizeOf(typeof(JoyWrapper.JOYINFOEX));
        public static readonly int JoyCapsSize = Marshal.SizeOf(typeof(JoyWrapper.JOYCAPS));

        [StructLayout(LayoutKind.Sequential)]   //dllに構造体を渡すために必要
        public struct JOYINFOEX {
            public int dwSize;
            public JoyReturns dwFlags;
            public uint dwXpos;
            public uint dwYpos;
            public uint dwZpos;
            public uint dwRpos;
            public uint dwUpos;
            public uint dwVpos;
            public uint dwButtons;
            public uint dwButtonNumber;
            public uint dwPOV;
            public uint dwReserved1;
            public uint dwReserved2;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct JOYCAPS {
            public ushort wMid;
            public ushort wPid;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szPname;
            public uint wXmin;
            public uint wXmax;
            public uint wYmin;
            public uint wYmax;
            public uint wZmin;
            public uint wZmax;
            public uint wNumButtons;
            public uint wPeriodMin;
            public uint wPeriodMax;
            public uint wRmin;
            public uint wRmax;
            public uint wUmin;
            public uint wUmax;
            public uint wVmin;
            public uint wVmax;
            public JoyCaps wCaps;
            public uint wMaxAxes;
            public uint wNumAxes;
            public uint wMaxButtons;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]    //char szRegKey[32];はこのような表現に
            public string szRegKey;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szOEMVxD;
        }
        public enum JoyError: int {  //元の型を合わせればenumにしても可
            NoError = 0,
            BadDeviceID = 2,
            NoDriver = 6,
            InvalParam = 11,
            Parms = 165,
            NoCanDo = 166,
            UnPluggeed = 167,
        }
        [Flags]
        public enum JoyCaps: uint {
            HasZ = 1,
            HasR = 2,
            HasU = 4,
            HasV = 8,
            HasPov = 0x10,
            Pov4Dir = 0x20,
            PovCts = 0x40,
        }
        [Flags]
        public enum JoyReturns: uint {
            X = 1,
            Y = 2,
            Z = 4,
            R = 8,
            U = 0x10,
            V = 0x20,
            Pov = 0x40,
            Buttons = 0x80,
            RawData = 0x100,
            PovCts = 0x200,
            Centered = 0x400,
            All = X | Y | Z | R | U | V | Pov | Buttons | PovCts | Centered,
        }
    }
#endif
}
