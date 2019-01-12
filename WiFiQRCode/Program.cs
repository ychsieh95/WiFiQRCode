using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Drawing;
using System.IO;
using static WiFiQRCode.LogExtensions;

namespace WiFiQRCode
{
    class Program
    {
        static string authType;
        static string ssid;
        static string password;
        static string hidden = "no";
        static string mode = "image";
        static string imageType = "png";
        static string imageWidth = "200";
        static string imageHeight = "200";
        static string outputDir = Environment.CurrentDirectory;

        static void Main(string[] args)
        {
            try
            {
                if (args.Length <= 0)
                {
                    Console.Write("Wi-Fi Authentication Type : "); Program.authType = Console.ReadLine();
                    Console.Write("Wi-Fi SSID                : "); ssid = Console.ReadLine();
                    Console.Write("Wi-Fi Password            : "); password = GetHiddenConsoleInput(); Console.WriteLine();
                    Console.Write("Wi-Fi Is Hidden           : "); hidden = GetHiddenConsoleInput(); Console.WriteLine();
                    Console.Write("Generate Mode             : ");
                    if (Enum.TryParse(enumType: typeof(GenerateMode), value: Console.ReadLine(), ignoreCase: true, out object mode))
                        switch ((GenerateMode)mode)
                        {
                            case GenerateMode.Image:
                            case GenerateMode.Both:
                                Console.Write("Image Type                : "); imageType = Console.ReadLine();
                                Console.Write("Image Width               : "); imageWidth = Console.ReadLine();
                                Console.Write("Image Height              : "); imageHeight = Console.ReadLine();
                                Console.Write("Output Directory          : "); outputDir = Console.ReadLine();
                                break;
                        }
                }
                else
                {
                    for (int i = 0; i < args.Length; i++)
                        if (args[i].Substring(0, 2) != "--")
                            switch (args[i])
                            {
                                case "-a": Program.authType = args[++i]; break;
                                case "-s": ssid = args[++i]; break;
                                case "-p": password = args[++i]; break;
                                case "-h": hidden = args[++i]; break;
                                case "-m": mode = args[++i]; break;
                                case "-it": imageType = args[++i]; break;
                                case "-iw": imageWidth = args[++i]; break;
                                case "-ih": imageHeight = args[++i]; break;
                                case "-od": outputDir = args[++i]; break;
                            }
                        else
                        {
                            string param = args[i].Remove(args[i].IndexOf('=') + 1);
                            string value = args[i].Substring(args[i].IndexOf('=') + 1);
                            switch (param)
                            {
                                case "--auth_type=": Program.authType = value; break;
                                case "--ssid=": ssid = value; break;
                                case "--password=": password = value; break;
                                case "--hidden=": hidden = value; break;
                                case "--mode=": mode = value; break;
                                case "--image_type=": imageType = value; break;
                                case "--image_width=": imageWidth = value; break;
                                case "--image_height=": imageHeight = value; break;
                                case "--output_dir=": outputDir = value; break;
                            }
                        }
                }
            }
            catch (Exception ex)
            {
                Log(contents: "Error input.", messageType: MessageType.ERROR);
                Log(contents: ex.Message, messageType: MessageType.ERROR);
                Console.WriteLine();
                return;
            }

            if (!Enum.TryParse(enumType: typeof(AuthenticationType), value: Program.authType, ignoreCase: true, out object authType))
                Log(contents: $@"""{ Program.authType }"" is NOT support for Wi-Fi Authentication Type (only WEP/WPA/WPA2 now)", messageType: MessageType.ERROR);
            else if (string.IsNullOrEmpty(ssid))
                Log(contents: $@"Wi-Fi SSID can NOT be empty", messageType: MessageType.ERROR);
            else if (!Enum.TryParse(enumType: typeof(GenerateMode), value: Program.mode, ignoreCase: true, out object mode))
                Log(contents: $@"""{ Program.authType }"" is NOT support for generate mode (only image/display/both now)", messageType: MessageType.ERROR);
            else if (hidden.ToLower() != "yes" && hidden.ToLower() != "no")
                Log(contents: $@"""{ hidden }"" is NOT support for hidden (only yes/no now)", messageType: MessageType.ERROR);
            else if ((AuthenticationType)authType == AuthenticationType.nopass && string.IsNullOrEmpty(password))
                Log(contents: $@"Password can NOT be empty", messageType: MessageType.ERROR);
            else
            {
                /*
                 * Generate full path of output directory.
                 */
                if (!Directory.Exists(outputDir))
                    Directory.CreateDirectory(outputDir);
                outputDir = Path.GetFullPath(outputDir).Replace('\\', '/').TrimEnd('/');

                /*
                 * Order of fields does not matter.
                 * Special characters "", ";", "," and ":" should be escaped with a backslash ("") as in MECARD encoding.
                 * For example, if an SSID was literally "foo;bar\baz" (with double quotes part of the SSID name itself)
                 * then it would be encoded like: WIFI:S:\"foo\;bar\\baz\";;
                 */
                ssid = ssid.Replace(@"""", @"\""").Replace(";", @"\;").Replace(",", @"\,").Replace(":", @"\:");
                password = password.Replace(@"""", @"\""").Replace(";", @"\;").Replace(",", @"\,").Replace(":", @"\:");

                /*
                 * Generate Wi-Fi Network config.
                 */
                string wifiContext = "";
                wifiContext += $"WIFI:T:{((AuthenticationType)authType).ToString()};";
                wifiContext += $"S:{ssid};";
                wifiContext += (AuthenticationType)authType != AuthenticationType.nopass ? $"P:{password};" : ";";
                wifiContext += hidden.ToLower() == "yes" ? "H:True;" : ";";

                /*
                 * Generate QR code or display on terminal.
                 */
                switch ((GenerateMode)mode)
                {
                    case GenerateMode.Image:
                    case GenerateMode.Both:
                        {
                            if (!Enum.TryParse(enumType: typeof(ImageType), value: Program.imageType, ignoreCase: true, out object imageType))
                                Log(contents: $@"""{ imageType }"" is NOT support for image type (only bmp/jpeg/jpg/png now)", messageType: MessageType.ERROR);
                            else if (!int.TryParse(imageWidth, out int width))
                                Log(contents: $@"""{ imageWidth }"" is NOT a numeric for image width", messageType: MessageType.ERROR);
                            else if (!int.TryParse(imageHeight, out int height))
                                Log(contents: $@"""{ imageHeight }"" is NOT a numeric for image height", messageType: MessageType.ERROR);
                            else
                            {
                                byte[] qrcodeBytes = wifiContext.ToBarcodeBytes(size: new Size(width, height));
                                using (var image = SixLabors.ImageSharp.Image.LoadPixelData<Rgba32>(qrcodeBytes, width, height))
                                {
                                    string filename = $"{string.Join("_", ssid.Split(Path.GetInvalidFileNameChars()))}.{imageType.ToString().ToLower()}";
                                    image.Save($"{outputDir}/{filename}");
                                    if ((GenerateMode)mode == GenerateMode.Both)
                                    {
                                        Console.WriteLine();
                                        DisplayQRCode(
                                            qrcodeBytes: wifiContext.ToBarcodeBytes(new Size(50, 50)),
                                            width: 50,
                                            height: 50);
                                        Console.WriteLine();
                                    }
                                    Log(contents: $"Generate Wi-Fi QRCode successfully ({outputDir}/{filename})", messageType: MessageType.INFO);
                                }
                            }
                            break;
                        }
                    case GenerateMode.Display:
                        {
                            Console.WriteLine();
                            DisplayQRCode(
                                qrcodeBytes: wifiContext.ToBarcodeBytes(size: new Size(50, 50)),
                                width: 50,
                                height: 50);
                            Console.WriteLine();
                            Log(contents: "Generate Wi-Fi QRCode successfully", messageType: MessageType.INFO);
                            break;
                        }
                }
            }
        }

        static void DisplayQRCode(byte[] qrcodeBytes, int width, int height)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (qrcodeBytes[y * width * 4 + x * 4] > 128)
                        Console.BackgroundColor = ConsoleColor.White;
                    else
                        Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write("  ");
                }
                Console.ResetColor();
                Console.WriteLine();
            }
        }

        static string GetHiddenConsoleInput()
        {
            var input = new System.Text.StringBuilder();
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter) break;
                if (key.Key == ConsoleKey.Backspace && input.Length > 0) input.Remove(input.Length - 1, 1);
                else if (key.Key != ConsoleKey.Backspace) input.Append(key.KeyChar);
            }
            return input.ToString();
        }

        enum AuthenticationType
        {
            nopass,
            WEP,
            WPA,
            WPA2
        }

        enum GenerateMode
        {
            Display,
            Image,
            Both
        }

        enum ImageType
        {
            BMP,
            GIF,
            JPEG,
            JPG,
            PNG
        }
    }
}
