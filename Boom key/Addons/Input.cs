using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using sysInput = System.Windows.Input;

namespace BoomKey.Addons
{
    #region Enums

    #region Public
    [Flags]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Key
    {
        [EnumMember(Value = "A")]
        A = sysInput.Key.A,

        [EnumMember(Value = "B")]
        B = sysInput.Key.B,

        [EnumMember(Value = "C")]
        C = sysInput.Key.C,

        [EnumMember(Value = "D")]
        D = sysInput.Key.D,

        [EnumMember(Value = "E")]
        E = sysInput.Key.E,

        [EnumMember(Value = "F")]
        F = sysInput.Key.F,

        [EnumMember(Value = "G")]
        G = sysInput.Key.G,

        [EnumMember(Value = "H")]
        H = sysInput.Key.H,

        [EnumMember(Value = "I")]
        I = sysInput.Key.I,

        [EnumMember(Value = "J")]
        J = sysInput.Key.J,

        [EnumMember(Value = "K")]
        K = sysInput.Key.K,

        [EnumMember(Value = "L")]
        L = sysInput.Key.L,

        [EnumMember(Value = "M")]
        M = sysInput.Key.M,

        [EnumMember(Value = "N")]
        N = sysInput.Key.N,

        [EnumMember(Value = "O")]
        O = sysInput.Key.O,

        [EnumMember(Value = "P")]
        P = sysInput.Key.P,

        [EnumMember(Value = "Q")]
        Q = sysInput.Key.Q,

        [EnumMember(Value = "R")]
        R = sysInput.Key.R,

        [EnumMember(Value = "S")]
        S = sysInput.Key.S,

        [EnumMember(Value = "T")]
        T = sysInput.Key.T,

        [EnumMember(Value = "U")]
        U = sysInput.Key.U,

        [EnumMember(Value = "V")]
        V = sysInput.Key.V,

        [EnumMember(Value = "W")]
        W = sysInput.Key.W,

        [EnumMember(Value = "X")]
        X = sysInput.Key.X,

        [EnumMember(Value = "Y")]
        Y = sysInput.Key.Y,

        [EnumMember(Value = "Z")]
        Z = sysInput.Key.Z,

        [EnumMember(Value = "0")]
        D0 = sysInput.Key.D0,

        [EnumMember(Value = "1")]
        D1 = sysInput.Key.D1,

        [EnumMember(Value = "2")]
        D2 = sysInput.Key.D2,

        [EnumMember(Value = "3")]
        D3 = sysInput.Key.D3,

        [EnumMember(Value = "4")]
        D4 = sysInput.Key.D4,

        [EnumMember(Value = "5")]
        D5 = sysInput.Key.D5,

        [EnumMember(Value = "6")]
        D6 = sysInput.Key.D6,

        [EnumMember(Value = "7")]
        D7 = sysInput.Key.D7,

        [EnumMember(Value = "8")]
        D8 = sysInput.Key.D8,

        [EnumMember(Value = "9")]
        D9 = sysInput.Key.D9,

        [EnumMember(Value = "F1")]
        F1 = sysInput.Key.F1,

        [EnumMember(Value = "F2")]
        F2 = sysInput.Key.F2,

        [EnumMember(Value = "F3")]
        F3 = sysInput.Key.F3,

        [EnumMember(Value = "F4")]
        F4 = sysInput.Key.F4,

        [EnumMember(Value = "F5")]
        F5 = sysInput.Key.F5,

        [EnumMember(Value = "F6")]
        F6 = sysInput.Key.F6,

        [EnumMember(Value = "F7")]
        F7 = sysInput.Key.F7,

        [EnumMember(Value = "F8")]
        F8 = sysInput.Key.F8,

        [EnumMember(Value = "F9")]
        F9 = sysInput.Key.F9,

        [EnumMember(Value = "F10")]
        F10 = sysInput.Key.F10,

        [EnumMember(Value = "F11")]
        F11 = sysInput.Key.F11,

        [EnumMember(Value = "F12")]
        F12 = sysInput.Key.F12,
    }

    [Flags]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MultModifierKey
    {
        [EnumMember(Value = "-")]
        None = sysInput.ModifierKeys.None,

        [EnumMember(Value = "Alt+")]
        Alt = sysInput.ModifierKeys.Alt,

        [EnumMember(Value = "Shift+")]
        Shift = sysInput.ModifierKeys.Shift,

        [EnumMember(Value = "Shift+Alt+")]
        Shift_Alt = sysInput.ModifierKeys.Shift | sysInput.ModifierKeys.Alt,

        [EnumMember(Value = "Ctrl+")]
        Ctrl = sysInput.ModifierKeys.Control,

        [EnumMember(Value = "Ctrl+Alt+")]
        Ctrl_Alt = sysInput.ModifierKeys.Control | sysInput.ModifierKeys.Alt,

        [EnumMember(Value = "Ctrl+ Shift+")]
        Ctrl_Shift = sysInput.ModifierKeys.Control | sysInput.ModifierKeys.Shift,

        [EnumMember(Value = "Ctrl+Shift+Alt+")]
        Ctrl_Shift_Alt = sysInput.ModifierKeys.Control | sysInput.ModifierKeys.Shift | sysInput.ModifierKeys.Alt
    }
    #endregion Public

    #endregion Enums
}
