using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace MemoryClock.Controls
{
    public class RadioToggle : RadioButton
    {
        public RadioToggle()
        {
            this.DefaultStyleKey = typeof(ToggleButton);
        }
    }
}
