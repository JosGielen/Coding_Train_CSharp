using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;


namespace Photo_Mozaic
{
    public partial class MainWindow : Window
    {
        private delegate void WaitDelegate();
        private readonly HttpClient client = new HttpClient();
        private int stage = 3; //1 if the APOD images need to be downloaded, 2 if thumbnails need to be made, 3 = Create the mozaic.
        private int dayCounter = 1;
        private BitmapImage bitmap;
        private int Stride;
        private byte[] PixelData;
        private int smallImageSize = 20;
        private string ThumbnailsDirectory = "Thumbnails20";
        private Random Rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }


        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (stage == 1)
            {
                //Download the APOD images and save them to disk.
                await GetAPODImagesAsync();
            }
            else if(stage == 2)
            {
                //Create thumbnails from the APOD images.
                CreateThumbnailImages();
            }
            else if(stage == 3)
            {
                //Create the Mozaic, Show it and save it to file.
                CreateMozaic();
            }
        }

        private async Task GetAPODImagesAsync()
        {
            string year;
            string month;
            string day;
            string htmlUri;
            string imageUri;
            string fileName;
            for (int i = 0; i < 3000; i++)
            {
                //Create the APOD html page URL
                DateTime currentDateTime = DateTime.Now.AddDays(-dayCounter);
                year = currentDateTime.Year.ToString().Substring(2, 2);
                month = currentDateTime.Month.ToString("00");
                day = currentDateTime.Day.ToString("00");
                htmlUri = "https://apod.nasa.gov/apod/ap" + year + month + day + ".html";
                //Get the image URL in that APOD html page
                Task<string> t = ReadHtmlAsync(htmlUri);
                imageUri = await t;
                //Download the APOD image and Save it in the Images folder
                if (imageUri.Length > 30)
                {
                    try
                    {
                        fileName = Environment.CurrentDirectory + "\\Images\\" + year + month + day + ".jpg";
                        await DownloadImageAsync(new Uri(imageUri), fileName);
                        Title = "Saving Image:" + imageUri;
                        Dispatcher.Invoke(new WaitDelegate(Wait), DispatcherPriority.ApplicationIdle);
                    }
                    catch (Exception)
                    {
                        //Do nothing
                    }
                }
                dayCounter++;
            }
        }

        private async Task<string> ReadHtmlAsync(string htmlFile)
        {
            string response;
            string[] lines;
            try
            {
                response = await client.GetStringAsync(new Uri(htmlFile));
                lines = response.Split("\n");
                foreach (string line in lines)
                {
                    if (line.Contains("\"image/") && line.Contains(".jpg"))
                    {
                        int startindex = line.LastIndexOf("image/") + 6;
                        int endindex = line.LastIndexOf(".jpg") + 4;
                        return "https://apod.nasa.gov/apod/image/" + line.Substring(startindex, endindex - startindex);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Debug.Print("\nException Caught for" + htmlFile);
                Debug.Print("Message :{0} ", ex.Message);
            }
            return "";
        }

        public async Task DownloadImageAsync(Uri uri, string fileName)
        {
            // Download the image and write to the file
            var imageBytes = await client.GetByteArrayAsync(uri);
            if (imageBytes != null && imageBytes.Length > 0)
            {
                await File.WriteAllBytesAsync(fileName, imageBytes);
            }
        }

        private void CreateThumbnailImages()
        {
            foreach (string f in Directory.GetFiles(Environment.CurrentDirectory + "\\Images"))
            {
                if (f.Contains(".jpg"))
                {
                    try
                    {
                        bitmap = new BitmapImage(new Uri(f));
                        if (bitmap == null) { continue; }
                        //Reduce the image size to smallImageSize x smallImageSize pixels
                        //Step 1 Square the image
                        //   Determine the pixel range of the max square that fits in the image
                        int startWidth, endWidth, startHeight, endHeight;
                        if (bitmap.PixelWidth > bitmap.PixelHeight)
                        {
                            startHeight = 0;
                            endHeight = bitmap.PixelHeight;
                            startWidth = (bitmap.PixelWidth - bitmap.PixelHeight) / 2;
                            endWidth = bitmap.PixelWidth - startWidth;
                        }
                        else
                        {
                            startHeight = (bitmap.PixelHeight - bitmap.PixelWidth) / 2;
                            endHeight = bitmap.PixelHeight - startHeight;
                            startWidth = 0;
                            endWidth = bitmap.PixelWidth;
                        }
                        //   Get the image pixels data
                        Stride = bitmap.PixelWidth * bitmap.Format.BitsPerPixel / 8;
                        PixelData = new byte[Stride * bitmap.PixelHeight];
                        bitmap.CopyPixels(PixelData, Stride, 0);
                        //Step 2 Get average pixel color for each thumbnail pixel
                        Color c;
                        double[] redtotals = new double[smallImageSize * smallImageSize];
                        double[] greentotals = new double[smallImageSize * smallImageSize];
                        double[] bluetotals = new double[smallImageSize * smallImageSize];
                        double[] pixelcounts = new double[smallImageSize * smallImageSize];
                        byte[] smallPixelData = new byte[smallImageSize * smallImageSize * bitmap.Format.BitsPerPixel / 8];
                        int smallIndex;
                        int pX, pY;
                        int Scale = (endWidth - startWidth) / smallImageSize;
                        //loop over each pixel of the thumbnail image
                        for (int smallrow = 0; smallrow < smallImageSize; smallrow++)
                        {
                            for (int smallcol = 0; smallcol < smallImageSize; smallcol++)
                            {
                                smallIndex = smallrow * smallImageSize + smallcol;
                                //Loop over each pixel of the bitmap that lies inside a pixel of the thumbnail image
                                for (int Y = 0; Y < Scale; Y++)
                                {
                                    for (int X = 0; X < Scale; X++)
                                    {
                                        pX = startWidth + smallcol * Scale + X;
                                        pY = startHeight + smallrow * Scale + Y;
                                        if (pX < bitmap.PixelWidth && pY < bitmap.PixelHeight)
                                        {
                                            c = getPixelColor(pX, pY);
                                            redtotals[smallIndex] += c.R;
                                            greentotals[smallIndex] += c.G;
                                            bluetotals[smallIndex] += c.B;
                                            pixelcounts[smallIndex] += 1;
                                        }
                                    }
                                }
                                if (bitmap.Format.BitsPerPixel / 8 >= 1) smallPixelData[smallIndex * bitmap.Format.BitsPerPixel / 8] = (byte)(bluetotals[smallIndex] / pixelcounts[smallIndex]);
                                if (bitmap.Format.BitsPerPixel / 8 >= 2) smallPixelData[smallIndex * bitmap.Format.BitsPerPixel / 8 + 1] = (byte)(greentotals[smallIndex] / pixelcounts[smallIndex]);
                                if (bitmap.Format.BitsPerPixel / 8 >= 3) smallPixelData[smallIndex * bitmap.Format.BitsPerPixel / 8 + 2] = (byte)(redtotals[smallIndex] / pixelcounts[smallIndex]);
                                if (bitmap.Format.BitsPerPixel / 8 >= 4) smallPixelData[smallIndex * bitmap.Format.BitsPerPixel / 8 + 3] = 255;
                            }
                        }
                        BitmapSource bmp = BitmapSource.Create(smallImageSize, smallImageSize, 96, 96, bitmap.Format, null, smallPixelData, smallImageSize * bitmap.Format.BitsPerPixel / 8);
                        string Thumbnailfile = Environment.CurrentDirectory + "\\" + ThumbnailsDirectory + "\\" + Path.GetFileName(f);
                        using (var fileStream = new FileStream(Thumbnailfile, FileMode.Create))
                        {
                            BitmapEncoder encoder = new JpegBitmapEncoder();
                            encoder.Frames.Add(BitmapFrame.Create(bmp));
                            encoder.Save(fileStream);
                        }
                        Title = "Creating Thumbnail Image:" + Path.GetFileName(f);
                        Dispatcher.Invoke(new WaitDelegate(Wait), DispatcherPriority.ApplicationIdle);
                    }
                    catch
                    {
                        //Do nothing
                    }
                }
            }
            CreateMozaic();
        }

        private Color getPixelColor(int x, int y)
        {
            int index = (y * bitmap.PixelWidth + x) * bitmap.Format.BitsPerPixel / 8;
            if (bitmap.Format.BitsPerPixel / 8 == 1)
            {
                return Color.FromRgb(PixelData[index], PixelData[index], PixelData[index]);
            }
            else
            {
                return Color.FromRgb(PixelData[index + 2], PixelData[index + 1], PixelData[index]);
            }
        }

        private void CreateMozaic()
        {
            //Create the Mozaic, Show it and save it to file.
            //Step1: Calculate the average RGB values of each thumbnail
            List<string> filenames = new List<string>();
            List<byte> redValues = new List<byte>();
            List<byte> greenValues = new List<byte>();
            List<byte> blueValues = new List<byte>();
            double red, green, blue;
            Color c;
            foreach (string f in Directory.GetFiles(Environment.CurrentDirectory + "\\" + ThumbnailsDirectory))
            {
                if (f.Contains(".jpg"))
                {
                    try
                    {
                        bitmap = new BitmapImage(new Uri(f));
                        if (bitmap == null) { continue; }
                        Stride = bitmap.PixelWidth * bitmap.Format.BitsPerPixel / 8;
                        PixelData = new byte[Stride * bitmap.PixelHeight];
                        bitmap.CopyPixels(PixelData, Stride, 0);
                        red = 0;
                        green = 0;
                        blue = 0;
                        for (int i = 0; i < smallImageSize; i++)
                        {
                            for (int j = 0; j < smallImageSize; j++)
                            {
                                c = getPixelColor(i, j);
                                red += c.R;
                                green += c.G;
                                blue += c.B;
                            }
                        }
                        filenames.Add(f);
                        redValues.Add((byte)(red / (smallImageSize * smallImageSize)));
                        greenValues.Add((byte)(green / (smallImageSize * smallImageSize)));
                        blueValues.Add((byte)(blue / (smallImageSize * smallImageSize)));
                    }
                    catch (Exception)
                    {
                        //Do nothing
                    }
                }
            }
            //Step 2: Determine the average color of each smallimage area of the main image.
            bitmap = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Earth4.jpg"));
            Stride = bitmap.PixelWidth * bitmap.Format.BitsPerPixel / 8;
            PixelData = new byte[Stride * bitmap.PixelHeight];
            bitmap.CopyPixels(PixelData, Stride, 0);
            int pX, pY;
            int ScaleX = bitmap.PixelWidth / smallImageSize;
            int ScaleY = bitmap.PixelHeight / smallImageSize;
            double[,] redtotals = new double[ScaleX, ScaleY];
            double[,] greentotals = new double[ScaleX, ScaleY];
            double[,] bluetotals = new double[ScaleX, ScaleY];
            double[,] pixelcounts = new double[ScaleX, ScaleY];
            //  loop over each smallImage area of the main image
            for (int row = 0; row < ScaleY; row++)
            {
                for (int col = 0; col < ScaleX; col++)
                {
                    //   Loop over each pixel of the main bitmap that lies inside the smallImage area
                    for (int Y = 0; Y < smallImageSize; Y++)
                    {
                        for (int X = 0; X < smallImageSize; X++)
                        {
                            pX = col * smallImageSize + X;
                            pY = row * smallImageSize + Y;
                            if (pX < bitmap.PixelWidth && pY < bitmap.PixelHeight)
                            {
                                c = getPixelColor(pX, pY);
                                redtotals[col,row] += c.R;
                                greentotals[col, row] += c.G;
                                bluetotals[col, row] += c.B;
                                pixelcounts[col, row] += 1;
                            }
                        }
                    }
                    redtotals[col, row] /= pixelcounts[col, row];
                    greentotals[col, row] /= pixelcounts[col, row];
                    bluetotals[col, row] /= pixelcounts[col, row];
                }
            }
            //Step3: For each smallImage area find the thumbnails with average RGB values within an accepted range
            List<int>[,] bestFitFiles = new List<int>[ScaleX, ScaleY];
            double acceptRange;
            //   loop over each smallImage area of the main image
            for (int row = 0; row < ScaleY; row++)
            {
                for (int col = 0; col < ScaleX; col++)
                {
                    acceptRange = 4;
                    bestFitFiles[col, row] = new List<int>();
                    do
                    {
                        acceptRange++;
                        //   Loop over each thumbnail (increase the accepted color deviation untill at least 5 matches are found).
                        for (int i = 0; i < filenames.Count - 1; i++)
                        {
                            if (Math.Abs(redValues[i] - redtotals[col, row]) < acceptRange && Math.Abs(greenValues[i] - greentotals[col, row]) < acceptRange && Math.Abs(blueValues[i] - bluetotals[col, row]) < acceptRange)
                            {
                                bestFitFiles[col, row].Add(i);
                            }
                        }
                    }while (bestFitFiles[col, row].Count < 5);
                }
            }
            //Step4: Replace the smallImage area with a random thumbnail out of the bestfitFiles list.
            WriteableBitmap newBitmap = new WriteableBitmap(bitmap.PixelWidth, bitmap.PixelHeight, bitmap.DpiX, bitmap.DpiY, bitmap.Format, bitmap.Palette);
            WriteableBitmap thumbnail;
            int chosenBestFitIndex;
            int chosenFileIndex;
            for (int row = 0; row < ScaleY; row++)
            {
                for (int col = 0; col < ScaleX; col++)
                {
                    chosenBestFitIndex = Rnd.Next(bestFitFiles[col, row].Count);
                    chosenFileIndex = bestFitFiles[col, row][chosenBestFitIndex];
                    bitmap = new BitmapImage(new Uri(filenames[chosenFileIndex]));

                    //Change to 32 bpp if needed
                    if (bitmap.Format.BitsPerPixel != 32)
                    {
                        FormatConvertedBitmap convertBitmap = new FormatConvertedBitmap(bitmap, PixelFormats.Bgra32, null, 0);
                        thumbnail = new WriteableBitmap(convertBitmap);
                    }
                    else
                    {
                        thumbnail = new WriteableBitmap(bitmap);
                    }
                    Stride = thumbnail.PixelWidth * thumbnail.Format.BitsPerPixel / 8;
                    PixelData = new byte[Stride * thumbnail.PixelHeight];
                    thumbnail.CopyPixels(PixelData, Stride, 0);
                    Int32Rect Intrect = new Int32Rect(col * smallImageSize, row * smallImageSize, smallImageSize, smallImageSize);
                    newBitmap.WritePixels(Intrect, PixelData, Stride, 0);
                    image1.Source = newBitmap;
                    Dispatcher.Invoke(new WaitDelegate(Wait), DispatcherPriority.ApplicationIdle);
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Wait()
        {
            Thread.Sleep(1000);
        }
    }
}