
namespace Wave_Function_Collapse
{
    public class Utilities
    {
        public static bool CompareUpPixelData(byte[] myData, byte[] UpData, int TileSize)
        //Compare the bottom 2 pixel rows of UpData with the top 2 rows of myData
        {
            int UpPixelIndex;
            int MyPixelIndex;
            for (int X = 0; X < TileSize; X++)
            {
                for (int Y = 0; Y < TileSize - 1; Y++)
                {
                    UpPixelIndex = 4 * ((Y + 1) * TileSize + X);
                    MyPixelIndex = 4 * (Y * TileSize + X);
                    if (myData[MyPixelIndex] != UpData[UpPixelIndex]) return false;
                    if (myData[MyPixelIndex + 1] != UpData[UpPixelIndex + 1]) return false;
                    if (myData[MyPixelIndex + 2] != UpData[UpPixelIndex + 2]) return false;
                }
            }
            return true;
        }

        public static bool CompareDownPixelData(byte[] myData, byte[] DownData, int TileSize)
        //Compare the top 2 pixel rows of DownData with the bottom 2 rows of myData
        {
            int DownPixelIndex;
            int MyPixelIndex;
            for (int X = 0; X < TileSize; X++)
            {
                for (int Y = 0; Y < TileSize - 1; Y++)
                {
                    DownPixelIndex = 4 * (Y * TileSize + X);
                    MyPixelIndex = 4 * ((Y + 1) * TileSize + X);
                    if (myData[MyPixelIndex] != DownData[DownPixelIndex]) return false;
                    if (myData[MyPixelIndex + 1] != DownData[DownPixelIndex + 1]) return false;
                    if (myData[MyPixelIndex + 2] != DownData[DownPixelIndex + 2]) return false;
                }
            }
            return true;
        }

        public static bool CompareRightPixelData(byte[] myData, byte[] RightData, int TileSize)
        //Compare the left 2 pixel columns of RightData with the right 2 columns of myData
        {
            int RightPixelIndex;
            int MyPixelIndex;
            for (int X = 0; X < TileSize - 1; X++)
            {
                for (int Y = 0; Y < TileSize; Y++)
                {
                    RightPixelIndex = 4 * (Y * TileSize + X);
                    MyPixelIndex = 4 * (Y * TileSize + X + 1);
                    if (myData[MyPixelIndex] != RightData[RightPixelIndex]) return false;
                    if (myData[MyPixelIndex + 1] != RightData[RightPixelIndex + 1]) return false;
                    if (myData[MyPixelIndex + 2] != RightData[RightPixelIndex + 2]) return false;
                }
            }
            return true;
        }

        public static bool CompareLeftPixelData(byte[] myData, byte[] LeftData, int TileSize)
        //Compare the right 2 pixel columns of LeftData with the left 2 columns of myData
        {
            int LeftPixelIndex;
            int MyPixelIndex;
            for (int X = 0; X < TileSize - 1; X++)
            {
                for (int Y = 0; Y < TileSize; Y++)
                {
                    LeftPixelIndex = 4 * (Y * TileSize + X + 1);
                    MyPixelIndex = 4 * (Y * TileSize + X);
                    if (myData[MyPixelIndex] != LeftData[LeftPixelIndex]) return false;
                    if (myData[MyPixelIndex + 1] != LeftData[LeftPixelIndex + 1]) return false;
                    if (myData[MyPixelIndex + 2] != LeftData[LeftPixelIndex + 2]) return false;
                }
            }
            return true;
        }

    }
}
