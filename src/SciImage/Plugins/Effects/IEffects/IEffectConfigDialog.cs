using System;
using System.Windows.Forms;

namespace SciImage.Plugins.Effects.IEffects
{
    public interface IEffectConfigDialog:IDisposable
    {
        Effect EffectControl { get; set; }
        void Close();
        void Hide();
        DialogResult ShowDialog(IWin32Window owner);
    }
}
