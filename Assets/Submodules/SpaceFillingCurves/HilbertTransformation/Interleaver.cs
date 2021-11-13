﻿using System;
using System.Collections.Concurrent;
using System.Numerics;


namespace HilbertTransformation
{
    /// <summary>
    /// Interleave the bits from an array of unsigned integers to form an array of bytes.
    /// 
    /// This is an optimization of FastHilbert.Interleave.
    /// It is part of the process of computing a Hilbert index.
    /// </summary>
    public class Interleaver
    {
		/// <summary>
		/// Maps a byte and bit from the source array to the target array.
		/// </summary>
        struct Indices
        {
            /// <summary>
            /// Index into source vector of source uint to read.
            /// </summary>
            public readonly int iFromUintVector;

            /// <summary>
            /// Index into target vector of target byte to write.
            /// </summary>
            public readonly short iToByteVector;

            /// <summary>
            /// Index into source uint of source bit to read.
            /// </summary>
            public readonly byte iFromUintBit;

            /// <summary>
            /// Index into target byte of target bit to write.
            /// </summary>
            public readonly byte iToByteBit;

            public Indices(int fromUintVector, byte fromUintBit, short toByteVector, byte toByteBit)
            {
                iFromUintVector = fromUintVector;
                iFromUintBit = fromUintBit;
                iToByteVector = toByteVector;
                iToByteBit = toByteBit;
            }

			public override string ToString()
			{
				return string.Format("Byte.Bit from {0}.{1} to {2}.{3}", iFromUintVector, iFromUintBit, iToByteVector, iToByteBit);
			}
        }

        #region Cache of already created Interleavers

        private static readonly Lazy<ConcurrentDictionary<int,Interleaver>> _cache
         = new Lazy<ConcurrentDictionary<int, Interleaver>>(() => new ConcurrentDictionary<int, Interleaver>());

        private static ConcurrentDictionary<int, Interleaver> Cache { get { return _cache.Value; } }

        public static Interleaver Instance(int dimensions, int bitDepth)
        {
            return Cache.GetOrAdd(MakeHashCode(dimensions, bitDepth), key => new Interleaver(dimensions, bitDepth));
        }

        #endregion

        /// <summary>
        /// Number of bits to use to encode each dimension.
        /// </summary>
        private readonly int BitDepth;

        /// <summary>
        /// Number of dimensins in the vector.
        /// </summary>
        private readonly int Dimensions;

        /// <summary>
        /// Number of Bits = BitDepth x Dimensions.
        /// </summary>
        private readonly int Bits;

        /// <summary>
        /// Number of full bytes needed to hold the interleaved data = Bits >> 3.
        /// This excludes the extra byte needed to hold the remaining bits that don't add up to a whole byte.
        /// </summary>
        private readonly int BytesNeeded;
        private Indices[] PrecomputedIndices;

		public Interleaver(int dimensions, int bitDepth)
		{
			Dimensions = dimensions;
			BitDepth = bitDepth;
			Bits = Dimensions * BitDepth;
			BytesNeeded = Bits >> 3; // If we have 7 bits, 7 >> 3 is zero. We will add one byte to this to handle the extra bits.

			// When we create the BigInteger, it expects little-endian ordering, but the
			// input comes in big-endian order.
			PrecomputedIndices = new Indices[Bits];
			var iBit = 0;
			for (byte iSourceBit = 0; iSourceBit < BitDepth; iSourceBit++)
				for (var iSourceInt = Dimensions - 1; iSourceInt >= 0; iSourceInt--)
				{
					var iTargetByte = (short)(iBit >> 3);
					var iTargetBit = (byte)(iBit % 8);

					PrecomputedIndices[iBit] = new Indices(
						iSourceInt,
						iSourceBit,
						iTargetByte,
						iTargetBit
					);
					iBit++;
				}
			Comparison<Indices> sortByTargetByteAndBit = (a, b) =>
			{
				if (a.iToByteVector < b.iToByteVector) return -1;
				if (a.iToByteVector > b.iToByteVector) return 1;
				if (a.iToByteBit < b.iToByteBit) return -1;
				if (a.iToByteBit > b.iToByteBit) return 1;
				return 0;
			};
			Array.Sort(PrecomputedIndices, sortByTargetByteAndBit);
		}

        /// <summary>
        /// Interleave the bits of an unsigned vector and generate a byte array in little-endian order, as needed for the BigInteger constructor.
        /// 
        /// The high-order bit from the last number in vector becomes the high-order bit of last byte in the generated byte array.
        /// The high-order bit of the next to last number becomes the second highest-ordered bit in the last byte in the generated byte array.
        /// The low-order bit of the first number becomes the low order bit of the first byte in the new array.
        /// </summary>
        public byte[] Interleave(uint[] vector)
        {
            var byteVector = new byte[BytesNeeded + 1]; // An extra byte is needed to hold the extra bits and a sign bit for the BigInteger.
            //var extraBits = Bits - BytesNeeded << 3;
            int iIndex = 0;
            var iByte = 0;
            for (; iByte < BytesNeeded; iByte++)
            {
                // Unroll the loop so we compute the bits for a whole byte at a time.
                uint bits = 0;
                var idx0 = PrecomputedIndices[iIndex];
                var idx1 = PrecomputedIndices[iIndex + 1];
                var idx2 = PrecomputedIndices[iIndex + 2];
                var idx3 = PrecomputedIndices[iIndex + 3];
                var idx4 = PrecomputedIndices[iIndex + 4];
                var idx5 = PrecomputedIndices[iIndex + 5];
                var idx6 = PrecomputedIndices[iIndex + 6];
                var idx7 = PrecomputedIndices[iIndex + 7];
                bits = (((vector[idx0.iFromUintVector] >> idx0.iFromUintBit) & 1U))
                     | (((vector[idx1.iFromUintVector] >> idx1.iFromUintBit) & 1U) << 1)
                     | (((vector[idx2.iFromUintVector] >> idx2.iFromUintBit) & 1U) << 2)
                     | (((vector[idx3.iFromUintVector] >> idx3.iFromUintBit) & 1U) << 3)
                     | (((vector[idx4.iFromUintVector] >> idx4.iFromUintBit) & 1U) << 4)
                     | (((vector[idx5.iFromUintVector] >> idx5.iFromUintBit) & 1U) << 5)
                     | (((vector[idx6.iFromUintVector] >> idx6.iFromUintBit) & 1U) << 6)
                     | (((vector[idx7.iFromUintVector] >> idx7.iFromUintBit) & 1U) << 7);
                byteVector[iByte] = (byte)bits;
                iIndex += 8;
            }
            for (; iIndex < PrecomputedIndices.Length; iIndex++)
            {
                var idx = PrecomputedIndices[iIndex];
                var bit = (byte)(((vector[idx.iFromUintVector] >> idx.iFromUintBit) & 1U) << idx.iToByteBit);
                byteVector[idx.iToByteVector] |= bit;
            }
            return byteVector;
        }

        /// <summary>
        /// Interleave the bits of an unsigned vector and generate a byte array in little-endian order, as needed for the BigInteger constructor.
        /// 
        /// The high-order bit from the last number in vector becomes the high-order bit of last byte in the generated byte array.
        /// The high-order bit of the next to last number becomes the second highest-ordered bit in the last byte in the generated byte array.
        /// The low-order bit of the first number becomes the low order bit of the first byte in the new array.
        /// </summary>
        public byte[] Interleave_Unordered(uint[] vector)
        {
            // Older way where each byte is updated once for each bit. Simpler to understand but less efficient.
            var byteVector = new byte[BytesNeeded + 1]; // An extra byte is needed to hold the extra bits and a sign bit for the BigInteger.
            foreach (var idx in PrecomputedIndices)
            {
                var bit = (byte)(((vector[idx.iFromUintVector] >> idx.iFromUintBit) & 1U) << idx.iToByteBit);
                byteVector[idx.iToByteVector] |= bit;
            }
            return byteVector;
        }

        /// <summary>
        /// Convert a transposed Hilbert index back into a BigInteger index.
        /// </summary>
        /// <param name="transposedIndex">A Hilbert index in transposed form.</param>
        /// <returns>The Hilbert index (or distance) expresssed as a BigInteger.</returns>
        public BigInteger Untranspose(uint[] transposedIndex)
        {
            // The high bit of the first coordinate becomes the high bit of the index.
            // The high bit of the second coordinate becomes the second bit of the index.

            var interleavedBytes = Interleave(transposedIndex);
            return new BigInteger(interleavedBytes);
        }

        #region GetHashCode, Equals and ToString

        private static int MakeHashCode(int dimensions, int bitDepth)
        {
            return bitDepth * dimensions << 6;
        }

        public override int GetHashCode()
        {
            return MakeHashCode(Dimensions, BitDepth);
        }

        public override bool Equals(object obj)
        {
            return obj != null && GetHashCode() == obj.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Interleaver for {0} dimensions of {1} bits each", Dimensions, BitDepth);
        }

        #endregion

    }
}
