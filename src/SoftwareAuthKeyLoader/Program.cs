using Mono.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SoftwareAuthKeyLoader
{
    class Program
    {
        static int Main(string[] args)
        {
            bool help = false;
            bool quiet = false;
            bool verbose = false;
            string ip = "192.168.128.1";
            string port = "49165";
            string timeout = "5000";
            bool load = false;
            bool zeroize = false;
            bool read = false;
            bool device = false;
            bool active = false;
            bool named = false;
            string wacn = string.Empty;
            string system = string.Empty;
            string unit = string.Empty;
            string key = string.Empty;

            Settings.OutputLevel = Output.Level.Info; // default to info to show command line option parsing errors

            OptionSet commandLineOptions = new OptionSet
            {
                { "h|?|help", "show this message and exit", h => help = h != null },
                { "q|quiet", "do not show output", q => quiet = q != null },
                { "v|verbose", "show debug messages", v => verbose = v != null },
                { "i=|ip=", "radio ip address [default 192.168.128.1]", i => ip = i },
                { "p=|port=", "radio udp port number [default 49165]", p => port = p },
                { "t=|timeout=", "radio receive timeout (ms) [default 5000]", t => timeout = t },
                { "l|load", "load key", l => load = l != null },
                { "z|zeroize", "zeroize key(s)", z => zeroize = z != null },
                { "r|read", "read key(s)", r => read = r != null },
                { "d|device", "device scope", d => device = d != null },
                { "a|active", "active scope", a => active = a != null },
                { "n|named", "named scope", n => named = n != null },
                { "w=|wacn=", "wacn id (hex)", w => wacn = w },
                { "s=|system=", "system id (hex)", s => system = s },
                { "u=|unit=", "unit id (hex)", u => unit = u },
                { "k=|key=", "aes-128 encryption key (hex)", k => key = k }
            };

            try
            {
                commandLineOptions.Parse(args);
            }
            catch (OptionException ex)
            {
                Output.ErrorLine(ex.Message);
                ExitPrompt();
                return -1;
            }

            if (quiet && verbose)
            {
                Output.ErrorLine("quiet and verbose both specified");
                ExitPrompt();
                return -1;
            }
            else if (quiet)
            {
                Settings.OutputLevel = Output.Level.None;
            }
            else if (verbose)
            {
                Settings.OutputLevel = Output.Level.Debug;
            }
            else
            {
                Settings.OutputLevel = Output.Level.Info;
            }

            Output.InfoLine("Software P25 Link Layer Authentication Key Loader");
            Output.InfoLine("Supports Manual Rekeying Features for Authentication per TIA-102.AACD-A");
            Output.InfoLine("Copyright 2019 Daniel Dugger");
            Output.InfoLine("Version: {0}", Settings.ApplicationVersion);
            Output.InfoLine("*** NOT FOR PRODUCTION USE ***");
            Output.InfoLine();

            if (help || args.Length == 0)
            {
                ShowHelp(commandLineOptions);
                ExitPrompt();
                return 0;
            }

            Output.DebugLine("help: {0}", help);
            Output.DebugLine("quiet: {0}", quiet);
            Output.DebugLine("verbose: {0}", verbose);
            Output.DebugLine("ip: {0}", ip);
            Output.DebugLine("port: {0}", port);
            Output.DebugLine("timeout: {0}", timeout);
            Output.DebugLine("load: {0}", load);
            Output.DebugLine("zeroize: {0}", zeroize);
            Output.DebugLine("read: {0}", read);
            Output.DebugLine("device: {0}", device);
            Output.DebugLine("active: {0}", active);
            Output.DebugLine("named: {0}", named);
            Output.DebugLine("wacn: {0}", wacn);
            Output.DebugLine("system: {0}", system);
            Output.DebugLine("unit: {0}", unit);
            Output.DebugLine("key: {0}", key);

            try
            {
                Settings.IpAddress = Network.ParseIpAddress(ip);
            }
            catch (Exception ex)
            {
                Output.ErrorLine("ip invalid: {0}", ex.Message);
                ExitPrompt();
                return -1;
            }

            try
            {
                Settings.UdpPort = Network.ParseUdpPort(port);
            }
            catch (Exception ex)
            {
                Output.ErrorLine("port invalid: {0}", ex.Message);
                ExitPrompt();
                return -1;
            }

            try
            {
                Settings.Timeout = Network.ParseTimeout(timeout);
            }
            catch (Exception ex)
            {
                Output.ErrorLine("timeout invalid: {0}", ex.Message);
                ExitPrompt();
                return -1;
            }

            if ((load && zeroize) || (load && read) || (zeroize && read) || (load && zeroize && read))
            {
                Output.ErrorLine("multiple actions specified");
                ExitPrompt();
                return -1;
            }

            if ((device && active) || (device && named) || (active && named) || (device && active && named))
            {
                Output.ErrorLine("multiple scopes specified");
                ExitPrompt();
                return -1;
            }

            byte[] keyData = new byte[0];

            if (load)
            {
                if (key.Equals(string.Empty))
                {
                    Output.ErrorLine("key missing");
                    ExitPrompt();
                    return -1;
                }

                if (!OnlyContainsHexCharacters(key))
                {
                    Output.ErrorLine("key invalid: contains character(s) other than [0-9] [a-f] [A-F]");
                    ExitPrompt();
                    return -1;
                }

                if (key.Length != 32)
                {
                    Output.ErrorLine("key invalid: expected 32 characters, got {0}", key.Length);
                    ExitPrompt();
                    return -1;
                }

                try
                {
                    keyData = ByteStringToByteArray(key);
                }
                catch (Exception ex)
                {
                    Output.ErrorLine("key invalid: {0}", ex.Message);
                    ExitPrompt();
                    return -1;
                }
            }

            int wacnId = 0;
            int systemId = 0;
            int unitId = 0;

            if (named)
            {
                if (wacn.Equals(string.Empty))
                {
                    Output.ErrorLine("wacn missing");
                    ExitPrompt();
                    return -1;
                }

                if (!OnlyContainsHexCharacters(wacn))
                {
                    Output.ErrorLine("wacn invalid: contains character(s) other than [0-9] [a-f] [A-F]");
                    ExitPrompt();
                    return -1;
                }

                if (wacn.Length > 5)
                {
                    Output.ErrorLine("wacn invalid: expected max 5 characters, got {0}", key.Length);
                    ExitPrompt();
                    return -1;
                }

                try
                {
                    wacnId = int.Parse(wacn, NumberStyles.HexNumber);
                }
                catch (Exception ex)
                {
                    Output.ErrorLine("wacn invalid: {0}", ex.Message);
                    ExitPrompt();
                    return -1;
                }

                if (system.Equals(string.Empty))
                {
                    Output.ErrorLine("system missing");
                    ExitPrompt();
                    return -1;
                }

                if (!OnlyContainsHexCharacters(system))
                {
                    Output.ErrorLine("system invalid: contains character(s) other than [0-9] [a-f] [A-F]");
                    ExitPrompt();
                    return -1;
                }

                if (system.Length > 3)
                {
                    Output.ErrorLine("system invalid: expected max 3 characters, got {0}", key.Length);
                    ExitPrompt();
                    return -1;
                }

                try
                {
                    systemId = int.Parse(system, NumberStyles.HexNumber);
                }
                catch (Exception ex)
                {
                    Output.ErrorLine("system invalid: {0}", ex.Message);
                    ExitPrompt();
                    return -1;
                }

                if (unit.Equals(string.Empty))
                {
                    Output.ErrorLine("unit missing");
                    ExitPrompt();
                    return -1;
                }

                if (!OnlyContainsHexCharacters(unit))
                {
                    Output.ErrorLine("unit invalid: contains character(s) other than [0-9] [a-f] [A-F]");
                    ExitPrompt();
                    return -1;
                }

                if (unit.Length > 6)
                {
                    Output.ErrorLine("unit invalid: expected max 6 characters, got {0}", key.Length);
                    ExitPrompt();
                    return -1;
                }

                try
                {
                    unitId = int.Parse(unit, NumberStyles.HexNumber);
                }
                catch (Exception ex)
                {
                    Output.ErrorLine("unit invalid: {0}", ex.Message);
                    ExitPrompt();
                    return -1;
                }
            }

            if (load)
            {
                if (device)
                {
                    Output.ErrorLine("device scope not supported for load action");
                    ExitPrompt();
                    return -1;
                }
                else if (active)
                {
                    int result = -1;

                    try
                    {
                        result = Actions.LoadAuthenticationKey(false, 0, 0, 0, keyData);
                    }
                    catch (Exception ex)
                    {
                        Output.ErrorLine("error during load operation: {0}\r\n{1}\r\n{2}", ex.Message, ex.TargetSite, ex.StackTrace);
                        ExitPrompt();
                        return -1;
                    }

                    if (result == 0)
                    {
                        Output.InfoLine("Loaded authentication key successfully");
                    }
                    else
                    {
                        Output.ErrorLine("Error while loading authentication key");
                    }

                    ExitPrompt();
                    return result;
                }
                else if (named)
                {
                    int result = -1;

                    try
                    {
                        result = Actions.LoadAuthenticationKey(true, wacnId, systemId, unitId, keyData);
                        
                    }
                    catch (Exception ex)
                    {
                        Output.ErrorLine("error during load operation: {0}\r\n{1}\r\n{2}", ex.Message, ex.TargetSite, ex.StackTrace);
                        ExitPrompt();
                        return -1;
                    }

                    if (result == 0)
                    {
                        Output.InfoLine("Loaded authentication key successfully");
                    }
                    else
                    {
                        Output.ErrorLine("Error while loading authentication key");
                    }

                    ExitPrompt();
                    return result;
                }
                else
                {
                    Output.ErrorLine("scope missing (only active and named scopes are supported for load action)");
                    ExitPrompt();
                    return -1;
                }
            }
            else if (zeroize)
            {
                if (device)
                {
                    int result = -1;

                    try
                    {
                        result = Actions.DeleteAuthenticationKey(false, true, 0, 0, 0);
                    }
                    catch (Exception ex)
                    {
                        Output.ErrorLine("error during zeroize operation: {0}\r\n{1}\r\n{2}", ex.Message, ex.TargetSite, ex.StackTrace);
                        ExitPrompt();
                        return -1;
                    }

                    if (result == 0)
                    {
                        Output.InfoLine("Zeroized all authentication keys successfully");
                    }
                    else
                    {
                        Output.ErrorLine("Error while zeroizing all authentication keys");
                    }

                    ExitPrompt();
                    return result;
                }
                else if (active)
                {
                    int result = -1;

                    try
                    {
                        result = Actions.DeleteAuthenticationKey(false, false, 0, 0, 0);
                    }
                    catch (Exception ex)
                    {
                        Output.ErrorLine("error during zeroize active operation: {0}\r\n{1}\r\n{2}", ex.Message, ex.TargetSite, ex.StackTrace);
                        ExitPrompt();
                        return -1;
                    }

                    if (result == 0)
                    {
                        Output.InfoLine("Zeroized active authentication key successfully");
                    }
                    else
                    {
                        Output.ErrorLine("Error while zeroizing active authentication key");
                    }

                    ExitPrompt();
                    return result;
                }
                else if (named)
                {
                    int result = -1;

                    try
                    {
                        result = Actions.DeleteAuthenticationKey(true, false, wacnId, systemId, unitId);
                    }
                    catch (Exception ex)
                    {
                        Output.ErrorLine("error during zeroize named operation: {0}\r\n{1}\r\n{2}", ex.Message, ex.TargetSite, ex.StackTrace);
                        ExitPrompt();
                        return -1;
                    }

                    if (result == 0)
                    {
                        Output.InfoLine("Zeroized named authentication key successfully");
                    }
                    else
                    {
                        Output.ErrorLine("Error while zeroizing named authentication key");
                    }

                    ExitPrompt();
                    return result;
                }
                else
                {
                    Output.ErrorLine("scope missing");
                    ExitPrompt();
                    return -1;
                }
            }
            else if (read)
            {
                if (device)
                {
                    int result = -1;

                    try
                    {
                        result = Actions.ListSuIdItems();
                    }
                    catch (Exception ex)
                    {
                        Output.ErrorLine("error during read device operation: {0}\r\n{1}\r\n{2}", ex.Message, ex.TargetSite, ex.StackTrace);
                        ExitPrompt();
                        return -1;
                    }

                    if (result == 0)
                    {
                        Output.InfoLine("Read device authentication key(s) successfully");
                    }
                    else
                    {
                        Output.ErrorLine("Error while reading device authentication key(s)");
                    }

                    ExitPrompt();
                    return result;
                }
                else if (active)
                {
                    int result = -1;

                    try
                    {
                        result = Actions.ListActiveSuId();
                    }
                    catch (Exception ex)
                    {
                        Output.ErrorLine("error during read active operation: {0}\r\n{1}\r\n{2}", ex.Message, ex.TargetSite, ex.StackTrace);
                        ExitPrompt();
                        return -1;
                    }

                    if (result == 0)
                    {
                        Output.InfoLine("Read active authentication key successfully");
                    }
                    else
                    {
                        Output.ErrorLine("Error while reading active authentication key");
                    }

                    ExitPrompt();
                    return result;
                }
                else if (named)
                {
                    Output.ErrorLine("named scope not supported for read action");
                    ExitPrompt();
                    return -1;
                }
                else
                {
                    Output.ErrorLine("scope missing (only device and active scopes are supported for read action)");
                    ExitPrompt();
                    return -1;
                }
            }
            else
            {
                Output.ErrorLine("action missing");
                ExitPrompt();
                return -1;
            }
        }

        private static bool OnlyContainsHexCharacters(string input)
        {
            return Regex.IsMatch(input, @"\A\b[0-9a-fA-F]+\b\Z");
        }

        private static byte[] ByteStringToByteArray(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        private static void ExitPrompt()
        {
            Output.InfoLine("Exiting...");
            //Output.InfoLine("Press any key to exit...");
            //Console.ReadKey();
        }

        private static void ShowHelp(OptionSet optionSet)
        {
            Output.InfoLine("Usage: sakl.exe [OPTIONS]");
            Output.InfoLine();

            Output.InfoLine("Options:");
            if (Settings.OutputLevel >= Output.Level.Info)
            {
                optionSet.WriteOptionDescriptions(Console.Out);
            }
            Output.InfoLine();

            Output.InfoLine("Examples:");
            Output.InfoLine("  load key to the active suid");
            Output.InfoLine("  /load /active /key 000102030405060708090a0b0c0d0e0f");
            Output.InfoLine();
            Output.InfoLine("  load key to the specified suid");
            Output.InfoLine("  /load /named /wacn a4398 /system f10 /unit 99b584 /key 000102030405060708090a0b0c0d0e0f");
            Output.InfoLine();
            Output.InfoLine("  zeroize all keys");
            Output.InfoLine("  /zeroize /device");
            Output.InfoLine();
            Output.InfoLine("  zeroize active key");
            Output.InfoLine("  /zeroize /active");
            Output.InfoLine();
            Output.InfoLine("  zeroize specified key");
            Output.InfoLine("  /zeroize /named /wacn a4398 /system f10 /unit 99b584");
            Output.InfoLine();
            Output.InfoLine("  read all keys");
            Output.InfoLine("  /read /device");
            Output.InfoLine();
            Output.InfoLine("  read active key");
            Output.InfoLine("  /read /active");
            Output.InfoLine();
        }
    }
}
