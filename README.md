# WiFiQRCode

A command-line tool of generating Wi-Fi network config to QR code, which is based on .NET Core 2.1 ,[ImageSharp](https://github.com/SixLabors/ImageSharp) and [Zxing.Net](https://github.com/micjahn/ZXing.Net).

> Scanning QR Code to configure the device's Wi-Fi only support **Android** and **iOS 11+**.

# Environment Requires

* [Microsoft .NET Core 2.1 Runtime](https://www.microsoft.com/net/download/dotnet-core/2.1)

> **Verify** that after successful installation of .NET Core Runtime:
>
> ```shell
> $ dotnet --version
> ```

# File Structure

```
─┬─ Microsoft.Win32.SystemEvents.dll.dll
 ├─ SixLabors.Core.dll
 ├─ SixLabors.ImageSharp.dll
 ├─ System.Drawing.Common.dll
 ├─ System.Runtime.CompilerServices.Unsafe.dll
 ├─ WiFiQRCode.dll
 ├─ WiFiQRCode.runtimeconfig.json
 └─ zxing.dll
```

# Parameters Description

| Parameter                | Desciption                                             | Options                 | Default Value     | Required           |
|:-------------------------|:-------------------------------------------------------|:------------------------|:------------------|:------------------:|
| -a,<br />--auth_type     | Wi-Fi authentication.                                  | WEP/WPA/WPA2/nopass     | WPA2              | :white_check_mark: |
| -s,<br />--ssid          | Wi-Fi SSID.                                            |                         |                   | :white_check_mark: |
| -p,<br />--password      | Wi-Fi password.                                        |                         |                   |                    |
| -h,<br />--hidden        | Wi-Fi is hidden or not.                                |                         |                   |                    |
| -m,<br />--mode          | Generate mode.                                         | display/image/both      | image             |                    |
| -it,<br />--image_type   | The output image type,<br />when mode is image/both.   | bmp/gif/jpeg/jpg/png    | png               |                    |
| -iw,<br />--image_width  | The output image width,<br />when mode is image/both.  | value >= 41             | 200               |                    |
| -ih,<br />--image_height | The output image height,<br />when mode is image/both. | value >= 41             | 200               |                    |
| -od,<br />--output_dir   | The output directory,<br />when mode is image/both.    |                         | Current directory |                    |

# Usage

| Terminal                                                                                                                     | Image                                                                                                          |
|:----------------------------------------------------------------------------------------------------------------------------:|:--------------------------------------------------------------------------------------------------------------:|
| ![example-qrcode-terminal.png](https://blog.holey.cc/2019/01/13/csharp-generate-wifi-qrcode/example-qrcode-terminal_min.png) | ![example-qrcode-image](https://blog.holey.cc/2019/01/13/csharp-generate-wifi-qrcode/example-qrcode-image.png) |

## Display Wi-Fi QR Code on Terminal

```shell
$ dotnet WiFiQRCode.dll --auth_type=WPA2 --ssid=example-qrcode --password=example-pass --mode=display
```

## Output Wi-Fi QR Code to BMP file

```shell
$ dotnet WiFiQRCode.dll --auth_type=WPA2 --ssid=example-qrcode --password=example-pass --mode=image --image_type=bmp
```

## Output Wi-Fi QR Code to BMP file in parent directory

```shell
$ dotnet WiFiQRCode.dll --auth_type=WPA2--ssid=example-qrcode --password=example-pass --mode=image --image_type=bmp --output_dir=../
```

## Run application and enter each value

```shell
$ dotnet WiFiQRCode.dll
Wi-Fi Authentication Type :
Wi-Fi SSID                :
Wi-Fi Password            :
Generate Mode             :

# If generate mode is image/both
Image Type                :
Image Width               :
Image Height              :
Output Directory          :
```

# References

* [Wi-Fi Network config (Android, iOS 11+) - ZXing](https://github.com/zxing/zxing/wiki/Barcode-Contents?fbclid=IwAR25PM4tTjhFeEs14FBN936o0hTTSA7oBnXUvSyRGE6n3SW3e0jBhp5PwkM#wi-fi-network-config-android-ios-11)