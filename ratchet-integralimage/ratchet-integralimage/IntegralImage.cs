using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ratchet.Math
{
    unsafe public class IntegralImage : IDisposable
    {
        int* _IntegralImageData = null;
        int _Width = 0;
        int _Height = 0;

        public int Width { get { return _Width; } }
        public int Height { get { return _Height; } }

        public enum Format
        {
            Integer,
            Byte
        }

        void BuildFromInteger(int* pImage, int Stride, int Width, int Height)
        {
            int* pIntegralImageData = _IntegralImageData;
            int* pIntegralImageData_00 = _IntegralImageData;
            int* pIntegralImageData_10 = _IntegralImageData;
            int* pIntegralImageData_11 = _IntegralImageData;


            *pIntegralImageData = (int)*pImage; pImage += Stride;
            pIntegralImageData++;

            for (int x = 1; x < Width; x++)
            {
                *pIntegralImageData = (int)*pImage + *pIntegralImageData_00;
                pImage += Stride;
                pIntegralImageData++;
                pIntegralImageData_00++;
            }

            for (int y = 1; y < Height; y++)
            {
                pIntegralImageData_00 = pIntegralImageData;

                *pIntegralImageData = (int)*pImage + *pIntegralImageData_10;
                pImage += Stride;
                pIntegralImageData++;
                pIntegralImageData_10++;
                for (int x = 1; x < Width; x++)
                {
                    *pIntegralImageData = (int)*pImage + *pIntegralImageData_00 + *pIntegralImageData_10 - *pIntegralImageData_11;
                    pImage += Stride;
                    pIntegralImageData++;
                    pIntegralImageData_00++;
                    pIntegralImageData_10++;
                    pIntegralImageData_11++;
                }

                pIntegralImageData_11++;
            }
        }

        void BuildFromByte(byte* pImage, int Stride, int Width, int Height)
        {
            int* pIntegralImageData = _IntegralImageData;
            int* pIntegralImageData_00 = _IntegralImageData;
            int* pIntegralImageData_10 = _IntegralImageData;
            int* pIntegralImageData_11 = _IntegralImageData;


            *pIntegralImageData = (int)*pImage; pImage += Stride;
            pIntegralImageData++;

            for (int x = 1; x < Width; x++)
            {
                *pIntegralImageData = (int)*pImage + *pIntegralImageData_00;
                pImage += Stride;
                pIntegralImageData++;
                pIntegralImageData_00++;
            }

            for (int y = 1; y < Height; y++)
            {
                pIntegralImageData_00 = pIntegralImageData;

                *pIntegralImageData = (int)*pImage + *pIntegralImageData_10;
                pImage += Stride;
                pIntegralImageData++;
                pIntegralImageData_10++;
                for (int x = 1; x < Width; x++)
                {
                    *pIntegralImageData = (int)*pImage + *pIntegralImageData_00 + *pIntegralImageData_10 - *pIntegralImageData_11;
                    pImage += Stride;
                    pIntegralImageData++;
                    pIntegralImageData_00++;
                    pIntegralImageData_10++;
                    pIntegralImageData_11++;
                }

                pIntegralImageData_11++;
            }
        }

        public IntegralImage(IntPtr ImageData, Format ImageFormat, int Stride, int Width, int Height)
        {
            _IntegralImageData = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(Width * Height * sizeof(int)).ToPointer();
            _Width = Width;
            _Height = Height;

            switch (ImageFormat)
            {
                case Format.Integer: BuildFromInteger((int*)ImageData.ToPointer(), Stride, Width, Height); break;
                case Format.Byte: BuildFromByte((byte*)ImageData.ToPointer(), Stride, Width, Height); break;
                default: throw new Exception("Invalid data format");
            }
        }

        public void Dispose()
        {
            if (_IntegralImageData != null)
            {
                System.Runtime.InteropServices.Marshal.FreeHGlobal(new IntPtr(_IntegralImageData));
            }
        }

        ~IntegralImage()
        {
            Dispose();
        }

        public int Get(int x, int y, int width, int height)
        {
            if (x < 0 || y < 0) { throw new Exception("Invalid region"); }

            int toX = x + width;
            int toY = y + height;

            if (toX >= _Width || toY >= _Height) { throw new Exception("Invalid region"); }


            int _11 = _IntegralImageData[toX + toY * _Width];
            int _10 = _IntegralImageData[toX + y * _Width];
            int _01 = _IntegralImageData[x + toY * _Width];
            int _00 = _IntegralImageData[x + y * _Width];

            return _00 + _11 - _01 - _10;
        }
    }
}
