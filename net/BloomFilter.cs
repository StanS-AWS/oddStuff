using System;
using System.Security.Cryptography;
using System.Text;

namespace Bloom
{
    internal class BloomFilter
    {
        private uint[] matcher;
        private MD5 md5;

        const int NUMBER_OF_HASHES = 4;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="allWords">Array of known words</param>
        public BloomFilter(String[] allWords)
        {
            if (allWords == null || allWords.Length == 0)
            {
                throw new ArgumentException("Empty or null input.");
            }

            //const int MATCHER_SIZE = 524288;//2^SPACE_DEPTH/sizeof(uint)
            int MATCHER_SIZE = 524288;//(2^24)/32

            matcher = new uint[MATCHER_SIZE];
            md5 = MD5.Create();
            foreach (string s in allWords)
            {
                for (int hashVar = 0; hashVar < NUMBER_OF_HASHES; hashVar++)
                {
                    uint pos = getHashIndex(s, hashVar);
                    setMatcher(pos);
                }
            }
        }

        /// <summary>
        /// Checks the input string in the list of the known words 
        /// </summary>
        /// <param name="s"></param>
        /// <returns>false if the value doesn't match to any of the known words</returns>
        public bool isMatch(string s)
        {
            if (String.IsNullOrEmpty(s))
            {
                throw new ArgumentNullException();
            }

            for (int hashVar = 0; hashVar < NUMBER_OF_HASHES; hashVar++)
            {
                uint pos = getHashIndex(s, hashVar);
                if (!checkMatcher(pos))
                {
                    return false;
                }
            }
            return true;
        }

        private uint getHashIndex(string s, int hashVar)
        {
            //using 24-bit hash function slicing long MD5 hash into 24-bit pieces
            byte[] hashed = md5.ComputeHash(Encoding.UTF8.GetBytes(s));
            byte[] buf = new byte[4];

            Array.Copy(hashed, hashVar * 3, buf, 0, 3); //copy 24-bit hash into 32-bit buffer 
            return BitConverter.ToUInt32(buf, 0);
        }

        private bool checkMatcher(uint pos)
        {
            uint index = pos / 32;
            uint bm = 1u << (int)(pos % 32);
            return (matcher[index] & bm) > 0;
        }

        private void setMatcher(uint pos)
        {
            uint index = pos / 32;
            uint bm = 1u << (int)(pos % 32);
            matcher[index] = (uint)(matcher[index] | bm);
        }

        delegate byte[] simpleHash(String s);
        
        private byte[] calculateHash(simpleHash  sh, String s)
        {
            return sh(s); 
        }

        private byte[] doDifferentHashes(String s, String hashType)
        {
            switch (hashType)
            {
                case ("MD5"): return md5.ComputeHash(Encoding.UTF8.GetBytes(s));
                case ("CRC-32"): return calculateHash(delegate {
                                                            return new byte[s.Length];
                                                        }, s);
                default: return calculateHash(delegate { return new byte[s.Length]; }, s);
            }

            
        }
        

    }
}