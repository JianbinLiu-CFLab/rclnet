using System;
using System.Text;

namespace Rcl.Unity
{
    internal static unsafe class NativeString
    {
        public static int GetUtf8BufferSize(string value, bool zeroTerminated = true)
        {
            return Encoding.UTF8.GetMaxByteCount(value.Length) + (zeroTerminated ? 1 : 0);
        }

        public static int GetUtf8BufferSize(string[] values, bool zeroTerminated = true)
        {
            var size = 0;
            for (var i = 0; i < values.Length; i++)
            {
                size += GetUtf8BufferSize(values[i], zeroTerminated);
            }

            return size;
        }

        public static void FillUtf8Buffer(string value, Span<byte> destination)
        {
            destination.Clear();
            Encoding.UTF8.GetBytes(value, destination);
        }

        public static void FillUtf8Buffer(string[] values, Span<byte> destination, byte** pointers)
        {
            var offset = 0;

            fixed (byte* destinationPointer = destination)
            {
                for (var i = 0; i < values.Length; i++)
                {
                    pointers[i] = destinationPointer + offset;
                    offset += Encoding.UTF8.GetBytes(values[i], destination.Slice(offset));
                    destination[offset++] = 0;
                }
            }
        }

        public static string? FromNullTerminatedUtf8(sbyte* value)
        {
            if (value == null)
            {
                return null;
            }

            var length = 0;
            var bytes = (byte*)value;
            while (bytes[length] != 0)
            {
                length++;
            }

            return Encoding.UTF8.GetString(bytes, length);
        }
    }
}
