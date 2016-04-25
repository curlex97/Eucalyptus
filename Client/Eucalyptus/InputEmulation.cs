using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Eucalyptus
{


    public static class VirtualInput
    {

        //public static InputSimulator Simulator = new InputSimulator();
        [DllImport("user32.dll")]
        public static extern bool GetAsyncKeyState(System.Windows.Forms.Keys vKey);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        public static void Press(MouseEventFlags[] list, int data = 0)
        {
            for (int i = 0; i < list.Length; i++)
                mouse_event((uint)list[i], Convert.ToUInt32(System.Windows.Forms.Cursor.Position.X), Convert.ToUInt32(System.Windows.Forms.Cursor.Position.Y), unchecked((uint)data), 0);
        }

        public static void KeyDown(byte key)
        {
            keybd_event((byte)key, 0, 0, 0);

        }

        public static void KeyUp(byte key)
        {
            keybd_event((byte)key, 0, 0x2, 0);

        }

        public static void SendCtrlhotKey(char key)
        {
            keybd_event(0x11, 0, 0, 0);
            keybd_event((byte)key, 0, 0, 0);
            keybd_event((byte)key, 0, 0x2, 0);
            keybd_event(0x11, 0, 0x2, 0);
        }

    }

    [Flags]
    public enum MouseEventFlags
    {
        LEFTDOWN = 0x00000002,
        LEFTUP = 0x00000004,
        MIDDLEDOWN = 0x00000020,
        MIDDLEUP = 0x00000040,
        MOVE = 0x00000001,
        ABSOLUTE = 0x00008000,
        RIGHTDOWN = 0x00000008,
        RIGHTUP = 0x00000010,
        WHEEL = 0x0800

    }


    public static class VirtualKeyboard
    {
        public static void Print(string str)
        {
            foreach (char c in str)
            {
                if (char.IsUpper(c))
                    VirtualInput.KeyDown(VK_LSHIFT);
                Thread.Sleep(5);

                char b = (c + "").ToLower()[0];
                if (b == ' ') { VirtualInput.KeyDown(VK_SPACE); Thread.Sleep(5); VirtualInput.KeyUp(VK_SPACE); }
                else if (b == 'а') { VirtualInput.KeyDown(VK_KEY_F); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_F); }
                else if (b == 'б') { VirtualInput.KeyDown(VK_OEM_COMMA); Thread.Sleep(5); VirtualInput.KeyUp(VK_OEM_COMMA); }
                else if (b == 'в') { VirtualInput.KeyDown(VK_KEY_D); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_D); }
                else if (b == 'г') { VirtualInput.KeyDown(VK_KEY_U); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_U); }
                else if (b == 'д') { VirtualInput.KeyDown(VK_KEY_L); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_L); }
                else if (b == 'е') { VirtualInput.KeyDown(VK_KEY_T); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_T); }
                else if (b == 'ё') { VirtualInput.KeyDown(VK_OEM_3); Thread.Sleep(5); VirtualInput.KeyUp(VK_OEM_3); }
                else if (b == 'ж') { VirtualInput.KeyDown(VK_OEM_1); Thread.Sleep(5); VirtualInput.KeyUp(VK_OEM_1); }
                else if (b == 'з') { VirtualInput.KeyDown(VK_KEY_P); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_P); }
                else if (b == 'и') { VirtualInput.KeyDown(VK_KEY_B); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_B); }
                else if (b == 'й') { VirtualInput.KeyDown(VK_KEY_Q); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_Q); }
                else if (b == 'к') { VirtualInput.KeyDown(VK_KEY_R); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_R); }
                else if (b == 'л') { VirtualInput.KeyDown(VK_KEY_K); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_K); }
                else if (b == 'м') { VirtualInput.KeyDown(VK_KEY_V); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_V); }
                else if (b == 'н') { VirtualInput.KeyDown(VK_KEY_Y); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_Y); }
                else if (b == 'о') { VirtualInput.KeyDown(VK_KEY_J); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_J); }
                else if (b == 'п') { VirtualInput.KeyDown(VK_KEY_G); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_G); }
                else if (b == 'р') { VirtualInput.KeyDown(VK_KEY_H); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_H); }
                else if (b == 'с') { VirtualInput.KeyDown(VK_KEY_C); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_C); }
                else if (b == 'т') { VirtualInput.KeyDown(VK_KEY_N); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_N); }
                else if (b == 'у') { VirtualInput.KeyDown(VK_KEY_E); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_E); }
                else if (b == 'ф') { VirtualInput.KeyDown(VK_KEY_A); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_A); }
                else if (b == 'х') { VirtualInput.KeyDown(VK_OEM_4); Thread.Sleep(5); VirtualInput.KeyUp(VK_OEM_4); }
                else if (b == 'ц') { VirtualInput.KeyDown(VK_KEY_W); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_W); }
                else if (b == 'ч') { VirtualInput.KeyDown(VK_KEY_X); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_X); }
                else if (b == 'ш') { VirtualInput.KeyDown(VK_KEY_I); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_I); }
                else if (b == 'щ') { VirtualInput.KeyDown(VK_KEY_O); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_O); }
                else if (b == 'ъ') { VirtualInput.KeyDown(VK_OEM_6); Thread.Sleep(5); VirtualInput.KeyUp(VK_OEM_6); }
                else if (b == 'ы') { VirtualInput.KeyDown(VK_KEY_S); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_S); }
                else if (b == 'ь') { VirtualInput.KeyDown(VK_KEY_M); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_M); }
                else if (b == 'э') { VirtualInput.KeyDown(VK_OEM_7); Thread.Sleep(5); VirtualInput.KeyUp(VK_OEM_7); }
                else if (b == 'ю') { VirtualInput.KeyDown(VK_OEM_PERIOD); Thread.Sleep(5); VirtualInput.KeyUp(VK_OEM_PERIOD); }
                else if (b == 'я') { VirtualInput.KeyDown(VK_KEY_Z); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_Z); }
                else if (b == '0') { VirtualInput.KeyDown(VK_KEY_0); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_0); }
                else if (b == '1') { VirtualInput.KeyDown(VK_KEY_1); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_1); }
                else if (b == '2') { VirtualInput.KeyDown(VK_KEY_2); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_2); }
                else if (b == '3') { VirtualInput.KeyDown(VK_KEY_3); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_3); }
                else if (b == '4') { VirtualInput.KeyDown(VK_KEY_4); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_4); }
                else if (b == '5') { VirtualInput.KeyDown(VK_KEY_5); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_5); }
                else if (b == '6') { VirtualInput.KeyDown(VK_KEY_6); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_6); }
                else if (b == '7') { VirtualInput.KeyDown(VK_KEY_7); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_7); }
                else if (b == '8') { VirtualInput.KeyDown(VK_KEY_8); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_8); }
                else if (b == '9') { VirtualInput.KeyDown(VK_KEY_9); Thread.Sleep(5); VirtualInput.KeyUp(VK_KEY_9); }
                Thread.Sleep(5);
                if (Char.IsUpper(c))
                    VirtualInput.KeyUp(VK_LSHIFT);
                Thread.Sleep(50);

            }
        }

        public static void SendCtrl_C()
        {
            VirtualInput.KeyDown(VK_LCONTROL);
            VirtualInput.KeyDown(VK_KEY_C);
            VirtualInput.KeyUp(VK_KEY_C);
            VirtualInput.KeyUp(VK_LCONTROL);
        }

        public static void PressEnter()
        {
            VirtualInput.KeyDown(VK_RETURN);
            Thread.Sleep(5);
            VirtualInput.KeyUp(VK_RETURN);
        }

        static byte VK_ABNT_C1 = 0xC1; //	Abnt C1
        static byte VK_ABNT_C2 = 0xC2; //	Abnt C2
        static byte VK_ADD = 0x6B; //	Numpad +
        static byte VK_ATTN = 0xF6; //	Attn
        static byte VK_BACK = 0x08; //	Backspace
        static byte VK_CANCEL = 0x03; //	Break
        static byte VK_CLEAR = 0x0C; //	Clear
        static byte VK_CRSEL = 0xF7; //	Cr Sel
        static byte VK_DECIMAL = 0x6E; //	Numpad .
        static byte VK_DIVIDE = 0x6F; //	Numpad /
        static byte VK_EREOF = 0xF9; //	Er Eof
        static byte VK_ESCAPE = 0x1B; //	Esc
        static byte VK_EXECUTE = 0x2B; //	Execute
        static byte VK_EXSEL = 0xF8; //	Ex Sel
        static byte VK_ICO_CLEAR = 0xE6; //	IcoClr
        static byte VK_ICO_HELP = 0xE3; //	IcoHlp
        static byte VK_KEY_0 = 0x30; // ('0')	0
        static byte VK_KEY_1 = 0x31; // ('1')	1
        static byte VK_KEY_2 = 0x32; // ('2')	2
        static byte VK_KEY_3 = 0x33; // ('3')	3
        static byte VK_KEY_4 = 0x34; // ('4')	4
        static byte VK_KEY_5 = 0x35; // ('5')	5
        static byte VK_KEY_6 = 0x36; // ('6')	6
        static byte VK_KEY_7 = 0x37; // ('7')	7
        static byte VK_KEY_8 = 0x38; // ('8')	8
        static byte VK_KEY_9 = 0x39; // ('9')	9
        static byte VK_KEY_A = 0x41; // ('A')	A
        static byte VK_KEY_B = 0x42; // ('B')	B
        static byte VK_KEY_C = 0x43; // ('C')	C
        static byte VK_KEY_D = 0x44; // ('D')	D
        static byte VK_KEY_E = 0x45; // ('E')	E
        static byte VK_KEY_F = 0x46; // ('F')	F
        static byte VK_KEY_G = 0x47; // ('G')	G
        static byte VK_KEY_H = 0x48; // ('H')	H
        static byte VK_KEY_I = 0x49; // ('I')	I
        static byte VK_KEY_J = 0x4A; // ('J')	J
        static byte VK_KEY_K = 0x4B; // ('K')	K
        static byte VK_KEY_L = 0x4C; // ('L')	L
        static byte VK_KEY_M = 0x4D; // ('M')	M
        static byte VK_KEY_N = 0x4E; // ('N')	N
        static byte VK_KEY_O = 0x4F; // ('O')	O
        static byte VK_KEY_P = 0x50; // ('P')	P
        static byte VK_KEY_Q = 0x51; // ('Q')	Q
        static byte VK_KEY_R = 0x52; // ('R')	R
        static byte VK_KEY_S = 0x53; // ('S')	S
        static byte VK_KEY_T = 0x54; // ('T')	T
        static byte VK_KEY_U = 0x55; // ('U')	U
        static byte VK_KEY_V = 0x56; // ('V')	V
        static byte VK_KEY_W = 0x57; // ('W')	W
        static byte VK_KEY_X = 0x58; // ('X')	X
        static byte VK_KEY_Y = 0x59; // ('Y')	Y
        static byte VK_KEY_Z = 0x5A; // ('Z')	Z
        static byte VK_MULTIPLY = 0x6A; //	Numpad *
        static byte VK_NONAME = 0xFC; //	NoName
        static byte VK_NUMPAD0 = 0x60; //	Numpad 0
        static byte VK_NUMPAD1 = 0x61; //	Numpad 1
        static byte VK_NUMPAD2 = 0x62; //	Numpad 2
        static byte VK_NUMPAD3 = 0x63; //	Numpad 3
        static byte VK_NUMPAD4 = 0x64; //	Numpad 4
        static byte VK_NUMPAD5 = 0x65; //	Numpad 5
        static byte VK_NUMPAD6 = 0x66; //	Numpad 6
        static byte VK_NUMPAD7 = 0x67; //	Numpad 7
        static byte VK_NUMPAD8 = 0x68; //	Numpad 8
        static byte VK_NUMPAD9 = 0x69; //	Numpad 9
        static byte VK_OEM_1 = 0xBA; //	OEM_1 (: ;)
        static byte VK_OEM_102 = 0xE2; //	OEM_102 (> <)
        static byte VK_OEM_2 = 0xBF; //	OEM_2 (? /)
        static byte VK_OEM_3 = 0xC0; //	OEM_3 (~ `)
        static byte VK_OEM_4 = 0xDB; //	OEM_4 ({ [)
        static byte VK_OEM_5 = 0xDC; //	OEM_5 (| \)
        static byte VK_OEM_6 = 0xDD; //	OEM_6 (} ])
        static byte VK_OEM_7 = 0xDE; //	OEM_7 (" ')
        static byte VK_OEM_8 = 0xDF; //	OEM_8 (§ !)
        static byte VK_OEM_ATTN = 0xF0; //	Oem Attn
        static byte VK_OEM_AUTO = 0xF3; //	Auto
        static byte VK_OEM_AX = 0xE1; //	Ax
        static byte VK_OEM_BACKTAB = 0xF5; //	Back Tab
        static byte VK_OEM_CLEAR = 0xFE; //	OemClr
        static byte VK_OEM_COMMA = 0xBC; //	OEM_COMMA (< ,)
        static byte VK_OEM_COPY = 0xF2; //	Copy
        static byte VK_OEM_CUSEL = 0xEF; //	Cu Sel
        static byte VK_OEM_ENLW = 0xF4; //	Enlw
        static byte VK_OEM_FINISH = 0xF1; //	Finish
        static byte VK_OEM_FJ_LOYA = 0x95; //	Loya
        static byte VK_OEM_FJ_MASSHOU = 0x93; //	Mashu
        static byte VK_OEM_FJ_ROYA = 0x96; //	Roya
        static byte VK_OEM_FJ_TOUROKU = 0x94; //	Touroku
        static byte VK_OEM_JUMP = 0xEA; //	Jump
        static byte VK_OEM_MINUS = 0xBD; //	OEM_MINUS (_ -)
        static byte VK_OEM_PA1 = 0xEB; //	OemPa1
        static byte VK_OEM_PA2 = 0xEC; //	OemPa2
        static byte VK_OEM_PA3 = 0xED; //	OemPa3
        static byte VK_OEM_PERIOD = 0xBE; //	OEM_PERIOD (> .)
        static byte VK_OEM_PLUS = 0xBB; //	OEM_PLUS (+ =)
        static byte VK_OEM_RESET = 0xE9; //	Reset
        static byte VK_OEM_WSCTRL = 0xEE; //	WsCtrl
        static byte VK_PA1 = 0xFD; //	Pa1
        static byte VK_PACKET = 0xE7; //	Packet
        static byte VK_PLAY = 0xFA; //	Play
        static byte VK_PROCESSKEY = 0xE5; //	Process
        static byte VK_RETURN = 0x0D; //	Enter
        static byte VK_SELECT = 0x29; //	Select
        static byte VK_SEPARATOR = 0x6C; //	Separator
        static byte VK_SPACE = 0x20; //	Space
        static byte VK_SUBTRACT = 0x6D; //	Num -
        static byte VK_TAB = 0x09; //	Tab
        static byte VK_ZOOM = 0xFB; //	Zoom
        static byte VK__none_ = 0xFF; //	no VK mapping
        static byte VK_ACCEPT = 0x1E; //	Accept
        static byte VK_APPS = 0x5D; //	Context Menu
        static byte VK_BROWSER_BACK = 0xA6; //	Browser Back
        static byte VK_BROWSER_FAVORITES = 0xAB; //	Browser Favorites
        static byte VK_BROWSER_FORWARD = 0xA7; //	Browser Forward
        static byte VK_BROWSER_HOME = 0xAC; //	Browser Home
        static byte VK_BROWSER_REFRESH = 0xA8; //	Browser Refresh
        static byte VK_BROWSER_SEARCH = 0xAA; //	Browser Search
        static byte VK_BROWSER_STOP = 0xA9; //	Browser Stop
        static byte VK_CAPITAL = 0x14; //	Caps Lock
        static byte VK_CONVERT = 0x1C; //	Convert
        static byte VK_DELETE = 0x2E; //	Delete
        static byte VK_DOWN = 0x28; //	Arrow Down
        static byte VK_END = 0x23; //	End
        static byte VK_F1 = 0x70; //	F1
        static byte VK_F10 = 0x79; //	F10
        static byte VK_F11 = 0x7A; //	F11
        static byte VK_F12 = 0x7B; //	F12
        static byte VK_F13 = 0x7C; //	F13
        static byte VK_F14 = 0x7D; //	F14
        static byte VK_F15 = 0x7E; //	F15
        static byte VK_F16 = 0x7F; //	F16
        static byte VK_F17 = 0x80; //	F17
        static byte VK_F18 = 0x81; //	F18
        static byte VK_F19 = 0x82; //	F19
        static byte VK_F2 = 0x71; //	F2
        static byte VK_F20 = 0x83; //	F20
        static byte VK_F21 = 0x84; //	F21
        static byte VK_F22 = 0x85; //	F22
        static byte VK_F23 = 0x86; //	F23
        static byte VK_F24 = 0x87; //	F24
        static byte VK_F3 = 0x72; //	F3
        static byte VK_F4 = 0x73; //	F4
        static byte VK_F5 = 0x74; //	F5
        static byte VK_F6 = 0x75; //	F6
        static byte VK_F7 = 0x76; //	F7
        static byte VK_F8 = 0x77; //	F8
        static byte VK_F9 = 0x78; //	F9
        static byte VK_FINAL = 0x18; //	Final
        static byte VK_HELP = 0x2F; //	Help
        static byte VK_HOME = 0x24; //	Home
        static byte VK_ICO_00 = 0xE4; //	Ico00 *
        static byte VK_INSERT = 0x2D; //	Insert
        static byte VK_JUNJA = 0x17; //	Junja
        static byte VK_KANA = 0x15; //	Kana
        static byte VK_KANJI = 0x19; //	Kanji
        static byte VK_LAUNCH_APP1 = 0xB6; //	App1
        static byte VK_LAUNCH_APP2 = 0xB7; //	App2
        static byte VK_LAUNCH_MAIL = 0xB4; //	Mail
        static byte VK_LAUNCH_MEDIA_SELECT = 0xB5; //	Media
        static byte VK_LBUTTON = 0x01; //	Left Button **
        static byte VK_LCONTROL = 0xA2; //	Left Ctrl
        static byte VK_LEFT = 0x25; //	Arrow Left
        static byte VK_LMENU = 0xA4; //	Left Alt
        static byte VK_LSHIFT = 0xA0; //	Left Shift
        static byte VK_LWIN = 0x5B; //	Left Win
        static byte VK_MBUTTON = 0x04; //	Middle Button **
        static byte VK_MEDIA_NEXT_TRACK = 0xB0; //	Next Track
        static byte VK_MEDIA_PLAY_PAUSE = 0xB3; //	Play / Pause
        static byte VK_MEDIA_PREV_TRACK = 0xB1; //	Previous Track
        static byte VK_MEDIA_STOP = 0xB2; //	Stop
        static byte VK_MODECHANGE = 0x1F; //	Mode Change
        static byte VK_NEXT = 0x22; //	Page Down
        static byte VK_NONCONVERT = 0x1D; //	Non Convert
        static byte VK_NUMLOCK = 0x90; //	Num Lock
        static byte VK_OEM_FJ_JISHO = 0x92; //	Jisho
        static byte VK_PAUSE = 0x13; //	Pause
        static byte VK_PRINT = 0x2A; //	Print
        static byte VK_PRIOR = 0x21; //	Page Up
        static byte VK_RBUTTON = 0x02; //	Right Button **
        static byte VK_RCONTROL = 0xA3; //	Right Ctrl
        static byte VK_RIGHT = 0x27; //	Arrow Right
        static byte VK_RMENU = 0xA5; //	Right Alt
        static byte VK_RSHIFT = 0xA1; //	Right Shift
        static byte VK_RWIN = 0x5C; //	Right Win
        static byte VK_SCROLL = 0x91; //	Scrol Lock
        static byte VK_SLEEP = 0x5F; //	Sleep
        static byte VK_SNAPSHOT = 0x2C; //	Print Screen
        static byte VK_UP = 0x26; //	Arrow Up
        static byte VK_VOLUME_DOWN = 0xAE; //	Volume Down
        static byte VK_VOLUME_MUTE = 0xAD; //	Volume Mute
        static byte VK_VOLUME_UP = 0xAF; //	Volume Up
        static byte VK_XBUTTON1 = 0x05; //	X Button 1 **
        static byte VK_XBUTTON2 = 0x06; //	X Button 2 **

    }

}
