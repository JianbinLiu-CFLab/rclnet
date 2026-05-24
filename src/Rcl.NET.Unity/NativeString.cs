// Copyright (c) 2026 Jianbin Liu.
// Licensed under the MIT License.
// See LICENSE in the repository root for license information.
using System;
using System.Text;

namespace Rcl.Unity
{
    /// <summary>
    /// Converts managed strings to and from stack-allocated UTF-8 buffers for rcl calls.
    /// </summary>
    internal static unsafe class NativeString
    {
        /// <summary>
        /// Calculates the UTF-8 byte buffer size required for a managed string.
        /// </summary>
        public static int GetUtf8BufferSize(string value, bool zeroTerminated = true)
        {
            return Encoding.UTF8.GetMaxByteCount(value.Length) + (zeroTerminated ? 1 : 0);
        }

        /// <summary>
        /// Calculates the UTF-8 byte buffer size required for an argv-style string array.
        /// </summary>
        public static int GetUtf8BufferSize(string[] values, bool zeroTerminated = true)
        {
            var size = 0;
            for (var i = 0; i < values.Length; i++)
            {
                size += GetUtf8BufferSize(values[i], zeroTerminated);
            }

            return size;
        }

        /// <summary>
        /// Writes a managed string into a UTF-8 destination buffer.
        /// </summary>
        public static void FillUtf8Buffer(string value, Span<byte> destination)
        {
            destination.Clear();
            Encoding.UTF8.GetBytes(value, destination);
        }

        /// <summary>
        /// Writes multiple managed strings into one UTF-8 buffer and records native pointers for each entry.
        /// </summary>
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

        /// <summary>
        /// Reads a null-terminated UTF-8 native string.
        /// </summary>
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
