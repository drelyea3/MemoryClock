using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryClock.Commands
{
    public class OpenSettingsCommand : CustomRoutedCommand { }
    public class CloseSettingsCommand : CustomRoutedCommand { }
    public class SaveSettingsCommand : CustomRoutedCommand { }
    public class StartTestingCommand : CustomRoutedCommand { }
    public class StopTestingCommand : CustomRoutedCommand { }
    public class AccessGrantedCommand : CustomRoutedCommand { }
    public class AccessRevokedCommand : CustomRoutedCommand { }
}
