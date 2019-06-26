using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

// ---------------------------------------------------------------------------------
// C# net2 port of https://github.com/PS3Xploit/PS3HEN/blob/master/PS3HEN_GEN/main.c
// ---------------------------------------------------------------------------------
namespace HEN_GEN
{
    class Hen_gen
    {
        static byte[] swap64(long data)
        {
            byte[] ret = BitConverter.GetBytes(data);
            if (BitConverter.IsLittleEndian) Array.Reverse(ret);
            return ret;
        }

        static void dump(FileStream target, FileStream src)
        {
            byte[] buffer = new byte[src.Length];

            if (src.Read(buffer, 0, buffer.Length) != buffer.Length)
                throw new InvalidDataException(src.Name + " is incomplete");

            target.Write(buffer, 0, buffer.Length);
            src.Close();
        }

        static void wipe(FileStream target, int amount)
        {
            byte[] buffer = new byte[amount];
            target.Write(buffer, 0, amount);

            target.Seek(-amount, SeekOrigin.Current);
        }

        static void setsize(FileStream target, int offset, long size)
        {
            byte[] buffer = swap64(size);
            target.Position = offset;
            target.Write(buffer, 0, buffer.Length);
        }

        static int Main(string[] args)
        {
            if (args.Length < 4)
            {
                Console.WriteLine("Usage:stackframe.bin stage2 stage0 sprx\n");
                return -1;
            }

            FileStream sp = null;
            FileStream stage2 = null;
            FileStream stage0 = null;
            FileStream sprx = null;

            try
            {
                sp = File.Open(args[0], FileMode.Open, FileAccess.ReadWrite);
                stage2 = File.Open(args[1], FileMode.Open, FileAccess.ReadWrite);
                stage0 = File.Open(args[2], FileMode.Open, FileAccess.ReadWrite);
                sprx = File.Open(args[3], FileMode.Open, FileAccess.ReadWrite);
            }
            catch (Exception e)
            {
                Console.WriteLine("error opening file(s)! {0}\n", e.Message);
                return -1;
            }

            const int max_stage2_size = 0x1fff8;
            const int max_stage0_size = 0xd000;
            const int max_sprx_size = 0xfff8;

            if (stage2.Length > 0x1fff8)
            {
                Console.WriteLine("stage2 too big!EXITING!\n");
                return -1;
            }
            if (stage0.Length > 0xd000)
            {
                Console.WriteLine("stage0 too big!EXITING!\n");
                return -1;
            }
            if (sprx.Length > 0xfff8)
            {
                Console.WriteLine("sprx too big!EXITING!\n");
                return -1;
            }

            uint truncate_len = 0x110000;

            setsize(sp, 0x7fff8, stage2.Length);
            sp.Position = 0x80000;
            wipe(sp, max_stage2_size);
            //	truncate_len+=stage2.Length;
            dump(sp, stage2);

            setsize(sp, 0x1008b8, stage0.Length);
            sp.Position = 0x102000;
            wipe(sp, max_stage0_size);
            dump(sp, stage0);

            sp.Position = 0x70000;
            wipe(sp, max_sprx_size);
            dump(sp, sprx);

            sp.SetLength(truncate_len);
            sp.Close();
            Console.WriteLine("DONE!\n");
            return 0;
        }
    }
}
