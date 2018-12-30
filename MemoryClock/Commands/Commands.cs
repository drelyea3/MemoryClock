using Common;

namespace MemoryClock.Commands
{
    public class OpenSettingsCommand : CustomRoutedCommand { }
    public class CloseSettingsCommand : CustomRoutedCommand { }
    public class SaveSettingsCommand : CustomRoutedCommand { }
    public class StartTestingCommand : CustomRoutedCommand { }
    public class StopTestingCommand : CustomRoutedCommand { }
    public class AccessGrantedCommand : CustomRoutedCommand { }
    public class AccessRevokedCommand : CustomRoutedCommand { }
    public class QuitApplicationCommand : CustomRoutedCommand { }
}
