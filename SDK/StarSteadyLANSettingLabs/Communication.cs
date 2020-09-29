using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

using StarIOPort;
using Windows.Storage.Streams;

namespace StarSteadyLANSettingLabs
{
    public class CommunicationResult
    {
        public Communication.Result Result { get; set; } = Communication.Result.ErrorUnknown;

        public int Code { get; set; } = StarResultCode.ErrorFailed;
    }

    public class SteadyLANSettingResult
    {
        public string Title { get; set; }
        public string Message { get; set; }

        public CommunicationResult CommunicationResult { get; set; }
    }

    public class Communication
    {
        public enum Result
        {
            Success,
            ErrorUnknown,
            ErrorOpenPort,
            ErrorBeginCheckedBlock,
            ErrorEndCheckedBlock,
            ErrorWritePort,
            ErrorReadPort,
        }


        public static async Task<CommunicationResult> SendCommands(IBuffer buffer, string portName, string portSettings, int timeout)
        {
            Result result = Result.ErrorUnknown;
            int code = StarResultCode.ErrorFailed;

            try
            {
                using (StarIOPort.Port port = new StarIOPort.Port(timeout))
                {
                    result = Result.ErrorOpenPort;
                    await port.ConnectAsync(portName, portSettings);

                    StarIOPort.Status status;


                    result = Result.ErrorWritePort;
                    status = await port.GetParsedStatusAsync();

                    if (status.Offline == true)
                    {
                        return new CommunicationResult()
                        {
                            Result = result,
                            Code = code
                        };
                    }


                    result = Result.ErrorWritePort;
                    if (await port.WriteAsync(buffer) != buffer.Length)
                    {
                        return new CommunicationResult()
                        {
                            Result = result,
                            Code = code
                        };
                    }

                    result = Result.Success;
                    code = StarResultCode.Succeeded;
                }
            }
            catch (Exception ex)
            {
                code = ex.HResult;
            }

            return new CommunicationResult()
            {
                Result = result,
                Code = code
            };
        }

 
        public static async Task<SteadyLANSettingResult> ConfirmSteadyLANSetting(string portName, string portSettings, int timeout)
        {
            SteadyLANSettingResult steadyLANSettingResult = new SteadyLANSettingResult();
            steadyLANSettingResult.Title = null;
            steadyLANSettingResult.Message = null;
            bool parseResult = false;
            string message = null;
            int amountRead = 0;
            IBuffer readBuffer;

            Result result = Result.ErrorUnknown;
            int code = StarResultCode.ErrorFailed;
            List<byte> readData = new List<byte>();

            try
            {
                using (StarIOPort.Port port = new StarIOPort.Port(timeout))
                {
                    result = Result.ErrorOpenPort;
                    await port.ConnectAsync(portName, portSettings);

                    StarIOPort.Status status;

                    result = Result.ErrorWritePort;
                    status = await port.GetParsedStatusAsync();

                    if (status.Offline == true)
                    {
                        steadyLANSettingResult.CommunicationResult = new CommunicationResult()
                        {
                            Result = result,
                            Code = code
                        };

                        steadyLANSettingResult.Title = "Communication Result";
                        steadyLANSettingResult.Message = GetCommunicationResultMessage(steadyLANSettingResult.CommunicationResult);

                        return steadyLANSettingResult;
                    }


                    result = Result.ErrorWritePort;
                    byte[] commands = new byte[] { 0x1b, 0x1d, 0x29, 0x4e, 0x02, 0x00, 0x49, 0x01 };  //confirm SteadyLAN setting

                    if (await port.WriteAsync(commands.AsBuffer()) != commands.Length)
                    {
                        steadyLANSettingResult.CommunicationResult = new CommunicationResult()
                        {
                            Result = result,
                            Code = code
                        };

                        steadyLANSettingResult.Title = "Communication Result";
                        steadyLANSettingResult.Message = GetCommunicationResultMessage(steadyLANSettingResult.CommunicationResult);

                        return steadyLANSettingResult;
                    }

                    UInt32 startime = (UInt32)Environment.TickCount;
                    while (true)
                    {
                        if (((UInt32)Environment.TickCount - startime) > 3000)//3000msec
                        {
                            result = Result.ErrorReadPort;
                            break;
                        }

                        result = Result.ErrorReadPort;
                        readBuffer = await port.ReadAsync();

                        if (readBuffer.Length == 0)
                        {
                            continue;
                        }

                        readData.AddRange(readBuffer.ToArray());

                        byte[] readBytes = readData.ToArray();

                        amountRead += (int)readBuffer.Length;

                        //Check the steadyLAN setting value
                        //The following format is transmitted.
                        //  0x1b 0x1d 0x29 0x4e 0x02 0x00 0x49 0x01 [n] 0x0a 0x00
                        //The value of [n] indicates the SteadyLAN setting.
                        //  0x00: Invalid, 0x01: Valid(For iOS), 0x02: Valid(For Android), 0x03: Valid(For Windows)
                        if (amountRead >= 11)
                        {
                            for (int i = 0; i < amountRead; i++)
                            {
                                if (readBytes[i + 0] == 0x1b &&
                                    readBytes[i + 1] == 0x1d &&
                                    readBytes[i + 2] == 0x29 &&
                                    readBytes[i + 3] == 0x4e &&
                                    readBytes[i + 4] == 0x02 &&
                                    readBytes[i + 5] == 0x00 &&
                                    readBytes[i + 6] == 0x49 &&
                                    readBytes[i + 7] == 0x01 &&
                                 // readBytes[i + 8] is stored the steadylan setting value.
                                    readBytes[i + 9] == 0x0a &&
                                    readBytes[i + 10] == 0x00)
                                {
                                    switch (readBytes[i + 8])
                                    {
                                    //  case 0x00:
                                        default:
                                            message = "SteadyLAN(Disable).";
                                            break;
                                        case 0x01:
                                            message = "SteadyLAN(for iOS).";
                                            break;
                                        case 0x02:
                                            message = "SteadyLAN(for Android).";
                                            break;
                                        case 0x03:
                                            message = "SteadyLAN(for Windows).";
                                            break;
                                    }

                                    result = Result.Success;
                                    code = StarResultCode.Succeeded;
                                    parseResult = true;
                                    break;
                                }
                            }
                        }


                        if (parseResult)
                        {
                             break;
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                code = ex.HResult;
            }

            steadyLANSettingResult.CommunicationResult = new CommunicationResult()
            {
                Result = result,
                Code = code
            };

            if (result == Result.Success)
            {
                steadyLANSettingResult.Title = "SteadyLAN Setting";
                steadyLANSettingResult.Message = message;
            }
            else
            {
                steadyLANSettingResult.Title = "Communication Result";
                steadyLANSettingResult.Message = GetCommunicationResultMessage(steadyLANSettingResult.CommunicationResult);
            }

            return steadyLANSettingResult;
        }

        public static string GetCommunicationResultMessage(CommunicationResult result)
        {
            StringBuilder builder = new StringBuilder();

            switch (result.Result)
            {
                case Result.Success:
                    builder.Append("Success!");
                    break;
                case Result.ErrorOpenPort:
                    builder.Append("Fail to openPort");
                    break;
                case Result.ErrorBeginCheckedBlock:
                    builder.Append("Printer is offline (BeginCheckedBlock)");
                    break;
                case Result.ErrorEndCheckedBlock:
                    builder.Append("Printer is offline (EndCheckedBlock)");
                    break;
                case Result.ErrorReadPort:
                    builder.Append("Read port error (ReadPort)");
                    break;
                case Result.ErrorWritePort:
                    builder.Append("Write port error (WritePort)");
                    break;
                default:
                case Result.ErrorUnknown:
                    builder.Append("Unknown error");
                    break;
            }

            if (result.Result != Result.Success)
            {
                builder.Append(Environment.NewLine);
                builder.Append(Environment.NewLine);
                builder.Append("Error code: ");
                builder.Append(result.Code.ToString());

                if (result.Code == StarResultCode.ErrorFailed)
                {
                    builder.Append(" (Failed)");
                }
                else if (result.Code == StarResultCode.ErrorInUse)
                {
                    builder.Append(" (In use)");
                }
            }

            return builder.ToString();
        }

    }
}
