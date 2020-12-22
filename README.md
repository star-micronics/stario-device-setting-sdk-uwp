# StarIODeviceSetting SDK for Universal Windows Platform C#

For more information about the API, supported OS, development environment, and supported printers, please refer to the [manual](https://www.star-m.jp/products/s_print/sdk/starprnt_sdk/manual/uwp_csharp/en/api_stariodevicesetting_library.html).

## Reference
### Online Manual
- [mC-Print2 Online Manual](http://www.star-m.jp/mcprint2-oml.html)
- [mC-Print3 Online Manual](http://www.star-m.jp/mcprint3-oml.html)

## What is SteadyLAN
- [SteadyLAN (English)](https://www.star-m.jp/products/s_print/mcprint3/manual/en/settings/SteadyLAN.htm)
- [SteadyLAN (Japanese)](https://www.star-m.jp/products/s_print/mcprint3/manual/ja/settings/SteadyLAN.htm)

## Note
- SteadyLAN function is not available from the UWP application even if you set it to SteadyLAN for Windows because UWP application cannot communicate with the Star Printer via USB I/F.
This function is available for [Windows Desktop applications](https://github.com/star-micronics/stario-device-setting-sdk-windows-desktop) when you set to SteadyLAN for Windows.

- SteadyLAN function must be configured to match the operating system (OS) of the device. It cannot be used with an OS different from the printer's settings.
  For more information, please refer to the [manual](https://www.star-m.jp/products/s_print/sdk/starprnt_sdk/manual/uwp_csharp/en/api_stariodevicesetting_steaylan_setting.html).


## Copyright

Copyright 2020 Star Micronics Co., Ltd. All rights reserved.